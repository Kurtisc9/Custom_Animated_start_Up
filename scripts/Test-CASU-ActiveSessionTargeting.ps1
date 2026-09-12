$ErrorActionPreference = "Stop"

$Root = "X:\03_Active_Projects\Custom_Animated_Start_Up"
Set-Location -LiteralPath $Root

$ResolverSource = Join-Path $Root "src\CASU.Runtime\CasuActiveSessionResolver.cs"
$RequestSource = Join-Path $Root "src\CASU.Orchestrator\CasuInteractiveLaunchRequest.cs"

foreach ($File in @(
    $ResolverSource,
    $RequestSource
)) {
    if (-not (Test-Path -LiteralPath $File -PathType Leaf)) {
        throw "R3D11_SOURCE_MISSING=$File"
    }
}

Add-Type `
    -TypeDefinition (Get-Content -LiteralPath $ResolverSource -Raw) `
    -Language CSharp

Write-Host "ACTIVE_SESSION_RESOLVER_COMPILE=PASS"

Add-Type `
    -TypeDefinition (Get-Content -LiteralPath $RequestSource -Raw) `
    -Language CSharp

Write-Host "LAUNCH_REQUEST_MODEL_COMPILE=PASS"

$Resolver = New-Object CASU.Runtime.CasuActiveSessionResolver
$Target = $Resolver.Resolve()

Write-Host "ACTIVE_CONSOLE_SESSION_ID=$($Target.ActiveConsoleSessionId)"
Write-Host "CURRENT_PROCESS_SESSION_ID=$($Target.CurrentProcessSessionId)"
Write-Host "HAS_INTERACTIVE_TARGET=$($Target.HasInteractiveTarget)"
Write-Host "CURRENT_PROCESS_MATCHES_TARGET=$($Target.CurrentProcessMatchesTarget)"

if (-not $Target.HasInteractiveTarget) {
    throw "NO_ACTIVE_INTERACTIVE_CONSOLE_SESSION"
}

if ($Target.ActiveConsoleSessionId -eq 0) {
    throw "ACTIVE_TARGET_IS_SESSION_ZERO"
}

$ExplorerProcesses = @(
    Get-Process explorer -ErrorAction SilentlyContinue |
    Where-Object {
        $_.SessionId -eq [int]$Target.ActiveConsoleSessionId
    }
)

if ($ExplorerProcesses.Count -lt 1) {
    throw "NO_EXPLORER_FOUND_IN_ACTIVE_CONSOLE_SESSION"
}

Write-Host "ACTIVE_CONSOLE_SESSION_HAS_EXPLORER=PASS"

$PowerShellExe = Join-Path $PSHOME "powershell.exe"

if (-not (Test-Path -LiteralPath $PowerShellExe -PathType Leaf)) {
    throw "POWERSHELL_EXECUTABLE_MISSING"
}

$Request = New-Object CASU.Orchestrator.CasuInteractiveLaunchRequest(
    $Target.ActiveConsoleSessionId,
    $PowerShellExe,
    '-NoProfile -NonInteractive -Command "exit 0"'
)

if ($Request.TargetSessionId -ne $Target.ActiveConsoleSessionId) {
    throw "LAUNCH_REQUEST_TARGET_SESSION_MISMATCH"
}

if ($Request.TargetSessionId -eq 0) {
    throw "LAUNCH_REQUEST_TARGETED_SESSION_ZERO"
}

Write-Host "LAUNCH_REQUEST_TARGET_SESSION=$($Request.TargetSessionId)"
Write-Host "LAUNCH_REQUEST_EXECUTABLE=$($Request.ExecutablePath)"

Write-Host "SESSION_ZERO_TARGET_REJECTED=PASS"
Write-Host "ACTIVE_SESSION_TARGET_RESOLUTION=PASS"
Write-Host "CROSS_SESSION_LAUNCH_REQUEST_MODEL=PASS"

# This build intentionally DOES NOT call:
# WTSQueryUserToken
# DuplicateTokenEx
# CreateProcessAsUser
# CreateProcessWithTokenW
#
# Those APIs cross the security/session boundary and belong to
# the later controlled Windows integration gate.

Write-Host "USER_TOKEN_ACQUIRED=FALSE"
Write-Host "TOKEN_DUPLICATION_PERFORMED=FALSE"
Write-Host "CROSS_SESSION_PROCESS_CREATED=FALSE"

Write-Host "CASU_SERVICE_CREATED=FALSE"
Write-Host "SCHEDULED_TASK_CREATED=FALSE"
Write-Host "STARTUP_CHANGED=FALSE"

Write-Host "CASU_02B_R3D11_ACTIVE_SESSION_TARGETING_TEST=PASS"
