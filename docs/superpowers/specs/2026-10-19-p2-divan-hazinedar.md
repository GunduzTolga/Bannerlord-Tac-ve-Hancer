# P2.12 — Divan: hazinedar

**Tarih:** 2026-10-19 · **Durum:** Kodlandı (oyunda test bekliyor) · **Yol haritası:** bölüm 4, sıra 12
**Kullanıcı isteği:** "Bir imparatorun etrafında ona yardımcı olan insanlar oluyor... Kervanların hangi rotadan ne kadar kazandıkları,
hangi rotaya gidip gitmemeleri gerektiği bilgisi var ama yanına gitmek gerekiyor... Sarayda bir hazinedar gibi birini atarım; ticaret
puanı ne kadar yüksekse o kadar iyi olur. Atölyelerin ne üretmesi gerektiğini söyler, x yerden kervan kurmanın kârlı olduğunu söyler.
Ticareti düşükse yanlış bilgiler verebilir."

## Neden

CK3'teki meclisin karşılığı. Casus başı zaten divanın ilk koltuğuydu; hazinedar ikincisi. Oyun atölye ve kervan gelirini hesaplıyor,
şehirlerin fiyatlarını biliyor, kervanların her alışverişini kaydediyor; ama bunları oyuncuya ancak yerinde ve dağınık gösteriyor.
Hazinedar hepsini uzaktan ve bir kişinin gözüyle sunar. Oyunun valileri tek bir yeri yönetir; hazinedar bütün klanın parasına bakar.

## Koltuk

- **Kim:** klanından bir yetişkin (sen değil, casus başı değil). Esir, vali ya da parti lideriyken çalışamaz (kervan başındaki yoldaş dahil).
- **Nerede:** Entrika ekranı > **Divan** sekmesi. Liste menüsü: Insert > Entrika > "Divan: hazinedar".
- **Beceri:** Trade ve Hesapçı kişilik. **Tahmin payı** = 45 − Trade/6 − 5 × Hesapçı, %5–60.
  Kartta "kesin" (≤10), "güvenilir" (≤20), "tahmini" (≤35), "kulaktan dolma" diye yazar.

## Kesin olanlar (oyunun kendi hesabı)

- Her atölyenin ve kervanın sana **günlük getirisi** (oyunun gelir hesabı), toplam ve haftalık.
- **Sefer defteri:** kervanların her şehirden çıkışı deftere yazılır. Sefer kazancı = (kesedeki altın + yükün temel değeri) farkı + bu arada
  sana gönderilen pay. Rotalar (şehirden şehre) toplanır: en kârlı beş ve en zararlı üç rota. Son üç seferi zararda olan kervan için
  "dağıtmayı düşün" uyarısı. Defter hazinedar olmasa da tutulur, gösterilmesi için hazinedar gerekir. En fazla 80 sefer saklanır.

## Hazinedarın gözüyle olanlar (Trade'e göre hata payı)

Hazinedar fiyatları kendi gözüyle görür: her şehirdeki her malın alış ve satış fiyatı, hata payı kadar sapar. Sapma her hafta aynıdır
(ekranı kapatıp açınca değişmez) ve hazinedar değişince değişir. Düşük Trade'de olmayan bir fırsatı varmış gibi, iyi bir atölyeyi kötü
gibi gösterebilir: kullanıcı isteğindeki "yanlış bilgi".

- **Bildiği şehirler:** sana en yakın 8 + Trade/6 şehir (en fazla 60).
- **Atölye önerisi:** her atölye türü için o şehirdeki girdi ve çıktı fiyatlarıyla günlük kâr tahmini (oyunun üretim hızı modeliyle).
  En iyisi şimdikinden günde en az 10 (ya da %20) fazlaysa önerir. **Üretimi değiştir:** ustabaşına mektup; oyunun değiştirme bedeli +
  200 dinar ulak ücreti, oyunun kendi işlemiyle. Şehre gitmek gerekmez.
- **Ticaret fırsatları:** her mal için bildiği şehirler arasında (kervan menzilinde, harita uzaklığı ≤ 250) en büyük al-sat farkı;
  en az %20 olanlardan en iyi altısı.
- **Kervan kurmaya uygun şehirler:** oradan alınıp yakın şehirlerde satılacak en iyi üç malın ortalama farkı; en iyi üç şehir
  ("Haritada göster"). Kervanı oyunun kendi yoluyla kurarsın (şehirdeki tüccar eşraf).

## Zimmet

- Onuru −1 olan hazinedar haftada %25, −2 olan %50 olasılıkla haftalık gelirin %5–14'ünü kendi kesesine geçirir (kesende varsa).
  Koltuk kartı uyarır: "Onuru düşük: kasadan çalabilir".
- **Casus başın fark eder:** haftada 15 + Roguery/10 (+20 karşı casuslukta), en fazla %75. Kart gelir; Divan'da **"Hesap sor..."**:
  - Parayı geri al, görevden al (ilişki −10)
  - Parayı geri al, görevde kalsın (ilişki −5)
  - Affet, para onda kalsın (ilişki +5)
  Geri alınabilecek para hazinedarın kesesindeki kadardır. Hesap sorulmadan hazinedar değiştirilemez.

## Kayıt

Hazinedar, zimmet, kervanların son çıkışı ("caravanbook") ve sefer defteri ("caravanleg") kampanya kaydına yazılır.

## Bilinmeyenler (oyunda denenecek)

- Oyun kervanın sana gönderdiği payı kervanın kesesinden mi düşüyor? Defter öyle varsayıyor; değilse sefer kazançları biraz şişkin görünür.
- Fiyat tahminleri oyunun pazar fiyatıyla; atölyenin gerçek alım satımı biraz farklı olabilir. Tahminin isabetini oyunda karşılaştır.
- Harita uzaklığı 250 kervan için doğru menzil mi?

## Resimler

`sonuc_hazine` (atölye kartı) ve `sonuc_zimmet` (zimmet), üçer sürüm.

## Test

- Hata payı, gürültü, atölye kârı, değişime değer mi, sefer kârı, defter sınırı, rotalar, zimmet, sabit zar, kayıt:
  `tests/CheatMenu.Tests` (TreasuryTests).
- Arayüz: GuiCheck (TvhCouncilPanel, TvhCouncilRow).
- Oyunda: YAPILACAKLAR > P2.12.

## Sonraki koltuklar

Saray hekimi (Medicine), başkomutan (Leadership/Tactics), kâhya (Steward): aynı divan sekmesine koltuk olarak eklenecek.
