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

# Arayüz dosyaları (Gauntlet prefab'ları) oyunun referans DLL'lerine ve ViewModel'lere karşı denetlenir.
# Oyun kuruluysa kullandığımız oyun sprite'ları ve fırçaları da oyunun kendi dosyalarında doğrulanır.
$packages = if ($env:NUGET_PACKAGES) { $env:NUGET_PACKAGES } else { Join-Path $env:USERPROFILE '.nuget\packages' }
$gameFolder = 'C:\Program Files (x86)\Steam\steamapps\common\Mount & Blade II Bannerlord'
$gameArgs = if (Test-Path (Join-Path $gameFolder 'Modules\Native')) { @('--game', $gameFolder) } else { @() }
dotnet run --project (Join-Path $PSScriptRoot 'tools\GuiCheck\GuiCheck.csproj') -c Release -- `
    (Join-Path $PSScriptRoot 'Module\GUI') (Join-Path $PSScriptRoot 'src\bin\Release\CheatMenu.dll') `
    (Join-Path $packages 'bannerlord.referenceassemblies.core\1.4.8.119303\ref\net472') `
    (Join-Path $packages 'bannerlord.referenceassemblies.sandbox\1.4.8.119303\ref\net472') `
    (Join-Path $packages 'bannerlord.uiextenderex\2.13.3\lib\netstandard2.0') @gameArgs
if ($LASTEXITCODE -ne 0) { throw 'Arayuz denetimi basarisiz: oyunda Entrika ekrani acilmayabilir' }
