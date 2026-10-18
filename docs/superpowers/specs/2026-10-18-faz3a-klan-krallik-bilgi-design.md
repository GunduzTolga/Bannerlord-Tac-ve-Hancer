# Faz 3A — Klan & Krallık Araçları ve Bilgi Araçları (Tasarım)

**Tarih:** 2026-10-18 · **Oyun:** v1.4.8 + War Sails v1.2.8 · **Dayanak:** `research_notes/.../technical_feasibility.md` §6

## Kullanıcı kararları
- Faz 3 bölündü: **3A = klan/krallık + bilgi araçları (bu belge)**, 3B = kural çarpanları + hedefli savaş hileleri (sonra).
- Krallık araçları **herkes** üzerinde (AI krallıkları ve klanları dahil).
- Harita sisi kaldırma **aç/kapa**.

## Yeni kök kategoriler: `Klan & krallık`, `Bilgi`

### Klan & krallık
Benim klanım:
- Vasal olarak katıl… (`ChangeKingdomAction.ApplyByJoinToKingdom`) — hükümdarken reddedilir (oyunun hilesiyle aynı);
  zaten o krallıkta vasalsan reddedilir (paralı askerken aynı krallığa vasal geçiş serbest).
- Paralı asker olarak katıl… (`ApplyByJoinFactionAsMercenary`, ödül çarpanı 50) — bir krallıktayken reddedilir.
- Krallığımdan ayrıl (`ApplyByLeaveKingdom` / `ApplyByLeaveKingdomAsMercenary`) — yönettiğin krallıktan ayrılamazsın.
- Kendi krallığını kur… — onay → ad kutusu (NameRules) → oyunun krallık adı şablonları (Renamer ile ortak yardımcı)
  → `KingdomManager.CreateKingdom(full, short, klan kültürü, oyuncu klanı, null…)`; bir krallıktayken reddedilir.
- Krallığımın hükümdarı ol — onay → `ChangeRulingClanAction.Apply`; zaten hükümdar / paralı asker / krallıksız ise ret.
- Klan kademesi (`ClanTierModel.MinClanTier..MaxClanTier`): `Renown = GetRequiredRenownForTier(t)` + `AddRenown(0)`.
- +500 şöhret (`AddRenown`).

Herkes için:
- Savaş ilan ettir… — 1. taraf → savaşta olmadığı taraflar → `DeclareWarAction.ApplyByDefault`.
- Barış yaptır… — 1. taraf → savaştığı (kalıcı savaş hariç) taraflar → `MakePeaceAction.Apply`.
  Taraflar: yaşayan krallıklar + (bağımsızsa) oyuncunun klanı.
- Bir klanı bir krallığa sok… — oyuncu, haydut, elenmiş ve hükümdar klanlar hariç → hedef krallık → `ApplyByJoinToKingdom`.
- Bir klanın liderini değiştir… — oyuncu klanı hariç → yaşayan, yetişkin, mevcut lider olmayan üye → onay →
  `ChangeClanLeaderAction.ApplyWithSelectedNewLeader`.
- Bir krallığın hükümdarını değiştir… — krallıktaki (paralı asker olmayan, mevcut hükümdar olmayan) klan → onay → `ChangeRulingClanAction.Apply`.
- Bir krallığı yok et… — oyuncunun krallığı hariç → onay → `DestroyKingdomAction.Apply`.

Onay metni: "Bu işlem geri alınamaz. Önce oyunu kaydetmeni öneririm. Devam edilsin mi?" (`MenuUI.Confirm`).

### Bilgi
- Bir lordu bul… — lord/yoldaş seçici (gerçek konum etiketli) → ekrana tam konum (esirse kimin elinde/hangi zindanda,
  seferdeyse en yakın yerleşim, yerleşimdeyse yerleşim adı) + `MapScreen.FastMoveCameraToPosition`.
- Harita sisini kaldır [AÇIK/KAPALI] — `Campaign.TrueSight` (save'e yazılmayan çalışma anı bayrağı; oyunun Ctrl+Alt+T
  hilesi de bunu kullanır). Seçili durum `CheatState.RevealMap`'te; her karede uygulanır, kapalıyken yalnızca kapatıldığı
  an bir kez yazılır.

## Bileşenler
- `src/Cheats/KingdomTools.cs` — listeler (taraflar, taşınabilir klanlar, lider adayları) ve eylemler.
- `src/Cheats/MapInfo.cs` — kahraman konumu + kamera; TrueSight Tick.
- `src/Cheats/Renamer.cs` — `KingdomNameTexts` ortak yardımcısı (RenameKingdom da kullanır).
- `src/Menu/MenuUI.cs` — `Confirm`.
- `src/Menu/Categories.Politics.cs` — iki menü; `Categories.cs` köke iki satır.
- `src/CheatState.cs` — `RevealMap`; `src/SubModule.cs` — `MapInfo.Tick()`.

## Kayıt ve başarımlar
Oyunun eylemleriyle, oyunun kendi alanlarına (krallık üyeliği, savaş durumu, hükümdar, klan lideri, şöhret).
TrueSight kaydedilmez. Konsol hileleri çağrılmaz (IL taraması).

## Doğrulama
Derleme; tip yükleme + IL taraması; oyun içi kontrol listesi. Yeni saf mantık yok, yeni birim testi yok.

## Kapsam dışı
İttifak kurma (API doğrulanmadı), oyuncu klanının liderini değiştirme, oyuncunun krallığını yok etme, kural çarpanları (3B).
