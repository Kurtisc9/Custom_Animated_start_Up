# CASU 02B-R3D16 — Persistent Integration Design + Approval Gate

Status: DESIGN VALIDATED / INSTALLATION NOT AUTHORIZED

Canonical source repository:

X:\03_Active_Projects\Custom_Animated_Start_Up

Validated source baseline:

Branch:
phase-02b-r3d-emergency-bypass

Commit:
467350de8100ff946e249244fa3d84f3a6db7955

## Purpose

Define the production Windows installation architecture for CASU after
successful validation of the LocalSystem SCM orchestrator prototype.

This build does NOT install CASU persistence.

Permanent integration requires explicit KurtisC approval after this
design and readiness gate pass.

---

## Production architecture

CASU uses two separated execution roles.

### 1. CASU Orchestrator

Windows service.

Account:

NT AUTHORITY\SYSTEM

Session:

Session 0

Responsibilities:

- start under Windows Service Control Manager
- detect the appropriate interactive console session
- acquire the interactive user token
- duplicate the token safely
- build the user environment
- launch CASU interactive runtime into the intended user session
- monitor runtime health
- restart CASU runtime once after unexpected failure
- request safe fallback after the restart limit is reached
- maintain diagnostic logging
- never render UI from Session 0

The orchestrator MUST NOT:

- collect Windows passwords
- collect Windows PINs
- replace Winlogon
- replace LogonUI
- replace Credential Providers
- intercept Ctrl+Alt+Delete
- disable Windows sign-in
- disable Windows Recovery
- alter Safe Mode
- permanently replace Explorer as the Windows shell

---

### 2. CASU Interactive Runtime

Runs only in an interactive Windows session.

Responsibilities:

- multi-display CASU presentation
- theme rendering
- emergency Left Shift bypass
- controlled runtime exit
- interaction with orchestrator watchdog
- visual transition toward Windows authentication where technically safe

Session 0 interactive UI is prohibited.

---

## Machine installation paths

Source repository remains:

X:\03_Active_Projects\Custom_Animated_Start_Up

Production executable installation root:

C:\Program Files\CASU

Runtime machine-data root:

C:\ProgramData\CASU

Logs:

C:\ProgramData\CASU\Logs

Configuration:

C:\ProgramData\CASU\Config

A production LocalSystem Windows service MUST NOT execute directly from
a user-session mapped drive such as X:.

---

## Windows service identity

Service name:

CASUOrchestrator

Display name:

CASU Orchestrator

Account:

LocalSystem

Service type:

WIN32_OWN_PROCESS

Proposed startup:

Automatic (Delayed Start)

Final startup type remains subject to implementation validation.

The production service MUST remain SCM-controlled.

The interactive runtime MUST NOT control the Windows service lifecycle.

---

## Watchdog policy

Locked policy:

1. CASU runtime starts.
2. Unexpected runtime failure is detected.
3. Orchestrator may restart CASU runtime exactly once.
4. A subsequent failure enters safe fallback.
5. No infinite service/runtime restart loop is allowed.

Maximum runtime restart count:

1

---

## Emergency bypass

Left Shift remains the locked emergency bypass.

Target device:

VID_1EA7
PID_0169
MI_00

Left Shift scan code:

0x2A

Emergency bypass must result in controlled CASU exit/fallback.

---

## Windows authentication boundary

Windows remains the sole authentication authority.

CASU must not request or store Windows:

- passwords
- PINs
- Windows Hello secrets
- authentication tokens

Ctrl+Alt+Delete remains Windows-controlled.

---

## Secure desktop boundary

Current R3D evidence proves interactive Session 1 launch.

It does NOT prove CASU can or should draw on the Windows secure desktop.

No secure-desktop injection is authorized by R3D16.

No LogonUI replacement is authorized.

No Credential Provider replacement is authorized.

WINDOWS_SIGN_IN_RUNTIME_PROOF remains PENDING.

---

## Installation gate

A future persistent installation build may proceed only after:

- this R3D16 design passes validation
- repository remains clean
- all rollback requirements exist
- existing Windows authentication baseline is preserved
- no conflicting CASU service exists
- no conflicting scheduled task exists
- installer/runtime destinations are verified
- KurtisC explicitly approves permanent integration

Until approval:

CASU_WINDOWS_INTEGRATION_INSTALLED=FALSE

---

## Rollback requirements

Before persistent installation:

1. Git rollback tag must exist.
2. Existing service state must be captured.
3. Existing registry baseline must be captured.
4. Existing Winlogon Shell/Userinit must be verified.
5. Existing BCD configuration must be read and preserved.
6. Installer must be able to stop and remove CASU service.
7. Program Files CASU files must be removable.
8. ProgramData CASU runtime files must be removable.
9. Windows authentication must remain immediately usable.
10. A failed install must return the PC to the verified pre-install state.

---

## Approval boundary

R3D16 may design and validate the installation model.

R3D16 MUST NOT install:

- CASUOrchestrator service
- persistent scheduled tasks
- startup entries
- Winlogon modifications
- BCD modifications
- Credential Providers
- authentication hooks
- firmware components
- drivers

Permanent Windows integration requires explicit approval from KurtisC.
