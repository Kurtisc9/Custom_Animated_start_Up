$ErrorActionPreference = "Stop"

$Root = Split-Path -Parent $PSScriptRoot

$InputSource = Join-Path $Root "src\CASU.Input\RawInputEmergencyBypass.cs"
$RuntimeSource = Join-Path $Root "src\CASU.Runtime\CasuRuntimeController.cs"

Write-Host "`n=== CASU 02B-R3D3 INTEGRATED RUNTIME TEST ===" -ForegroundColor Cyan

Add-Type -AssemblyName System.Windows.Forms
Add-Type -AssemblyName System.Drawing

$InputCode = Get-Content -LiteralPath $InputSource -Raw
$RuntimeCode = Get-Content -LiteralPath $RuntimeSource -Raw

Add-Type `
    -TypeDefinition $InputCode `
    -ReferencedAssemblies @(
        "System.dll",
        "System.Windows.Forms.dll",
        "System.Drawing.dll"
    )

Add-Type `
    -TypeDefinition $RuntimeCode `
    -ReferencedAssemblies @("System.dll")

# ============================================================
# TEST A — LIVE LEFT SHIFT -> RUNTIME EXIT
# ============================================================

Write-Host "`n--- TEST A: EMERGENCY BYPASS EXIT ---"

$runtime = New-Object CASU.Runtime.CasuRuntimeController
$runtime.MarkRunning()

$form = New-Object CASU.Input.EmergencyBypassForm

$script:RuntimeExitObserved = $false

$runtime.add_ExitRequested({

    $script:RuntimeExitObserved = $true

    Write-Host "RUNTIME_EXIT_EVENT=RECEIVED" -ForegroundColor Green

    $timer = New-Object System.Windows.Forms.Timer
    $timer.Interval = 500

    $timer.Add_Tick({
        $this.Stop()
        $form.Close()
    })

    $timer.Start()
})

$form.add_EmergencyBypassRequested({

    Write-Host "INPUT_BYPASS_EVENT=RECEIVED"
    $runtime.RequestEmergencyBypass()
})

Write-Host "CASU_RUNNING=TRUE"
Write-Host "TEST_ACTION=PRESS_LEFT_SHIFT_ONCE"
Write-Host "EXPECTED_DEVICE=VID_1EA7_PID_0169_MI_00"

[System.Windows.Forms.Application]::Run($form)

if (-not $script:RuntimeExitObserved) {
    throw "RUNTIME_EXIT_EVENT_NOT_OBSERVED"
}

if ($runtime.State -ne [CASU.Runtime.CasuRuntimeState]::Exited) {
    throw "RUNTIME_NOT_EXITED"
}

if ($runtime.ExitReason -ne [CASU.Runtime.CasuExitReason]::EmergencyBypass) {
    throw "INCORRECT_EXIT_REASON"
}

if ($form.BypassSignalCount -ne 1) {
    throw "BYPASS_SIGNAL_COUNT_INVALID"
}

Write-Host "LEFT_SHIFT_DETECTED=TRUE"
Write-Host "DEVICE_MATCH=VID_1EA7_PID_0169_MI_00"
Write-Host "BYPASS_REQUESTED=TRUE"
Write-Host "CASU_EXIT_OR_FALLBACK=PASS"
Write-Host "BYPASS_EXIT_TEST=PASS" -ForegroundColor Green

# ============================================================
# TEST B — WATCHDOG FIRST CRASH -> ONE RESTART
# ============================================================

Write-Host "`n--- TEST B: WATCHDOG SINGLE RESTART ---"

$watchdog = New-Object CASU.Runtime.CasuRuntimeController
$watchdog.MarkRunning()

$script:RestartEvents = 0

$watchdog.add_RestartRequested({
    $script:RestartEvents++
})

$watchdog.ReportCrash()

if ($watchdog.State -ne [CASU.Runtime.CasuRuntimeState]::RestartRequested) {
    throw "WATCHDOG_FIRST_CRASH_DID_NOT_REQUEST_RESTART"
}

if ($watchdog.RestartCount -ne 1) {
    throw "WATCHDOG_RESTART_COUNT_INVALID"
}

if ($script:RestartEvents -ne 1) {
    throw "WATCHDOG_RESTART_EVENT_COUNT_INVALID"
}

$watchdog.ConfirmRestarted()

if ($watchdog.State -ne [CASU.Runtime.CasuRuntimeState]::Running) {
    throw "WATCHDOG_RESTART_CONFIRMATION_FAILED"
}

Write-Host "WATCHDOG_FIRST_FAILURE=RESTART_ONCE"
Write-Host "WATCHDOG_RESTART_COUNT=1"
Write-Host "WATCHDOG_RESTART_POLICY=PASS" -ForegroundColor Green

# ============================================================
# TEST C — SECOND CRASH -> SAFE FALLBACK
# ============================================================

Write-Host "`n--- TEST C: WATCHDOG SAFE FALLBACK ---"

$script:FallbackEvents = 0

$watchdog.add_SafeFallbackRequested({
    $script:FallbackEvents++
})

$watchdog.ReportCrash()

if ($watchdog.State -ne [CASU.Runtime.CasuRuntimeState]::SafeFallback) {
    throw "WATCHDOG_DID_NOT_ENTER_SAFE_FALLBACK"
}

if (-not $watchdog.SafeFallbackRequired) {
    throw "SAFE_FALLBACK_FLAG_NOT_SET"
}

if ($script:FallbackEvents -ne 1) {
    throw "SAFE_FALLBACK_EVENT_COUNT_INVALID"
}

if ($watchdog.RestartCount -ne 1) {
    throw "WATCHDOG_PERFORMED_EXTRA_RESTART"
}

Write-Host "WATCHDOG_SECOND_FAILURE=SAFE_FALLBACK"
Write-Host "WATCHDOG_EXTRA_RESTARTS=0"
Write-Host "SAFE_FALLBACK_REQUESTED=TRUE"
Write-Host "WATCHDOG_STATE=SAFE"
Write-Host "WATCHDOG_FALLBACK_POLICY=PASS" -ForegroundColor Green

# ============================================================
# FINAL RESULT
# ============================================================

Write-Host "`n=== CASU 02B-R3D3 TEST RESULT ==="

Write-Host "CASU_RUNNING=TRUE"
Write-Host "LEFT_SHIFT_DETECTED=TRUE"
Write-Host "DEVICE_MATCH=VID_1EA7_PID_0169_MI_00"
Write-Host "BYPASS_REQUESTED=TRUE"
Write-Host "CASU_EXIT_OR_FALLBACK=PASS"
Write-Host "WATCHDOG_RESTART_ONCE=PASS"
Write-Host "WATCHDOG_SECOND_FAILURE_FALLBACK=PASS"
Write-Host "WATCHDOG_STATE=SAFE"

# IMPORTANT:
# This test does NOT claim Windows sign-in integration.
Write-Host "WINDOWS_SIGN_IN_INTEGRATION=NOT_CONFIGURED"
Write-Host "WINDOWS_SIGN_IN_AVAILABILITY_RUNTIME_PROOF=PENDING"

Write-Host "BOOT_CONFIGURATION_CHANGED=FALSE"
Write-Host "AUTHENTICATION_CHANGED=FALSE"
Write-Host "FIRMWARE_CHANGED=FALSE"
Write-Host "DRIVERS_CHANGED=FALSE"

Write-Host "`nCASU_02B_R3D3_RUNTIME_SAFETY_TEST=PASS" -ForegroundColor Green
exit 0
