# CASU_02B-R3D6 — Windows Pre-Sign-In Integration Design

## Status

DESIGNED / NOT INSTALLED

## Canonical Project Root

X:\03_Active_Projects\Custom_Animated_Start_Up

## Authentication Boundary

Windows remains the sole authentication authority.

CASU must not collect, process, store, forward, or validate Windows
passwords, PINs, biometric credentials, authentication secrets, or
Credential Provider data.

CASU must not replace:

- Winlogon
- LogonUI
- Windows Credential Providers
- Windows authentication

Ctrl+Alt+Delete remains Windows-controlled.

## Runtime Sequence

1. Windows boots normally.
2. Windows bootloader remains unchanged.
3. Required Windows system components initialize normally.
4. CASU starts only through a separately approved integration mechanism.
5. CASU discovers connected displays.
6. All connected displays initialize simultaneously.
7. Monitor 2 is primary when connected.
8. Unified randomized real-time CASU presentation begins.
9. Left Shift emergency bypass remains active.
10. Independent watchdog monitors CASU.
11. CASU transitions toward the Windows authentication presentation.
12. Windows performs authentication.
13. Ctrl+Alt+Delete remains available and Windows-controlled.
14. CASU transitions out.
15. Windows desktop continues normally.

## Emergency Bypass

Left Shift

Verified target:

VID_1EA7
PID_0169
MI_00
MakeCode 0x2A

## Watchdog Policy

Maximum CASU restart count:

1

First CASU failure:

One controlled CASU restart.

Second CASU runtime failure:

No additional CASU restart.
Safe fallback to Windows continuation is required.

## Failsafe Model

Level 1:
Left Shift emergency bypass.

Level 2:
Runtime watchdog with maximum one restart.

Level 3:
Independent safe fallback to normal Windows continuation.

## Prohibited Integration Methods

CASU must not:

- permanently replace explorer.exe
- replace winlogon.exe
- replace LogonUI.exe
- replace Windows Credential Providers
- collect passwords
- collect PINs
- intercept or disable Ctrl+Alt+Delete
- enable AutoAdminLogon
- store DefaultPassword
- modify BCD for cosmetic presentation
- disable Windows Recovery
- disable Safe Mode
- disable Windows sign-in
- create an unrecoverable boot dependency

## Installation Gate

R3D6 DOES NOT authorize installation.

CASU_WINDOWS_INTEGRATION_INSTALLED=FALSE
WINDOWS_SIGN_IN_RUNTIME_PROOF=PENDING
