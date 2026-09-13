# CASU 02B-R3D15 — Windows SCM Service Orchestrator Prototype

Status: PASS

Canonical project root:
X:\03_Active_Projects\Custom_Animated_Start_Up

Architecture:
SEPARATE_ORCHESTRATOR_AND_INTERACTIVE_RUNTIME

## Verified Windows SCM execution

A temporary demand-start Windows service was executed under:

NT AUTHORITY\SYSTEM

The service ran in Session 0 and remained under Windows SCM control.

The service successfully:

1. Reached ServiceBase.OnStart.
2. Remained in RUNNING state.
3. Resolved the active interactive console session.
4. Acquired the active user's token using WTSQueryUserToken.
5. Duplicated the token to a primary token.
6. Created the user's environment block.
7. Called CreateProcessAsUser.
8. Launched a benign PowerShell child in the target interactive session.
9. Verified the child Session ID matched the target.
10. Verified child exit code 0.
11. Released process, thread, token, and environment resources.
12. Remained RUNNING after probe completion.
13. Was explicitly stopped and removed by the validation controller.

## D1 finding

The earlier R3D15 Start-Service failure was caused by service lifecycle
timing in the validation prototype.

The earlier service called Stop() immediately after probe completion,
which allowed it to transition out of RUNNING before the controlling
PowerShell command reliably observed stable startup.

The corrected architecture leaves service lifecycle control with SCM /
the external controller.

## Runtime placement

Machine-level LocalSystem runtime binaries must use a machine-accessible
local path and must not depend on a user-session mapped drive.

The authoritative source repository remains:

X:\03_Active_Projects\Custom_Animated_Start_Up

## Safety

Session 0 interactive UI remains prohibited.

Interactive CASU presentation must execute in an intended interactive
Windows session.

Windows authentication remains authoritative.

Ctrl+Alt+Delete remains Windows-controlled.

No permanent CASU service was installed.
No persistent scheduled task was installed.
No startup integration was installed.
No Winlogon/Userinit modification occurred.
No BCD modification occurred.
No Credential Provider modification occurred.
No firmware or driver modification occurred.

Secure desktop / LogonUI presentation is NOT proven.

WINDOWS_SIGN_IN_RUNTIME_PROOF remains PENDING.
