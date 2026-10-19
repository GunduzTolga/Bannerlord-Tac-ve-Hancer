# CheatMenu — Bannerlord Hile Menüsü Modu (Tasarım)

**Tarih:** 2026-10-18
**Hedef oyun sürümü:** Mount & Blade II: Bannerlord v1.4.8 (NavalDLC kurulu)
**Kapsam:** Sadece tek oyunculu Campaign / Sandbox. Multiplayer ve Custom Battle kapsam dışı.

## 1. Amaç

Oyun içinde bir kısayol tuşuyla açılan, oyunun kendi popup pencerelerini kullanan bir hile menüsü.
Ek bağımlılık yok (Harmony, MCM vb. yok); yalnızca TaleWorlds'ün resmi mod API'leri.

## 2. Kullanıcı akışı

1. **Insert** tuşu → *Ana Menü* popup'ı (oyun duraklar).
   - Ctrl+F1 seçilmedi: savaşta F1 "hareket emri" menüsünü açıyor.
   - Tuş `SubModule.cs` içinde tek bir sabittir.
2. Ana menü: `Para & Ekonomi`, `Karakter`, `Ordu / Parti`, `Savaş içi`.
3. Kategori seçilince o kategorinin eylem listesi açılır.
4. Eylem seçilip onaylanınca:
   - eylem çalışır,
   - yeşil bilgi mesajı gösterilir (`✔ +10.000 altın eklendi`),
   - **aynı kategori yeniden açılır** (art arda kullanım için).
5. "Geri" butonu ana menüye döner; ana menüde "Kapat" popup'ı kapatır.
6. Aç/kapa eylemleri listede güncel durumuyla görünür: `Ölümsüzlük (Ben): ✓ AÇIK` / `✗ KAPALI`.
7. "Özel miktar…" eylemleri bir metin kutusu (TextInquiry) açar; sayı doğrulanır, sonra uygulanır.
8. Campaign dışında (ana menü, Multiplayer, Custom Battle) tuş hiçbir şey yapmaz.

## 3. Özellikler

### Para & Ekonomi
- +1.000 / +10.000 / +100.000 altın — `Hero.MainHero.ChangeHeroGold`
- Özel miktar altın…
- +100 / +500 etki — `ChangeClanInfluenceAction.Apply(Clan.PlayerClan, x)`
- Özel miktar etki…
- +100 tahıl — `MobileParty.MainParty.ItemRoster.AddToCounts(grain, 100)`

### Karakter
- +5 odak puanı — `HeroDeveloper.UnspentFocusPoints += 5`
- +2 attribute puanı — `HeroDeveloper.UnspentAttributePoints += 2`
- Özel miktar odak / attribute…
- Tüm yeteneklere +25 seviye — `HeroDeveloper.ChangeSkillLevel` (her yetenek için)
- Canı doldur — `Hero.MainHero.HitPoints = MaxHitPoints`

### Ordu / Parti
- +10 / +50 temel kültür askeri — `Hero.MainHero.Culture.BasicTroop`
- +10 elit kültür askeri — `Hero.MainHero.Culture.EliteBasicTroop`
- Tüm askerlere terfi XP'si — `MemberRoster.AddXpToTroop` (terfi yine parti ekranından yapılır)
- Yaralıları iyileştir — roster'daki yaralı sayıları sıfırlanır
- +50 moral — `MobileParty.MainParty.RecentEventsMorale += 50`
- **[Aç/Kapa] Parti hızı ×2** — `CheatPartySpeedModel`

### Savaş içi (6 bağımsız anahtar)
| Hile | [Ben] | [Ordu] |
|---|---|---|
| Ölümsüzlük | oyuncu | ordu sayılan ajanlar |
| Tek vuruş | oyuncunun vuruşları | ordu sayılanların vuruşları |
| Sınırsız cephane | oyuncunun ok/cıvata/fırlatmalıkları | ordu sayılanlarınki |

Hangi ajanın (atlar dahil) [Ben] / [Ordu] kapsamına girdiği tek bir fonksiyonda (`CheatTargets.IsInScope`)
toplanır — **kullanıcı yazacak** (bkz. §7).

## 4. Mimari

```
Desktop\Bannerlord\                (proje kökü)
├─ Module/SubModule.xml            mod manifesti (Id = CheatMenu)
└─ src/CheatMenu.csproj            net472 class library, oyun DLL'lerine Private=false referans
   ├─ SubModule.cs                 MBSubModuleBase: tuş dinleme, model + mission logic kaydı
   ├─ CheatState.cs                aç/kapa bayrakları (statik, save'e YAZILMAZ)
   ├─ CleanSaveGuard.cs            "temiz kayıt" uyarısı (bkz. §6)
   ├─ Menu/MenuUI.cs               genel popup yardımcıları: ShowList, AskNumber, Notify
   ├─ Menu/Categories.cs           4 kategorinin eylem listeleri (etiket + Action)
   ├─ Cheats/CampaignCheats.cs     kampanya işlemleri (altın, XP, asker…)
   ├─ Cheats/CheatTargets.cs       "kim etkilenir" kuralı (savaş)
   ├─ Cheats/CheatMissionLogic.cs  MissionLogic: ölümsüzlük, tek vuruş, cephane
   └─ Cheats/CheatPartySpeedModel.cs  PartySpeedModel: BaseModel'e delege + ×2
```

### Bileşenler

- **SubModule** — `OnApplicationTick`: `Campaign.Current != null` ve `Input.IsKeyPressed(Insert)` ise ana menüyü açar.
  `OnGameStart`: `CampaignGameStarter` ise `AddModel(new CheatPartySpeedModel())`.
  `OnMissionBehaviorInitialize`: Campaign içindeyse `mission.AddMissionBehavior(new CheatMissionLogic())`.
- **CheatState** — `GodModeMe`, `GodModeArmy`, `OneHitMe`, `OneHitArmy`, `InfiniteAmmoMe`, `InfiniteAmmoArmy`, `FastParty`.
  Oyun kapanınca sıfırlanır. Save'e yazılmadığı için mod kaldırılsa bile kayıtlar bozulmaz.
- **MenuUI** — `MBInformationManager.ShowMultiSelectionInquiry` (liste), `InformationManager.ShowTextInquiry` (sayı),
  `InformationManager.DisplayMessage` (renkli mesaj). Her callback `SafeRun` ile sarılır.
- **CheatPartySpeedModel** — v1.4 model zinciri: `AddModel` önceki modeli (NavalDLC dahil) `BaseModel` olarak verir.
  `CalculateBaseSpeed` / `CalculateFinalSpeed` / `BaseSpeed` / `MinimumSpeed` → `BaseModel`'e delege edilir;
  `FastParty` açık ve parti `MainParty` ise sonuca `AddFactor(1.0f)` (= ×2) eklenir.
- **CheatMissionLogic** (`MissionLogic`)
  - *Ölümsüzlük:* her 0,25 sn kapsam içindeki ajanlara `SetMortalityState(Invulnerable)`, kapsam dışındakilere
    (yalnızca bizim değiştirdiklerimize) `Mortal`. Anahtar kapanınca hepsi `Mortal`'a döner.
  - *Tek vuruş:* `OnAgentHit` hasar uygulandıktan sonra çağrılır ve `Blow` salt okunurdur; bu yüzden saldıran kapsam
    içindeyse ve kurban düşmansa kurban + blow kopyası kuyruğa alınır, sonraki `OnMissionTick`'te kurban hâlâ aktifse
    `agent.Die(blow)` çağrılır (kill/XP saldırana yazılır, vuruş hattına yeniden girilmez).
  - *Sınırsız cephane:* her 0,25 sn kapsam içindeki ajanların `Weapon0..Weapon3` slotlarında `Amount < MaxAmmo` ise
    `SetWeaponAmountInSlot(slot, MaxAmmo, false)`.

## 5. Hata güvenliği

- Popup callback'lerinde yakalanmayan istisna oyunu çökertir → her eylem `MenuUI.SafeRun(name, action)` ile sarılır;
  hata kırmızı mesaj olarak gösterilir, oyun devam eder.
- Mission tick içindeki hile kodu da `try/catch` ile sarılır; ilk hatada bir kez mesaj, sonra sessiz loglama
  (her tick'te mesaj spam'i olmasın).
- Eylemler önkoşulları kontrol eder (ör. `Hero.MainHero.Clan == null` ise etki eklenmez, açıklayıcı mesaj).

## 6. Başarımlar ve "temiz kayıt" uyarısı

Oyun, `SandBox.CampaignBehaviors.DumpIntegrityCampaignBehavior.IsGameIntegrityAchieved` ile başarımları kapatır;
kontrollerden biri `Campaign.PreviouslyUsedModules` içinde resmi olmayan modül olmasıdır
(`ModuleHelper.GetOfficialModuleIds`). Bu liste save'e yazılır → mod bir kez kullanılınca o kayıt kalıcı olarak
işaretlenir. Mod bu kontrolü **atlatmaz**.

**CleanSaveGuard:** kayıt yüklendiğinde `PreviouslyUsedModules` içinde `CheatMenu` dışında resmi olmayan modül yoksa
(kayıt temizse) bir onay popup'ı gösterir:

> "Bu kayıt temiz görünüyor (başarımlar açık). CheatMenu aktifken KAYDEDERSEN bu kayıtta başarımlar kalıcı olarak
> kapanır. Başarımları korumak istiyorsan kaydetmeden çık ve Launcher'da CheatMenu'yu kapat."

Yeni kampanya başlatıldığında tek satırlık sarı bilgi mesajı: "CheatMenu aktif: bu kampanyada başarımlar kapalı olacak."

Kayıt girdileri `"<modülId><ModuleVersionSeperator><sürüm>"` biçimindedir; Id, `MBSaveLoad.ModuleVersionSeperator`
ile ayrılarak alınır.

## 7. Kullanıcının yazacağı kısımlar (learning mode)

1. `CheatTargets.IsInScope(Agent agent, bool me, bool army)` — "ordum" tanımı: yalnızca `PlayerTeam` mı, müttefik
   takımlar da mı; atlar dahil mi; oyuncu `me` kapalıyken `army` açıksa oyuncu dahil mi.
2. `MenuUI.TryParseAmount(string input, out int value)` — özel miktar kuralları: negatife izin, üst sınır,
   "10k" gibi kısaltmalar.

## 8. Derleme ve kurulum

- MSBuild (VS 2026 Community), hedef `net472`. Referanslar oyun klasöründen, `Private=false` (DLL'ler kopyalanmaz).
- Post-build: `CheatMenu.dll` → `...\Modules\CheatMenu\bin\Win64_Shipping_Client\`, `SubModule.xml` → `...\Modules\CheatMenu\`.
- `SubModule.xml` bağımlılıkları: `Native`, `SandBoxCore`, `Sandbox`.
- Launcher'da "CheatMenu" modu işaretlenerek etkinleştirilir.

## 9. Doğrulama

Oyun motoru native olduğu için birim testi yok. Doğrulama:
1. Derleme hatasız.
2. Oyun içi kontrol listesi: tuş → menü açılır; her kategoride en az bir eylem; 6 savaş anahtarı bir savaşta
   (ör. haydut partisi) denenir; parti hızı haritada ×2; temiz kayıt uyarısı yedeklenmiş temiz bir kayıtta görülür;
   mod kapatılınca hileli kayıt hâlâ yüklenebilir.

## 10. Kapsam dışı (YAGNI)

Gauntlet özel panel, ayar dosyası / tuş yeniden atama arayüzü, aç/kapa durumlarının kalıcılığı, Multiplayer,
başarım kontrolünün atlatılması.
