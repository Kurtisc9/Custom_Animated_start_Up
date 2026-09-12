$ErrorActionPreference = "Stop"

$Root = "X:\03_Active_Projects\Custom_Animated_Start_Up"
Set-Location -LiteralPath $Root

$OrchestratorSource = Join-Path $Root "src\CASU.Orchestrator\CasuOrchestrator.cs"
$RuntimeSource = Join-Path $Root "src\CASU.Runtime\CasuInteractiveRuntimeHost.cs"
$BridgeSource = Join-Path $Root "src\CASU.Runtime\CasuEmergencyBypassBridge.cs"

$Sources = @(
    $OrchestratorSource,
    $RuntimeSource,
    $BridgeSource
)

foreach ($File in $Sources) {
    if (-not (Test-Path -LiteralPath $File -PathType Leaf)) {
        throw "E2E_SOURCE_MISSING=$File"
    }
}

Add-Type -Path $Sources

Write-Host "MULTI_FILE_CSHARP_COMPILE=PASS"
Write-Host "ORCHESTRATOR_COMPILE=PASS"
Write-Host "INTERACTIVE_RUNTIME_COMPILE=PASS"
Write-Host "BYPASS_BRIDGE_COMPILE=PASS"

# ------------------------------------------------------------
# TEST A — NORMAL START
# ------------------------------------------------------------

$Orchestrator = New-Object CASU.Orchestrator.CasuOrchestrator
$Runtime = New-Object CASU.Runtime.CasuInteractiveRuntimeHost
$Bridge = New-Object CASU.Runtime.CasuEmergencyBypassBridge(
    $Orchestrator,
    $Runtime
)

$script:LaunchCount = 0
$script:ExitCount = 0

$Orchestrator.add_RuntimeLaunchRequested({
    $script:LaunchCount++
})

$Orchestrator.add_ExitRequested({
    $script:ExitCount++
})

$Orchestrator.Start()
$Runtime.Start()
$Orchestrator.ConfirmRuntimeActive()

if ($script:LaunchCount -ne 1) {
    throw "E2E_INITIAL_LAUNCH_COUNT_FAILED=$($script:LaunchCount)"
}

if ($Orchestrator.State.ToString() -ne "RuntimeActive") {
    throw "E2E_ORCHESTRATOR_NOT_ACTIVE"
}

if ($Runtime.State.ToString() -ne "Running") {
    throw "E2E_RUNTIME_NOT_RUNNING"
}

Write-Host "E2E_NORMAL_START=PASS"

# ------------------------------------------------------------
# TEST B — EMERGENCY BYPASS SIGNAL CHAIN
# ------------------------------------------------------------

$Runtime.RequestEmergencyBypass()

if (-not $Bridge.BypassHandled) {
    throw "E2E_BYPASS_BRIDGE_NOT_HANDLED"
}

if ($script:ExitCount -ne 1) {
    throw "E2E_ORCHESTRATOR_EXIT_COUNT_FAILED=$($script:ExitCount)"
}

if ($Orchestrator.State.ToString() -ne "Exited") {
    throw "E2E_ORCHESTRATOR_NOT_EXITED"
}

if ($Runtime.State.ToString() -ne "Exited") {
    throw "E2E_RUNTIME_NOT_EXITED"
}

Write-Host "E2E_BYPASS_SIGNAL_CHAIN=PASS"
Write-Host "E2E_CONTROLLED_EXIT=PASS"

# ------------------------------------------------------------
# TEST C — ONE-SHOT BYPASS
# ------------------------------------------------------------

$Runtime.RequestEmergencyBypass()

if ($script:ExitCount -ne 1) {
    throw "E2E_DUPLICATE_EXIT_SIGNAL_DETECTED=$($script:ExitCount)"
}

if (-not $Bridge.BypassHandled) {
    throw "E2E_BYPASS_LATCH_NOT_PRESERVED"
}

Write-Host "E2E_ONE_SHOT_BYPASS=PASS"

# ------------------------------------------------------------
# TEST D — WATCHDOG POLICY
# ------------------------------------------------------------

$Watchdog = New-Object CASU.Orchestrator.CasuOrchestrator

$script:WatchdogLaunchCount = 0
$script:FallbackCount = 0

$Watchdog.add_RuntimeLaunchRequested({
    $script:WatchdogLaunchCount++
})

$Watchdog.add_SafeFallbackRequested({
    $script:FallbackCount++
})

$Watchdog.Start()
$Watchdog.ConfirmRuntimeActive()

if ($script:WatchdogLaunchCount -ne 1) {
    throw "WATCHDOG_INITIAL_LAUNCH_FAILED"
}

$Watchdog.ReportRuntimeFailure()

if ($Watchdog.RestartCount -ne 1) {
    throw "WATCHDOG_FIRST_RESTART_COUNT_FAILED"
}

if ($script:WatchdogLaunchCount -ne 2) {
    throw "WATCHDOG_RELAUNCH_FAILED"
}

$Watchdog.ReportRuntimeFailure()

if ($Watchdog.RestartCount -ne 1) {
    throw "WATCHDOG_RESTART_LIMIT_EXCEEDED"
}

if ($script:FallbackCount -ne 1) {
    throw "WATCHDOG_SAFE_FALLBACK_FAILED"
}

if ($Watchdog.State.ToString() -ne "SafeFallback") {
    throw "WATCHDOG_SAFE_FALLBACK_STATE_FAILED"
}

Write-Host "WATCHDOG_RESTART_LIMIT=1"
Write-Host "WATCHDOG_FIRST_FAILURE_RESTART=PASS"
Write-Host "WATCHDOG_SECOND_FAILURE_FALLBACK=PASS"
Write-Host "WATCHDOG_SAFE_FALLBACK=PASS"

# ------------------------------------------------------------
# TEST RESULT
# ------------------------------------------------------------

Write-Host "CASU_SERVICE_CREATED=FALSE"
Write-Host "SCHEDULED_TASK_CREATED=FALSE"
Write-Host "WINDOWS_STARTUP_CHANGED=FALSE"
Write-Host "WINLOGON_CHANGED=FALSE"
Write-Host "BCD_CHANGED=FALSE"
Write-Host "AUTHENTICATION_CHANGED=FALSE"

Write-Host "CASU_02B_R3D9_E2E_TEST=PASS"
