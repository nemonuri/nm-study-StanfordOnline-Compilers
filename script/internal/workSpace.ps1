using module './ErrorCode.psm1'
using namespace System
using namespace System.Collections.Generic
using namespace System.Linq

. $PSScriptRoot/Common.ps1
. $PSScriptRoot/RootConfig.ps1

class FstConfig {
    [string] $fstar_exe = ""
    [string[]] $options = @()
    [string[]] $include_dirs = @()

    static [psobject] Current() {
        $meta = Get-Meta
        $path = Join-Path (Get-Root) $meta.FstConfig

        if (-not (Test-Path $path -PathType Leaf)) {
            return [ErrorCode]::FileNotExist
        }

        if (-not (Test-Json -Path $path)) {
            return [ErrorCode]::InvalidJson
        }

        return <#[FstConfig]#>(Get-Content $path | ConvertFrom-Json | ConvertTo-TypedObject 'FstConfig')
    }
}

function Get-FStarProjectFiles { [OutputType([string[]])] param ()
    $meta = Get-Meta
    $root = Get-Root
    $src = Join-Path $root $meta.Src -Resolve
    return [System.IO.Directory]::EnumerateFiles($src, $meta.FStarProject, 'AllDirectories')
}

class FstConfigState {
    [bool] $_exist = $false
    [bool] $isValidJson = $false
    [bool] $hasFStarExe = $false
    [bool] $hasOptions = $false
    [bool] $hasIncludeDirs = $false
    [string] $fstarExePath = ""
    [string] $z3ExePath = ""
    [string[]] $includeDirsForRootConfig = @()
    [string[]] $includeDirsForFStarProject = @()
    [string[]] $includeDirsUnspecified = @()

    static [FstConfigState] Desired([psobject]$RootConfig) {
        $r = [FstConfigState]::new()
        $memberNames = Get-Member -InputObject $r -MemberType 'Property' | Where-Object {$_.Definition.IndexOf('bool ') -eq 0} | ForEach-Object {$_.Name}
        foreach ($memberName in $memberNames) {
            $r.$memberName = $true
        }
        $r.fstarExePath = $RootConfig.fstarExe
        $r.z3ExePath = $RootConfig.z3Exe
        $r.includeDirsForRootConfig = $RootConfig.fstarLibs
        $r.includeDirsForFStarProject = Get-FStarProjectFiles | ForEach-Object {Join-Path $_ ".." -Resolve}
        $r.includeDirsUnspecified = @()
        return $r
    }

    static [FstConfigState] Current([psobject]$RootConfig) {
        [FstConfigState]$desired = [FstConfigState]::Desired($RootConfig)

        $c = [FstConfig]::Current()

        $r = [FstConfigState]::new()
        if ($c -eq [ErrorCode]::FileNotExist) { return $r }
        $r._exist = $true

        if ($c -eq [ErrorCode]::InvalidJson) { return $r }
        $r.isValidJson = $true

        $r.hasFStarExe = ($null -ne $c.fstar_exe)
        $r.hasOptions = ($null -ne $c.options)
        $r.hasIncludeDirs = ($null -ne $c.include_dirs)
        $r.fstarExePath = if ($r.hasFStarExe) {$c.fstar_exe} else {""}
        
        # parse .fst.config.json option
        function Get-Z3Path {
            [int]$smtPosition = -1
            for ($i = 0; $i -lt $c.options.Count; $i++) {
                $curOption = $c.options[$i]
                if ($smtPosition -eq -1) {
                    if ($curOption -eq '--smt') {
                        $smtPosition = $i
                    }
                } elseif ($smtPosition -eq ($i-1)) {
                    return $curOption
                }
            }
            return ""
        }
        $r.z3ExePath = (Get-Z3Path)

        $r.includeDirsForRootConfig = Remove-EqualAsRootFullPath $c.include_dirs $desired.includeDirsForRootConfig -Not
        if ($null -eq $r.includeDirsForRootConfig) {$r.includeDirsForRootConfig = @()}

        $r.includeDirsForFStarProject = Remove-EqualAsRootFullPath $c.include_dirs $desired.includeDirsForFStarProject -Not
        if ($null -eq $r.includeDirsForFStarProject) {$r.includeDirsForFStarProject = @()}
        
        $r.includeDirsUnspecified = Remove-EqualAsRootFullPath $c.include_dirs ($r.includeDirsForRootConfig + $r.includeDirsForFStarProject)
        if ($null -eq $r.includeDirsUnspecified) {$r.includeDirsUnspecified = @()}
        
        return $r
    }

    static [Dictionary[string,Func[psobject,psobject,bool]]] Tester() {
        $r = [Dictionary[string,Func[psobject,psobject,bool]]]::new()

        $r.Add('fstarExePath', { param($d, $a); return Compare-Path (Get-Root) $d $a })
        $r.Add('z3ExePath', { param($d, $a);
            if ([string]::IsNullOrWhiteSpace($d)) { return $true }
            return Compare-Path (Get-Root) $d $a
        })

        $r.Add('includeDirsForRootConfig', { param($d, $a); return Compare-SetEqual (Get-RootFullPathes $d) ((Get-RootFullPathes $a)) })
        $r.Add('includeDirsForFStarProject', { param($d, $a); return Compare-SetEqual (Get-RootFullPathes $d) ((Get-RootFullPathes $a)) })
        $r.Add('includeDirsUnspecified', { param($d, $a); return Compare-SetEqual (Get-RootFullPathes $d) ((Get-RootFullPathes $a)) })

        return $r
    }
}
function Get-DesiredFstConfigState { [FstConfigState]::Desired((Get-CurrentRootConfig)) }
function Get-CurrentFstConfigState { [FstConfigState]::Current((Get-CurrentRootConfig)) }
function Get-FstConfigStateTester { [FstConfigState]::Tester() }

function Test-DesiredFStarConfigStateValidity {
    param ([FstConfigState]$InputObject, [ref]$Diagnostics)
    function Format-Message ([string]$Msg) { "Not supproted: $Msg = false" }

    $des = $InputObject
    $Diagnostics = @()
    
    if ($des._exist -eq $false) { $Diagnostics += Format-Message('_exist') }
    if ($des.isValidJson -eq $false) { $Diagnostics += Format-Message('isValidJson') }
    if ($des.hasFStarExe -eq $false) { $Diagnostics += Format-Message('hasFStarExe') }
    if ($des.hasOptions -eq $false) { $Diagnostics += Format-Message('hasOptions') }
    if ($des.hasIncludeDirs -eq $false) { $Diagnostics += Format-Message('hasIncludeDirs') }

    #--- fstarExePath ---
    if (-not (Test-Version (Get-RootFullPath $des.fstarExePath))) {  
        $Diagnostics += "Fstar exe path is invalid: fstarExePath = $($des.fstarExePath)"
    }
    #---|

    #--- z3ExePath ---
    if (-not (Test-Version (Get-RootFullPath $des.z3ExePath))) {  
        $Diagnostics += "Z3 exe path is invalid: z3ExePath = $($des.fstarExePath)"
    }
    #---|

    #--- dirs ---
    function Test-Dirs { param ([string]$MemberName)
        $des.$MemberName 
            | Where-Object {Test-RootFullPath $_ -PathType 'Container'}
            | ForEach-Object {$Diagnostics += "Directory is not exist in ${MemberName}: Item = $_"}
    }

    Test-Dirs 'includeDirsForRootConfig'
    Test-Dirs 'includeDirsForFStarProject'
    Test-Dirs 'includeDirsUnspecified'
    #---|

    return $Diagnostics.Count -eq 0
}


function Set-FStarConfig { param ([switch]$TestOnly, [switch]$PassThru, [switch]$Silent)
    $meta = Get-Meta
    $path = Join-Path (Get-Root) $meta.FstConfig
    $des = Get-DesiredFstConfigState
    $prev = Get-CurrentFstConfigState
    $tester = Get-FstConfigStateTester

    #--- Test ---
    if (-not $Silent) { Write-HostWithTime "Test $($meta.FstConfig)" }

    $testOut = Invoke-DscTest $des $prev $tester

    if (-not $Silent) { ConvertTo-Json $testOut | Write-Host }
    if ($PassThru) { $testOut }
    #---|

    if ($TestOnly) { return }
    if ($testOut.inDesiredState) { return }

    #--- Validate desired state ---
    if (-not (Test-DesiredFStarConfigStateValidity $des $dg)) {
        $dg | Write-Error
        exit 1
    }
    #---|

    #--- Set ---
    if (-not $Silent) { Write-HostWithTime "Set $($meta.FstConfig)" }
    #TODO
    #---|
}

class WorkSpace {
    [string]$rootConfig = ""
    [string]$fstarConfig = ""
}

class WorkSpaceState {
    [bool]$isRootConfigDesired = $false
    [bool]$isFStarConfigDesired = $false
}
