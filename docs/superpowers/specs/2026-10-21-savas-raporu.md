# Savaş Raporu: bizim pencere, Diplomacy'nin üst kısmıyla

21 Ekim 2026. Kullanıcı Diplomacy'nin Savaş Raporu'nu çok beğendi ("üst kısım güzel"), daha fazla bilgi ve oyun görünümü istedi.

## Ne değişti

- Diplomacy raporu açmak istediğinde (haritadaki savaş yorgunluğu göstergesine tıklayınca; `DetailWarViewInterface.ShowInterface`)
  Harmony ön yaması (`BundleShim.WarReportWindow`) bizim `UI.WarReportScreen`'i açar. Açılamazsa Diplomacy'nin kendi raporu açılır
  (büyütülmüş hâliyle). Insert > Entrika > **Savaş raporları** da açar (savaştığın krallığı seçersin).
- **Üst kısım aynı:** Diplomacy'nin prefab'ından aynen alındı ve onun ViewModel'ine (`Diplomacy.ViewModel.DetailWarVM`, `WarReportVM.Report`)
  bağlı: çerçeve, iki büyük sancak, başlık, iki tarafın savaş yorgunluğu (kritikse kırmızı), ilişki kutuları, yorgunluk hızı, başlangıç
  tarihi. Yeni: savaşın kaç gündür sürdüğü (VM'de vardı, gösterilmiyordu). Sancak ve başlık sprite'ları için pencere `ui_encyclopedia` ve
  `ui_kingdom` kategorilerini yükler (yalnızca kendisi yüklediyse kapanınca bırakır).
- **Alt kısım bizim** (1500x980 pencere, iki sütun, kaydırılır; `Intrigue.WarReport`, prefab'lar `TvhWarSection`, `TvhWarRow`, `TvhWarLine`):

| Bölüm | Kaynak | İçerik |
|---|---|---|
| Savaşın gidişatı | oyunun `StanceLink`'i, `DiplomacyModel.GetWarProgressScore` | savaştaki üstünlük (oyunun puanı; üstüne gelince açıklaması), kayıplar, fethedilen şehir/kale, yağmalanan köy, esir düşen lordları, ödenen haraç |
| Kuvvetler | krallık verisi | toplam güç, sahadaki ordular, klanlar (+paralı), lordlar, şehir/kale/köy, müttefikler, başka cepheler |
| Savaş yorgunluğu | Diplomacy'nin dökümü | her olay türü için sayı ve yüzde; üstüne gelince olaylar |
| Serdarın raporu | `Marshal` | tahmini güç, ordular, topraklarına yürüyenler, tavsiye; tehlikeler ve bu düşmanın zayıf kaleleri |
| Barış | oyunun barış puanı, Diplomacy'nin barış koşulları | iki tarafın barışa bakışı, tükenme uyarısı, şu an barış teklif edilebilir mi (değilse Diplomacy'nin nedeni) |
| Esirler | kahramanlar | onların elindeki lordların ve senin elindekiler (nerede tutuldukları) |
| Savaş günlüğü | oyunun günlüğü (`IWarLog`) | bu savaşın son 10 olayı, kaç gün önce |

- Karşılaştırma satırı: solda biz, ortada ne, sağda onlar; altında iki krallığın renginde oran çubuğu (`WarReportRules.Bar`; koyu krallık
  rengi okunur hâle getirilir: `WarReportRules.Readable`). Bir bölüm okunamazsa pencere yine açılır, bölümde nedeni yazar.
- Eskiden Diplomacy'nin raporuna eklenen serdar satırları (RefreshValues sonrası yaması) kalktı: artık kendi bölümünde.

## Denetim

GuiCheck, türü yalnızca çalışırken belli olan bir veri kaynağının (`ViewModel` türünde özellik: başka bir modun iç türü) alt ağacını
denetlemez ve sayısını yazar. Bu alt ağaç Diplomacy'nin kendi prefab'ından aynen alındığı için onun parçalarını (`CriticalThresholdTextWidget`,
`Relationship`, `StaticDiplomacyButton`) ve fırçalarını kullanır.

## Sonraya

- Diplomacy'nin barış teklifi düğmesini pencereye koymak (şimdilik Krallık > Diplomasi).
- Oyunun Krallık ekranındaki savaş listesinden de açmak (oyunun prefab'ı PC'de görülmeli).
