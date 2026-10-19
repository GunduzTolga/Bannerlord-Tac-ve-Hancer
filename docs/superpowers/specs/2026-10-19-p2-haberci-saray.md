# P2.2 — Haberci ve saray istihbaratı

**Tarih:** 2026-10-19 · **Durum:** Kodlandı (oyunda test bekliyor) · **Yol haritası:** bölüm 4, sıra 2
**Kullanıcı isteği:** "Lordların kralıyla ilişkisini de öğrenebilelim; lordları kendi tarafımıza çekmek için iyi olur."

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

Hoşnutsuzlar (≤ −10) kırmızı ve en üstte. Kişi dosyasında da "Hükümdarı X ile: ..." satırı var.

## Haberci

- **Mektup süresi:** atlı haberci günde 150 harita birimi; 1–8 gün. **Ücret:** 50 + 25 × gün.
- Aynı lorda aynı anda tek haberci. Mektup varınca sonuçlanır; bu arada koşullar değiştiyse (koz bitti, lord krallık değiştirdi)
  sonuçsuz döner, koz harcanmaz.
- **Koz talebi:** yüz yüzeyken (aynı yerleşim ya da çok yakın) hemen; uzaktaysa haberciyle. Kozu bırakmak her zaman hemen.
  Lord diyaloğu zaten yüz yüze, değişmedi.
- **Taraf değiştirme daveti:** başka bir krallıktaki klan liderini kendi krallığına çağırırsın.
  - Şart: bir krallıkta olmak, hedefin klan lideri olması (hükümdar ve paralı asker değil), 200 etkin olması ve onun havasını bilmek
    (rapor ya da söylenti).
  - Bedel: haberci ücreti + hediye (500 × klan kademesi, geri gelmez). Kabul ederse 200 etki.
  - Şans: 10 − (hükümdarla ilişki)/2 + (seninle ilişki)/3 − 12 × Onur; krallığın 1,25 kat güçlüyse +10, 0,8 katından zayıfsa −10;
    güncel raporla +10; zorluk ±10. Sınır %0–80. Raporun yoksa şans yaklaşık (onluk aralık) gösterilir.
  - Örnek: hükümdarıyla −40, seninle +20, Onur 0, eşit güç, raporlu: %46. Sadık ve onurlu bir lord: %0.
  - Reddederse: ilişki −5, 60 gün yeni davet yok; %25 + 15 × Onur ihtimalle mektubu hükümdarına götürür (hükümdarla −15).
  - Kozun varsa "krallığıma katılsın" talebi kesin yoldur; davet kozsuz yoldur.

## Görünüm

- "Süren" sekmesinde entrikaların altında **Yoldaki haberciler** (ne, kime, kaç gün kaldı).
- Sonuçlar resimli kartla gelir (`sonuc_haberci.png`).

## Test

- Formüller ve kayıt: `tests/CheatMenu.Tests` (LetterTests, durum testleri).
- Oyunda: YAPILACAKLAR > P2.2.
