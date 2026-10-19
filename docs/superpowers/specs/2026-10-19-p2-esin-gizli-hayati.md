# Eşinin gizli hayatı ve boşanma

**Tarih:** 2026-10-19 · **Durum:** Kodlandı (oyunda test bekliyor) · **Yol haritası:** bölüm 4, sıra 6b
**Kullanıcı isteği:** "Kendi eşimi de araştırabileyim; başkasından çocuk yaptıysa sırrını öğreneyim." "Ayar varsayılan açık olsun.
Eşi boşama seçeneği de olsun."

## Sınırlar

- Ayar: MCM > Entrika > **"Eşin seni aldatabilir"** (Kurallar'da da). Varsayılan açık. "Gizli ilişki ve gayrimeşru çocuk sırları"
  ayarı da açık olmalı.
- Sevgili P2.4 kurallarıyla seçilir: yetişkin, yaşı yakın, karşı cinsten, akraba değil, senin klanından değil, önce aynı krallıktan.
- Gebelik P2.6 sistemiyle: anne 18–45 yaş, oyunun Doğum ve Ölüm ayarına uyar. Yalnızca metin.
- Boşanma her zaman açık (ayardan bağımsız).

## Eşin nasıl sevgili edinir

Her hafta, seninle ilişkisine, Onuruna ve ne kadar ayrı kaldığınıza göre (%0–8):

- Seninle ilişkisi **50 ve üstüyse hiç** aldatmaz.
- Formül: 1 + (20 − ilişki)/10 (ilişki 20'nin altındaysa) − 2 × Onur, **8 haftadan uzun ayrıysanız** +2.
- Örnek: ilişki 20 ve Onur 0 ise haftada %1. İlişki −20, Onur −1 ve aylardır ayrıysanız %8.
- **Ayrı:** eşin senin partinde değil ve aynı yerleşimde değilsiniz.

İlişki başladıktan sonra iki haftada bir buluşurlar; gebelik olabilir. Eşin Hesapçıysa tedbirli davranır.

- **Gebelik gizli kalır:**
  - Eşin hamileyse sıradan bir gebelik gibi görünür, babası yazmaz. Çocuk doğunca senin klanında büyür; resmî babası sensin.
  - Erkek eşin bir leydiden çocuk yaptıysa hiç görünmez; çocuk annesinin klanında büyür.

## Nasıl ortaya çıkar

- **Eşini araştır** (Kişiler'de eşinin dosyası ya da klasik menü > Kur ve sevgililer):
  - casus başı gerekir, 1.000 dinar, 14 gün;
  - ilişki varsa %40 + Roguery/4 ihtimalle bulur (en fazla %95);
  - hiçbir şey bulunmazsa %10 ihtimalle eşin izlendiğini fark eder ve kırılır (−10).
- **Karşı casusluk görevi:** casus başı bu görevdeyken haftada %10 ihtimalle tesadüfen öğrenir.

Ortaya çıkınca bir kart açılır:

- Kim, ne kadar süredir ve bu ilişkiden doğan çocuklar ("X senin çocuğun değil: gerçek babası Y").
- Sevgili için bir gizli ilişki sırrı, her çocuk için bir gayrimeşru çocuk sırrı Kişiler'e eklenir. Şantaj ya da ifşa (P2.4) için
  kullanabilirsin.
- **Seçenekler:**
  - **Yüzleş:** %50 + ilişki/2 + 10 × Onur (%10–90) ihtimalle ilişkiyi bitirir (seninle −10). Bitirmezse seninle −20.
  - **Görmezden gel:** sır elinde kalır.
  - **Boşa.**
- Ortaya çıktıktan sonra doğan çocuk doğrudan gerçek babasıyla açıklanır.

## Boşanma

Oyunda boşanma yok. Mod eş bağını kaldırır:

- **Nereye gider:** eşin doğduğu klana döner; o yoksa babasının ya da annesinin klanına. Hiçbiri yoksa senin klanında kalır ama artık
  eşin değildir.
- Çocuklar senin klanında kalır. Sonra oyunun kendi sistemiyle yeniden evlenebilirsin.
- **Engeller:** eşin hamileyken boşanamazsın (doğumdan sonra). Eşin bir partiye liderlik ediyorsa önce onu görevden almalısın.
- **Haklı boşanma** (aldatma ortaya çıktıysa): eşinle −40, ailesinin lideriyle −5.
- **Haksız boşanma:** eşinle −40, ailesinin lideriyle −20, krallığındaki onurlu klan liderleriyle −5.

## Görünüm

- Kişiler sekmesinde eşin her zaman listenin sonunda.
- Eşinin dosyasında "Eşini araştır", (biliyorsan) "Yüzleş" ve "Boşa...". Boşa onayında sonuçlar yazılır.
- "Süren" sekmesinde "Casus başın eşini araştırıyor" satırı ve kalan gün.
- Resimler: `sonuc_aldatma`, `sonuc_bosanma`.

## Kayıt

İlişki ("spouseaffair": sevgili, başlangıç, ortaya çıktı mı, nasıl, çocuklar), araştırma ve ayrılık süresi ("spouse") ve gizli
gebelikler ("pregnancy" + "hidden") kampanya kaydına yazılır.

## Test

- Formüller, kayıt ve ayar: `tests/CheatMenu.Tests` (FidelityTests, durum ve ayar testleri).
- Oyunda: YAPILACAKLAR > Eşinin gizli hayatı.
