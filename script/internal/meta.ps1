
function Get-Meta {
    Import-PowerShellDataFile "$PSScriptRoot/data/Meta.psd1"
}

function Get-Root {
    Join-Path $PSScriptRoot "../.." -Resolve
}
