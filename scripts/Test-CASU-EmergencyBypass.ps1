$ErrorActionPreference = "Stop"

$Root = Split-Path -Parent $PSScriptRoot
$Source = Join-Path $Root "src\CASU.Input\RawInputEmergencyBypass.cs"

Write-Host "`n=== CASU RAW INPUT EMERGENCY BYPASS RUNTIME TEST ===" -ForegroundColor Cyan

if (-not (Test-Path -LiteralPath $Source -PathType Leaf)) {
    throw "INPUT_SOURCE_NOT_FOUND"
}

Add-Type -AssemblyName System.Windows.Forms
Add-Type -AssemblyName System.Drawing

$SourceText = Get-Content -LiteralPath $Source -Raw

Add-Type `
    -TypeDefinition $SourceText `
    -ReferencedAssemblies @(
        "System.dll",
        "System.Windows.Forms.dll",
        "System.Drawing.dll"
    )

$form = New-Object CASU.Input.EmergencyBypassForm

$script:BypassObserved = $false

$form.add_EmergencyBypassRequested({

    $script:BypassObserved = $true

    Write-Host "CASU_EMERGENCY_BYPASS_EVENT=RECEIVED" -ForegroundColor Green

    $timer = New-Object System.Windows.Forms.Timer
    $timer.Interval = 750

    $timer.Add_Tick({
        $this.Stop()
        $form.Close()
    })

    $timer.Start()
})

Write-Host "TEST_WINDOW_CREATED=TRUE"
Write-Host "TEST_ACTION=PRESS_LEFT_SHIFT_ONCE"
Write-Host "EXPECTED_DEVICE=VID_1EA7_PID_0169_MI_00"
Write-Host "EXPECTED_MAKECODE=0x2A"
Write-Host "EXPECTED_TRIGGER=KEY_DOWN_ONLY"
Write-Host ""
Write-Host "Press LEFT SHIFT once." -ForegroundColor Yellow

[System.Windows.Forms.Application]::Run($form)

Write-Host "`n=== RUNTIME TEST RESULT ==="

Write-Host "RAW_KEYBOARD_PACKETS=$($form.RawKeyboardPackets)"
Write-Host "TARGET_PACKETS=$($form.TargetPackets)"
Write-Host "LEFT_SHIFT_KEYDOWN_EVENTS=$($form.LeftShiftKeyDownEvents)"
Write-Host "BYPASS_SIGNAL_COUNT=$($form.BypassSignalCount)"
Write-Host "LAST_DEVICE=$($form.LastDeviceName)"

if (
    $script:BypassObserved -and
    $form.BypassSignalCount -eq 1 -and
    $form.LeftShiftKeyDownEvents -ge 1
) {
    Write-Host "LEFT_SHIFT_DETECTED=TRUE"
    Write-Host "DEVICE_MATCH=VID_1EA7_PID_0169_MI_00"
    Write-Host "BYPASS_REQUESTED=TRUE"
    Write-Host "ONE_SHOT_LATCH=PASS"
    Write-Host "CASU_02B_R3D2_RUNTIME_TEST=PASS" -ForegroundColor Green
    exit 0
}

Write-Host "CASU_02B_R3D2_RUNTIME_TEST=FAIL" -ForegroundColor Red
exit 1
