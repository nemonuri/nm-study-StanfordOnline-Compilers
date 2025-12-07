using module './ErrorCode.psm1'

. $PSScriptRoot/Common.ps1
. $PSScriptRoot/Meta.ps1
. $PSScriptRoot/Dsc.ps1

class RootConfig {
    [string]$fstarExe = ""
    [string]$z3Exe = ""
    [string[]]$fstarLibs = @()

    static [psobject] Current() {
        $meta = Get-Meta
        $path = Join-Path (Get-Root) $meta.RootConfig

        if (-not (Test-Path $path -PathType Leaf)) {
            return [ErrorCode]::FileNotExist
        }

        if (-not (Test-Json -Path $path)) {
            return [ErrorCode]::InvalidJson
        }

        return <#[RootConfig]#>(Get-Content $path | ConvertFrom-Json | ConvertTo-TypedObject 'RootConfig')
    }
}
function Get-CurrentRootConfig { [RootConfig]::Current() }

class RootConfigState {
    [bool] $_exist = $false
    [bool] $isValidJson = $false
    [bool] $hasFStarExe = $false
    [bool] $hasZ3Exe = $false
    [bool] $hasFStarLibs = $false

    static [RootConfigState] Desired() {
        return [RootConfigState]@{
            _exist = $true
            isValidJson = $true
            hasFStarExe = $true
            hasZ3Exe = $true
            hasFStarLibs = $true
        }
    }

    static [RootConfigState] Current() {
        $rc = Get-CurrentRootConfig

        $r = [RootConfigState]::new()
        if ($rc -eq [ErrorCode]::FileNotExist) { return $r }
        $r._exist = $true

        if ($rc -eq [ErrorCode]::InvalidJson) { return $r }
        $r.isValidJson = $true

        $r.hasFStarExe = ($null -ne $rc.fstarExe)
        $r.hasZ3Exe = ($null -ne $rc.z3Exe)
        $r.hasFStarLibs = ($null -ne $rc.fstarLibs)
        return $r
    }
}
function Get-DesiredRootConfigState { [RootConfigState]::Desired() }
function Get-CurrentRootConfigState { [RootConfigState]::Current() }

function New-RootConfigComment {
    @{
        fstarExe = "Path for F* binary. F* download link: https://github.com/FStarLang/FStar/releases"
        z3Exe = "Path for Z3 binary."
        fstarLibs = "Pathes for default F* libraries."
    }
}

function Set-DesiredRootConfigState {
    $meta = Get-Meta
    $path = Join-Path (Get-Root) $meta.RootConfig
    
    $des = Get-DesiredRootConfigState
    $cur = Get-CurrentRootConfigState

    if (($des._exist -ne $cur._exist) -or ($des.isValidJson -ne $cur.isValidJson)) {
        if ($des._exist -eq $false) { throw "Not supported." }
        if ($des.isValidJson -eq $false) { throw "Not supported." }

        $newRc = [RootConfig]::new() | ConvertTo-HashTable
        $newRc._comment = (New-RootConfigComment)
        ConvertTo-Json $newRc | Out-File $path

        $cur._exist = $true
        $cur.isValidJson = $true
    }

    $rc = Get-Content $path | ConvertFrom-Json -AsHashtable

    if ($des.hasFStarExe -ne $cur.hasFStarExe) {
        if ($des.hasFStarExe -eq $false) { throw "Not supported." }
        $rc.fstarExe = ""
        $cur.hasFStarExe = $true
    }

    if ($des.hasZ3Exe -ne $cur.hasZ3Exe) {
        if ($des.hasZ3Exe -eq $false) { throw "Not supported." }
        $rc.z3Exe = ""
        $cur.hasZ3Exe = $true
    }

    if ($des.hasFStarLibs -ne $cur.hasFStarLibs) {
        if ($des.hasFStarLibs -eq $false) { throw "Not supported." }
        $rc.fstarLibs = @()
        $cur.hasFStarLibs = $true
    }

    $rc | ConvertTo-Json | Out-File $path

    return $cur
}

function Test-DesiredRootConfigStateValidity {
    param ([RootConfigState]$InputObject, [ref]$Diagnostics)
    function Format-Message ([string]$Msg) { "Not supproted: $Msg = false" }

    $des = $InputObject
    $Diagnostics = @()

    if ($des._exist -eq $false) { $Diagnostics += Format-Message('_exist') }
    if ($des.isValidJson -eq $false) { $Diagnostics += Format-Message('isValidJson') }
    if ($des.hasFStarExe -eq $false) { $Diagnostics += Format-Message('hasFStarExe') }
    if ($des.hasZ3Exe -eq $false) { $Diagnostics += Format-Message('hasZ3Exe') }
    if ($des.hasFStarLibs -eq $false) { $Diagnostics += Format-Message('hasFStarLibs') }

    return $Diagnostics.Count -eq 0
}

function Set-RootConfig { param ([switch]$TestOnly, [switch]$PassThru, [switch]$Silent)
    $meta = Get-Meta
    $path = Join-Path (Get-Root) $meta.RootConfig
    $des = Get-DesiredRootConfigState
    $prev = Get-CurrentRootConfigState 

    #--- Test ---
    if (-not $Silent) { Write-HostWithTime "Test $($meta.RootConfig)" }

    $testOut = Invoke-DscTest $des $prev

    if (-not $Silent) { ConvertTo-Json $testOut | Write-Host }
    if ($PassThru) { $testOut }
    #---|

    if ($TestOnly) { return }
    if ($testOut.inDesiredState) { return }

    #--- Validate desired state ---
    if (-not (Test-DesiredRootConfigStateValidity($des, $dg))) {
        $dg | Write-Error
        exit 1
    }
    #---|

    $cur = $prev | ConvertTo-Json | ConvertFrom-Json
    #--- Set ---
    if (-not $Silent) { Write-HostWithTime "Set $($meta.RootConfig)" }
    if (($des._exist -ne $cur._exist) -or ($des.isValidJson -ne $cur.isValidJson)) {
        if ($des._exist -eq $false) { throw "Not supported." }
        if ($des.isValidJson -eq $false) { throw "Not supported." }

        $newRc = [RootConfig]::new() | ConvertTo-HashTable
        $newRc._comment = (New-RootConfigComment)
        ConvertTo-Json $newRc | Out-File $path

        $cur._exist = $true
        $cur.isValidJson = $true
    }

    $rc = Get-Content $path | ConvertFrom-Json -AsHashtable

    if ($des.hasFStarExe -ne $cur.hasFStarExe) {
        if ($des.hasFStarExe -eq $false) { throw "Not supported." }
        $rc.fstarExe = ""
        $cur.hasFStarExe = $true
    }

    if ($des.hasZ3Exe -ne $cur.hasZ3Exe) {
        if ($des.hasZ3Exe -eq $false) { throw "Not supported." }
        $rc.z3Exe = ""
        $cur.hasZ3Exe = $true
    }

    if ($des.hasFStarLibs -ne $cur.hasFStarLibs) {
        if ($des.hasFStarLibs -eq $false) { throw "Not supported." }
        $rc.fstarLibs = @()
        $cur.hasFStarLibs = $true
    }

    $rc | ConvertTo-Json | Out-File $path

    $setOut = New-DscSetOutput $prev $cur
    if (-not $Silent) { ConvertTo-Json $setOut | Write-Host }
    if ($PassThru) { $setOut }
    #---|
}

