# CheatMenu build: tests first (if present), then the mod (DeployToGame copies it into the game).
$ErrorActionPreference = 'Stop'
$vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
$msbuild = & $vswhere -latest -requires Microsoft.Component.MSBuild -find 'MSBuild\**\Bin\MSBuild.exe' | Select-Object -First 1
if (-not $msbuild) { throw 'MSBuild not found (is Visual Studio installed?)' }

function Invoke-Build([string]$project) {
    & $msbuild $project -nologo -v:minimal -p:Configuration=Release
    if ($LASTEXITCODE -ne 0) { throw "Build failed: $project" }
}

$testProject = Join-Path $PSScriptRoot 'tests\CheatMenu.Tests\CheatMenu.Tests.csproj'
if (Test-Path $testProject) {
    Invoke-Build $testProject
    & (Join-Path $PSScriptRoot 'tests\CheatMenu.Tests\bin\Release\CheatMenu.Tests.exe')
    if ($LASTEXITCODE -ne 0) { throw 'Tests failed' }
}

Invoke-Build (Join-Path $PSScriptRoot 'src\CheatMenu.csproj')
