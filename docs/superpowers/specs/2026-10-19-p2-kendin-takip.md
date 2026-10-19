# P2.11 — Casusluğu kendin yap (gizli takip)

**Tarih:** 2026-10-19 · **Durum:** Kodlandı (oyunda test bekliyor) · **Yol haritası:** bölüm 4, sıra 11
**Kullanıcı isteği:** "Casusluğu kendin yapma kısmına geçelim."

## Neden

Casusluk şimdiye kadar hep casus başının işiydi: başlat, haftalar bekle, rapor gelsin. Hedefle aynı şehirdeyken işi kendin yapmak
oyuncuya hem risk hem karar verir. Oyunda bunun için bir görev var (`OpenTownCenterShadowATargetMission`), ama sonucu bize
bildirmiyor (düellodaki gibi bir "bitti, kim kazandı" çağrısı yok). Bu yüzden asıl sistem resimli kartlarla; oyunun 3B görevi
deneysel bir ayarla denenebilir.

## Kim, nerede

- Hedefle **aynı şehirde** olmalısın (kale ve köy değil). Hedef: başka klandan, yetişkin, esir olmayan bir lord.
- Şehir menüsü: **"Birini gizlice takip et"** (şehirde takip edilebilecek lord varsa görünür) → lord listesi.
- Entrika ekranı > Kişiler > lord dosyası: **"Kendin gizlice takip et"** (aynı şehirde değilsen düğme kapalı ve nerede olduğunu söyler).
- Insert > Entrika > **"Kendin gizlice takip et (bulunduğun şehirde)"**.
- Aynı kişiyi 7 günde bir. Kuşatma altındaki şehirde olmaz. Casus başı gerekmez.

## Akış

Üç sahne: iki sokak sahnesi (çarşı, meyhane, tapınak, konak kapısı, ara sokak) ve bir son sahne (gizli buluşma, mühürlü mektup,
hesap defterleri). Her sahnede üç seçenek; düğmede bedeli, şüphe aralığı, ipucu ve dayandığı beceri yazar, ör.
"Kalabalığa karışıp yaklaş (şüphe 15-24, ipucu +25, Roguery)".

| Kavram | Kural |
|---|---|
| **Şüphe** | 0'dan başlar (hedef tetikteyse 20). Her adımda artar; 100'e varırsa bir muhafız seni fark eder. |
| **Şüphe artışı** | Seçeneğin temel değeri; becerin her 5 puanda %1 düşürür (en fazla %60), hedefin Roguery'si her 30 puanda +1, zar ±%25, **gece** %25 düşürür, o şehirdeki **gölge ağın** seviye başına %10 düşürür. Temel 0 ise artmaz. |
| **İpucu** | Seçeneklerden toplanır. Sondaki sır şansı = ipucunun üçte ikisi, en fazla %75. |
| **Beceriler** | Roguery (sızmak), Charm (laflamak), Athletics (damdan izlemek); paralı seçenekler beceri istemez. Her adım ilgili beceriye 80 XP. |
| **Yeterince gördün** | Bazı sahnelerde takibi o ana kadarki ipuçlarıyla bitirme seçeneği var. |

### Fark edilince

| Seçenek | Olasılık | Başarırsan |
|---|---|---|
| Kalabalığa dalıp kaç | 30 + Roguery/5 + Athletics/10 (+15 gece), %10–90 | Kurtuldun ama hedef 60 gün tetikte |
| Yolunu kaybetmiş yolcu gibi yol sor | 25 + Charm/4 (−15 hedef tetikteyse), %5–85 | Muhafız inandı; kimse bir şey bilmiyor |
| Muhafızın avucuna kese bırak | her zaman (300 + 200 × klan kademesi dinar) | Muhafız bir şey görmedi |

Kurtulursan o ana kadarki ipuçlarıyla rapor gelir, sır şansı yarıya iner. **Yakalanırsan:** hedefle ilişki −15, hedef 60 gün
tetikte, yabancı bir krallığın şehrindeysen oyunun suç puanı +20; gördüklerin elinden gider.

"Tetikte" mevcut entrika sistemindeki kavram: o hedefe karşı entrikaların başarısı düşer.

### Sonuç

- **Rapor:** casusluk raporuyla aynı (konum, parti, altın, ilişkiler, kişilik); Kişiler'de görünür, suikast ve kaçırmaya rapor bonusu verir.
- **Sır:** gerçek kişilikten türeyen, henüz bilinmeyen bir sır (casuslukla aynı sistem).
- **Süren casusluk entrikası:** aynı hedefe casus başın çalışıyorsa ve en az 30 ipucu topladıysan entrika hemen bir aşama ilerler
  (keşif zarı ve olay yok). Son aşamaysa sonuçlanır.

## Sahnede takip (deneysel, varsayılan kapalı)

Ayar: **Kurallar > "Takibi sahnede dene (deneysel)"** ya da MCM. Açıkken kartlar yerine oyunun şehir merkezinde gizli takip görevi
açılır ve mod kendi ölçümünü ekler:

- Hedefi **3–20 metre** arasından toplam **60 saniye** izlersen başarı: ipucu 30 + her 10 saniye için 5 (en fazla 70), rapor ve sır zarı.
- **3 metreden yakında 8 saniye** kalırsan fark edilirsin: ilişki −5, hedef tetikte.
- Az izlediysen "izini kaybettin": ceza yok.
- Hedef sahnede hiç görünmezse deneme sayılmaz (bekleme süresi geri verilir) ve kartlarla oynaman önerilir.
- Görev açılamazsa kırmızı mesajla kartlara geçilir.

**Bilinmeyenler (oyunda denenecek):** görev hedefi sahneye getiriyor mu, hedef yürüyor mu, oyunun kendi şüphe sistemi devreye giriyor mu,
görev bitince şehir menüsüne düzgün dönülüyor mu. Görev oyunun kendi içinde çökerse mod bunu yakalayamaz: denemeden önce kaydet.

## Kayıt

Takip bekleme süreleri ("tailcd") kampanya kaydına yazılır. Takibin kendisi tek oturuşta biter; kayda yazılmaz.

## Resimler

`olay_takip` (gündüz) ve `olay_takip_gece` (gece), üçer sürüm. Fark edilince `sonuc_yakalandi`, sonuç kartında rapor ya da sır resmi.

## Test

- Sahne verisi, çekiliş, şüphe/kaçış/sır formülleri, sahnede takip sınıflandırması, kayıt: `tests/CheatMenu.Tests` (TailTests).
- Oyunda: YAPILACAKLAR > P2.11.
