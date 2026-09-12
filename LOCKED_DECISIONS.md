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
