# Faz 2 — Editörler (Tasarım)

**Tarih:** 2026-10-18 · **Oyun:** v1.4.8 + War Sails v1.2.8 · **Dayanak:** `research_notes/.../technical_feasibility.md` §3–5, §7–8

## Kullanıcı kararları
- Yerleşim sahibi: **istenen herhangi bir klana** verilebilir (oyuncu klanı listenin başında).
- İleri sarma hızı: **4x / 8x / 16x**.
- Seçilen hız **hatırlanır**: `Belgeler\Mount and Blade II Bannerlord\Configs\CheatMenu.txt` (save'e yazılmaz).
- Kahraman editörü kapsamı Faz 1 kararıyla aynı: **her kahraman** (görünüm yalnızca ana kahraman).

## Yeni kök kategoriler
`Kahraman editörü`, `İlişkiler`, `Yerleşim editörü`, `Eşya ekle`; zaman hızı `Harita` kategorisine eklenir.

### Kahraman editörü
Seçici: "★ Kendim" + lordlar/yoldaşlar (çocuklar dahil). Kahraman menüsü:
- Yaş — `AgeModel.HeroComesOfAge..MaxAge`; `SetBirthDay(HeroHelper.GetRandomBirthDayForAge(yaş))`.
- Bir yetenek / tüm yetenekler — 0..330; `SetInitialSkillLevel` + `InitializeSkillXp` (konsol hilesiyle aynı).
- Nitelik — 0..`CharacterDevelopmentModel.MaxAttribute`; fark kadar `AddAttribute(..., false)` / `RemoveAttribute`.
- Odak — 0..`MaxFocusPerSkill`; `AddFocus(..., false)` / `RemoveFocus`.
- Perk ekle — sahip olunmayan perkler; `HeroDeveloper.AddPerk` (alternatif perk de alınabilir).
- Kişilik — `TraitObject.MinValue..MaxValue`; `SetTraitLevel`; ana kahramanda ayrıca
  `TraitLevelingHelper.UpdateTraitXPAccordingToTraitLevels()` + `OnPlayerTraitChanged`.
- Görünümümü düzenle (yalnızca ana kahraman) — `BarberState(MainHero.CharacterObject, CharacterHelper.GetFaceGeneratorFilter())`.

### İlişkiler
Seçici: lordlar/yoldaşlar + eşraf, etikette `[mevcut ilişki]`. Menü: +10, +50, -10, Tam dost (`MaxRelationLimit`),
Tam düşman (`MinRelationLimit`), Özel değer (aralıklı, eksi olabilir). `Hero.MainHero.SetPersonalRelation`.
Açıklama satırı: oyun bazı ilişkileri klan liderine yönlendirir.

### Yerleşim editörü
Seçici: şehir/kale/köy. Engeller: kuşatma, yağma, `Party.MapEvent`.
- Sahibini değiştir (köy hariç) → klan seçici → `ChangeOwnerOfSettlementAction.ApplyByDefault(clan.Leader, s)`;
  oyuncu klanı paralı askerken reddedilir (oyunun hilesiyle aynı kural).
- Şehir/kale: Refah +1000 / özel; Sadakat 100; Güvenlik 100; Yiyecek = `FoodStocksUpperLimit()`;
  Tüm projeleri bitir (her adımda `BuildingsInProgress.Peek()`, ilerleme = maliyet,
  `BuildingHelper.CheckIfBuildingIsComplete`; en fazla 100 tur güvenlik sınırı).
- Köy: Hane +100 / özel. Hepsi: Milis +50.

### Eşya ekle
Tür menüsü (18 tür: tek el, iki el, mızrak, yay, arbalet, ok, cıvata, fırlatma, kalkan, kafa, gövde, bacak,
eldiven, pelerin, at, koşum, ticaret malı, hayvan) → ikonlu (`ItemImageIdentifier`) arama kutulu liste,
`Ad (T<tier+1>, <değer> dinar)`, `NotMerchandise` olanlar hariç → adet 1..9999 → `ItemRoster.AddToCounts`.

### Zaman hızı
`Harita` menüsünde 4x (normal) / 8x / 16x, seçili olan `[SEÇİLİ]`. `Campaign.SpeedUpMultiplier`
yalnızca ileri sarma (">>") modunu etkiler. Oyun bu değeri her yüklemede 4'e sıfırladığı ve kaydetmediği için
seçili değer `OnApplicationTick`'te yeniden uygulanır; seçili değer 4 iken modun dokunmaz (Better Time vb.
modlarla çakışmasın), yalnızca 4'e geri dönüldüğü an bir kez yazar.

## Bileşenler
- `src/AmountParser.cs` — `TryParseInRange(input, min, max, out value)` (eksi destekli) — test edilir.
- `src/ModSettings.cs` — ayarların `Anahtar=Değer` okuma/yazma, izinli hız listesi — test edilir.
- `src/SettingsStore.cs` — dosya okuma/yazma (hata olursa varsayılan, oyun çökmez).
- `src/Cheats/HeroEditor.cs`, `Relations.cs`, `SettlementEditor.cs`, `ItemSpawner.cs`, `TimeSpeed.cs`.
- `src/Menu/MenuUI.cs` — `AskNumberInRange`; `src/Menu/MenuEntry.cs` — `Range` satırı;
  `src/Menu/Picker.cs` — isteğe bağlı ikon ve sıralama parametresi.
- `src/Menu/Categories*.cs` — `static partial class` olarak üç dosyaya bölünür:
  `Categories.cs` (kök + hileler), `Categories.World.cs` (Harita, Adlandır, zaman), `Categories.Editors.cs` (Faz 2).
- `src/SubModule.cs` — `TimeSpeed.Tick()`.

## Kayıt ve başarımlar
Yalnızca oyunun kendi kaydettiği alanlar (doğum günü, yetenek/nitelik/odak/perk/kişilik, ilişki, yerleşim
istatistikleri ve binalar, sahiplik, envanter). Zaman hızı save'e değil config dosyasına. Özel kayıt tipi yok,
konsol hileleri çağrılmaz (IL taramasıyla doğrulanır).

## Doğrulama
Birim testleri (TryParseInRange, ModSettings); derleme; tip yükleme + oyunun `CampaignCheats` sınıfına çağrı
olmadığının IL taraması; oyun içi kontrol listesi.

## Kapsam dışı
Yoldaş/başka kahraman görünümü, tek perk silme, toplu ilişki değişikliği, krallık/klan araçları (Faz 3).
