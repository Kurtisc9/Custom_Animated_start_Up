$ErrorActionPreference = "Stop"

$Root = "X:\03_Active_Projects\Custom_Animated_Start_Up"
Set-Location -LiteralPath $Root

$ServiceName = "CASUOrchestrator"

Write-Host "=============================================" -ForegroundColor Yellow
Write-Host " CASU EMERGENCY SERVICE DISABLE" -ForegroundColor Yellow
Write-Host "=============================================" -ForegroundColor Yellow

$Principal =
    New-Object Security.Principal.WindowsPrincipal(
        [Security.Principal.WindowsIdentity]::GetCurrent()
    )

if (
    -not $Principal.IsInRole(
        [Security.Principal.WindowsBuiltInRole]::Administrator
    )
) {
    throw "ADMINISTRATOR_REQUIRED"
}

$Service =
    Get-Service `
        -Name $ServiceName `
        -ErrorAction SilentlyContinue

if (-not $Service) {

    Write-Host "CASU_SERVICE_PRESENT=FALSE"
    Write-Host "RECOVERY_ACTION_REQUIRED=FALSE"
    exit 0
}

if ($Service.Status -ne "Stopped") {

    Stop-Service `
        -Name $ServiceName `
        -Force

    $Service.WaitForStatus(
        "Stopped",
        [TimeSpan]::FromSeconds(15)
    )
}

Set-Service `
    -Name $ServiceName `
    -StartupType Disabled

$Final =
    Get-Service `
        -Name $ServiceName `
        -ErrorAction Stop

$Cim =
    Get-CimInstance Win32_Service `
        -Filter "Name='$ServiceName'"

if ($Final.Status -ne "Stopped") {
    throw "RECOVERY_SERVICE_STOP_FAILED"
}

if ($Cim.StartMode -ne "Disabled") {
    throw "RECOVERY_SERVICE_DISABLE_FAILED"
}

Write-Host "CASU_SERVICE_STATUS=STOPPED"
Write-Host "CASU_SERVICE_START_MODE=DISABLED"
Write-Host "WINDOWS_AUTHENTICATION_UNCHANGED=TRUE"
Write-Host "CASU_EMERGENCY_DISABLE=PASS" -ForegroundColor Green
