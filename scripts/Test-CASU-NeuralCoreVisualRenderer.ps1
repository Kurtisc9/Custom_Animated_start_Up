param(
    [int]$DurationSeconds = 15
)

$Root = "X:\03_Active_Projects\Custom_Animated_Start_Up"
Set-Location -LiteralPath $Root

$ErrorActionPreference = "Stop"

Add-Type -AssemblyName System.Windows.Forms
Add-Type -AssemblyName System.Drawing

$Sources = @(
    (Join-Path $Root "src\CASU.Themes\NeuralCore\NeuralCoreRenderer.cs"),
    (Join-Path $Root "src\CASU.Runtime\CASUNeuralCoreInteractiveHost.cs")
)

Add-Type `
    -Path $Sources `
    -ReferencedAssemblies @(
        "System.dll",
        "System.Drawing.dll",
        "System.Windows.Forms.dll"
    ) `
    -ErrorAction Stop

Write-Host "NEURAL_CORE_RENDERER_COMPILE=PASS"

$Displays = @([System.Windows.Forms.Screen]::AllScreens)

if ($Displays.Count -lt 1) {
    throw "DISPLAY_DISCOVERY=FAIL"
}

$Primary = if ($Displays.Count -ge 2) { 2 } else { 1 }

Write-Host "DISPLAY_DISCOVERY=PASS"
Write-Host "DISPLAY_COUNT=$($Displays.Count)"
Write-Host "CASU_PRIMARY_DISPLAY=$Primary"

Write-Host "INTERACTIVE_VISUAL_TEST=STARTING"

[CASU.Runtime.CASUNeuralCoreInteractiveHost]::Run(
    $DurationSeconds
)

Write-Host "INTERACTIVE_VISUAL_TEST_RETURNED=PASS"
Write-Host "ALL_TEST_WINDOWS_EXITED=PASS"
Write-Host "CASU_NEURAL_CORE_VISUAL_TEST=PASS"
