# CASU 02B-R3D14 — LocalSystem Cross-Session Prototype

Status: PASS

Canonical root:
X:\03_Active_Projects\Custom_Animated_Start_Up

Architecture:
SEPARATE_ORCHESTRATOR_AND_INTERACTIVE_RUNTIME

## Verified execution chain

LocalSystem Session 0 successfully:

1. Resolved the active console session.
2. Acquired the active user's token with WTSQueryUserToken.
3. Duplicated the token into a primary token.
4. Created the user's environment block.
5. Created a benign PowerShell process using CreateProcessAsUser.
6. Launched the child into the target interactive session.
7. Verified the child exited with code 0.
8. Closed process, thread, token, and environment resources.

Session 0 hosted no interactive UI.

## Security boundary

The LocalSystem component is suitable only for orchestration.

Interactive CASU presentation must execute in an approved interactive
Windows session.

Windows authentication remains authoritative.

Ctrl+Alt+Delete remains Windows-controlled.

This test does not prove secure-desktop / LogonUI presentation.

WINDOWS_SIGN_IN_RUNTIME_PROOF remains PENDING.

## Persistence

No CASU service was installed.
No persistent scheduled task was created.
No startup mechanism was installed.
No Winlogon/Userinit changes occurred.
No BCD changes occurred.
No authentication changes occurred.
No Credential Provider changes occurred.
No firmware or driver changes occurred.
