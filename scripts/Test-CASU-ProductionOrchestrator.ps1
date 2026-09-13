& {
$ErrorActionPreference = "Stop"

$Root =
    "X:\03_Active_Projects\Custom_Animated_Start_Up"

$ServiceName =
    "CASU_R3D17_ProductionHost_Probe"

$TempRoot =
    "C:\ProgramData\CASU\R3D17_Temp"

$ExePath =
    Join-Path $TempRoot "CASUOrchestrator.exe"

$EvidencePath =
    Join-Path $TempRoot "R3D17.result.txt"

$BuildScript =
    Join-Path $Root "scripts\Build-CASU-Orchestrator.ps1"

Set-Location -LiteralPath $Root

if (Test-Path -LiteralPath $TempRoot) {
    Remove-Item `
        -LiteralPath $TempRoot `
        -Recurse `
        -Force
}

New-Item `
    -ItemType Directory `
    -Path $TempRoot `
    -Force |
    Out-Null

& powershell.exe `
    -NoProfile `
    -ExecutionPolicy Bypass `
    -File $BuildScript `
    -OutputPath $ExePath

if ($LASTEXITCODE -ne 0) {
    throw "R3D17_BUILD_FAILED"
}

$Existing =
    Get-Service `
        -Name $ServiceName `
        -ErrorAction SilentlyContinue

if ($Existing) {

    if ($Existing.Status -ne "Stopped") {
        Stop-Service `
            -Name $ServiceName `
            -Force `
            -ErrorAction SilentlyContinue
    }

    & sc.exe delete $ServiceName |
        Out-Null

    Start-Sleep -Seconds 2
}

try {

    $BinaryPath =
        "`"$ExePath`" --service-name=$ServiceName --evidence=`"$EvidencePath`""

    & sc.exe create `
        $ServiceName `
        "binPath=" `
        $BinaryPath `
        "start=" `
        "demand" `
        "obj=" `
        "LocalSystem" |
        Out-Host

    if ($LASTEXITCODE -ne 0) {
        throw "R3D17_TEMP_SERVICE_CREATE_FAILED"
    }

    & sc.exe start $ServiceName |
        Out-Host

    if ($LASTEXITCODE -ne 0) {
        throw "R3D17_TEMP_SERVICE_START_FAILED"
    }

    $Deadline =
        (Get-Date).AddSeconds(30)

    while ((Get-Date) -lt $Deadline) {

        if (Test-Path -LiteralPath $EvidencePath) {

            $Text =
                Get-Content `
                    -LiteralPath $EvidencePath `
                    -Raw

            if (
                $Text -match
                "R3D17_RUNTIME_PROBE_COMPLETED=TRUE"
            ) {
                break
            }
        }

        Start-Sleep -Milliseconds 500
    }

    if (-not (Test-Path -LiteralPath $EvidencePath)) {
        throw "R3D17_EVIDENCE_MISSING"
    }

    $Lines =
        @(Get-Content -LiteralPath $EvidencePath)

    $Text =
        $Lines -join "`n"

    $Lines |
        ForEach-Object {
            Write-Host $_
        }

    $Required = @(
        "SERVICE_ONSTART=PASS",
        "SERVICE_IDENTITY=NT AUTHORITY\SYSTEM",
        "SERVICE_SESSION_ID=0",
        "ACTIVE_SESSION_TARGET_RESOLUTION=PASS",
        "USER_TOKEN_ACQUISITION=PASS",
        "TOKEN_DUPLICATION=PASS",
        "ENVIRONMENT_BLOCK_CREATION=PASS",
        "CROSS_SESSION_PROCESS_CREATION=PASS",
        "TARGET_SESSION_MATCH=PASS",
        "CHILD_EXIT_CODE=0",
        "INTERACTIVE_RUNTIME_LAUNCH=PASS",
        "THREAD_HANDLE_CLEANUP=True",
        "PROCESS_HANDLE_CLEANUP=True",
        "ENVIRONMENT_BLOCK_CLEANUP=True",
        "PRIMARY_TOKEN_HANDLE_CLEANUP=True",
        "USER_TOKEN_HANDLE_CLEANUP=True",
        "R3D17_RUNTIME_PROBE_COMPLETED=TRUE",
        "ORCHESTRATOR_WORKER_STATE=RUNNING"
    )

    foreach ($Token in $Required) {

        if (
            $Text -notmatch
            [regex]::Escape($Token)
        ) {
            throw "R3D17_RESULT_MISSING=$Token"
        }

        Write-Host "VALIDATED=$Token"
    }

    $Service =
        Get-Service `
            -Name $ServiceName `
            -ErrorAction Stop

    if ($Service.Status -ne "Running") {
        throw "R3D17_SERVICE_NOT_RUNNING_AFTER_PROBE"
    }

    Write-Host "SERVICE_RUNNING_AFTER_PROBE=PASS"
    Write-Host "PRODUCTION_HOST_TEMPORARY_TEST=PASS"
}
finally {

    $Service =
        Get-Service `
            -Name $ServiceName `
            -ErrorAction SilentlyContinue

    if ($Service) {

        if ($Service.Status -ne "Stopped") {

            Stop-Service `
                -Name $ServiceName `
                -Force `
                -ErrorAction SilentlyContinue
        }

        Start-Sleep -Seconds 1

        & sc.exe delete $ServiceName |
            Out-Host

        Start-Sleep -Seconds 2
    }

    if (
        Get-Service `
            -Name $ServiceName `
            -ErrorAction SilentlyContinue
    ) {
        throw "R3D17_TEMP_SERVICE_REMOVAL_FAILED"
    }

    Write-Host "TEMP_SERVICE_REMOVED=TRUE"
}

if (Test-Path -LiteralPath $TempRoot) {

    Remove-Item `
        -LiteralPath $TempRoot `
        -Recurse `
        -Force
}

if (Test-Path -LiteralPath $TempRoot) {
    throw "R3D17_TEMP_ARTIFACT_REMOVAL_FAILED"
}

Write-Host "TEMP_ARTIFACTS_REMOVED=TRUE"
Write-Host "CASU_02B_R3D17_PRODUCTION_HOST_TEST=PASS"
}
