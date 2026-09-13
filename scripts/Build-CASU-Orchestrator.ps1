param(
    [Parameter(Mandatory=$true)]
    [string]$OutputPath
)

$ErrorActionPreference = "Stop"

$Root = "X:\03_Active_Projects\Custom_Animated_Start_Up"

Set-Location -LiteralPath $Root

if ((Get-Location).Path -ne $Root) {
    throw "PROJECT_PATH_LOCK_FAILED"
}

$Source =
    Join-Path $Root "src\CASU.Orchestrator\CasuWindowsServiceHost.cs"

if (-not (Test-Path -LiteralPath $Source -PathType Leaf)) {
    throw "CASU_ORCHESTRATOR_SOURCE_MISSING"
}

$CompilerCandidates = @(
    "$env:WINDIR\Microsoft.NET\Framework64\v4.0.30319\csc.exe",
    "$env:WINDIR\Microsoft.NET\Framework\v4.0.30319\csc.exe"
)

$Csc =
    $CompilerCandidates |
    Where-Object {
        Test-Path -LiteralPath $_
    } |
    Select-Object -First 1

if (-not $Csc) {
    throw "CSHARP_COMPILER_NOT_FOUND"
}

$OutputDir =
    Split-Path -Parent $OutputPath

if (-not $OutputDir) {
    throw "INVALID_OUTPUT_PATH=$OutputPath"
}

if (-not (Test-Path -LiteralPath $OutputDir)) {

    New-Item `
        -ItemType Directory `
        -Path $OutputDir `
        -Force |
        Out-Null
}

& $Csc `
    /nologo `
    /target:exe `
    /out:$OutputPath `
    /reference:System.dll `
    /reference:System.Core.dll `
    /reference:System.ServiceProcess.dll `
    $Source

if ($LASTEXITCODE -ne 0) {
    throw "CASU_ORCHESTRATOR_COMPILE_FAILED"
}

if (-not (Test-Path -LiteralPath $OutputPath -PathType Leaf)) {
    throw "CASU_ORCHESTRATOR_BINARY_MISSING"
}

Write-Host "CASU_ORCHESTRATOR_COMPILE=PASS"
Write-Host "CASU_ORCHESTRATOR_OUTPUT=$OutputPath"
