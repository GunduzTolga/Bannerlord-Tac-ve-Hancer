# P2.1 — Esir sorgulama

**Tarih:** 2026-10-19 · **Durum:** Kodlandı (oyunda test bekliyor) · **Yol haritası:** bölüm 4, sıra 1

## Neden

Elindeki esir lordlar fidye beklerken boşa duruyor. Sorgulama onları entrikanın bilgi kaynağına çevirir: rapor, sır ve
efendileri hakkında bilgi. Mevcut rapor, sır ve kart sistemlerini kullanır, yeni bir alt sistem getirmez.

## Kim, nerede

- **Sorgulanabilir:** senin partindeki ya da klanının yerleşimlerinin zindanındaki kahraman esirler.
- **Erişim:** esir partindeyse her zaman. Bir zindandaysa ancak sen (partin) o yerleşimdeyken; değilse "o yerleşime git ya da esiri partine al".
- **Sıklık:** her esir 30 günde bir sorgulanır.
- **Nereden:** Entrika ekranı > **Esirler** sekmesi (kart: yöntemler ve olasılıklar), Insert > Entrika (liste menüsü) > Esirleri sorgula.

## Yöntemler

| Yöntem | Başarı | Sır şansı | Efendisi hakkında | Bedel |
|---|---|---|---|---|
| **Konuştur** | 30 + Charm/5 + ilişki/2 − 5×Hesapçı (10–85) | casuslukla aynı taban | %25 | yok |
| **Baskı yap** | 45 + Roguery/5 − 5×Cesaret (15–90) | taban +20 | %50 | esirle ilişki −15, Merhamet −; %30 ihtimalle klanı duyar (klan lideriyle −5) |
| **Anlaşma teklif et** | 70 + ilişki/2 + 5×Onur (40–95) | taban | %35 | esir serbest bırakılır (fidye gider); esirle ilişki +10 |

- Roguery: senin ya da (esirken değilse) casus başının yüksek olanı. Charm: senin.
- Zorluk ayarı başarıya entrikalardaki gibi +10 / −10.
- **Başarı:** esir hakkında 60 gün geçerli rapor; sır zarı tutarsa casuslukla aynı kurallarla bir sır; efendi zarı tutarsa klan liderinin
  (o yoksa hükümdarın) raporu. Efendinin raporu, tanışmadığın biri olsa da onu suikast ve kaçırma listesine ekler.
- **Başarısızlık:** bir şey öğrenilmez; bedeller yine ödenir (baskıda ilişki ve Merhamet, anlaşmada esir yine de bırakılır: söz verdin).
- Zarlar kaydın tohumuyla atılır: kaydı yükleyip tekrar denemek sonucu değiştirmez.

## Hizmetine davet

Kullanıcı isteği: "Esire seçenek sunarsın, sana katılıp belirlediğin işi yapması için. Katılmazsa zindana gider ya da fidyesini
ödeyip çıkartırlar." Oyunda esir soylular yalnızca fidye, serbest bırakma ya da idam için var; hizmete almak yok.

- **Kim:** elindeki (partindeki ya da o an bulunduğun kalenin zindanındaki) yetişkin lord ya da leydi. Klan liderleri, hükümdarlar
  ve evliler kabul etmez (klanını ya da eşini bırakmaz).
- **Kabul olasılığı** (%5–75): 10 + seninle ilişki/2; kendi klan lideriyle arası kötüyse +, iyiyse −; −10 × Onur (sadık kalır);
  +5 × Hesapçı (fırsatı tartar); + Charm/10; +5 × (senin klan kademen − onunki); zorluk ±10. Örnek: seni seven (+30), klan
  liderinden nefret eden (−30) biri, Charm 100: %50.
- **Kabul ederse:** serbest kalır, kendi isteğiyle klanına ve partine katılır (ilişki +10). Ona oyunun kendi görevlerini verirsin:
  parti, vali, kervan; ya da casus başı yaparsın. Eski klan lideri bunu ihanet sayar (onunla −20, seninle −10).
- **Reddederse:** esirin olarak kalır; fidye ve serbest bırakma oyunun kendi sistemi. 30 gün yeniden sorulmaz.
- **Nerede:** Entrika ekranı > Esirler > "Esirle konuş..." kartında "Hizmetine davet et (%N)"; klasik menüde de aynı kart.
  Kabul kartının resmi `sonuc_hizmet_yemini`.

## İçerik politikası

Sahne yok, yalnızca metin. Baskı "tehdit ve korkutma"dır; işkence, şiddet tasviri ya da cinsel içerik yok.

## Test

- Formüller ve sınırlar: `tests/CheatMenu.Tests` (InterrogationTests).
- Oyunda: YAPILACAKLAR > P2.1.
