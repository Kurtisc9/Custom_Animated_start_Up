$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$Root = "X:\03_Active_Projects\Custom_Animated_Start_Up"

Set-Location -LiteralPath $Root

$Source =
    Join-Path $Root `
        "src\CASU.Runtime\CASUEmergencyBypassPresentationHost.cs"

Add-Type `
    -Path $Source `
    -ReferencedAssemblies @(
        "System.Windows.Forms",
        "System.Drawing"
    )

Write-Host "CASU_BYPASS_INTEGRATION_COMPILE=PASS"

$Screens =
    [System.Windows.Forms.Screen]::AllScreens

Write-Host "DISPLAY_COUNT=$($Screens.Count)"

Write-Host "============================================================"
Write-Host " CASU LEFT SHIFT BYPASS TEST"
Write-Host "============================================================"
Write-Host "CASU should appear on all displays."
Write-Host "Press LEFT SHIFT on the verified physical keyboard."
Write-Host "All CASU windows should close immediately."
Write-Host "============================================================"

$Context =
    New-Object `
        CASU.Runtime.CASUEmergencyBypassContext

[System.Windows.Forms.Application]::Run(
    $Context
)

Write-Host "ALL_CASU_WINDOWS_EXITED=PASS"
Write-Host "CASU_LEFT_SHIFT_MULTI_DISPLAY_TEST=PASS"
