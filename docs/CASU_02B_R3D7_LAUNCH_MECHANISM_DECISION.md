# CASU_02B-R3D7 — Windows Launch Mechanism Decision

## Status

DESIGN SELECTED / NOT INSTALLED

## Canonical Root

X:\03_Active_Projects\Custom_Animated_Start_Up

## Objective

Select the safest Windows-side mechanism for launching the CASU
runtime while preserving Windows authentication, recovery, secure
attention sequence handling, and deterministic rollback.

This document does not authorize installation.

## Rejected Mechanisms

### Winlogon Shell Replacement

REJECTED.

CASU must not replace explorer.exe as the permanent Windows shell.

### Userinit Replacement

REJECTED.

CASU must not replace or modify the standard Windows userinit chain.

### LogonUI Replacement

REJECTED.

CASU must not replace LogonUI.exe.

### Credential Provider Replacement

REJECTED.

CASU does not own Windows credentials or authentication.

### BCD Cosmetic Modification

REJECTED.

CASU must not modify the Windows bootloader solely to provide
presentation behavior.

### AutoAdminLogon

REJECTED.

CASU must not bypass Windows authentication.

## Candidate Windows Integration Architecture

The Windows-side CASU runtime shall use a separately removable,
least-privilege launch architecture whose lifecycle is independent
from Windows authentication.

The future integration mechanism must:

1. Leave Winlogon Shell as explorer.exe.
2. Leave Userinit at the Windows baseline.
3. Leave LogonUI unchanged.
4. Leave Credential Providers unchanged.
5. Leave BCD unchanged.
6. Leave Ctrl+Alt+Delete under Windows control.
7. Never collect Windows credentials.
8. Permit Left Shift emergency bypass.
9. Permit one CASU watchdog restart only.
10. Fall back safely after the restart limit.
11. Have a deterministic independent removal path.
12. Never become required for Windows authentication or recovery.

## Session Boundary

A Windows service, if later used for orchestration, must not attempt
to provide the interactive CASU UI directly from Session 0.

Any future service component must be narrowly scoped to orchestration,
health monitoring, and controlled lifecycle operations.

Interactive presentation must use a separately validated interactive
session mechanism.

The exact live launch implementation remains subject to a later
installation gate and runtime proof.

## Current Decision

ARCHITECTURE_DIRECTION=SEPARATE_ORCHESTRATOR_AND_INTERACTIVE_RUNTIME
WINDOWS_SHELL_REPLACEMENT=PROHIBITED
USERINIT_REPLACEMENT=PROHIBITED
LOGONUI_REPLACEMENT=PROHIBITED
CREDENTIAL_PROVIDER_REPLACEMENT=PROHIBITED
AUTOADMINLOGON=PROHIBITED
BCD_PRESENTATION_MODIFICATION=PROHIBITED

## Installation State

CASU_WINDOWS_INTEGRATION_INSTALLED=FALSE
WINDOWS_SIGN_IN_RUNTIME_PROOF=PENDING

R3D7 DOES NOT authorize live Windows integration.
