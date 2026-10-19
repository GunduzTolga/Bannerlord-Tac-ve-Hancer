# CheatMenu (Bannerlord Hile Menüsü) Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Bannerlord v1.4.8 Campaign'inde Insert tuşuyla açılan, oyunun kendi popup'larını kullanan, bağımlılıksız bir hile menüsü modu.

**Architecture:** `MBSubModuleBase` giriş noktası tuşu dinler ve iki genişletme noktası kaydeder: zincirlenen bir `PartySpeedModel` (hız ×2) ve her savaşa eklenen bir `MissionLogic` (ölümsüzlük / tek vuruş / cephane). Menü `MBInformationManager.ShowMultiSelectionInquiry` + `InformationManager.ShowTextInquiry` ile kurulur; popup içinden popup açma çakışmasını önlemek için "sonraki kare" kuyruğu kullanılır. Aç/kapa durumu statiktir, save'e yazılmaz.

**Tech Stack:** C# 9, .NET Framework 4.8.1 (oyun host'u 4.7.2, TaleWorlds DLL'leri netstandard2.0), SDK-style OLMAYAN csproj + VS 2026 MSBuild 18 (makinede .NET SDK yok), oyun DLL'lerine `Private=false` referans.

**Spec:** `docs/superpowers/specs/2026-10-18-bannerlord-cheat-menu-design.md`

**Spec'ten bilinçli sapma:** Spec'teki `MenuUI.TryParseAmount` → `AmountParser.TryParse` (oyun API'sine bağımlı olmayan ayrı dosya), çünkü böylece oyunsuz bir test runner'ında test edilebiliyor.

**Git yok:** Proje klasörü git deposu değil ve kullanıcı commit istemedi; bu yüzden commit adımı yok. Her görevin sonunda "derleme yeşil" kontrol noktası var.

**Oyun açıkken derleme:** Oyun çalışırken `CheatMenu.dll` kilitlidir; DeployToGame adımı "being used by another process" hatası verir. Derlemeden önce oyunu kapat.

---

## Dosya yapısı

```
Desktop\Bannerlord\
├─ build.ps1                                 testleri derle+çalıştır, sonra modu derle+oyuna kopyala
├─ Module\SubModule.xml                      mod manifesti
├─ src\
│  ├─ CheatMenu.csproj                       mod projesi (+ DeployToGame hedefi)
│  ├─ SubModule.cs                           giriş noktası: tuş, model, mission logic, guard kablolaması
│  ├─ CheatState.cs                          aç/kapa bayrakları
│  ├─ AmountParser.cs                        "özel miktar" metni → int (saf mantık, test edilir)
│  ├─ CleanSaveGuard.cs                      temiz kayıt / başarım uyarısı
│  ├─ Menu\MenuEntry.cs                      menü satırı türleri (Simple, Custom, Toggle, Submenu)
│  ├─ Menu\MenuUI.cs                         popup yardımcıları, SafeRun, Defer kuyruğu, mesajlar
│  ├─ Menu\Categories.cs                     ana menü + 4 kategori içeriği
│  ├─ Cheats\CampaignCheats.cs               tek seferlik kampanya hileleri
│  ├─ Cheats\CheatPartySpeedModel.cs         hız ×2 modeli
│  ├─ Cheats\CheatTargets.cs                 savaşta "kim etkilenir" kuralı (kullanıcı katkısı)
│  └─ Cheats\CheatMissionLogic.cs            savaş hileleri
└─ tests\CheatMenu.Tests\
   ├─ CheatMenu.Tests.csproj                 konsol test runner'ı (NuGet yok)
   └─ Program.cs                             AmountParser testleri
```

`CheatMenu.csproj` `**\*.cs` joker karakteri kullanır; yeni dosya eklemek csproj değişikliği gerektirmez.

---

### Task 1: Proje iskeleti ve derleme hattı

**Files:**
- Create: `Module/SubModule.xml`
- Create: `src/CheatMenu.csproj`
- Create: `src/SubModule.cs` (geçici, Task 9'da son hâline gelir)
- Create: `build.ps1`

- [ ] **Step 1: Mod manifestini yaz**

`Module/SubModule.xml`:

```xml
<?xml version="1.0" encoding="utf-8"?>
<Module>
	<Name value="CheatMenu"/>
	<Id value="CheatMenu"/>
	<Version value="v1.0.0"/>
	<SingleplayerModule value="true"/>
	<MultiplayerModule value="false"/>
	<DefaultModule value="false"/>
	<ModuleCategory value="Singleplayer"/>
	<ModuleType value="Community"/>
	<DependedModules>
		<DependedModule Id="Native" DependentVersion="v1.4.8" Optional="false"/>
		<DependedModule Id="SandBoxCore" DependentVersion="v1.4.8" Optional="false"/>
		<DependedModule Id="Sandbox" DependentVersion="v1.4.8" Optional="false"/>
	</DependedModules>
	<Xmls>
	</Xmls>
	<SubModules>
		<SubModule>
			<Name value="CheatMenu"/>
			<DLLName value="CheatMenu.dll"/>
			<SubModuleClassType value="CheatMenu.SubModule"/>
			<Tags>
				<Tag key="DedicatedServerType" value="none"/>
			</Tags>
		</SubModule>
	</SubModules>
</Module>
```

- [ ] **Step 2: Mod projesini yaz**

`src/CheatMenu.csproj`:

```xml
<?xml version="1.0" encoding="utf-8"?>
<Project ToolsVersion="Current" DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <Import Project="$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props" Condition="Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')" />
  <PropertyGroup>
    <Configuration Condition=" '$(Configuration)' == '' ">Release</Configuration>
    <Platform Condition=" '$(Platform)' == '' ">AnyCPU</Platform>
    <OutputType>Library</OutputType>
    <RootNamespace>CheatMenu</RootNamespace>
    <AssemblyName>CheatMenu</AssemblyName>
    <TargetFrameworkVersion>v4.8.1</TargetFrameworkVersion>
    <LangVersion>9.0</LangVersion>
    <PlatformTarget>x64</PlatformTarget>
    <OutputPath>bin\$(Configuration)\</OutputPath>
    <Optimize>true</Optimize>
    <DebugSymbols>true</DebugSymbols>
    <DebugType>portable</DebugType>
    <Deterministic>true</Deterministic>
    <GameFolder Condition=" '$(GameFolder)' == '' ">C:\Program Files (x86)\Steam\steamapps\common\Mount &amp; Blade II Bannerlord</GameFolder>
    <GameBin>$(GameFolder)\bin\Win64_Shipping_Client</GameBin>
    <ModuleOut>$(GameFolder)\Modules\CheatMenu</ModuleOut>
  </PropertyGroup>
  <ItemGroup>
    <Reference Include="System" />
    <Reference Include="System.Core" />
    <Reference Include="TaleWorlds.CampaignSystem"><HintPath>$(GameBin)\TaleWorlds.CampaignSystem.dll</HintPath><Private>False</Private></Reference>
    <Reference Include="TaleWorlds.Core"><HintPath>$(GameBin)\TaleWorlds.Core.dll</HintPath><Private>False</Private></Reference>
    <Reference Include="TaleWorlds.DotNet"><HintPath>$(GameBin)\TaleWorlds.DotNet.dll</HintPath><Private>False</Private></Reference>
    <Reference Include="TaleWorlds.Engine"><HintPath>$(GameBin)\TaleWorlds.Engine.dll</HintPath><Private>False</Private></Reference>
    <Reference Include="TaleWorlds.InputSystem"><HintPath>$(GameBin)\TaleWorlds.InputSystem.dll</HintPath><Private>False</Private></Reference>
    <Reference Include="TaleWorlds.Library"><HintPath>$(GameBin)\TaleWorlds.Library.dll</HintPath><Private>False</Private></Reference>
    <Reference Include="TaleWorlds.Localization"><HintPath>$(GameBin)\TaleWorlds.Localization.dll</HintPath><Private>False</Private></Reference>
    <Reference Include="TaleWorlds.ModuleManager"><HintPath>$(GameBin)\TaleWorlds.ModuleManager.dll</HintPath><Private>False</Private></Reference>
    <Reference Include="TaleWorlds.MountAndBlade"><HintPath>$(GameBin)\TaleWorlds.MountAndBlade.dll</HintPath><Private>False</Private></Reference>
    <Reference Include="TaleWorlds.ObjectSystem"><HintPath>$(GameBin)\TaleWorlds.ObjectSystem.dll</HintPath><Private>False</Private></Reference>
    <Reference Include="TaleWorlds.SaveSystem"><HintPath>$(GameBin)\TaleWorlds.SaveSystem.dll</HintPath><Private>False</Private></Reference>
  </ItemGroup>
  <ItemGroup>
    <Compile Include="**\*.cs" Exclude="bin\**;obj\**" />
  </ItemGroup>
  <Import Project="$(MSBuildToolsPath)\Microsoft.CSharp.targets" />
  <Target Name="DeployToGame" AfterTargets="Build">
    <MakeDir Directories="$(ModuleOut)\bin\Win64_Shipping_Client" />
    <Copy SourceFiles="$(MSBuildProjectDirectory)\..\Module\SubModule.xml" DestinationFolder="$(ModuleOut)" />
    <Copy SourceFiles="$(TargetPath);$(TargetDir)$(TargetName).pdb" DestinationFolder="$(ModuleOut)\bin\Win64_Shipping_Client" />
    <Message Importance="high" Text="CheatMenu -> $(ModuleOut)" />
  </Target>
</Project>
```

- [ ] **Step 3: Geçici giriş noktasını yaz**

`src/SubModule.cs`:

```csharp
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace CheatMenu
{
    public sealed class SubModule : MBSubModuleBase
    {
        private static bool _loadMessageShown;

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            base.OnBeforeInitialModuleScreenSetAsRoot();
            if (_loadMessageShown) return;
            _loadMessageShown = true;
            InformationManager.DisplayMessage(new InformationMessage("[CheatMenu] yüklendi"));
        }
    }
}
```

- [ ] **Step 4: Derleme betiğini yaz**

`build.ps1` (mesajlar ASCII: PowerShell 5.1 BOM'suz UTF-8'i yanlış okur):

```powershell
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
```

- [ ] **Step 5: Derle**

Run: `powershell -NoProfile -ExecutionPolicy Bypass -File C:\Users\tolgaa\Desktop\Bannerlord\build.ps1`
Expected: `CheatMenu -> C:\Program Files (x86)\Steam\steamapps\common\Mount & Blade II Bannerlord\Modules\CheatMenu`, uyarı yok, çıkış kodu 0.

- [ ] **Step 6: Kurulumu doğrula**

Run: `ls "/c/Program Files (x86)/Steam/steamapps/common/Mount & Blade II Bannerlord/Modules/CheatMenu" "/c/Program Files (x86)/Steam/steamapps/common/Mount & Blade II Bannerlord/Modules/CheatMenu/bin/Win64_Shipping_Client"`
Expected: `SubModule.xml`, `bin`; altında `CheatMenu.dll`, `CheatMenu.pdb`.

---

### Task 2: AmountParser (TDD)

**Files:**
- Create: `tests/CheatMenu.Tests/CheatMenu.Tests.csproj`
- Create: `tests/CheatMenu.Tests/Program.cs`
- Create: `src/AmountParser.cs`

- [ ] **Step 1: Test projesini yaz**

`tests/CheatMenu.Tests/CheatMenu.Tests.csproj`:

```xml
<?xml version="1.0" encoding="utf-8"?>
<Project ToolsVersion="Current" DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <Import Project="$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props" Condition="Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')" />
  <PropertyGroup>
    <Configuration Condition=" '$(Configuration)' == '' ">Release</Configuration>
    <Platform Condition=" '$(Platform)' == '' ">AnyCPU</Platform>
    <OutputType>Exe</OutputType>
    <RootNamespace>CheatMenu.Tests</RootNamespace>
    <AssemblyName>CheatMenu.Tests</AssemblyName>
    <TargetFrameworkVersion>v4.8.1</TargetFrameworkVersion>
    <LangVersion>9.0</LangVersion>
    <OutputPath>bin\$(Configuration)\</OutputPath>
  </PropertyGroup>
  <ItemGroup>
    <Reference Include="System" />
  </ItemGroup>
  <ItemGroup>
    <Compile Include="Program.cs" />
    <Compile Include="..\..\src\AmountParser.cs"><Link>AmountParser.cs</Link></Compile>
  </ItemGroup>
  <Import Project="$(MSBuildToolsPath)\Microsoft.CSharp.targets" />
</Project>
```

- [ ] **Step 2: Başarısız testleri yaz**

`tests/CheatMenu.Tests/Program.cs` (konsol çıktısı ASCII):

```csharp
using System;

namespace CheatMenu.Tests
{
    internal static class Program
    {
        private static int _failures;

        private static int Main()
        {
            // Temel sözleşme: pozitif tam sayı kabul, gerisi red.
            Expect("500", true, 500);
            Expect("  42  ", true, 42);
            Expect("", false);
            Expect("   ", false);
            Expect(null, false);
            Expect("abc", false);
            Expect("0", false);
            Expect("99999999999", false); // int taşması

            Console.WriteLine(_failures == 0 ? "ALL TESTS PASSED" : _failures + " TEST(S) FAILED");
            return _failures == 0 ? 0 : 1;
        }

        private static void Expect(string input, bool expectedOk, int expectedValue = 0)
        {
            bool ok = AmountParser.TryParse(input, out int value);
            bool pass = ok == expectedOk && (!expectedOk || value == expectedValue);
            if (!pass) _failures++;
            Console.WriteLine((pass ? "PASS " : "FAIL ") + "\"" + input + "\" -> ok=" + ok + ", value=" + value
                + " (expected ok=" + expectedOk + ", value=" + expectedValue + ")");
        }
    }
}
```

- [ ] **Step 3: Testin başarısız olduğunu gör**

Run: `powershell -NoProfile -ExecutionPolicy Bypass -File C:\Users\tolgaa\Desktop\Bannerlord\build.ps1`
Expected: FAIL — `CSC : error CS2001: Source file '...\src\AmountParser.cs' could not be found` ve `Build failed: ...CheatMenu.Tests.csproj`.

- [ ] **Step 4: En küçük uygulamayı yaz**

`src/AmountParser.cs`:

```csharp
using System.Globalization;

namespace CheatMenu
{
    /// <summary>
    /// "Özel miktar" kutusuna yazılan metni sayıya çevirir. Oyun API'sine bağımlı değildir;
    /// tests/CheatMenu.Tests altında oyunsuz test edilir.
    /// </summary>
    public static class AmountParser
    {
        public static bool TryParse(string input, out int value)
        {
            value = 0;
            if (string.IsNullOrWhiteSpace(input)) return false;
            if (!int.TryParse(input.Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out int parsed)) return false;
            if (parsed <= 0) return false;
            value = parsed;
            return true;
        }
    }
}
```

- [ ] **Step 5: Testlerin geçtiğini gör**

Run: `powershell -NoProfile -ExecutionPolicy Bypass -File C:\Users\tolgaa\Desktop\Bannerlord\build.ps1`
Expected: 8 satır `PASS`, `ALL TESTS PASSED`, ardından `CheatMenu -> ...Modules\CheatMenu`.

---

### Task 3: CheatState + menü altyapısı

**Files:**
- Create: `src/CheatState.cs`
- Create: `src/Menu/MenuEntry.cs`
- Create: `src/Menu/MenuUI.cs`

- [ ] **Step 1: Aç/kapa durumunu yaz**

`src/CheatState.cs`:

```csharp
namespace CheatMenu
{
    /// <summary>
    /// Aç/kapa hilelerin durumu. Bilerek statik ve save'e yazılmıyor:
    /// mod kaldırılsa bile kayıt dosyaları bu moda bağımlı hâle gelmez.
    /// </summary>
    public static class CheatState
    {
        public static bool GodModeMe;
        public static bool GodModeArmy;
        public static bool OneHitMe;
        public static bool OneHitArmy;
        public static bool InfiniteAmmoMe;
        public static bool InfiniteAmmoArmy;
        public static bool FastParty;

        public static void ResetAll()
        {
            GodModeMe = GodModeArmy = false;
            OneHitMe = OneHitArmy = false;
            InfiniteAmmoMe = InfiniteAmmoArmy = false;
            FastParty = false;
        }
    }
}
```

- [ ] **Step 2: Menü satırı türlerini yaz**

`src/Menu/MenuEntry.cs`:

```csharp
using System;

namespace CheatMenu.Menu
{
    /// <summary>
    /// Menüdeki tek satır. Run, menüyü yeniden açan "reopen" geri çağrısını alır; böylece kendi
    /// popup'ını açan eylemler (özel miktar) işleri bitince menüye kendileri dönebilir.
    /// </summary>
    public sealed class MenuEntry
    {
        private MenuEntry(string label, Action<Action> run)
        {
            Label = label;
            Run = run;
        }

        public string Label { get; }
        public Action<Action> Run { get; }

        /// <summary>Tek tıkla çalışan eylem; ardından aynı menü yeniden açılır.</summary>
        public static MenuEntry Simple(string label, Action action) =>
            new MenuEntry(label, reopen =>
            {
                MenuUI.SafeRun(label, action);
                reopen();
            });

        /// <summary>Sayı soran eylem; girilen miktarla apply çalışır, ardından menü yeniden açılır.</summary>
        public static MenuEntry Custom(string label, Action<int> apply) =>
            new MenuEntry(label, reopen => MenuUI.AskNumber(label, apply, reopen));

        /// <summary>Aç/kapa anahtarı; etiket menü açıldığı andaki durumu gösterir.</summary>
        public static MenuEntry Toggle(string name, Func<bool> get, Action<bool> set)
        {
            string label = name + ": " + (get() ? "[AÇIK]" : "[KAPALI]");
            return new MenuEntry(label, reopen =>
            {
                bool now = !get();
                set(now);
                MenuUI.Success(name + (now ? " açıldı" : " kapatıldı"));
                reopen();
            });
        }

        /// <summary>Alt menüye geçer.</summary>
        public static MenuEntry Submenu(string label, Action show) =>
            new MenuEntry(label, _ => MenuUI.Defer(show));
    }
}
```

- [ ] **Step 3: Popup yardımcılarını yaz**

`src/Menu/MenuUI.cs`:

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace CheatMenu.Menu
{
    public static class MenuUI
    {
        private static readonly Color GreenColor = Color.FromUint(0xFF7CD67C);
        private static readonly Color RedColor = Color.FromUint(0xFFFF6B6B);
        private static readonly Color YellowColor = Color.FromUint(0xFFFFD166);

        // Bir popup'ın callback'i içinden yeni popup açmak, kapanmakta olan popup'la çakışabilir.
        // Bu yüzden "sonraki popup" işleri kuyruğa alınır ve bir sonraki karede
        // (SubModule.OnApplicationTick → RunDeferred) çalıştırılır.
        private static readonly Queue<Action> Deferred = new Queue<Action>();

        /// <summary>Tek seçimli liste gösterir. Seçilen satır çalışır; reshow menüyü yeniden açar.</summary>
        public static void ShowList(string title, List<MenuEntry> entries, Action reshow, string backText, Action onBack)
        {
            List<InquiryElement> elements = entries.Select(e => new InquiryElement(e, e.Label, null)).ToList();
            MBInformationManager.ShowMultiSelectionInquiry(
                new MultiSelectionInquiryData(
                    title, string.Empty, elements,
                    true, 1, 1,
                    "Seç", backText,
                    selected => OnPicked(selected, reshow),
                    _ => Defer(onBack)),
                true, false);
        }

        /// <summary>Sayı sorar; geçerliyse apply çalışır. Her iki durumda da sonunda onDone çağrılır.</summary>
        public static void AskNumber(string title, Action<int> apply, Action onDone)
        {
            InformationManager.ShowTextInquiry(
                new TextInquiryData(
                    title, "Bir miktar yaz:", true, true, "Uygula", "İptal",
                    text =>
                    {
                        if (AmountParser.TryParse(text, out int value)) SafeRun(title, () => apply(value));
                        else Error("Geçersiz miktar: " + text);
                        Defer(onDone);
                    },
                    () => Defer(onDone),
                    false,
                    text => AmountParser.TryParse(text, out _)
                        ? new Tuple<bool, string>(true, string.Empty)
                        : new Tuple<bool, string>(false, "Geçerli bir sayı yaz")),
                true, false);
        }

        /// <summary>
        /// Popup callback'lerinde yakalanmayan istisna oyunu çökertir; her eylem buradan geçer.
        /// </summary>
        public static void SafeRun(string what, Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                Error("'" + what + "' uygulanamadı: " + ex.Message);
                TaleWorlds.Library.Debug.Print("[CheatMenu] " + what + " failed: " + ex);
            }
        }

        public static void Defer(Action action)
        {
            if (action != null) Deferred.Enqueue(action);
        }

        /// <summary>Kuyruktaki işleri çalıştırır. Bu karede eklenenler bir sonraki kareye kalır.</summary>
        public static void RunDeferred()
        {
            int count = Deferred.Count;
            for (int i = 0; i < count; i++) SafeRun("Menü", Deferred.Dequeue());
        }

        public static void Success(string message) => Show(message, GreenColor);
        public static void Error(string message) => Show(message, RedColor);
        public static void Info(string message) => Show(message, YellowColor);

        private static void Show(string message, Color color) =>
            InformationManager.DisplayMessage(new InformationMessage("[CheatMenu] " + message, color));

        private static void OnPicked(List<InquiryElement> selected, Action reshow)
        {
            if (selected == null || selected.Count == 0 || !(selected[0].Identifier is MenuEntry entry)) return;
            SafeRun(entry.Label, () => entry.Run(() => Defer(reshow)));
        }
    }
}
```

- [ ] **Step 4: Derle**

Run: `powershell -NoProfile -ExecutionPolicy Bypass -File C:\Users\tolgaa\Desktop\Bannerlord\build.ps1`
Expected: `ALL TESTS PASSED`, `CheatMenu -> ...`, hata yok.

---

### Task 4: Kampanya hileleri

**Files:**
- Create: `src/Cheats/CampaignCheats.cs`

- [ ] **Step 1: Kampanya hilelerini yaz**

`src/Cheats/CampaignCheats.cs`:

```csharp
using System;
using CheatMenu.Menu;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;

namespace CheatMenu.Cheats
{
    /// <summary>
    /// Kampanyadaki tek seferlik hileler. Her metot sonucu ekrana yazar;
    /// istisnaları çağıran MenuUI.SafeRun yakalar.
    /// </summary>
    public static class CampaignCheats
    {
        private static Hero Me => Hero.MainHero ?? throw new InvalidOperationException("Ana karakter bulunamadı");
        private static MobileParty MyParty => MobileParty.MainParty ?? throw new InvalidOperationException("Ana parti bulunamadı");

        // ---- Para & Ekonomi ----

        public static void AddGold(int amount)
        {
            Me.ChangeHeroGold(amount);
            MenuUI.Success("Altın " + Signed(amount) + " (toplam " + Me.Gold.ToString("N0") + ")");
        }

        public static void AddInfluence(int amount)
        {
            Clan clan = Clan.PlayerClan ?? throw new InvalidOperationException("Oyuncu klanı bulunamadı");
            ChangeClanInfluenceAction.Apply(clan, amount);
            MenuUI.Success("Etki " + Signed(amount) + " (toplam " + clan.Influence.ToString("N0") + ")");
        }

        public static void AddGrain(int amount)
        {
            MyParty.ItemRoster.AddToCounts(DefaultItems.Grain, amount);
            MenuUI.Success(amount + " tahıl eklendi");
        }

        // ---- Karakter ----

        public static void AddFocusPoints(int amount)
        {
            var developer = Me.HeroDeveloper;
            developer.UnspentFocusPoints = Math.Max(0, developer.UnspentFocusPoints + amount);
            MenuUI.Success("Odak puanı " + Signed(amount) + " (harcanmamış " + developer.UnspentFocusPoints + ")");
        }

        public static void AddAttributePoints(int amount)
        {
            var developer = Me.HeroDeveloper;
            developer.UnspentAttributePoints = Math.Max(0, developer.UnspentAttributePoints + amount);
            MenuUI.Success("Attribute puanı " + Signed(amount) + " (harcanmamış " + developer.UnspentAttributePoints + ")");
        }

        public static void AddAllSkills(int levels)
        {
            foreach (SkillObject skill in Skills.All)
                Me.HeroDeveloper.ChangeSkillLevel(skill, levels, false);
            MenuUI.Success("Tüm yetenekler " + Signed(levels));
        }

        public static void HealHero()
        {
            Me.HitPoints = Me.MaxHitPoints;
            MenuUI.Success("Can dolduruldu");
        }

        // ---- Ordu / Parti ----

        public static void AddBasicTroops(int count) => AddTroops(Me.Culture?.BasicTroop, count);

        public static void AddEliteTroops(int count) => AddTroops(Me.Culture?.EliteBasicTroop, count);

        /// <summary>Her asker türüne, tamamının bir üst seviyeye terfi edebileceği kadar XP verir.</summary>
        public static void GiveUpgradeXp()
        {
            TroopRoster roster = MyParty.MemberRoster;
            int readyTypes = 0;
            for (int i = 0; i < roster.Count; i++)
            {
                TroopRosterElement element = roster.GetElementCopyAtIndex(i);
                CharacterObject troop = element.Character;
                if (troop.IsHero || troop.UpgradeTargets == null || troop.UpgradeTargets.Length == 0) continue;

                int needed = troop.GetUpgradeXpCost(PartyBase.MainParty, 0) * element.Number - roster.GetElementXp(i);
                if (needed <= 0) continue;
                roster.AddXpToTroop(troop, needed);
                readyTypes++;
            }
            MenuUI.Success(readyTypes + " asker türü terfiye hazır (parti ekranından terfi ettir)");
        }

        public static void HealWounded()
        {
            TroopRoster roster = MyParty.MemberRoster;
            int healed = 0;
            for (int i = 0; i < roster.Count; i++)
            {
                CharacterObject troop = roster.GetCharacterAtIndex(i);
                if (troop.IsHero)
                {
                    Hero hero = troop.HeroObject;
                    if (hero != null && hero.HitPoints < hero.MaxHitPoints)
                    {
                        hero.HitPoints = hero.MaxHitPoints;
                        healed++;
                    }
                    continue;
                }

                int wounded = roster.GetElementWoundedNumber(i);
                if (wounded <= 0) continue;
                roster.AddToCountsAtIndex(i, 0, -wounded, 0, false);
                healed += wounded;
            }
            MenuUI.Success(healed + " yaralı iyileştirildi");
        }

        public static void AddMorale(int amount)
        {
            MyParty.RecentEventsMorale += amount;
            MenuUI.Success("Moral " + Signed(amount) + " (şu an " + MyParty.Morale.ToString("0") + ")");
        }

        private static void AddTroops(CharacterObject troop, int count)
        {
            if (troop == null) throw new InvalidOperationException("Kültürünün bu tür askeri yok");
            if (count <= 0) throw new InvalidOperationException("Asker sayısı pozitif olmalı");
            MyParty.MemberRoster.AddToCounts(troop, count);
            MenuUI.Success(count + " x " + troop.Name + " eklendi");
        }

        private static string Signed(int value) => value.ToString("+#,0;-#,0;0");
    }
}
```

- [ ] **Step 2: Derle**

Run: `powershell -NoProfile -ExecutionPolicy Bypass -File C:\Users\tolgaa\Desktop\Bannerlord\build.ps1`
Expected: `ALL TESTS PASSED`, `CheatMenu -> ...`, hata yok.

---

### Task 5: Parti hızı modeli

**Files:**
- Create: `src/Cheats/CheatPartySpeedModel.cs`

- [ ] **Step 1: Modeli yaz**

v1.4'te `PartySpeedModel : MBGameModel<PartySpeedModel>`; `CampaignGameStarter.AddModel<T>` önceki modeli (NavalDLC'ninki dahil) `BaseModel` olarak verir. Dört üye de abstract olduğundan hepsi override edilir ve `BaseModel`'e delege edilir.

`src/Cheats/CheatPartySpeedModel.cs`:

```csharp
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Localization;

namespace CheatMenu.Cheats
{
    /// <summary>
    /// Hızı önce zincirdeki önceki model (vanilla / NavalDLC) hesaplar; FastParty açıksa
    /// yalnızca oyuncunun partisi için sonuca +%100 çarpan eklenir.
    /// </summary>
    public sealed class CheatPartySpeedModel : PartySpeedModel
    {
        private static readonly TextObject CheatText = new TextObject("CheatMenu: parti hızı x2", null);

        public override float BaseSpeed => BaseModel.BaseSpeed;

        public override float MinimumSpeed => BaseModel.MinimumSpeed;

        public override ExplainedNumber CalculateBaseSpeed(MobileParty party, bool includeDescriptions = false,
            int additionalTroopOnFootCount = 0, int additionalTroopOnHorseCount = 0) =>
            BaseModel.CalculateBaseSpeed(party, includeDescriptions, additionalTroopOnFootCount, additionalTroopOnHorseCount);

        public override ExplainedNumber CalculateFinalSpeed(MobileParty mobileParty, ExplainedNumber finalSpeed)
        {
            ExplainedNumber result = BaseModel.CalculateFinalSpeed(mobileParty, finalSpeed);
            if (CheatState.FastParty && mobileParty == MobileParty.MainParty)
                result.AddFactor(1f, CheatText);
            return result;
        }
    }
}
```

- [ ] **Step 2: Derle**

Run: `powershell -NoProfile -ExecutionPolicy Bypass -File C:\Users\tolgaa\Desktop\Bannerlord\build.ps1`
Expected: `ALL TESTS PASSED`, `CheatMenu -> ...`, hata yok.

---

### Task 6: Savaş hileleri

**Files:**
- Create: `src/Cheats/CheatTargets.cs`
- Create: `src/Cheats/CheatMissionLogic.cs`

- [ ] **Step 1: Kapsam kuralını yaz (geçici varsayılan; Task 10'da kullanıcı yeniden yazar)**

`src/Cheats/CheatTargets.cs`:

```csharp
using TaleWorlds.MountAndBlade;

namespace CheatMenu.Cheats
{
    public static class CheatTargets
    {
        /// <summary>
        /// Bir savaş hilesinin bu ajana uygulanıp uygulanmayacağına karar verir.
        /// </summary>
        /// <param name="agent">Savaştaki ajan (insan veya at).</param>
        /// <param name="me">Hilenin [Ben] anahtarı açık mı.</param>
        /// <param name="army">Hilenin [Ordu] anahtarı açık mı.</param>
        public static bool IsInScope(Agent agent, bool me, bool army)
        {
            if (agent == null || !agent.IsActive()) return false;

            // Geçici varsayılan kural (Task 10'da kullanıcı katkısıyla değişir):
            // oyuncu yalnızca [Ben]'e, atlar binicilerine, diğerleri oyuncunun takımındaysa [Ordu]'ya tabi.
            if (agent.IsMainAgent) return me;
            if (agent.IsMount) return agent.RiderAgent != null && IsInScope(agent.RiderAgent, me, army);
            return army && agent.Team != null && agent.Team.IsPlayerTeam;
        }
    }
}
```

- [ ] **Step 2: Mission logic'i yaz**

Notlar:
- `OnAgentHit` hasar uygulandıktan sonra çağrılır ve `Blow` `in` (salt okunur) gelir; tek vuruş için kurban kuyruğa alınıp bir sonraki tick'te `Die(blow)` ile öldürülür (kill/XP saldırana yazılır).
- `in` parametreleri lambda'ya yakalanamaz; bu yüzden düz `try/catch` kullanılır.
- Yalnızca `Mortal` ajanlar öldürülür ve yalnızca `Mortal` olanlar `Invulnerable` yapılır: oyunun kendisinin `Immortal` yaptığı (hikâye/turnuva) ajanlara dokunulmaz.

`src/Cheats/CheatMissionLogic.cs`:

```csharp
using System;
using System.Collections.Generic;
using CheatMenu.Menu;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace CheatMenu.Cheats
{
    public sealed class CheatMissionLogic : MissionLogic
    {
        private const float RefreshInterval = 0.25f;

        private readonly HashSet<Agent> _madeInvulnerable = new HashSet<Agent>();
        private readonly List<KeyValuePair<Agent, Blow>> _pendingKills = new List<KeyValuePair<Agent, Blow>>();
        private float _timer;
        private bool _errorShown;

        public override void OnAgentHit(Agent affectedAgent, Agent affectorAgent, in MissionWeapon affectorWeapon,
            in Blow blow, in AttackCollisionData attackCollisionData)
        {
            if (!CheatState.OneHitMe && !CheatState.OneHitArmy) return;
            try
            {
                if (affectedAgent == null || affectorAgent == null || affectedAgent == affectorAgent) return;
                if (!affectedAgent.IsActive() || affectedAgent.CurrentMortalityState != Agent.MortalityState.Mortal) return;
                if (!CheatTargets.IsInScope(affectorAgent, CheatState.OneHitMe, CheatState.OneHitArmy)) return;
                if (!affectorAgent.IsEnemyOf(affectedAgent)) return;
                _pendingKills.Add(new KeyValuePair<Agent, Blow>(affectedAgent, blow));
            }
            catch (Exception ex)
            {
                ReportError(ex);
            }
        }

        public override void OnMissionTick(float dt)
        {
            try
            {
                ProcessPendingKills();
                _timer += dt;
                if (_timer < RefreshInterval) return;
                _timer = 0f;
                RefreshGodMode();
                RefreshAmmo();
            }
            catch (Exception ex)
            {
                ReportError(ex);
            }
        }

        private void ProcessPendingKills()
        {
            if (_pendingKills.Count == 0) return;
            foreach (KeyValuePair<Agent, Blow> kill in _pendingKills)
            {
                Agent victim = kill.Key;
                if (victim.IsActive() && victim.CurrentMortalityState == Agent.MortalityState.Mortal)
                    victim.Die(kill.Value);
            }
            _pendingKills.Clear();
        }

        private void RefreshGodMode()
        {
            bool anyOn = CheatState.GodModeMe || CheatState.GodModeArmy;
            if (!anyOn && _madeInvulnerable.Count == 0) return;

            foreach (Agent agent in Mission.Agents)
            {
                bool wanted = anyOn && CheatTargets.IsInScope(agent, CheatState.GodModeMe, CheatState.GodModeArmy);
                if (wanted && agent.CurrentMortalityState == Agent.MortalityState.Mortal)
                {
                    agent.SetMortalityState(Agent.MortalityState.Invulnerable);
                    _madeInvulnerable.Add(agent);
                }
                else if (!wanted && _madeInvulnerable.Remove(agent))
                {
                    agent.SetMortalityState(Agent.MortalityState.Mortal);
                }
            }
            _madeInvulnerable.RemoveWhere(agent => !agent.IsActive());
        }

        private void RefreshAmmo()
        {
            if (!CheatState.InfiniteAmmoMe && !CheatState.InfiniteAmmoArmy) return;

            foreach (Agent agent in Mission.Agents)
            {
                if (!agent.IsHuman) continue;
                if (!CheatTargets.IsInScope(agent, CheatState.InfiniteAmmoMe, CheatState.InfiniteAmmoArmy)) continue;

                for (EquipmentIndex slot = EquipmentIndex.WeaponItemBeginSlot; slot < EquipmentIndex.NumAllWeaponSlots; slot++)
                {
                    MissionWeapon weapon = agent.Equipment[slot];
                    if (weapon.IsEmpty || !weapon.IsAnyConsumable()) continue;
                    if (weapon.Amount < weapon.ModifiedMaxAmount)
                        agent.SetWeaponAmountInSlot(slot, weapon.ModifiedMaxAmount, false);
                }
            }
        }

        /// <summary>Her tick'te mesaj spam'i olmasın: ilk hata ekrana, hepsi loga.</summary>
        private void ReportError(Exception ex)
        {
            TaleWorlds.Library.Debug.Print("[CheatMenu] mission error: " + ex);
            if (_errorShown) return;
            _errorShown = true;
            MenuUI.Error("Savaş hilesinde hata: " + ex.Message);
        }
    }
}
```

- [ ] **Step 3: Derle**

Run: `powershell -NoProfile -ExecutionPolicy Bypass -File C:\Users\tolgaa\Desktop\Bannerlord\build.ps1`
Expected: `ALL TESTS PASSED`, `CheatMenu -> ...`, hata yok.

---

### Task 7: Temiz kayıt uyarısı

**Files:**
- Create: `src/CleanSaveGuard.cs`

- [ ] **Step 1: Guard'ı yaz**

Mod kimliği ayrıştırması, oyunun kendi `DumpIntegrityCampaignBehavior.CheckIfModulesAreDefault` mantığıyla aynıdır:
`entry.Split(ModuleCodeSeperator)[0].Split(ModuleVersionSeperator)[0]`.

`src/CleanSaveGuard.cs`:

```csharp
using System.Linq;
using CheatMenu.Menu;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.ModuleManager;

namespace CheatMenu
{
    /// <summary>
    /// Başarımların açık olduğu bir kayıt CheatMenu ile yüklenirse kullanıcıyı uyarır.
    /// Başarım kontrolünü ATLATMAZ; yalnızca yanlışlıkla temiz bir kaydı işaretlemeyi önler.
    /// </summary>
    public static class CleanSaveGuard
    {
        private const string OwnModuleId = "CheatMenu";

        private static bool _pending;
        private static bool _isNewGame;

        public static void OnGameLoaded()
        {
            _pending = true;
            _isNewGame = false;
        }

        public static void OnNewGame()
        {
            _pending = true;
            _isNewGame = true;
        }

        /// <summary>Harita ekranı ilk kez aktif olduğunda bir kez çalışır.</summary>
        public static void Tick()
        {
            if (!_pending) return;
            if (!(Game.Current?.GameStateManager?.ActiveState is MapState)) return;
            _pending = false;

            if (_isNewGame)
            {
                MenuUI.Info("CheatMenu aktif: bu kampanyada başarımlar kapalı olacak.");
                return;
            }
            if (!IsSaveClean()) return;

            InformationManager.ShowInquiry(
                new InquiryData(
                    "CheatMenu - Başarım uyarısı",
                    "Bu kayıt temiz görünüyor (başarımlar açık).\n\n"
                    + "CheatMenu aktifken KAYDEDERSEN bu kayıtta başarımlar kalıcı olarak kapanır.\n\n"
                    + "Başarımları korumak istiyorsan kaydetmeden çık ve Launcher'da CheatMenu'yu kapat.",
                    true, false, "Anladım", string.Empty, () => { }, null),
                true, false);
        }

        private static bool IsSaveClean()
        {
            Campaign campaign = Campaign.Current;
            if (campaign == null || campaign.EnabledCheatsBefore) return false;

            MBList<string> official = ModuleHelper.GetOfficialModuleIds();
            foreach (string entry in campaign.PreviouslyUsedModules)
            {
                string id = entry.Split(MBSaveLoad.ModuleCodeSeperator)[0].Split(MBSaveLoad.ModuleVersionSeperator)[0];
                if (id != OwnModuleId && !official.Contains(id)) return false;
            }
            return true;
        }
    }
}
```

- [ ] **Step 2: Derle**

Run: `powershell -NoProfile -ExecutionPolicy Bypass -File C:\Users\tolgaa\Desktop\Bannerlord\build.ps1`
Expected: `ALL TESTS PASSED`, `CheatMenu -> ...`, hata yok.

---

### Task 8: Menü içeriği

**Files:**
- Create: `src/Menu/Categories.cs`

- [ ] **Step 1: Kategorileri yaz**

Her `ShowX` listeyi her seferinde yeniden kurar; böylece aç/kapa etiketleri güncel durumu gösterir.

`src/Menu/Categories.cs`:

```csharp
using System.Collections.Generic;
using CheatMenu.Cheats;

namespace CheatMenu.Menu
{
    public static class Categories
    {
        public static void ShowRoot()
        {
            MenuUI.ShowList("HİLE MENÜSÜ", new List<MenuEntry>
            {
                MenuEntry.Submenu("Para & Ekonomi", ShowEconomy),
                MenuEntry.Submenu("Karakter", ShowCharacter),
                MenuEntry.Submenu("Ordu / Parti", ShowParty),
                MenuEntry.Submenu("Savaş içi", ShowCombat),
            }, ShowRoot, "Kapat", null);
        }

        private static void ShowEconomy()
        {
            ShowCategory("Para & Ekonomi", new List<MenuEntry>
            {
                MenuEntry.Simple("+1.000 altın", () => CampaignCheats.AddGold(1000)),
                MenuEntry.Simple("+10.000 altın", () => CampaignCheats.AddGold(10000)),
                MenuEntry.Simple("+100.000 altın", () => CampaignCheats.AddGold(100000)),
                MenuEntry.Custom("Özel miktar altın...", CampaignCheats.AddGold),
                MenuEntry.Simple("+100 etki", () => CampaignCheats.AddInfluence(100)),
                MenuEntry.Simple("+500 etki", () => CampaignCheats.AddInfluence(500)),
                MenuEntry.Custom("Özel miktar etki...", CampaignCheats.AddInfluence),
                MenuEntry.Simple("+100 tahıl", () => CampaignCheats.AddGrain(100)),
            }, ShowEconomy);
        }

        private static void ShowCharacter()
        {
            ShowCategory("Karakter", new List<MenuEntry>
            {
                MenuEntry.Simple("+5 odak puanı", () => CampaignCheats.AddFocusPoints(5)),
                MenuEntry.Custom("Özel miktar odak puanı...", CampaignCheats.AddFocusPoints),
                MenuEntry.Simple("+2 attribute puanı", () => CampaignCheats.AddAttributePoints(2)),
                MenuEntry.Custom("Özel miktar attribute puanı...", CampaignCheats.AddAttributePoints),
                MenuEntry.Simple("Tüm yeteneklere +25", () => CampaignCheats.AddAllSkills(25)),
                MenuEntry.Simple("Canı doldur", CampaignCheats.HealHero),
            }, ShowCharacter);
        }

        private static void ShowParty()
        {
            ShowCategory("Ordu / Parti", new List<MenuEntry>
            {
                MenuEntry.Simple("+10 temel kültür askeri", () => CampaignCheats.AddBasicTroops(10)),
                MenuEntry.Simple("+50 temel kültür askeri", () => CampaignCheats.AddBasicTroops(50)),
                MenuEntry.Custom("Özel sayıda temel asker...", CampaignCheats.AddBasicTroops),
                MenuEntry.Simple("+10 elit kültür askeri", () => CampaignCheats.AddEliteTroops(10)),
                MenuEntry.Simple("Tüm askerlere terfi XP'si", CampaignCheats.GiveUpgradeXp),
                MenuEntry.Simple("Yaralıları iyileştir", CampaignCheats.HealWounded),
                MenuEntry.Simple("+50 moral", () => CampaignCheats.AddMorale(50)),
                MenuEntry.Toggle("Parti hızı x2", () => CheatState.FastParty, v => CheatState.FastParty = v),
            }, ShowParty);
        }

        private static void ShowCombat()
        {
            ShowCategory("Savaş içi", new List<MenuEntry>
            {
                MenuEntry.Toggle("Ölümsüzlük (Ben)", () => CheatState.GodModeMe, v => CheatState.GodModeMe = v),
                MenuEntry.Toggle("Ölümsüzlük (Ordu)", () => CheatState.GodModeArmy, v => CheatState.GodModeArmy = v),
                MenuEntry.Toggle("Tek vuruş (Ben)", () => CheatState.OneHitMe, v => CheatState.OneHitMe = v),
                MenuEntry.Toggle("Tek vuruş (Ordu)", () => CheatState.OneHitArmy, v => CheatState.OneHitArmy = v),
                MenuEntry.Toggle("Sınırsız cephane (Ben)", () => CheatState.InfiniteAmmoMe, v => CheatState.InfiniteAmmoMe = v),
                MenuEntry.Toggle("Sınırsız cephane (Ordu)", () => CheatState.InfiniteAmmoArmy, v => CheatState.InfiniteAmmoArmy = v),
            }, ShowCombat);
        }

        private static void ShowCategory(string title, List<MenuEntry> entries, System.Action reshow) =>
            MenuUI.ShowList(title, entries, reshow, "Geri", ShowRoot);
    }
}
```

- [ ] **Step 2: Derle**

Run: `powershell -NoProfile -ExecutionPolicy Bypass -File C:\Users\tolgaa\Desktop\Bannerlord\build.ps1`
Expected: `ALL TESTS PASSED`, `CheatMenu -> ...`, hata yok.

---

### Task 9: Giriş noktasını bağla

**Files:**
- Modify: `src/SubModule.cs` (tamamı değişir)

- [ ] **Step 1: SubModule'ün son hâlini yaz**

`src/SubModule.cs`:

```csharp
using CheatMenu.Cheats;
using CheatMenu.Menu;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace CheatMenu
{
    public sealed class SubModule : MBSubModuleBase
    {
        // Ctrl+F1 değil: savaşta F1 "hareket emri" menüsünü açar.
        private const InputKey MenuKey = InputKey.Insert;

        private static bool _loadMessageShown;

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            base.OnBeforeInitialModuleScreenSetAsRoot();
            if (_loadMessageShown) return;
            _loadMessageShown = true;
            InformationManager.DisplayMessage(new InformationMessage("[CheatMenu] yüklendi - kampanyada Insert ile aç"));
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            base.OnGameStart(game, gameStarterObject);
            if (game.GameType is Campaign && gameStarterObject is CampaignGameStarter starter)
                starter.AddModel<PartySpeedModel>(new CheatPartySpeedModel());
        }

        public override void OnGameLoaded(Game game, object initializerObject)
        {
            base.OnGameLoaded(game, initializerObject);
            if (game.GameType is Campaign) CleanSaveGuard.OnGameLoaded();
        }

        public override void OnNewGameCreated(Game game, object initializerObject)
        {
            base.OnNewGameCreated(game, initializerObject);
            if (game.GameType is Campaign) CleanSaveGuard.OnNewGame();
        }

        public override void OnMissionBehaviorInitialize(Mission mission)
        {
            base.OnMissionBehaviorInitialize(mission);
            if (Campaign.Current != null) mission.AddMissionBehavior(new CheatMissionLogic());
        }

        public override void OnGameEnd(Game game)
        {
            base.OnGameEnd(game);
            CheatState.ResetAll();
        }

        protected override void OnApplicationTick(float dt)
        {
            base.OnApplicationTick(dt);
            if (Campaign.Current == null) return;

            MenuUI.RunDeferred();
            CleanSaveGuard.Tick();

            if (Input.IsKeyPressed(MenuKey) && !InformationManager.IsAnyInquiryActive())
                MenuUI.SafeRun("Hile menüsü", Categories.ShowRoot);
        }
    }
}
```

- [ ] **Step 2: Derle**

Run: `powershell -NoProfile -ExecutionPolicy Bypass -File C:\Users\tolgaa\Desktop\Bannerlord\build.ps1`
Expected: `ALL TESTS PASSED`, `CheatMenu -> ...`, hata yok.

- [ ] **Step 3: Oyun içi duman testi (kullanıcı)**

1. Launcher → Mods → `CheatMenu`'yu işaretle → Play.
2. Ana menüde sol altta `[CheatMenu] yüklendi - kampanyada Insert ile aç` görünür.
3. Hileli/yedekli bir kampanyayı yükle → haritada **Insert** → "HİLE MENÜSÜ" açılır.
4. Para & Ekonomi → `+10.000 altın` → yeşil mesaj, altın artar, menü yeniden açılır → Geri → ana menü → Kapat.

Çökme olursa log: `C:\ProgramData\Mount and Blade II Bannerlord\logs\` içindeki en yeni `rgl_log_*.txt`; `[CheatMenu]` ara.

---

### Task 10: Kullanıcı katkıları (learning mode)

**Files:**
- Modify: `src/Cheats/CheatTargets.cs` — `IsInScope` gövdesi
- Modify: `src/AmountParser.cs` — `TryParse` kuralları
- Modify: `tests/CheatMenu.Tests/Program.cs` — yeni kurallar için `Expect(...)` satırları

- [ ] **Step 1: Kullanıcı `IsInScope` kuralını yazar**

Kararlar: müttefik lordların takımları (`agent.Team.IsPlayerAlly`) [Ordu]'ya dahil mi; atlar dahil mi; [Ben] kapalı ama [Ordu] açıkken oyuncu dahil mi. Kullanılabilir üyeler: `agent.IsMainAgent`, `agent.IsMount`, `agent.RiderAgent`, `agent.Team.IsPlayerTeam`, `agent.Team.IsPlayerAlly`, `agent.IsHero`. İmza değişmez.

- [ ] **Step 2: Kullanıcı `AmountParser` kurallarını önce test olarak yazar**

Kararlar: negatif sayılar (altın/etki azaltmak için) kabul mü; üst sınır (ör. 100.000.000) var mı; `"10k"` / `"1m"` kısaltmaları; Türkçe binlik ayırıcı `"10.000"`. Her karar için `Program.cs` `Main` içine `Expect(...)` satırı eklenir.

Not: Negatif kabul edilirse `CampaignCheats.AddTroops` zaten `count <= 0` için hata verir; `AddFocusPoints` / `AddAttributePoints` 0'ın altına inmez.

- [ ] **Step 3: Testlerin kırmızı olduğunu gör**

Run: `powershell -NoProfile -ExecutionPolicy Bypass -File C:\Users\tolgaa\Desktop\Bannerlord\build.ps1`
Expected: yeni satırlar için `FAIL`, `Tests failed`.

- [ ] **Step 4: Kullanıcı `TryParse`'ı genişletir, testler yeşile döner**

Run: `powershell -NoProfile -ExecutionPolicy Bypass -File C:\Users\tolgaa\Desktop\Bannerlord\build.ps1`
Expected: tüm satırlar `PASS`, `ALL TESTS PASSED`, `CheatMenu -> ...`.

---

### Task 11: Oyun içi doğrulama listesi (kullanıcı)

- [ ] Haritada Insert → menü; her kategoride en az bir eylem çalışır ve menü yeniden açılır.
- [ ] Özel miktar: geçersiz metinde "Uygula" pasif; geçerli sayıda uygulanır; İptal menüye döner.
- [ ] Ordu / Parti → `Parti hızı x2` AÇIK → harita hız ipucunda "CheatMenu: parti hızı x2" satırı, parti belirgin hızlı; KAPALI → normal.
- [ ] Bir haydut partisiyle savaş:
  - [ ] `Ölümsüzlük (Ben)` → oyuncu hasar almaz; kapatınca alır.
  - [ ] `Ölümsüzlük (Ordu)` → kendi askerlerin ölmez.
  - [ ] `Tek vuruş (Ben)` → her isabet düşmanı öldürür; kill sayacı artar.
  - [ ] `Sınırsız cephane (Ben)` → ok sayısı düşmez (≤0,25 sn içinde dolar).
  - [ ] Savaşta Insert → menü açılır, aç/kapa savaş ortasında etkili olur.
- [ ] Turnuva: tek vuruş açıkken turnuva normal biter (Immortal ajanlara dokunulmuyor).
- [ ] Temiz kayıt uyarısı: başarımları açık bir kaydın **kopyasını** CheatMenu ile yükle → uyarı popup'ı çıkar.
- [ ] Yeni kampanya → sarı "başarımlar kapalı olacak" mesajı.
- [ ] Launcher'da CheatMenu kapatılınca hileli kayıt yine yüklenir (save bağımlılığı yok).
