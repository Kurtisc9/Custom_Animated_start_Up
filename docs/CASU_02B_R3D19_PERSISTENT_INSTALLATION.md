# CASU 02B-R3D19 — Persistent CASUOrchestrator Installation

Status: PASS — VERIFIED BY R3D19-R2

Canonical root:

X:\03_Active_Projects\Custom_Animated_Start_Up

Repair base commit:

6e1da0f59c47e214432061937423114dbbdba5d4

Rollback:

rollback-casu-02b-r3d19-r2-6e1da0f59c

## Prior R3D19 attempt

The original persistent-service creation attempt failed before
CASUOrchestrator was created.

The original PASS documentation committed at 6e1da0f was therefore
invalid and is superseded by this R3D19-R2 verified record.

## Verified installation

Service:
CASUOrchestrator

Service account:
LocalSystem

Service state:
RUNNING

Production binary:
C:\Program Files\CASU\CASUOrchestrator.exe

Runtime root:
C:\ProgramData\CASU

## Runtime validation

LocalSystem service execution:
PASS

Session 0 service context:
PASS

Session 0 interactive UI:
PROHIBITED

Active interactive session targeting:
PASS

WTSQueryUserToken:
PASS

DuplicateTokenEx:
PASS

CreateEnvironmentBlock:
PASS

CreateProcessAsUser:
PASS

Target session match:
PASS

Interactive child exit code:
0

Persistent runtime validation:
PASS

## Safety

Windows authentication:
AUTHORITATIVE

Ctrl+Alt+Delete:
WINDOWS CONTROLLED

Winlogon:
UNCHANGED

BCD:
UNCHANGED

Credential Provider:
UNCHANGED

## Remaining gate

WINDOWS_SIGN_IN_RUNTIME_PROOF=PENDING

No reboot or pre-sign-in runtime proof is claimed by R3D19.
