# CASU 02B-R3D18 — Permanent Integration Approval Gate

Status:

READY FOR KURTISC APPROVAL

Canonical repository:

X:\03_Active_Projects\Custom_Animated_Start_Up

Validated branch:

phase-02b-r3d-emergency-bypass

Validated base commit:

c1bbd93be70e9e93338b7e41740c0d52cd03b491

Rollback point:

rollback-casu-02b-r3d18-c1bbd93be7

---

## Production architecture validated

Service:

CASUOrchestrator

Service account:

NT AUTHORITY\SYSTEM

Service execution context:

Session 0

Session 0 interactive UI:

PROHIBITED

Interactive CASU runtime:

Must execute in an intended interactive Windows session.

Production binary destination:

C:\Program Files\CASU

Runtime data destination:

C:\ProgramData\CASU

Logs:

C:\ProgramData\CASU\Logs

Configuration:

C:\ProgramData\CASU\Config

---

## Locked watchdog policy

Maximum runtime restart count:

1

First unexpected runtime failure:

restart runtime once

Second unexpected runtime failure:

safe fallback

Infinite restart loops:

PROHIBITED

---

## Locked emergency bypass

Emergency key:

Left Shift

Physical target:

VID_1EA7
PID_0169
MI_00

Scan code:

0x2A

---

## Authentication boundary

Windows remains the sole authentication authority.

CASU must not collect or replace:

- Windows passwords
- Windows PINs
- Windows Hello secrets
- Credential Providers
- Winlogon
- LogonUI authentication authority

Ctrl+Alt+Delete remains Windows-controlled.

---

## Existing proof

Validated:

- LocalSystem SCM service hosting
- Session 0 orchestration
- active interactive session targeting
- WTSQueryUserToken
- DuplicateTokenEx
- CreateEnvironmentBlock
- CreateProcessAsUser
- interactive-session child process
- target-session match
- process/thread/token/environment cleanup
- production-shaped orchestrator source
- production build pipeline
- temporary SCM lifecycle
- service removal
- rollback model

Not yet proven:

WINDOWS_SIGN_IN_RUNTIME_PROOF

No secure-desktop claim is authorized from the current evidence.

---

## Permanent installation boundary

The next build may install CASUOrchestrator persistently ONLY after
explicit KurtisC approval.

Proposed future installation action:

1. Build production CASU orchestrator.
2. Stage binaries to C:\Program Files\CASU.
3. Create C:\ProgramData\CASU runtime structure.
4. Register CASUOrchestrator with Windows SCM.
5. Configure LocalSystem identity.
6. Configure approved startup mode.
7. Start and validate service.
8. Verify interactive runtime launch.
9. Verify Left Shift bypass.
10. Verify watchdog restart-once policy.
11. Verify second-failure safe fallback.
12. Verify Windows authentication remains available.
13. Verify Ctrl+Alt+Delete remains Windows-controlled.
14. Verify rollback/uninstall capability.

---

## Prohibited during permanent installation

Without a separately approved design change, installation must NOT:

- replace Winlogon
- replace Userinit
- replace Explorer permanently
- replace LogonUI
- install a Credential Provider
- enable AutoAdminLogon
- disable Windows Recovery
- disable Safe Mode
- intercept Ctrl+Alt+Delete
- modify firmware
- install kernel drivers
- change BCD presentation for cosmetic purposes

---

## Current state

CASU_SERVICE_INSTALLED=FALSE

CASU_WINDOWS_INTEGRATION_INSTALLED=FALSE

WINDOWS_SIGN_IN_RUNTIME_PROOF=PENDING

PERMANENT_INTEGRATION_APPROVAL_REQUIRED=TRUE

FINAL_APPROVAL_AUTHORITY=KurtisC
