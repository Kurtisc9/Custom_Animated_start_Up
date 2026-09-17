# CASU_THEME_01-I2 Renderer Technology Decision

DATE: 2026-09-16 20:32:37 -04:00

SOURCE HEAD:
543565df887ad8c3f5d20846b296362015c25306

BRANCH:
phase-02b-r3d-emergency-bypass

## Locked Design Authority

CASU_THEME_01 — KurtisC Neural Core

VISUAL_DESIGN=APPROVED
VISUAL_DESIGN=LOCKED

## Current Renderer Evidence

WinForms detected: True

System.Drawing detected: True

Classification:

WINFORMS_GDI_TECHNICAL_PROTOTYPE

Existing GPU/3D dependencies:

NONE DETECTED

## Production Visual Requirements

The locked Neural Core visual design requires:

- genuine GPU rendering
- real-time 3D geometry
- shader support
- volumetric effects
- physically coherent lighting
- advanced particles
- cinematic camera system
- multi-display rendering
- smooth cinematic transitions
- controlled randomization
- metallic materials
- dark-glass materials
- emissive materials

## Architecture Finding

The existing WinForms/System.Drawing renderer remains preserved as a
technical prototype.

It proves important CASU runtime behavior including:

- display ownership
- window lifecycle
- multi-display targeting
- interactive-session execution
- runtime validation

It is not the approved production visual-quality renderer.

The locked Neural Core target requires a GPU-accelerated rendering
architecture capable of delivering the approved real-time 3D design.

## Decision Boundary

CURRENT_GDI_RENDERER=TECHNICAL_PROTOTYPE_ONLY

GPU_ACCELERATED_RENDERER=REQUIRED_FOR_PRODUCTION_TARGET

RENDERER_REPLACEMENT=NOT_AUTHORIZED_BY_THIS_TASK

DEPENDENCY_INSTALLATION=NOT_AUTHORIZED_BY_THIS_TASK

PRODUCTION_DEPLOYMENT=NOT_AUTHORIZED

WINDOWS_AUTHENTICATION_CHANGES=NONE

CREDENTIAL_CAPTURE=NO

CREDENTIAL_PROVIDER_CHANGES=NONE

WINLOGON_CHANGES=NONE

BCD_CHANGES=NONE

FIRMWARE_CHANGES=NONE

DRIVER_CHANGES=NONE

REBOOT=NO

## Required Technology Selection Criteria

The production renderer selection must evaluate:

1. Windows 11 compatibility
2. .NET/C# integration
3. GPU acceleration
4. Direct3D capability
5. shader support
6. particle capability
7. volumetric-effect capability
8. multi-display performance
9. startup footprint
10. runtime footprint
11. packaging
12. licensing
13. maintenance status
14. security exposure
15. dependency size
16. recovery compatibility
17. fallback compatibility
18. preservation of CASU safety architecture

## Gate

STATUS=ARCHITECTURE_EVIDENCE_COMPLETE

RENDERER_TECHNOLOGY_SELECTION=PENDING_KURTISC

NEXT_GATE=KURTISC_RENDERER_TECHNOLOGY_SELECTION_APPROVAL
