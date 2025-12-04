
class WorkSpace {
    [string]$rootConfig = ""
    [string]$fstarConfig = ""
}

class WorkSpaceState {
    [bool]$isRootConfigDesired = $false
    [bool]$isFStarConfigDesired = $false
}