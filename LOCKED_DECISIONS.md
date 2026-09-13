# LOCKED DECISIONS

## Project Authority
Final approval authority: KurtisC.

## Canonical Path
X:\03_Active_Projects\Custom_Animated_Start_Up

## Emergency Bypass
Emergency bypass key: Left Shift.

Verified physical input:
VID_1EA7
PID_0169
MI_00
MakeCode 0x2A

Bypass must trigger on a single physical key-down event and must not double-trigger on key release.

## Safety
Windows authentication remains authoritative.
Ctrl+Alt+Delete remains available.
CASU may not disable Windows recovery or authentication.
Crash policy: restart CASU once, then fall back.
Three-level failsafe and independent watchdog remain required.

## Phase 02B Restrictions
No firmware modification.
No boot-configuration modification.
No driver modification.
No credential interception.

## CASU_02B-R3D17 Production Orchestrator Boundary

- Source repository remains X:\03_Active_Projects\Custom_Animated_Start_Up.
- Production binaries must use a machine-local installation path.
- LocalSystem service UI in Session 0 is prohibited.
- Interactive runtime must target an interactive Windows session.
- Windows Service Control Manager owns service lifecycle.
- Windows authentication remains authoritative.
- Ctrl+Alt+Delete remains Windows-controlled.
- No permanent CASU Windows integration may be installed without explicit KurtisC approval.

## CASU_02B-R3D18 Permanent Integration Approval Boundary

- R3D17 production orchestrator baseline is validated.
- Permanent CASU Windows service installation is not yet authorized.
- CASUOrchestrator may be installed persistently only after explicit KurtisC approval.
- Windows authentication remains authoritative.
- Ctrl+Alt+Delete remains Windows-controlled.
- Session 0 interactive UI remains prohibited.
- Windows sign-in runtime proof remains pending.

## CASU_02B-R3D19

KurtisC-approved persistent CASU integration installed.

CASUOrchestrator:
INSTALLED

Service account:
LocalSystem

Session 0 UI:
PROHIBITED

Windows authentication:
AUTHORITATIVE

Ctrl+Alt+Delete:
WINDOWS CONTROLLED

## CASU_02B-R3D19-R2 Corrected Persistent Integration Decision

The inaccurate R3D19 PASS record from commit
6e1da0f59c47e214432061937423114dbbdba5d4
is superseded.

Authoritative R3D19 state is determined by R3D19-R2 evidence.

CASUOrchestrator:
INSTALLED AND RUNNING

Account:
LocalSystem

Session 0 interactive UI:
PROHIBITED

Windows authentication:
AUTHORITATIVE

Ctrl+Alt+Delete:
WINDOWS CONTROLLED

Windows sign-in runtime proof:
PENDING

## CASU_02B-R3D20 Reboot Readiness

- Persistent CASUOrchestrator must remain recoverable.
- Emergency disable script is required before reboot validation.
- R3D20 does not reboot Windows.
- Windows authentication remains authoritative.
- Ctrl+Alt+Delete remains Windows-controlled.
- Session 0 interactive UI remains prohibited.
- Actual reboot/pre-sign-in behavior requires a separate validation gate.

## CASU_02B-R3D21 Controlled Reboot Gate

- Pre-reboot verification must pass before restart.
- Emergency recovery script must remain available.
- Windows authentication remains authoritative.
- Ctrl+Alt+Delete remains Windows-controlled.
- Session 0 interactive UI remains prohibited.
- No Credential Provider, Winlogon Shell, Userinit, BCD, firmware, or driver modification is authorized.
- Post-reboot evidence is required before sign-in runtime proof may be marked PASS.
