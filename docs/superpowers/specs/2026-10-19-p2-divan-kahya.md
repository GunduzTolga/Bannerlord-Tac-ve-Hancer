# P2.15 — Divan: kâhya

**Tarih:** 2026-10-19 · **Durum:** Kodlandı (oyunda test bekliyor) · **Yol haritası:** bölüm 4, sıra 15
**Kullanıcı isteği:** "Tamam knka, kâhyayla devam et" (divanın son koltuğu; öneri: bütün şehir ve kalelerine tek bakış).

## Neden

Oyunda bir şehrin inşaatını ya da günlük projesini değiştirmek için oraya gitmen gerekir; sadakatin isyana doğru kaydığını çoğu zaman
isyan çıkınca fark edersin. Valiler tek bir yeri yönetir. Kâhya bütün şehir ve kalelerine sarayından bakar. Yeni bir yönetim sistemi yok:
oyunun kendi sadakat modeli, inşaat kuyruğu ve günlük proje işlemleri kullanılıyor.

## Koltuk

- **Kim:** klanından bir yetişkin (sen değil), başka divan koltuğunda oturmayan. Esir, vali ya da parti lideriyken sarayda değildir.
- **Beceri:** Steward. **30'un altı yolsuz**: sadakate eksi.
- **Sadakat katkısı:** klanının bütün şehir ve kalelerinde günlük sadakate (Steward − 30) / 200, en fazla +1; yolsuzsa −0,3.
  Oyunun sadakat modeli zincirlenir; şehrin sadakat açıklamasında "Kâhya (ad)" satırı olarak görünür.
- Çalıştığı her hafta Steward'a 40 XP.

## Divan sekmesinde "ŞEHİR VE KALELER"

Her şehir ve kale: sadakat (ve günlük değişimi), güvenlik, refah, sürmekte olan inşaat (seviye, kalan gün: oyunun hesabı), günlük proje,
sorunlar ve sağda vergi + köylerin günlük geliri (oyunun hesabı). Sorun varsa satır kırmızı:

- **Huzursuzluk:** sadakat oyunun "isyan hali" eşiğine 10 günden az kaldıysa ya da şehir zaten isyan halindeyse.
- **Kıtlık:** erzak 5 günden az.
- **Güvenlik** 30'un altında, **kuşatma** altında.

**"Yönet..."** kartı: durumu, kâhyanın önerisini (huzursuzlukta şenlik günlük projesi, aynı kültürden vali, garnizon; kıtlıkta erzak;
güvenlikte milis eğitimi) gösterir ve iki uzaktan emir sunar (her biri 100 dinar ulak):

- **İnşaatı değiştir:** seçilen yapı kuyruğun başına geçer (oyunun inşaat kuyruğu işlemi; yarım kalanın emeği kaybolmaz).
- **Günlük projeyi değiştir:** oyunun günlük proje işlemi.

## Uyarılar

Her gün bakılır; aynı yer ve aynı tür için 14 günde bir:

- "Kâhyan uyarıyor: huzursuzluk" (eşiğe 10 gün kala) ve önerisi.
- "Kâhyan uyarıyor: kıtlık" (erzak 5 günden az; kuşatma altındaysa serdar ve oyun zaten haber verir).

## Divan düzeni

Dört koltuk yan yana: serdar, kâhya, hazinedar, hekim (kartlar daraltıldı, düğmeler küçük yazıyla). Raporlar: savaş meclisi, şehir ve
kaleler, sağlık, hazine.

## Kayıt

Kâhya ve uyarıların tekrar tarihleri ("stewardwarn") kampanya kaydına yazılır.

## Bilinmeyenler (oyunda denenecek)

- Oyunun günlük proje kimlikleri ("festival", "militia" geçenler) öneride adıyla çıkıyor mu; çıkmazsa öneri genel adı yazar.
- "İsyan hali" eşiği oyunun modelinden okunuyor; uyarının zamanı makul mü?

## Resimler

`sonuc_kahya`, üç sürüm.

## Test

- Sadakat katkısı, yolsuzluk sınırı, eşiğe kalan gün, işaretli sayı, kayıt: `tests/CheatMenu.Tests` (StewardTests).
- Arayüz: GuiCheck (dört koltuk, küçük düğme yazısı).
- Oyunda: YAPILACAKLAR > P2.15.
