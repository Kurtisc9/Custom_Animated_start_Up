# CASU 02B-R3D21 — Post-Reboot Validation

After the controlled reboot:

1. Observe whether Windows reaches sign-in normally.
2. Confirm Ctrl+Alt+Delete remains functional.
3. Confirm no black-screen or boot loop occurs.
4. Confirm CASU behavior does not block Windows authentication.
5. Sign in normally.
6. Open Administrator PowerShell.
7. Continue with the R3D21 post-reboot validation command provided by the CASU project task flow.

Emergency recovery script:

X:\03_Active_Projects\Custom_Animated_Start_Up\scripts\Disable-CASUOrchestrator-Emergency.ps1

If CASU causes an issue after sign-in, run the emergency recovery script from Administrator PowerShell.

Windows authentication remains authoritative.

This build does not authorize Credential Provider, Winlogon Shell,
Userinit, firmware, driver, or BCD modifications.
