# P2.2 — Haberci ve saray istihbaratı

**Tarih:** 2026-10-19 · **Durum:** Kodlandı (oyunda test bekliyor) · **Yol haritası:** bölüm 4, sıra 2
**Kullanıcı isteği:** "Lordların kralıyla ilişkisini de öğrenebilelim; lordları kendi tarafımıza çekmek için iyi olur."
"Oyunda herhangi bir lordu 3 soruyu cevaplayıp kendine çekebilirsin; oyunun kendi sistemini kullan."
"Davet mektubunu kaldırma: lordu haritada bulmam gerekmesin; hediyeyle ilişki artsın ve konuşma penceresi açılsın. Uzaktan ulak gibi."

## Neden

- Koz kullanmak için lordu haritada kovalamak gereksiz zahmet; ama ekrandan anında uzaktan kullanmak da gerçekçi değil.
- Lordları kendi tarafına çekmek için kimin kralından hoşnutsuz olduğunu bilmek gerekir. Bu bilgi değerli olmalı: casusluk, sorgu ve
  gölge ağlarının ürünü.

## Saray istihbaratı (Entrika ekranı > Saray)

Krallık krallık gezilir: hükümdar, krallığın gücü (seninkiyle karşılaştırma) ve hükümdar dışındaki klan liderleri.
Her lordun **hükümdarıyla ilişkisi**, bildiğin kadarıyla görünür:

| Bilgi kaynağı | Ne görünür |
|---|---|
| Hakkında güncel rapor (casusluk ya da esir sorgusu, 60 gün) | Tam sayı: "hoşnutsuz (-24)" |
| O krallığın bir şehrinde gölge ağın var / kendi sarayın | Söylenti: sadık, hoşnut, kayıtsız, hoşnutsuz, düşmanca |
| Hiçbiri | "bilinmiyor" (casusluk düğmesi) |

Hoşnutsuzlar (≤ −10) kırmızı ve en üstte. Kişi dosyasında da "Hükümdarı X ile: ..." satırı var. Her lordda: **ulak gönder**
(ya da ulak döndüyse **Görüş**), haritada izle, hakkında söylenti yay (P2.3), bilgi tam değilse casusluk.

## Haberci

Haberci iki tür mektup taşır: **koz talebi** ve **görüşme daveti (ulak)**.

- **Mektup süresi:** atlı haberci günde 150 harita birimi; 1–8 gün. **Ücret:** 50 + 25 × gün.
- Aynı lorda aynı anda tek haberci. Mektup varınca sonuçlanır; bu arada koşullar değiştiyse (koz bitti, lord krallık değiştirdi)
  sonuçsuz döner, koz harcanmaz.
- **Koz talebi:** yüz yüzeyken (aynı yerleşim ya da çok yakın) hemen; uzaktaysa haberciyle. Kozu bırakmak her zaman hemen.
  Lord diyaloğu zaten yüz yüze, değişmedi.

### Görüşme daveti (ulak)

Oyunda bir lordla konuşmak için onu haritada bulup yanına gitmek gerekir. Ulak bu yolculuğu kaldırır; konuşmanın kendisi oyunun
kendi diyaloğudur (krallığa davet ve ikna turları, evlilik, görevler). Yeni bir "katılma zarı" yok.

- **Gönder:** Saray sekmesi, kişi dosyası ya da klasik menü (Entrika > Ulak ve görüşme). Kart: hediyesiz ya da 1.000 / 3.000 /
  6.000 dinar hediye; her seçenekte ilişki artışı ve toplam bedel yazar.
- **Hediye:** ilişki azalan getiriyle artar (1.000 → +2, 3.000 → +3, 6.000 → +5; en fazla +10). Altınla ilişki satın alınmasın
  diye aynı lorda **30 günde bir** ilişki getirir; arada giden hediye yine lorda ulaşır ama ilişki yazmaz (kart önceden söyler).
  Altın lordun kesesine geçer.
- **Varınca:** lord esirse ya da savaştaysa ulak bekler (gönderilişinden 10 gün sonra vazgeçer, hediye geri gelir). Seninle ilişkisi
  −30 ya da kötüyse görüşmeyi reddeder, hediyeyi geri yollar. Yoksa kabul eder: resimli kart "Şimdi görüş / Sonra / İptal".
- **Görüş:** haritada konuşma penceresi açılır (`CampaignMapConversation`); lordun partisi uzakta olsa da. "Sonra" denirse görüşme
  **5 gün** bekler; Saray'da ve kişi dosyasında "Görüş" düğmesi çıkar.
- Hakkında bilgin varsa kart, ikna turlarında alacağın bonusu da söyler (aşağıda).

## Lordları kendi tarafına çekmek: oyunun kendi sistemi

İlk taslakta kendi "katılma mektubu" zarımız vardı. Kullanıcının haklı uyarısıyla kaldırıldı: oyunda lordu krallığına davet etmek
(ikna turları + pazarlık) zaten var. Aynı işi yapan ikinci bir mekanik yerine **ulak** lordla konuşmayı uzaktan açar, bilgi de
**oyunun ikna sistemini besler**:

- Oyunun ikna modeli zincirlenir (`IntriguePersuasionModel`): konuştuğun kişi hakkında güncel raporun varsa **her ikna turunda
  başarı +10 puan**, bildiğin ve ifşa edilmemiş bir sırrı varsa **+5 daha**. Puan "başarısız"dan alınır; kritikler oyunun hesabında kalır.
  Davet, evlilik ve görev iknalarının hepsinde geçerli (bilgi her konuşmada işe yarar).
- Saray sekmesi kimi seçmen gerektiğini söyler (hoşnutsuzlar üstte, "ikna +15" etiketi); ona ulak gönderirsin ya da lordu
  **haritada izlemeye** alırsın.
- Casus başının **söylenti** görevi (P2.3) seçtiğin lordun kralıyla arasını açar; ikna turlarını kolaylaştırır.
- Kozla "krallığıma katılsın" talebi ayrı kalır: o şantajdır, oyunda karşılığı yok.

## Görünüm

- "Süren" sekmesinde entrikaların altında **Yoldaki haberciler** (koz talebi ya da görüşme daveti, hediye, kime, kaç gün kaldı).
- Sonuçlar resimli kartla gelir (`sonuc_haberci.png`).

## Test

- Formüller ve kayıt: `tests/CheatMenu.Tests` (mektup ve hediye formülleri; görüşme, hediye bekleme süresi ve ulak mektubunun kaydı).
- Oyunda: YAPILACAKLAR > P2.2.
