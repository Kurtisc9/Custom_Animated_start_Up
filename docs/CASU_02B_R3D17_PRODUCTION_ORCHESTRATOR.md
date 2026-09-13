# CASU 02B-R3D17 — Production Orchestrator Source

Status: PASS

Canonical source root:

X:\03_Active_Projects\Custom_Animated_Start_Up

Validated branch:

phase-02b-r3d-emergency-bypass

Base commit:

217e48d856e28f0bdb949360821d1e11afc66e94

## Implemented

Production-shaped Windows service host:

src\CASU.Orchestrator\CasuWindowsServiceHost.cs

Build script:

scripts\Build-CASU-Orchestrator.ps1

Temporary production-host validation:

scripts\Test-CASU-ProductionOrchestrator.ps1

## R3D17-R1 repair

Initial R3D17 execution safely failed because the build script placed a
PowerShell param block inside an already-started script block.

PowerShell requires param(...) to be the first executable construct in
that script scope.

R3D17-R1 corrected the build script and verified it through parser and
direct build smoke testing before re-running the SCM validation.

## Architecture

Service account:

LocalSystem

Service execution:

Session 0

Interactive UI in Session 0:

PROHIBITED

Interactive runtime:

Launched into the active interactive Windows session through
WTSQueryUserToken, primary-token duplication, environment construction,
and CreateProcessAsUser.

Service lifecycle:

Owned by Windows Service Control Manager / external controller.

## Production destination model

Source repository:

X:\03_Active_Projects\Custom_Animated_Start_Up

Future production binaries:

C:\Program Files\CASU

Future runtime data:

C:\ProgramData\CASU

## Validation

Build-script parser validation passed.

Direct production service build smoke test passed.

Production-shaped service source compiled successfully.

Temporary SCM-hosted LocalSystem execution passed.

Active session resolution passed.

Interactive child process creation passed.

Target-session verification passed.

Child exit code was 0.

Resource cleanup passed.

Temporary service removal passed.

Temporary runtime artifact removal passed.

## Safety

No permanent CASU service installed.

No startup persistence installed.

No Winlogon modification.

No BCD modification.

No Credential Provider modification.

No Windows authentication modification.

Ctrl+Alt+Delete remains Windows-controlled.

Windows authentication remains authoritative.

WINDOWS_SIGN_IN_RUNTIME_PROOF remains PENDING.

Permanent CASU Windows integration remains blocked until explicit
KurtisC approval.
