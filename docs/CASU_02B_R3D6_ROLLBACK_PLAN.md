# CASU_02B-R3D6 — Rollback Plan

## Canonical Project Root

X:\03_Active_Projects\Custom_Animated_Start_Up

## Windows Baseline

Winlogon Shell:
explorer.exe

Winlogon Userinit:
C:\WINDOWS\system32\userinit.exe,

CASU Windows integration:
NOT INSTALLED

## Rollback Principle

Rollback must remain possible without CASU running.

Windows authentication, recovery, and normal continuation must remain
independently accessible.

## Recovery Layers

### Level 1

Left Shift emergency bypass.

### Level 2

CASU watchdog permits one controlled restart.

### Level 3

After the restart limit is reached, CASU must enter safe fallback.

### Level 4

Future Windows integration must have an independent deterministic
removal and recovery path.

## Required Evidence Before Live Installation

Before any future live integration:

- record Git branch
- record Git commit
- create rollback point
- record CASU runtime version
- record original Windows configuration
- export any Windows configuration that would be changed
- record future startup trigger
- record future installation location
- define deterministic removal command
- verify Windows recovery independently of CASU
- verify normal Windows sign-in independently of CASU

## Immediate Rollback Conditions

Rollback is required if:

1. Windows sign-in does not appear.
2. Ctrl+Alt+Delete becomes unavailable.
3. Left Shift emergency bypass fails.
4. CASU enters a restart loop.
5. watchdog performs more than one restart.
6. safe fallback fails.
7. CASU interferes with Windows credentials.
8. Windows Recovery becomes unavailable.
9. CASU causes repeated boot failure.
10. CASU prevents normal Windows continuation.

## Current Gate

ROLLBACK_DESIGN=DEFINED
ROLLBACK_EXECUTION=NOT_REQUIRED
WINDOWS_CONFIGURATION_CHANGED=FALSE
LIVE_INTEGRATION_INSTALLED=FALSE
WINDOWS_SIGN_IN_RUNTIME_PROOF=PENDING
