using namespace System.Linq

. $PSScriptRoot/Meta.ps1

function ConvertTo-TypedObject {
    [CmdletBinding()]
    param (
        [Parameter(Mandatory, Position=0)]
        [string] $TypeName,

        [Parameter(Mandatory, Position=1, ValueFromPipeline)]
        [psobject] $InputObject
    )

    $r = New-Object -TypeName $TypeName
    $findingMemberNames = Get-Member -InputObject $r -MemberType 'Property' | ForEach-Object {$_.Name}
    #$foundMemberNames = Get-Member -InputObject $InputObject -MemberType 'Property' -Name $findingMemberNames | ForEach-Object {$_.Name}
    foreach ($memberName in $findingMemberNames) {
        if ($null -ne $InputObject.$memberName) {
            $r.$memberName = $InputObject.$memberName
        }
    }   
    return $r
}

function ConvertTo-HashTable {
    [CmdletBinding()]
    param (
        [Parameter(Mandatory, Position=0, ValueFromPipeline)]
        [psobject] $InputObject
    )

    $r = @{}
    $memberNames = Get-Member -InputObject $InputObject -MemberType 'Property' | ForEach-Object {$_.Name}
    foreach ($memberName in $memberNames) {
        $r.$memberName = $InputObject.$memberName
    }   
    return $r
}

function Get-FullPath ([string]$Path, [string]$BasePath) {
    if ([System.IO.Path]::IsPathFullyQualified($Path)) { 
        return [System.IO.Path]::GetFullPath($Path)
    } else {
        return [System.IO.Path]::GetFullPath($Path, $BasePath)
    }
}

function Compare-Path ([string]$BasePath, [string] $Path1, [string] $Path2) {
    $fullPath1 = Get-FullPath $Path1 $BasePath
    $fullPath2 = Get-FullPath $Path2 $BasePath
    return ($fullPath1 -eq $fullPath2)
}

function Write-HostWithTime([string] $Text) {
    Write-Host "[$(Get-Date -Format 'yyyy-MM-ddTHH:mm:ssz')] $Text"
}

function Compare-SetEqual { param ([array] $l, [array] $r)
    if (($null -eq $l)) { $l = @() }
    if (($null -eq $r)) { $r = @() }
    $lset = [Enumerable]::ToHashSet[object]([Enumerable]::Cast[object]($l))
    return $lset.SetEquals([Enumerable]::Cast[object]($r))
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

function Test-Version ([string]$Path) {
    (& $Path --version 2>&1) | Write-Debug
    return $LASTEXITCODE -ne 0
}

function Test-RootFullPath {
    [CmdletBinding()]
    param (
        [Parameter(Mandatory, Position=0, ValueFromPipeline)]
        [string]$Path, 
        [Microsoft.PowerShell.Commands.TestPathType]$PathType = 'Any'
    )

    $fullPath = Get-RootFullPath $Path
    Test-Path -LiteralPath $fullPath -PathType $PathType
}