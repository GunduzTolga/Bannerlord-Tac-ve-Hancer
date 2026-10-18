# Faz 3B — Kural Çarpanları ve Müttefik Kapsamı (Tasarım)

**Tarih:** 2026-10-18 · **Oyun:** v1.4.8 + War Sails v1.2.8
**Karar notu:** Kullanıcı "devam et, yap sen" dedi; aşağıdaki seçimler önceki kararlarla tutarlı varsayılanlardır
(oyun içi test sonraya bırakıldı).

## Özellikler
Yeni kök kategori **Kurallar** (ayarlar `Belgeler\...\Configs\CheatMenu.txt`'te kalıcı, save'e yazılmaz):
- **Yetenek XP çarpanı** (oyuncu klanının kahramanları): 1x / 2x / 3x / 5x / 10x.
- **Asker XP çarpanı** (oyuncu partisindeki kahraman olmayan askerlerin vuruşlardan kazandığı XP): 1x / 2x / 3x / 5x / 10x.
- **Parti boyutu bonusu** (oyuncu partisinin asker ve esir sınırı): +0 / +50 / +100 / +200 / +500.
- **Öğrenme sınırını kaldır** (oyuncu klanının kahramanları) — aç/kapa.

**Savaş içi**'ne: **"(Ordu) hilelerine müttefikleri de kat"** aç/kapa (oturumluk, `CheatState`):
açıkken [Ordu] anahtarları `Team.IsPlayerAlly` takımlarını da kapsar (ortak savaşlarda müttefik lordların askerleri).

Seçimler alt listelerde (`[SEÇİLİ]` işaretli); değişiklik anında etkili olur (modeller ayarı canlı okur).

## Teknik — model zincirleme (Harmony yok)
`CampaignGameStarter.AddModel<T>` önceki modeli (vanilla / NavalDLC) `BaseModel` olarak verir. Doğrulandı:
modellerin **abstract olmayan virtual üyesi yok**, yani tüm abstract üyeleri delege etmek tam zincirleme için yeterli
(NavalDLC de bu modelleri zincirliyor: NavalCharacterDevelopmentModel, NavalDLCPartySizeLimitModel, NavalDLCCombatXpModel).

| Model | Üye sayısı | Değişen üye | Kural |
|---|---|---|---|
| `GenericXpModel` | 1 | `GetXpMultiplier(hero)` | `HeroDeveloper.AddSkillXp` bunu çağırır; oyuncu klanındaysa × çarpan |
| `CombatXpModel` | 4 | `GetXpFromHit(...)` | `attackerParty == PartyBase.MainParty && !attackerTroop.IsHero` → `AddFactor(çarpan-1)` (kahraman XP'si GenericXp'te; çift sayılmaz) |
| `PartySizeLimitModel` | 10 | `GetPartyMemberSizeLimit`, `GetPartyPrisonerSizeLimit` | `party == PartyBase.MainParty` → `Add(bonus)` |
| `CharacterDevelopmentModel` | 20 | `CalculateLearningLimit(...)` | açık + öznitelik sahibi oyuncu klanından (`ReferenceEquals(hero.CharacterAttributes, …)`) → `Add(1000)` |

Öğrenme sınırı: `DefaultCharacterDevelopmentModel.CalculateLearningRate` sınırı `Campaign.Current.Models.CharacterDevelopmentModel`
üzerinden (zincirin tepesi) sorar ve "sınır aşıldı" cezasını buna göre uygular; sınır +1000 olunca ceza oluşmaz.

## Bileşenler
- `src/ModSettings.cs` — yeni alanlar (`SkillXpMultiplier`, `TroopXpMultiplier`, `PartySizeBonus`, `NoLearningLimit`) +
  izinli değer listeleri; okuma/yazma — test edilir.
- `src/Cheats/Rules.cs` — ayarı değiştir + kaydet + mesaj.
- `src/Cheats/Models/` — `CheatGenericXpModel`, `CheatCombatXpModel`, `CheatPartySizeLimitModel`,
  `CheatCharacterDevelopmentModel`, `RuleModels.Register(starter)` (hız modeli dahil tüm modelleri kaydeder).
- `src/Cheats/CheatTargets.cs` + `src/CheatState.cs` — `ArmyIncludesAllies`.
- `src/Menu/Categories.Rules.cs` — Kurallar menüsü + genel "seçenek listesi" yardımcısı; `Categories.cs` kök + Savaş içi toggle.
- `src/SubModule.cs` — `RuleModels.Register`.

## Kayıt ve başarımlar
Modeller kaydedilmez; ayarlar config dosyasında. Kazanılan XP/asker oyunun kendi alanlarında. Konsol hilesi çağrısı yok.

## Doğrulama
`ModSettings` testleri; derleme; tip yükleme + IL taraması. Oyun içi kontrol listesi sonra.

## Kapsam dışı
Krallık/klan bazlı savaş hilesi hedefleme, günlük asker eğitimi XP'si (PartyTrainingModel), AI kahramanların XP'si.
