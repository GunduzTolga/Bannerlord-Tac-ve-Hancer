# Savaş ve barış > Krallıklar: bütün krallıklar ve bağlar tek yerde

21 Ekim 2026. Kullanıcı seçimi: "Krallıklar sekmesi" — Diplomacy'nin Krallık > Diplomasi ekranına dağılmış düğmeleri (savaş ilanı,
saldırmazlık, ittifak bozma) ve oyunun kendi ittifak, ticaret anlaşması ve savaşa çağırma sistemi tek bir sekmede. Yeri: **Savaş ve
barış** bölümünün ilk alt sekmesi: "Krallıklar · Savaşlar · İsyanlar".

## Ne var

Liste ve ayrıntı (Savaşlar ve Krallık sekmeleri gibi):

- **Başlık şeridi:** krallığın ve yetkin ("hükümdarsın: divansız karar verirsin" ya da "hükümdar X: anlaşmaları o yapar"); sağda savaş ve
  müttefik sayısı, nüfuzun, kesen.
- **Solda krallıklar** (`TvhKingdomItem`): sancak, ad, hükümdar ve ilişkin, yerleri, gücü; seninle durumu renkli (savaş kırmızı; ittifak,
  saldırmazlık, ticaret altın; barış soluk). Sıra: savaştıkların, müttefiklerin, saldırmazlık, ticaret, ötekiler; sonra güç.
- **Sağda seçili krallık:** sancak, hükümdarın portresi, ilişkin, kültürü; güç, klan, şehir/kale/köy, Diplomacy'nin yayılmacılığı
  (düşük, orta, yüksek, kritik). İki sütun:
  - **DURUM:** savaştaysan kaç gündür ve yorgunluk; ittifak, saldırmazlık ve ticaretin kalan günü; senin çağrınla kime karşı savaştığı;
    kimlerle savaştığı, müttefikleri; diplomatın gözüyle tutumu (Divan'da diplomat yoksa "diplomat ata"); ittifak ve ticarete bakışı
    (oyunun ve Diplomacy'nin puanı; istemiyorsa nedeni tek satır).
  - **NE YAPABİLİRSİN** (`TvhActionRow`; yapılamayanın nedeni yanında, geri alınamayanlar onay ister):
    - Savaştaysan: Barış Masası'na otur (Savaşlar sekmesi), Savaş Raporu.
    - Barıştaysan: İttifak öner, Ticaret anlaşması öner, Saldırmazlık öner, Savaş ilan et (divansız).
    - Müttefiksen: Savaşa çağır (düşmanlarından her biri için), İttifakı boz.
    - Her zaman: hükümdarına ulak (Lordlar'daki ulak kartı).

## Kim yapar

Anlaşmalar Diplomacy'nin "divanı toplamadan" düğmeleri gibi **yalnızca hükümdarın**: oyuncu hükümdar değilse yalnızca bilgi ve ulak
görür, DURUM'da "anlaşmaları hükümdarın yapar; Krallık ekranından divana sunabilirsin" yazar. Krallığı olmayan oyuncu (bağımsız klan)
bağları izler ve ulak gönderir.

## Kurallar ve bedeller

| İş | Kimin | Koşul | Bedel |
|---|---|---|---|
| Savaş ilanı | Diplomacy (`DeclareWarConditions`, `DeclareWarAction.ApplyByKingdomDecision`) | yetki, nüfuz, saldırmazlık yok, müttefik değil, süre, ilişki | Diplomacy'nin savaş bedeli (nüfuz) |
| Saldırmazlık | Diplomacy (`NonAggressionPactConditions`, `FormNonAggressionPactAction`) | barış, ittifak yok, karşı tarafın puanı, bedel | Diplomacy'nin hesabı (nüfuz + altın) |
| İttifak | oyun (`AllianceModel.CanMakeAlliance`, `GetScoreOfStartingAlliance`, `IAllianceCampaignBehavior.StartAlliance`) | oyunun (Diplomacy'nin modeliyle) koşulları ve karşı tarafın puanı | oyunun ittifak önerme bedeli (nüfuz). Diplomacy'nin `DetermineCostForFormingAlliance`'ı denenip bırakıldı: kendi kodunda kullanılmıyor, büyük krallıkta ~750 nüfuz + ~75.000 dinar çıkıyordu |
| İttifakı boz | oyun (`EndAlliance`; Diplomacy'nin düğmesi de bunu yapar) | müttefik olmak | yok |
| Ticaret | oyun (`TradeAgreementModel.CanMakeTradeAgreement` karşı tarafın isteğiyle, `MakeTradeAgreement`) | oyunun koşulları | oyunun önerme bedeli (nüfuz) |
| Savaşa çağır | oyun (`GetKingdomsToCallToWarAgainst`, `GetScoreOfCallingToWar`, `StartCallToWarAgreement`, oyuncu öder) | müttefikin isteği | oyunun çağırma bedeli (nüfuz) + savaş bedeli (altın, oyunun kuralıyla) |

Saf hesaplar `Core/KingdomsRules` (testli): sıra, durum yazısı, kalan gün, yayılmacılık basamağı, çok satırlı gerekçeyi tek satıra indirme.
Oyun katmanı `Intrigue/KingdomTies`; Diplomacy köprüsü `Bundle/DiplomacyCompat` (Krallıklar bölümü: bedeller, koşullar, saldırmazlığın
kalan günü, yayılmacılık).

## PC'de doğrulanacak

- `StartCallToWarAgreement(..., callToWarCost, isPlayerPaying: true)` altını kendisi mi alıyor (biz ayrıca almıyoruz).
- İttifakın ve ticaretin kalan günü (`GetAllianceEndDate`, `GetTradeAgreementEndDate`) doğru mu.
- `CanMakeAlliance`'ın `evaluatingFaction` parametresi (biz kendi krallığımızı veriyoruz; karşı tarafın isteğini puanla ayrıca bakıyoruz).

## Kod

`UI/ViewModels/KingdomsVM.cs` (`KingdomsVM`, `KingdomItemVM`), prefab'lar `TvhKingdomsPanel`, `TvhKingdomItem`; `IntrigueScreenVM.TabKingdoms`,
`FocusKingdoms(kingdom)`; `IntrigueScreen.OpenKingdoms(kingdom)`; Insert > Entrika > "Krallıklar".
