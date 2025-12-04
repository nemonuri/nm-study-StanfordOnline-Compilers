
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
    $foundMemberNames = Get-Member -InputObject $InputObject -MemberType 'Property' -Name $findingMemberNames | ForEach-Object {$_.Name}
    foreach ($memberName in $foundMemberNames) {
        $r.$memberName = $InputObject.$memberName
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