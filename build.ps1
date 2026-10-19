# Taç ve Hançer derleme: önce testler, sonra mod (DeployToGame oyuna kopyalar).
# Gerekenler: .NET SDK 8 ya da üstü (ücretsiz) ve ilk derlemede internet (NuGet paketleri bir kez iner).
# Oyun klasörü farklıysa: .\build.ps1 -p:GameFolder="D:\Oyunlar\Mount & Blade II Bannerlord"
$ErrorActionPreference = 'Stop'
if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    throw '.NET SDK bulunamadi. Kur: winget install Microsoft.DotNet.SDK.8  (ya da Visual Studio Installer > ".NET masaustu gelistirme")'
}

dotnet build (Join-Path $PSScriptRoot 'tests\CheatMenu.Tests\CheatMenu.Tests.csproj') -c Release -f net472 -nologo -v:minimal
if ($LASTEXITCODE -ne 0) { throw 'Test derlemesi basarisiz' }
& (Join-Path $PSScriptRoot 'tests\CheatMenu.Tests\bin\Release\net472\CheatMenu.Tests.exe')
if ($LASTEXITCODE -ne 0) { throw 'Testler basarisiz' }

dotnet build (Join-Path $PSScriptRoot 'src\CheatMenu.csproj') -c Release -nologo -v:minimal @args
if ($LASTEXITCODE -ne 0) { throw 'Mod derlemesi basarisiz' }
