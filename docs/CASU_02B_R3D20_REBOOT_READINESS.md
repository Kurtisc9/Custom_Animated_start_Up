# CASU 02B-R3D20 — Reboot / Pre-Sign-In Validation Readiness

Status: PASS

Canonical root:

X:\03_Active_Projects\Custom_Animated_Start_Up

Base commit:

97744f8e79ecbd4c14d01b4b076aaf9d16aa68c0

Rollback point:

rollback-casu-02b-r3d20-97744f8e79

## Persistent integration

CASUOrchestrator:
INSTALLED

Service status:
RUNNING

Service account:
LocalSystem

Production binary:
C:\Program Files\CASU\CASUOrchestrator.exe

## Recovery

Emergency recovery script:

X:\03_Active_Projects\Custom_Animated_Start_Up\scripts\Disable-CASUOrchestrator-Emergency.ps1

Recovery script parse validation:
PASS

Recovery script logic validation:
PASS

## Safety boundaries

Windows authentication:
AUTHORITATIVE

Ctrl+Alt+Delete:
WINDOWS CONTROLLED

Session 0 interactive UI:
PROHIBITED

Winlogon Shell:
UNCHANGED

Winlogon Userinit:
UNCHANGED

BCD:
READ-ONLY / UNCHANGED

## Reboot status

REBOOT_PERFORMED=FALSE

WINDOWS_SIGN_IN_RUNTIME_PROOF=PENDING

R3D20 establishes readiness and recovery capability only.

A later controlled validation must provide actual post-reboot
service-start and sign-in behavior evidence.
