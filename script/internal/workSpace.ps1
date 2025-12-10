using module './Cache.psm1'
. $PSScriptRoot/FstConfig.ps1

class WorkSpace {
    [string]$rootConfig = ""
    [string]$fstarConfig = ""

    static [WorkSpace] Current() {
        $r = [WorkSpace]::new()
        
        $meta = Get-Meta
        $r.rootConfig = Join-Path (Get-Root) $meta.RootConfig
        $r.fstarConfig = Join-Path (Get-Root) $meta.FstConfig

        return $r
    }
}

class WorkSpaceState {
    [bool]$isRootConfigDesired = $false
    [bool]$isFStarConfigDesired = $false

    static [WorkSpaceState] Desired() {
        $r = [WorkSpaceState]::new()
        $r.isRootConfigDesired = $true
        $r.isFStarConfigDesired = $true
        return $r
    }

    static [WorkSpaceState] Current() {
        $r = [WorkSpaceState]::new()

        $r.isRootConfigDesired = Test-TestSimpleOutput (Set-RootConfig -TestOnly -PassThru)

        $cache = [Cache]::new()
        $cache.TestRootConfigResult = $r.isRootConfigDesired

        $r.isFStarConfigDesired = Test-TestSimpleOutput (Set-FStarConfig -TestOnly -PassThru -Cache $cache)
        return $r
    }
}

function Test-DesiredWorkSpaceStateValidity {
    param ([WorkSpaceState]$InputObject, [ref]$Diagnostics)
    function Format-Message ([string]$Msg) { "Not supproted: $Msg = false" }

    $des = $InputObject
    $Diagnostics = @()

    if ($des.isRootConfigDesired -eq $false) { $Diagnostics += Format-Message('isRootConfigDesired') }
    if ($des.isFStarConfigDesired -eq $false) { $Diagnostics += Format-Message('isFStarConfigDesired') }

    return $Diagnostics.Value.Count -eq 0
}

function Set-WorkSpace { param ([switch]$TestOnly, [switch]$PassThru, [switch]$Silent)
    $meta = Get-Meta
    $des = [WorkSpaceState]::Desired()
    $prev = [WorkSpaceState]::Current()

    $str0 = 'WorkSpace'
    #--- Test ---
    if (-not $Silent) { Write-HostWithTime "Test $str0" }

    $testOut = Invoke-DscTest $des $prev

    if (-not $Silent) { ConvertTo-Json $testOut | Write-Host }
    if ($PassThru) { $testOut }
    #---|

    if ($TestOnly) { return }
    if ($testOut.inDesiredState) { return }

    #--- Validate desired state ---
    if (-not (Test-DesiredWorkSpaceStateValidity $des $dg)) {
        if ($null -ne $dg) { $dg | Write-Warning }
        exit 1
    }
    #---|

    $cur = $prev | ConvertTo-Json | ConvertFrom-Json
    #--- Set ---
    if (-not $Silent) { Write-HostWithTime "Set $str0" }

    if ($des.isRootConfigDesired -ne $cur.isRootConfigDesired) {
        if (-not (Test-TestSimpleOutput (Set-RootConfig -TestOnly -PassThru))) {
            Write-Warning "$($meta.RootConfig) is not in desired state. Run Set-RootConfig.ps1 first."
            exit 1
        } else {
            $cur.isRootConfigDesired = $true
        }
    }
    $cache = [Cache]::new()
    $cache.TestRootConfigResult = $cur.isRootConfigDesired

    if ($des.isFStarConfigDesired -ne $cur.isFStarConfigDesired) {
        $cur.isFStarConfigDesired = Test-TestSimpleOutput (Set-FStarConfig -Cache $cache)
    }

    $setOut = New-DscSetOutput $prev $cur
    if (-not $Silent) { ConvertTo-Json $setOut | Write-Host }
    if ($PassThru) { $setOut }
    #---|
}