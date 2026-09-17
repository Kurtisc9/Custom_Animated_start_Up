# CASU_THEME_01-I2 Production Renderer Selection

DATE:
2026-09-16 20:34:14 -04:00

APPROVED BY:
KurtisC

FINAL APPROVAL AUTHORITY:
KurtisC

SOURCE HEAD:
ceac77e0183e77fbac4e5075da985fa3d33c5492

## Selected Production Renderer Architecture

RENDERER_API=Direct3D_11

DOTNET_BINDING=Vortice.Windows

LANGUAGE=CSharp

OPERATING_SYSTEM=Windows_11

## Architecture Decision

CASU will move from the preserved WinForms/System.Drawing technical
prototype to a GPU-accelerated Direct3D 11 production renderer using
Vortice.Windows for managed DirectX interoperability.

The existing GDI renderer remains preserved as technical evidence and
fallback development reference until the new renderer passes all
required validation gates.

## Why This Architecture Fits CASU

The renderer must support the locked Neural Core requirements:

- GPU acceleration
- real-time 3D geometry
- HLSL shaders
- metallic materials
- dark-glass materials
- emissive materials
- particle systems
- cinematic camera movement
- multi-display rendering
- high frame-rate animation
- custom post-processing
- controlled visual randomization
- low-level Windows integration

Direct3D 11 provides a native Windows graphics path suitable for CASU.

Vortice.Windows provides managed .NET/C# access while allowing CASU to
retain its existing C# architecture.

## Preserved Architecture

The following remain unchanged:

- Windows authentication authority
- Left Shift emergency bypass
- Ctrl+Alt+Delete behavior
- watchdog maximum restart of one
- safe Windows fallback
- Session 0 contains no UI
- interactive-session presentation ownership
- CASUOrchestrator service architecture
- Monitor 2 focal-display rule
- five-display environment
- recovery architecture

## Locked Boundary

RENDERER_TECHNOLOGY=APPROVED_BY_KURTISC

RENDERER_TECHNOLOGY=LOCKED

PRODUCTION_GRAPHICS_API=DIRECT3D_11

DOTNET_GRAPHICS_BINDING=VORTICE_WINDOWS

CURRENT_GDI_RENDERER=TECHNICAL_PROTOTYPE_ONLY

DEPENDENCY_INSTALLATION=NOT_PERFORMED

DIRECT3D_IMPLEMENTATION=NOT_STARTED

PRODUCTION_RENDERER_REPLACEMENT=NOT_STARTED

PERSISTENT_INTEGRATION=NOT_STARTED

PRODUCTION_DEPLOYMENT=NO

WINDOWS_AUTHENTICATION_CHANGES=NONE

CREDENTIAL_CAPTURE=NO

CREDENTIAL_PROVIDER_CHANGES=NONE

WINLOGON_CHANGES=NONE

BCD_CHANGES=NONE

FIRMWARE_CHANGES=NONE

DRIVER_CHANGES=NONE

REBOOT=NO

## Next Task

CASU_THEME_01-I2-ARCH3

Direct3D 11 / Vortice.Windows dependency acquisition,
isolated renderer foundation and smoke-test preparation.

ARCH3 must:

1. preserve the existing technical prototype
2. verify the installed .NET environment
3. establish dependency versions explicitly
4. verify package licensing
5. establish isolated Direct3D renderer source structure
6. compile before integration
7. perform a GPU initialization smoke test
8. avoid production deployment
9. avoid CASUOrchestrator replacement
10. avoid reboot

STATUS=APPROVED_AND_LOCKED
NEXT_TASK=CASU_THEME_01-I2-ARCH3
