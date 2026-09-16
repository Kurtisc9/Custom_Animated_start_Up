# CASU_02B-R3D22 — Post-Reboot Persistent Integration Validation

Status: TESTED -> PASS -> LOCKED

Validation timestamp: 2026-09-16 17:57:04 -04:00

## Repository Baseline

- Canonical root: X:\03_Active_Projects\Custom_Animated_Start_Up
- Branch: phase-02b-r3d-emergency-bypass
- Pre-closeout commit: f98b94e22cb70bd63cad0e2706c9a5a677151496
- Worktree before closeout: CLEAN

## Controlled Reboot Evidence

- Windows reboot completed successfully.
- Normal Windows sign-in remained available.
- No boot loop observed.
- No black screen observed.
- Ctrl+Alt+Delete remained Windows-controlled.
- Normal Windows authentication remained authoritative.

## Persistent CASU Integration

- CASUOrchestrator survived reboot.
- Service state: RUNNING.
- Service identity: LocalSystem.
- Service execution boundary: Session 0.
- Session 0 interactive UI remains prohibited.

## Elevated BCD Validation

CASU_02B-R3D22-R3 completed from an elevated Administrator PowerShell session.

- ADMINISTRATOR=True
- BCD_EXIT_CODE=0
- BCD_BOOT_MANAGER=PASS
- BCD_BOOT_LOADER=PASS
- BCD_STORE_READ=PASS
- BCD_MODIFICATION=NONE

## Windows Authentication Safety

- WINLOGON_SHELL=PASS
- WINLOGON_USERINIT=PASS
- WINDOWS_AUTHENTICATION=AUTHORITATIVE
- CTRL_ALT_DELETE=WINDOWS_CONTROLLED

## Final Gate

CASU_02B_R3D22_R3_ELEVATED_VALIDATION=PASS

R3D22 is TESTED -> PASS -> LOCKED.

The earlier non-elevated BCD read failures are superseded by the successful elevated R3D22-R3 validation.

No additional reboot is required for R3D22 closeout.
