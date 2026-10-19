# P2.2 — Haberci ve saray istihbaratı

**Tarih:** 2026-10-19 · **Durum:** Kodlandı (oyunda test bekliyor) · **Yol haritası:** bölüm 4, sıra 2
**Kullanıcı isteği:** "Lordların kralıyla ilişkisini de öğrenebilelim; lordları kendi tarafımıza çekmek için iyi olur."
"Oyunda herhangi bir lordu 3 soruyu cevaplayıp kendine çekebilirsin; oyunun kendi sistemini kullan."

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

Hoşnutsuzlar (≤ −10) kırmızı ve en üstte. Kişi dosyasında da "Hükümdarı X ile: ..." satırı var. Her lordda: haritada izle,
hakkında söylenti yay (P2.3), bilgi tam değilse casusluk.

## Haberci

Haberci yalnızca **koz talebi** taşır.

- **Mektup süresi:** atlı haberci günde 150 harita birimi; 1–8 gün. **Ücret:** 50 + 25 × gün.
- Aynı lorda aynı anda tek haberci. Mektup varınca sonuçlanır; bu arada koşullar değiştiyse (koz bitti, lord krallık değiştirdi)
  sonuçsuz döner, koz harcanmaz.
- **Koz talebi:** yüz yüzeyken (aynı yerleşim ya da çok yakın) hemen; uzaktaysa haberciyle. Kozu bırakmak her zaman hemen.
  Lord diyaloğu zaten yüz yüze, değişmedi.

## Lordları kendi tarafına çekmek: oyunun kendi sistemi

İlk taslakta kendi "davet mektubu" zarımız vardı. Kullanıcının haklı uyarısıyla kaldırıldı: oyunda lordun yanına gidip krallığına davet
etmek (ikna turları + pazarlık) zaten var. Aynı işi yapan ikinci bir mekanik yerine bilgi **oyunun ikna sistemini besler**:

- Oyunun ikna modeli zincirlenir (`IntriguePersuasionModel`): konuştuğun kişi hakkında güncel raporun varsa **her ikna turunda
  başarı +10 puan**, bildiğin ve ifşa edilmemiş bir sırrı varsa **+5 daha**. Puan "başarısız"dan alınır; kritikler oyunun hesabında kalır.
  Davet, evlilik ve görev iknalarının hepsinde geçerli (bilgi her konuşmada işe yarar).
- Saray sekmesi kime gitmen gerektiğini söyler (hoşnutsuzlar üstte, "ikna +15" etiketi) ve lordu **haritada izlemeye** alır.
- Casus başının **söylenti** görevi (P2.3) seçtiğin lordun kralıyla arasını açar; ikna turlarını kolaylaştırır.
- Kozla "krallığıma katılsın" talebi ayrı kalır: o şantajdır, oyunda karşılığı yok.

## Görünüm

- "Süren" sekmesinde entrikaların altında **Yoldaki haberciler** (ne, kime, kaç gün kaldı).
- Sonuçlar resimli kartla gelir (`sonuc_haberci.png`).

## Test

- Formüller ve kayıt: `tests/CheatMenu.Tests` (LetterTests, durum testleri).
- Oyunda: YAPILACAKLAR > P2.2.
