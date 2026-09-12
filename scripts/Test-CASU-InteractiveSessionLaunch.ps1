$ErrorActionPreference = "Stop"

$Root = "X:\03_Active_Projects\Custom_Animated_Start_Up"
Set-Location -LiteralPath $Root

$LauncherSource = Join-Path $Root "src\CASU.Runtime\CasuInteractiveSessionLauncher.cs"

if (-not (Test-Path -LiteralPath $LauncherSource -PathType Leaf)) {
    throw "SESSION_LAUNCHER_SOURCE_MISSING"
}

Add-Type `
    -TypeDefinition (Get-Content -LiteralPath $LauncherSource -Raw) `
    -Language CSharp

Write-Host "SESSION_LAUNCHER_COMPILE=PASS"

$Launcher = New-Object CASU.Runtime.CasuInteractiveSessionLauncher

$ParentSession = $Launcher.CurrentSessionId

Write-Host "PARENT_SESSION_ID=$ParentSession"

if (-not $Launcher.IsInteractiveSession) {
    throw "SESSION_ZERO_LAUNCH_PROHIBITED"
}

if ($ParentSession -eq 0) {
    throw "PARENT_SESSION_ZERO"
}

Write-Host "PARENT_INTERACTIVE_SESSION=PASS"

$PowerShellExe = Join-Path $PSHOME "powershell.exe"

if (-not (Test-Path -LiteralPath $PowerShellExe -PathType Leaf)) {
    throw "POWERSHELL_TEST_EXECUTABLE_MISSING"
}

$Arguments = '-NoProfile -NonInteractive -Command "exit 0"'

$Result = $Launcher.LaunchAndWait(
    $PowerShellExe,
    $Arguments,
    15000
)

Write-Host "CHILD_PROCESS_ID=$($Result.ProcessId)"
Write-Host "CHILD_SESSION_ID=$($Result.SessionId)"
Write-Host "CHILD_EXIT_CODE=$($Result.ExitCode)"

if ($Result.SessionId -eq 0) {
    throw "CHILD_PROCESS_LAUNCHED_IN_SESSION_ZERO"
}

if ($Result.SessionId -ne $ParentSession) {
    throw "SESSION_BOUNDARY_MISMATCH_PARENT_$($ParentSession)_CHILD_$($Result.SessionId)"
}

if ($Result.ExitCode -ne 0) {
    throw "CHILD_PROCESS_EXIT_FAILED=$($Result.ExitCode)"
}

Write-Host "SESSION_ZERO_UI_AVOIDED=PASS"
Write-Host "CHILD_SESSION_MATCH=PASS"
Write-Host "INTERACTIVE_RUNTIME_LAUNCH=PASS"

Write-Host "SERVICE_INSTALLATION=FALSE"
Write-Host "SCHEDULED_TASK_INSTALLATION=FALSE"
Write-Host "STARTUP_INSTALLATION=FALSE"

Write-Host "CASU_02B_R3D10_SESSION_LAUNCH_TEST=PASS"
