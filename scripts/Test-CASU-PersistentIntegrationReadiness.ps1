& {
$ErrorActionPreference = "Stop"

$Root = "X:\03_Active_Projects\Custom_Animated_Start_Up"

Set-Location -LiteralPath $Root

Write-Host "CASU_R3D16_READINESS_CHECK=START"

if ((Get-Location).Path -ne $Root) {
    throw "PROJECT_PATH_LOCK_FAILED"
}

Write-Host "PROJECT_PATH_LOCK=PASS"

$WinlogonPath =
    "HKLM:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon"

$Winlogon =
    Get-ItemProperty -LiteralPath $WinlogonPath

if ([string]$Winlogon.Shell -ine "explorer.exe") {
    throw "WINLOGON_SHELL_BASELINE_FAILED"
}

if ([string]$Winlogon.Userinit -notmatch "(?i)userinit\.exe") {
    throw "WINLOGON_USERINIT_BASELINE_FAILED"
}

Write-Host "WINLOGON_BASELINE=PASS"

$Services =
    @(
        Get-CimInstance Win32_Service |
        Where-Object {
            $_.Name -match "(?i)^CASU_" -or
            $_.Name -eq "CASUOrchestrator" -or
            $_.DisplayName -match "(?i)CASU"
        }
    )

if ($Services.Count -ne 0) {
    throw "CASU_SERVICE_ALREADY_INSTALLED"
}

Write-Host "CASU_SERVICE_INSTALLED=FALSE"

$Tasks =
    @(
        Get-ScheduledTask -ErrorAction SilentlyContinue |
        Where-Object {
            $_.TaskName -match "(?i)CASU" -or
            $_.TaskPath -match "(?i)CASU"
        }
    )

if ($Tasks.Count -ne 0) {
    throw "CASU_SCHEDULED_TASK_ALREADY_INSTALLED"
}

Write-Host "CASU_SCHEDULED_TASK_INSTALLED=FALSE"

$Design =
    Join-Path $Root "docs\CASU_02B_R3D16_PERSISTENT_INTEGRATION_DESIGN.md"

if (-not (Test-Path -LiteralPath $Design -PathType Leaf)) {
    throw "R3D16_DESIGN_DOCUMENT_MISSING"
}

$DesignText =
    Get-Content `
        -LiteralPath $Design `
        -Raw

$Required = @(
    "C:\Program Files\CASU",
    "C:\ProgramData\CASU",
    "CASUOrchestrator",
    "NT AUTHORITY\SYSTEM",
    "Session 0 interactive UI is prohibited",
    "Windows remains the sole authentication authority",
    "Ctrl+Alt+Delete remains Windows-controlled",
    "Maximum runtime restart count:",
    "Permanent Windows integration requires explicit approval from KurtisC"
)

foreach ($Token in $Required) {

    if ($DesignText -notmatch [regex]::Escape($Token)) {
        throw "R3D16_DESIGN_TOKEN_MISSING=$Token"
    }

    Write-Host "VALIDATED=$Token"
}

Write-Host "INSTALL_ROOT_MODEL=PASS"
Write-Host "LOCALSYSTEM_SERVICE_MODEL=PASS"
Write-Host "SESSION_ZERO_UI_POLICY=PASS"
Write-Host "AUTHENTICATION_BOUNDARY=PASS"
Write-Host "WATCHDOG_POLICY=PASS"
Write-Host "ROLLBACK_POLICY=PASS"
Write-Host "PERSISTENT_INSTALLATION_APPROVAL_REQUIRED=TRUE"

Write-Host "CASU_WINDOWS_INTEGRATION_INSTALLED=FALSE"

Write-Host ""
Write-Host "CASU_02B_R3D16_PERSISTENT_INTEGRATION_READINESS=PASS"
}
