$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$Root = "X:\03_Active_Projects\Custom_Animated_Start_Up"

Set-Location -LiteralPath $Root

$Source =
    Join-Path $Root "src\CASU.Runtime\CASUPresentationHost.cs"

if (-not (Test-Path -LiteralPath $Source)) {
    throw "PRESENTATION_SOURCE_MISSING"
}

Add-Type `
    -Path $Source `
    -ReferencedAssemblies @(
        "System.Windows.Forms",
        "System.Drawing"
    )

Write-Host "CASU_PRESENTATION_COMPILE=PASS"

$Screens =
    [System.Windows.Forms.Screen]::AllScreens

Write-Host "DISPLAY_COUNT=$($Screens.Count)"

for ($i = 0; $i -lt $Screens.Count; $i++) {
    Write-Host (
        "DISPLAY_{0}=Bounds:{1};Primary:{2}" -f
        ($i + 1),
        $Screens[$i].Bounds,
        $Screens[$i].Primary
    )
}

$Form =
    New-Object CASU.Runtime.CASUPresentationHost

Write-Host "CASU_PRESENTATION_RUNTIME=STARTING"
Write-Host "ESCAPE_KEY=CONTROLLED_TEST_EXIT"

[System.Windows.Forms.Application]::Run($Form)

Write-Host "CASU_PRESENTATION_RUNTIME=EXITED"
Write-Host "CASU_PRESENTATION_RUNTIME_TEST=PASS"
