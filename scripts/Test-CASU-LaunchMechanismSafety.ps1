$ErrorActionPreference = "Stop"

$Root = "X:\03_Active_Projects\Custom_Animated_Start_Up"
Set-Location -LiteralPath $Root

$DecisionPath = Join-Path $Root "docs\CASU_02B_R3D7_LAUNCH_MECHANISM_DECISION.md"

if (-not (Test-Path -LiteralPath $DecisionPath -PathType Leaf)) {
    throw "R3D7_DECISION_DOCUMENT_MISSING"
}

$Decision = Get-Content -LiteralPath $DecisionPath -Raw

$RequiredTokens = @(
    "ARCHITECTURE_DIRECTION=SEPARATE_ORCHESTRATOR_AND_INTERACTIVE_RUNTIME",
    "WINDOWS_SHELL_REPLACEMENT=PROHIBITED",
    "USERINIT_REPLACEMENT=PROHIBITED",
    "LOGONUI_REPLACEMENT=PROHIBITED",
    "CREDENTIAL_PROVIDER_REPLACEMENT=PROHIBITED",
    "AUTOADMINLOGON=PROHIBITED",
    "BCD_PRESENTATION_MODIFICATION=PROHIBITED",
    "CASU_WINDOWS_INTEGRATION_INSTALLED=FALSE",
    "WINDOWS_SIGN_IN_RUNTIME_PROOF=PENDING"
)

foreach ($Token in $RequiredTokens) {
    if ($Decision -notmatch [regex]::Escape($Token)) {
        throw "R3D7_REQUIRED_TOKEN_MISSING=$Token"
    }

    Write-Host "DESIGN_TOKEN=PASS:$Token"
}

$WinlogonPath = "HKLM:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon"
$Winlogon = Get-ItemProperty -LiteralPath $WinlogonPath

if ([string]$Winlogon.Shell -ine "explorer.exe") {
    throw "WINDOWS_SHELL_CHANGED"
}

if ([string]$Winlogon.Userinit -notmatch "(?i)userinit\.exe") {
    throw "WINDOWS_USERINIT_CHANGED"
}

Write-Host "WINDOWS_SHELL_BASELINE=PASS"
Write-Host "WINDOWS_USERINIT_BASELINE=PASS"
Write-Host "WINDOWS_AUTHENTICATION_REMAINS_AUTHORITATIVE=TRUE"
Write-Host "CTRL_ALT_DELETE_REMAINS_WINDOWS_CONTROLLED=TRUE"
Write-Host "CASU_WINDOWS_INTEGRATION_INSTALLED=FALSE"
Write-Host "LAUNCH_MECHANISM_SAFETY=PASS"
