
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