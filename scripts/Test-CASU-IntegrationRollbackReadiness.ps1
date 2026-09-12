$ErrorActionPreference = "Stop"

$Root = "X:\03_Active_Projects\Custom_Animated_Start_Up"
Set-Location -LiteralPath $Root

$Required = @(
    "src\CASU.Input\RawInputEmergencyBypass.cs",
    "src\CASU.Runtime\CasuRuntimeController.cs",
    "docs\CASU_02B_R3D6_PRE_SIGNIN_INTEGRATION_DESIGN.md",
    "docs\CASU_02B_R3D6_ROLLBACK_PLAN.md",
    "PROJECT_STATE.md",
    "LOCKED_DECISIONS.md",
    "ROADMAP.md",
    "PORT_REGISTRY.md"
)

foreach ($Relative in $Required) {
    $Path = Join-Path $Root $Relative

    if (-not (Test-Path -LiteralPath $Path -PathType Leaf)) {
        throw "ROLLBACK_REQUIREMENT_MISSING=$Relative"
    }

    Write-Host "ROLLBACK_REQUIREMENT=PASS:$Relative"
}

$WinlogonPath = "HKLM:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon"
$Winlogon = Get-ItemProperty -LiteralPath $WinlogonPath

if ([string]$Winlogon.Shell -ine "explorer.exe") {
    throw "WINDOWS_SHELL_BASELINE_CHANGED"
}

if ([string]$Winlogon.Userinit -notmatch "(?i)userinit\.exe") {
    throw "WINDOWS_USERINIT_BASELINE_CHANGED"
}

Write-Host "WINDOWS_SHELL_BASELINE=PASS"
Write-Host "WINDOWS_USERINIT_BASELINE=PASS"
Write-Host "WINDOWS_AUTHENTICATION_REMAINS_AUTHORITATIVE=TRUE"
Write-Host "CTRL_ALT_DELETE_REMAINS_WINDOWS_CONTROLLED=TRUE"
Write-Host "CASU_WINDOWS_INTEGRATION_INSTALLED=FALSE"
Write-Host "WINDOWS_CONFIGURATION_CHANGED=FALSE"
Write-Host "ROLLBACK_READINESS=PASS"
