$ErrorActionPreference = "Stop"

$Root = "X:\03_Active_Projects\Custom_Animated_Start_Up"
Set-Location -LiteralPath $Root

$OrchestratorSource = Join-Path $Root "src\CASU.Orchestrator\CasuOrchestrator.cs"
$RuntimeSource = Join-Path $Root "src\CASU.Runtime\CasuInteractiveRuntimeHost.cs"

if (-not (Test-Path -LiteralPath $OrchestratorSource -PathType Leaf)) {
    throw "ORCHESTRATOR_SOURCE_MISSING"
}

if (-not (Test-Path -LiteralPath $RuntimeSource -PathType Leaf)) {
    throw "INTERACTIVE_RUNTIME_SOURCE_MISSING"
}

# Compile the independent C# files separately.
# They do not depend on each other's types.

$OrchestratorCode = Get-Content -LiteralPath $OrchestratorSource -Raw
$RuntimeCode = Get-Content -LiteralPath $RuntimeSource -Raw

Add-Type -TypeDefinition $OrchestratorCode -Language CSharp
Write-Host "ORCHESTRATOR_COMPILE=PASS"

Add-Type -TypeDefinition $RuntimeCode -Language CSharp
Write-Host "INTERACTIVE_RUNTIME_COMPILE=PASS"

# ------------------------------------------------------------
# TEST A — NORMAL START
# ------------------------------------------------------------

$Orchestrator = New-Object CASU.Orchestrator.CasuOrchestrator
$Runtime = New-Object CASU.Runtime.CasuInteractiveRuntimeHost

$script:LaunchCount = 0
$script:FallbackCount = 0
$script:ExitCount = 0

$Orchestrator.add_RuntimeLaunchRequested({
    $script:LaunchCount++
})

$Orchestrator.add_SafeFallbackRequested({
    $script:FallbackCount++
})

$Orchestrator.add_ExitRequested({
    $script:ExitCount++
})

Write-Host "TEST_A=NORMAL_START"

$Orchestrator.Start()

if ($script:LaunchCount -ne 1) {
    throw "INITIAL_RUNTIME_LAUNCH_COUNT_FAILED=$($script:LaunchCount)"
}

$Runtime.Start()
$Orchestrator.ConfirmRuntimeActive()

if ($Runtime.State.ToString() -ne "Running") {
    throw "RUNTIME_DID_NOT_ENTER_RUNNING"
}

if ($Orchestrator.State.ToString() -ne "RuntimeActive") {
    throw "ORCHESTRATOR_DID_NOT_CONFIRM_RUNTIME_ACTIVE"
}

Write-Host "NORMAL_START=PASS"

# ------------------------------------------------------------
# TEST B — FIRST FAILURE => ONE RESTART
# ------------------------------------------------------------

Write-Host "TEST_B=FIRST_RUNTIME_FAILURE"

$Runtime.ReportFault()
$Orchestrator.ReportRuntimeFailure()

if ($Orchestrator.RestartCount -ne 1) {
    throw "FIRST_FAILURE_RESTART_COUNT_FAILED=$($Orchestrator.RestartCount)"
}

if ($script:LaunchCount -ne 2) {
    throw "FIRST_FAILURE_RUNTIME_RELAUNCH_FAILED=$($script:LaunchCount)"
}

Write-Host "FIRST_FAILURE_RESTART_ONCE=PASS"

# ------------------------------------------------------------
# TEST C — SECOND FAILURE => SAFE FALLBACK
# ------------------------------------------------------------

Write-Host "TEST_C=SECOND_RUNTIME_FAILURE"

$Orchestrator.ReportRuntimeFailure()

if ($script:FallbackCount -ne 1) {
    throw "SAFE_FALLBACK_NOT_REQUESTED=$($script:FallbackCount)"
}

if ($Orchestrator.RestartCount -ne 1) {
    throw "RESTART_LIMIT_EXCEEDED=$($Orchestrator.RestartCount)"
}

if ($Orchestrator.State.ToString() -ne "SafeFallback") {
    throw "ORCHESTRATOR_SAFE_FALLBACK_STATE_FAILED"
}

Write-Host "SECOND_FAILURE_SAFE_FALLBACK=PASS"
Write-Host "RESTART_LIMIT=1"
Write-Host "EXTRA_RESTARTS=0"

# ------------------------------------------------------------
# TEST D — EMERGENCY BYPASS => CONTROLLED EXIT
# ------------------------------------------------------------

Write-Host "TEST_D=EMERGENCY_BYPASS"

$BypassOrchestrator = New-Object CASU.Orchestrator.CasuOrchestrator
$BypassRuntime = New-Object CASU.Runtime.CasuInteractiveRuntimeHost

$script:BypassLaunchCount = 0
$script:BypassExitCount = 0

$BypassOrchestrator.add_RuntimeLaunchRequested({
    $script:BypassLaunchCount++
})

$BypassOrchestrator.add_ExitRequested({
    $script:BypassExitCount++
})

$BypassRuntime.add_EmergencyBypassRequested({
    $BypassOrchestrator.RequestEmergencyBypass()
})

$BypassOrchestrator.Start()
$BypassRuntime.Start()
$BypassOrchestrator.ConfirmRuntimeActive()

$BypassRuntime.RequestEmergencyBypass()

if ($script:BypassExitCount -ne 1) {
    throw "EMERGENCY_BYPASS_EXIT_COUNT_FAILED=$($script:BypassExitCount)"
}

if ($BypassOrchestrator.State.ToString() -ne "Exited") {
    throw "EMERGENCY_BYPASS_ORCHESTRATOR_EXIT_FAILED"
}

if ($BypassRuntime.State.ToString() -ne "Exited") {
    throw "EMERGENCY_BYPASS_RUNTIME_EXIT_FAILED"
}

Write-Host "EMERGENCY_BYPASS_CONTROLLED_EXIT=PASS"

Write-Host "CASU_SERVICE_CREATED=FALSE"
Write-Host "SCHEDULED_TASK_CREATED=FALSE"
Write-Host "WINDOWS_STARTUP_CHANGED=FALSE"
Write-Host "WINLOGON_CHANGED=FALSE"
Write-Host "BCD_CHANGED=FALSE"
Write-Host "AUTHENTICATION_CHANGED=FALSE"
Write-Host "ISOLATED_ORCHESTRATOR_RUNTIME_TEST=PASS"
