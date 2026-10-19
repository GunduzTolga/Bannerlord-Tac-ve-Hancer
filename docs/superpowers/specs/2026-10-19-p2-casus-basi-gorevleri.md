# P2.3 — Casus başı görevleri

**Tarih:** 2026-10-19 · **Durum:** Kodlandı (oyunda test bekliyor) · **Yol haritası:** bölüm 4, sıra 3

## Neden

Casus başı entrika yokken boşta. Karşı casusluğa ve saray siyasetine oyuncu kararı katar; P2.2'nin saray bilgisini ve
oyunun ikna sistemini besler.

## Kurallar

- Casus başı entrikalarını yürütürken **arka planda tek bir görev** de yapar. Görevler haftalıktır.
- Casus başı yoksa ya da meşgulse (esir, vali, parti lideri) görev durur; görev kayıtta kalır, geri dönünce sürer.
- Seçim: Entrika ekranı > casus başı kartı > **Görev...** (kart). Söylenti hedefi Saray sekmesinden ya da kişi dosyasından da seçilir.
- Her çalışan hafta casus başına 50 Roguery XP.

| Görev | Etkisi | Bedel / risk |
|---|---|---|
| **Karşı casusluk** | Sana karşı entrikaları haftalık sezme şansı +25 (en fazla %95). Roguery 100'de %40 → %65 | yok |
| **Muhalefeti izle** | Her hafta kendi krallığındaki en hoşnutsuz (hükümdarıyla ≤ −10) 1–3 klan lideri hakkında rapor (Roguery 150'de bir fazlası) | bir krallıkta olmak gerekir |
| **Söylenti yay** | Seçilen lordun hükümdarıyla ilişkisi haftada −1 ile −4 arası (1 + Roguery/100) | haftada 100 dinar; haftalık yakalanma 8 − Roguery/50 (%2–12, zorlukla ±3): lordla −15, hükümdarıyla −5, görev biter |

- **Söylenti hedefi:** başka bir klanın, bir krallıktaki lordu; hükümdarın kendisi değil; kendi hükümdarı sen değilsen. Sarayında kulağın olmalı
  (hakkında rapor, o krallıkta gölge ağı ya da kendi sarayın). Hedef bu şartları kaybederse ya da ödeyemezsen görev biter.
- Örnek: Roguery 100 casus başı 8 hafta söylenti yayar: ilişki −16; bu sürede yakalanma ihtimali yaklaşık %39.

## Kayıt

`meta` kaydında `mission` ve `rumor` (hedef). Eski kayıtlarda görev "Boşta".

## Test

- Formüller: `tests/CheatMenu.Tests` (LetterTests içindeki mission satırları), kayıt: durum testleri.
- Oyunda: YAPILACAKLAR > P2.3.
