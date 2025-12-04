
. $PSScriptRoot/common.ps1
. $PSScriptRoot/meta.ps1

enum RootConfigErrorCode : int {
    None = 0
    FileNotExist = 1
    InvalidJson = 2
}

class RootConfig {
    [string]$fstarExe = ""
    [string]$z3Exe = ""
    [string[]]$fstarLibs = @()

    static [psobject] Current() {
        $meta = Get-Meta
        $path = Join-Path (Get-Root) $meta.RootConfig

        if (-not (Test-Path $path -PathType Leaf)) {
            return [RootConfigErrorCode]::FileNotExist
        }

        if (-not (Test-Json -Path $path)) {
            return [RootConfigErrorCode]::InvalidJson
        }

        return [RootConfig](Get-Content $Path | ConvertFrom-Json | ConvertTo-TypedObject 'RootConfig')
    }
}

class RootConfigState {
    [bool] $_exist = $false
    [bool] $isValidJson = $false
    [bool] $hasFStarExe = $false
    [bool] $hasZ3Exe = $false
    [bool] $hasFStarLibs = $false

    static [RootConfigState] Desired() {
        return [RootConfigState]@{
            _exist = $true
            isValidJson = $true
            hasFStarExe = $true
            hasZ3Exe = $true
            hasFStarLibs = $true
        }
    }

    static [RootConfigState] Current() {
        $rc = [RootConfig]::Current()

        $r = [RootConfigState]::new()
        if ($rc -eq [RootConfigErrorCode]::FileNotExist) { return $r }
        $r._exist = $true

        if ($rc -eq [RootConfigErrorCode]::InvalidJson) { return $r }
        $r.isValidJson = $true

        $r.hasFStarExe = ($null -ne $rc.fstarExe)
        $r.hasZ3Exe = ($null -ne $rc.z3Exe)
        $r.hasFStarLibs = ($null -ne $rc.fstarLibs)
        return $r
    }
}

function New-RootConfigComment {
    @{
        fstarExe = "Path for F* binary. F* download link: https://github.com/FStarLang/FStar/releases"
        z3Exe = "Path for Z3 binary."
        fstarLibs = "Pathes for default F* libraries."
    }
}

function Set-DesiredRootConfigState {
    $meta = Get-Meta
    $path = Join-Path (Get-Root) $meta.RootConfig
    
    $des = [RootConfigState]::Desired()
    $cur = [RootConfigState]::Current()

    if (($des._exist -ne $cur._exist) -or ($des.isValidJson -ne $cur.isValidJson)) {
        if ($des._exist -eq $false) { throw "Not supported." }
        if ($des.isValidJson -eq $false) { throw "Not supported." }

        $newRc = [RootConfig]::new() | ConvertTo-HashTable
        $newRc._comment = (New-RootConfigComment)
        ConvertTo-Json $newRc | Out-File $path

        $cur._exist = $true
        $cur.isValidJson = $true
    }

    $rc = Get-Content $path | ConvertFrom-Json -AsHashtable

    if ($des.hasFStarExe -ne $cur.hasFStarExe) {
        if ($des.hasFStarExe -eq $false) { throw "Not supported." }
        $rc.fstarExe = ""
        $cur.hasFStarExe = $true
    }

    if ($des.hasZ3Exe -ne $cur.hasZ3Exe) {
        if ($des.hasZ3Exe -eq $false) { throw "Not supported." }
        $rc.z3Exe = ""
        $cur.hasZ3Exe = $true
    }

    if ($des.hasFStarLibs -ne $cur.hasFStarLibs) {
        if ($des.hasFStarLibs -eq $false) { throw "Not supported." }
        $rc.fstarLibs = @()
        $cur.hasFStarLibs = $true
    }

    $rc | ConvertTo-Json | Out-File $path

    return $cur
}
