
. $PSScriptRoot/Meta.ps1
. $PSScriptRoot/Common.ps1
. $PSScriptRoot/Dsc.ps1
. $PSScriptRoot/RootConfig.ps1
. $PSScriptRoot/WorkSpace.ps1

function Test-Windows {
    [OutputType([bool])] param()
    
    ([System.Environment]::OSVersion.Platform -eq [System.PlatformID]::Win32NT)
}

function Format-Exe {
    [OutputType([string])]
    param ([string] $ExePath)
    
    if (Test-Windows) {
        [System.IO.Path]::ChangeExtension($ExePath, '.exe')
    } else {
        [System.IO.Path]::ChangeExtension($ExePath, $null)
    }
}
