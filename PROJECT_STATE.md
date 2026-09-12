# PROJECT STATE

Project: Custom Animated Start Up
Canonical Root: X:\03_Active_Projects\Custom_Animated_Start_Up
Active Phase: 02B — Watchdog / Emergency Bypass
Active Task: CASU_02B-R3D — Implementation Foundation + Left Shift Emergency Bypass Integration
State: IMPLEMENTATION IN PROGRESS
Baseline Branch: main
Baseline Commit: a8a94d9e50b78d9c568950927becbc6ac19becea
Implementation Branch: phase-02b-r3d-emergency-bypass

Verified Input:
- VID: 1EA7
- PID: 0169
- Interface: MI_00
- Left Shift MakeCode: 0x2A
- Raw Input capture: PASS

Safety:
- Windows authentication remains authoritative.
- Ctrl+Alt+Delete remains available.
- Left Shift is the emergency bypass.
- Boot configuration changes are prohibited during 02B-R3D.
- Firmware changes are prohibited during 02B-R3D.
- Driver changes are prohibited during 02B-R3D.

Current Blocker:
- Emergency bypass implementation and runtime validation remain incomplete.

Next Action:
- Implement isolated Raw Input emergency-bypass component.

## CASU_02B-R3D2
State: IMPLEMENTED / RUNTIME TEST PENDING

Implementation:
- Win32 Raw Input keyboard registration
- Physical device filtering: VID_1EA7 / PID_0169 / MI_00
- Left Shift scan code: 0x2A
- Key-down-only bypass triggering
- One-shot latch prevents release/double triggering
- No boot, firmware, driver, authentication, or startup integration changes

Validation:
- Static source validation: PASS
- C# compilation validation: PASS
- Physical runtime bypass test: PENDING

## CASU_02B-R3D3
State: IMPLEMENTED / PHYSICAL RUNTIME TEST PENDING

Runtime behavior:
- Emergency bypass requests controlled CASU runtime exit.
- Emergency bypass remains one-shot.
- First runtime crash requests one CASU restart.
- Second runtime crash requests safe fallback.
- Restart count cannot exceed one.

Safety boundary:
- Windows startup integration remains unconfigured.
- Windows sign-in runtime availability proof remains pending.
- No boot configuration changes.
- No authentication changes.
- No firmware changes.
- No driver changes.
