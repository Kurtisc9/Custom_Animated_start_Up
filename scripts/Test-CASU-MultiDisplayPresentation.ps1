$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$Root = "X:\03_Active_Projects\Custom_Animated_Start_Up"

Set-Location -LiteralPath $Root

$Source =
    Join-Path $Root `
        "src\CASU.Runtime\CASUMultiDisplayPresentationHost.cs"

if (-not (Test-Path -LiteralPath $Source)) {
    throw "MULTI_DISPLAY_SOURCE_MISSING"
}

Add-Type `
    -Path $Source `
    -ReferencedAssemblies @(
        "System.Windows.Forms",
        "System.Drawing"
    )

Write-Host "CASU_MULTI_DISPLAY_COMPILE=PASS"

$Screens =
    [System.Windows.Forms.Screen]::AllScreens

if ($Screens.Count -eq 0) {
    throw "NO_DISPLAYS_DETECTED"
}

Write-Host "DISPLAY_COUNT=$($Screens.Count)"

for ($i = 0; $i -lt $Screens.Count; $i++) {

    if (
        ($Screens.Count -ge 2 -and $i -eq 1) -or
        ($Screens.Count -eq 1 -and $i -eq 0)
    ) {
        $Role = "CASU_PRIMARY"
    }
    else {
        $Role = "CASU_SECONDARY"
    }

    Write-Host (
        "DISPLAY_{0}=Bounds:{1};WindowsPrimary:{2};CASURole:{3}" -f
        ($i + 1),
        $Screens[$i].Bounds,
        $Screens[$i].Primary,
        $Role
    )
}

Write-Host "ALL_DISPLAYS_INITIALIZATION=STARTING"
Write-Host "ESCAPE_KEY=CONTROLLED_EXIT_ALL_DISPLAYS"

$Context =
    New-Object `
        CASU.Runtime.CASUMultiDisplayContext

[System.Windows.Forms.Application]::Run(
    $Context
)

Write-Host "ALL_DISPLAY_WINDOWS_EXITED=PASS"
Write-Host "CASU_MULTI_DISPLAY_RUNTIME_TEST=PASS"
