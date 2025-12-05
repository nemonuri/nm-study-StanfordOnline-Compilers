using module './ErrorCode.psm1'
using namespace System
using namespace System.Collections.Generic
using namespace System.Linq

. $PSScriptRoot/Common.ps1
. $PSScriptRoot/Meta.ps1
. $PSScriptRoot/RootConfig.ps1

class FstConfig {
    [string] $fstar_exe = ""
    [string[]] $options = @()
    [string[]] $include_dirs = @()

    static [psobject] Current() {
        $meta = Get-Meta
        $path = Join-Path (Get-Root) $meta.RootConfig

        if (-not (Test-Path $path -PathType Leaf)) {
            return [ErrorCode]::FileNotExist
        }

        if (-not (Test-Json -Path $path)) {
            return [ErrorCode]::InvalidJson
        }

        return [FstConfig](Get-Content $path | ConvertFrom-Json | ConvertTo-TypedObject 'FstConfig')
    }
}

function Get-FStarProjectFiles { [OutputType([IEnumerable[IO.FileInfo]])] param ()
    $meta = Get-Meta
    $root = Get-Root
    $src = Join-Path $root $meta.Src -Resolve
    return [System.IO.Directory]::EnumerateFiles($src, $meta.FStarProject, 'AllDirectories')
}

function Compare-SetEqual { param ([string[]] $l, [string[]] $r)
    if (($null -eq $l) -or ($null -eq $r)) {return $false}
    $lset = [Enumerable]::ToHashSet[string]($l)
    return $lset.SetEquals($r)
}

function Get-RootFullPath { param ([string] $Path)
    return Get-FullPath $Path (Get-Root)
}

function Get-RootFullPathes { param ([string[]] $Pathes)
    return $Pathes | ForEach-Object {Get-RootFullPath $_}
}

function Remove-EqualAsRootFullPath {
    param ([string[]] $SourcePathes, [string[]] $ComparandPathes, [switch] $Not)
    
    $pathes = Get-RootFullPathes $ComparandPathes
    if ($Not) {
        return $SourcePathes | Where-Object { $pathes -contains (Get-RootFullPath $_) }
    } else {
        return $SourcePathes | Where-Object { $pathes -notcontains (Get-RootFullPath $_) }
    }
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
        $r.includeDirsForFStarProject = Get-FStarProjectFiles
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
                    if ($curOption -eq '-smt') {
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
        $r.includeDirsForFStarProject = Remove-EqualAsRootFullPath $c.include_dirs $desired.includeDirsForFStarProject -Not
        $r.includeDirsUnspecified = Remove-EqualAsRootFullPath $c.include_dirs ($r.includeDirsForRootConfig + $r.includeDirsForFStarProject)

        <#
        $r.isIncludeDirsRootConfigDesired = [Enumerable]::All[string]($RootConfig.fstarLibs,
            [Func[string,bool]] { param($sr); return [Enumerable]::Any[string]($c.include_dirs, 
                [Func[string,bool]] { param($sf); return Compare-Path $root $sr $sf } )
            } ) # ∀sr∈'$RootConfig.fstarLibs'.∃sf∈'$c.include_dirs'.'Compare-Path $root'(sr, sf)
        
        $r.isIncludeDirsFStarConfigDesired
        #>
        
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


class WorkSpace {
    [string]$rootConfig = ""
    [string]$fstarConfig = ""
}

class WorkSpaceState {
    [bool]$isRootConfigDesired = $false
    [bool]$isFStarConfigDesired = $false
}
