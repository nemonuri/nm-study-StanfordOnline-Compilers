
function Get-Meta {
    Import-PowerShellDataFile "$PSScriptRoot/data/meta.psd1"
}

function Get-Root {
    Join-Path $PSScriptRoot "../.." -Resolve
}
