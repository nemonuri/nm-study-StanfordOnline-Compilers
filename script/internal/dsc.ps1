using namespace System
using namespace System.Collections.Generic

# https://raw.githubusercontent.com/PowerShell/DSC/main/schemas/v3.1.0/outputs/resource/test.simple.json
class TestSimpleOutput {
    [psobject] $desiredState
    [psobject] $actualState
    [bool] $inDesiredState
    [string[]] $differingProperties = @();
}

function Invoke-DscTest {
    [OutputType([TestSimpleOutput])]
    param ([psobject] $Desired, [psobject] $Actual, [Dictionary[string,Func[psobject,psobject,bool]]] $Testers = $null)

    $names = $Desired | Get-Member -MemberType Property | ForEach-Object { $_.Name }

    function Get-Tester { [OutputType([Func[psobject,psobject,bool]])] param([string] $MemberName)
        if ($null -eq $Testers) { return $null }
        if (-not $Testers.ContainsKey($MemberName)) {return $null}
        return $Testers[$MemberName]
    }

    [string[]]$diffs = @()
    foreach ($name in $names) {
        [bool]$passed = $false
        [Func[psobject,psobject,bool]]$tester = Get-Tester $name
        if ($null -eq $tester) { $passed = ($Desired.$name -eq $Actual.$name) }
        else { $passed = $tester.Invoke($Desired.$name, $Actual.$name) }

        if (-not $passed) {
            $diffs += $name
        }
    }

    return [TestSimpleOutput]@{
        desiredState = $Desired
        actualState = $Actual
        inDesiredState = ($diffs.Count -eq 0)
        differingProperties = $diffs
    }
}

# https://raw.githubusercontent.com/PowerShell/DSC/main/schemas/v3.1.0/outputs/resource/set.simple.json
class SetSimpleOutput {
    [psobject] $beforeState
    [psobject] $afterState
    [string[]] $changedProperties = @();
}

function New-DscSetOutput {
    [OutputType([SetSimpleOutput])]
    param ([psobject] $Before, [psobject] $After)

    $names = $Before | Get-Member -MemberType Property | ForEach-Object { $_.Name }

    [string[]]$changes = @()
    foreach ($name in $names) {
        if ($Desired.$name -ne $Actual.$name) {
            $changes += $name
        }
    }

    return [SetSimpleOutput]@{
        beforeState = $Before
        afterState = $After
        changedProperties = $changes
    }
}

function Test-TestSimpleOutput {
    [OutputType([bool])] param ([psobject]$InputObject)

    if ($null -eq $InputObject) { return $false }
    if ($InputObject -is [TestSimpleOutput]) { return $InputObject.inDesiredState }
    if ($InputObject -is [array]) {
        foreach ($item in $InputObject) {
            if ($InputObject -is [TestSimpleOutput]) { return $InputObject.inDesiredState }
        }
    }
    return $false
}