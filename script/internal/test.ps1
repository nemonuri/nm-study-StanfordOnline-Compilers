Invoke-Command {
    . $PSScriptRoot/Dsc.ps1
    . $PSScriptRoot/WorkSpace.ps1

    $ErrorActionPreference = 'Break'
    Set-StrictMode -Off

    Invoke-DscTest (Get-DesiredFstConfigState) (Get-CurrentFstConfigState) (Get-FstConfigStateTester) | ConvertTo-Json | Write-Host
}