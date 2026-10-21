# P2.14 — Divan: başkomutan (savaş meclisi)

**Tarih:** 2026-10-19 · **Durum:** Kodlandı (oyunda test bekliyor) · **Yol haritası:** bölüm 4, sıra 14
**Kullanıcı isteği:** "Tamam knka, başkomutanla devam et" (divanın üçüncü koltuğu; öneri: düşman ordularının gücü, hangi kalenin
garnizonu zayıf, becerisi düşükse yanlış tahmin).

## Neden

Oyunda düşman ordularını ancak görüş menzilinde, kalelerin gücünü tek tek üzerine gelerek görürsün; erzaklarını hiç göremezsin. Bir
ordunun şehrine yürüdüğünü kuşatma başlayınca öğrenirsin. Başkomutan bunları tek yerde, uzaktan ve kendi gözüyle verir. Yeni bir savaş
sistemi yok: sayılar oyunun kendi ordu, garnizon, milis, erzak ve krallık gücü değerleri.

## Koltuk

- **Kim:** klanından bir yetişkin (sen değil), başka divan koltuğunda oturmayan.
- **Sahada da çalışır:** diğer koltuklardan farklı olarak parti ya da ordu başındayken de görev yapar (haberler ulakla gelir).
  Yalnızca esirken durur.
- **Beceriler:**
  - **Tactics:** asker sayısı tahminlerinin hata payı = 45 − Tactics/6, %5–60.
  - **Scouting:** görüş menzili = 200 + Scouting × 2 harita birimi (en fazla 900); erken uyarı olasılığı = 30 + Scouting/4, en fazla %95.
- Çalıştığı her hafta Tactics'e 40, verdiği her uyarıda Scouting'e 60 XP.

## Divan sekmesinde "SAVAŞ MECLİSİ"

| Bölüm | Ne gösterir |
|---|---|
| **Savaşlar** | Savaştığın her krallık: onların gücü (oyunun krallık gücü, başkomutanın gözüyle) ve seninki, ordu sayısı, oran ve tavsiye: ≥1,5 "barışı düşün", ≥1,15 "savunmada kal", ≥0,85 "fırsat kolla", ≥0,6 "saldırı zamanı", altı "kalelerine yürü". |
| **Savunman** | Klanının şehir ve kaleleri: garnizon, milis, sur, erzak günü (kesin, kendi verin), savunma puanı; kuşatma, yürüyen ordu ya da 10 günden az erzak varsa kırmızı. |
| **Düşman orduları** | Görüş menzilindeki düşman orduları ve ordusuz büyük lord partileri (100+): kaç kişi (tahmini), nerede, hedefi (kuşatma ya da yağma) ve sana uzaklığı. "Haritada izle". |
| **Zayıf hedefler** | Menzildeki düşman şehir ve kalelerinden en zayıf beşi: garnizon, milis, erzak (tahmini), sur; savunma puanı = (garnizon + milis × 0,6) × (1 + 0,25 × sur). "Haritada göster". |

Tahminler haftalık sabit (ekran her açılışta değişmez); başkomutan değişince değişir. Yürüyüş süresi = uzaklık / partinin saatlik hızı.

## Erken uyarı

Her gün bakılır: bir düşman ordusu ya da lord partisi klanının şehrini, kalesini kuşatmaya ya da köyünü yağmaya gidiyorsa, her
ordu-hedef çifti için bir kez Scouting zarı atılır (10 gün sonra yeniden). Tutarsa "Başkomutanın uyarıyor" kartı: kim, kaç kişi, nereye,
ne kadar uzakta, hedefteki garnizon, milis ve erzak. Tutmazsa oyunun kendi kuşatma bildirimini beklersin.

## Kayıt

Başkomutan ve sorulmuş uyarılar ("marshalwarn") kampanya kaydına yazılır.

## Bilinmeyenler (oyunda denenecek)

- Ordunun hedefi: liderin hedef yerleşimi ve oyunun ordu davranışı (kuşatma, yağma) okunuyor. Toplanma aşamasındaki ordular "hedefi belli
  değil" görünebilir.
- Harita birimi ve parti hızına göre "~N gün" tahminleri makul mü?
- Krallık gücü oyunun kendi "toplam güç" değeri; asker sayısı değil.

## Resimler

`sonuc_serdar` (uyarı), üç sürüm.

## Divan düzeni

Üç koltuk yan yana: başkomutan, hazinedar, hekim. Raporlar aynı sırayla aşağıda.

## Test

- Hata payı, görüş, uyarı, savunma puanı, erzak, yol süresi, yuvarlama, tavsiye, kayıt: `tests/CheatMenu.Tests` (MarshalTests).
- Arayüz: GuiCheck.
- Oyunda: YAPILACAKLAR > P2.14.
