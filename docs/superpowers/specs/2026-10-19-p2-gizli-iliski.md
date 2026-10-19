# P2.4 — Gizli ilişki ve gayrimeşru çocuk sırları (sahnesiz)

**Tarih:** 2026-10-19 · **Durum:** Kodlandı (oyunda test bekliyor) · **Yol haritası:** bölüm 4, sıra 4
**Politika:** yol haritası bölüm 5. Yalnızca metin; sahne, animasyon ya da çıplaklık yok. İki taraf da yetişkin olmak **kodda zorunlu**.

## Neden

Şantajın en doğal yakıtı. Eskiden "gizli ilişki" sırrı soyuttu: evli bir lordda %15 ihtimalle "eşinden gizli bir ilişkisi var", kiminle
olduğu yok. Artık sevgili gerçek bir lord. Ondan da ikinci bir sır çıkabilir: gayrimeşru çocuk. İkisi de mevcut sır, ifşa ve şantaj
sistemini besliyor; yeni bir kopuk mekanik yok.

## Gizli ilişki

- **Nasıl bulunur:** casusluk ya da esir sorgusu sır bulduğunda, evli bir hedefte %15 ihtimalle aday olur (eskisi gibi). Fark:
  sevgili gerçek bir lord seçilir.
- **Sevgili adayı:** yetişkin (18+), yaş farkı en fazla 20, karşı cinsten (oyunun kendi evlilik sistemi gibi), hedefin eşi değil.
  Akraba olmaz: anne-baba, çocuk, kardeş, büyükanne-büyükbaba, eşin anne-babası ya da çocuğu. Önce aynı krallıktan seçilir.
  Oyuncunun klanından kimse, oyuncunun kendisi de seçilmez: senin ailene dair hiçbir şey uydurulmaz.
- **İki taraflı:** sır iki kişide de kayıtlıdır. Kişiler sekmesinde ikisi de görünür, **ikisine de şantaj** yapılabilir (sevgili
  evli değilse bile: evli biriyle ilişki onun için de skandal).
- Seçim tohumlu zarla: kaydı yükleyip tekrar denemek başka bir sevgili getirmez.

## Gayrimeşru çocuk

- **Nasıl bulunur:** gizli ilişkisini bildiğin birinde yeniden sır aranırken (casusluk, sorgu) %35 ihtimalle aday olur. Koşul:
  çiftin annesinin, resmî babası başkası olan, hayattaki bir çocuğu var.
- **18+ kuralı:** iki ebeveyn de çocuğun doğumunda en az 19 yaşında olmalı (gebeliğin başında da 18+). `SecretRules.CouldBeParent`
  her yaş için testte denetleniyor.
- Oyuncunun klanındaki çocuklar ve babası oyuncu olan çocuklar seçilmez.
- **Çocuğa dokunulmaz:** aile ağacı, klan ya da miras değişmez; çocuk hiçbir entrikanın hedefi olamaz (eskisi gibi). Sır yalnızca
  ebeveynlerin itibarıyla ilgili.

## Ne yapılır (sırrı bilen sen)

| Karar | Etkisi |
|---|---|
| **İfşa et** (herkese) | Gizli ilişki: aldatan, eşiyle −30 ve eşin klan lideriyle −15. Aldatılan eş, sevgiliyle −15. İki taraf evliyse ikisi için de. Gayrimeşru çocuk: resmî baba iki ebeveynle de −40, gerçek babanın eşi onunla −20, resmî babanın klan lideri anneyle −15; gizli ilişki de açığa çıkar. %30 ihtimalle kaynağın sen olduğun anlaşılır: ikisiyle de −20. Kart: "Skandal". |
| **Eşine söyle** (yeni) | Saray yerine yalnızca aldatılan eşe (gayrimeşru çocukta resmî babaya) baş başa söylenir. Aldatanla arası tam ceza, sevgiliyle de bozulur; sana minnettar kalır (+10). Kayınlar ve hükümdar duymaz. %30 ihtimalle aldatan söyleyenin sen olduğunu öğrenir (−20). Güçlü bir eşi (hükümdar, büyük klan lideri) kazanmanın yolu. |
| **Şantaj** | Mevcut şantaj entrikası; iki taraftan birine. Şantaj tutmazsa iz örtülür: sır iki taraftan da silinir. |

- Şantaj sürerken (iki taraftan birine) sır ifşa edilemez, eşe de söylenemez.
- İfşa ya da eşe söylemek sırrı iki tarafta da kullanır.
- Hakkında sır bildiğin lordla konuşurken oyunun ikna turlarında +5 (P2.2) iki taraf için de geçerli.

## Meşrulaştırma

Yol haritasındaki "meşrulaştırma kararı" **P2.8'e (varis seçimi ve meşrulaştırma)** kaldı. Bannerlord'da bir kahramanın babasını oyun
ortasında değiştirmek aile ağacını bozar: eski babanın çocuk listesinden düşmez. Bu yüzden P2.4 aile ağacına dokunmuyor. Meşrulaştırma
oyuncunun kendi hanedanı için, varis seçimiyle birlikte tasarlanacak.

## Ayar

MCM > Entrika > **Gizli ilişki ve gayrimeşru çocuk sırları** (Insert > Kurallar'da da). Varsayılan açık. Kapalıyken bu sırlar bulunmaz;
bilinenler kalır.

## Görünüm

- Kişi dosyasında sır satırı isimli: "Ira ile evli olduğu hâlde Lucon ile gizli bir ilişkisi var." / "Kızı Elys'in gerçek babası
  Lucon; resmî babası Garios bunu bilmiyor."
- Sır satırında ikinci düğme: **Eşine söyle**. Neden olmadığı kırmızı notla yazar.
- Klasik menü: İstihbarat raporları > lord > "Eşine söyle (X): ..." girdisi.
- Resimli kartlar: `sonuc_gizli_bulusma` (sır bulundu), `sonuc_skandal` (ifşa), `sonuc_fisilti` (eşine söylendi). Prompt'lar:
  `docs/gorseller/olay-kartlari.md`.

## Test

- 18+ ve yaş kuralları, sır türleri, kayıt (sevgili, çocuk, "eşine söylendi"), ayar: `tests/CheatMenu.Tests` (RomanceTests, durum ve
  ayar testleri).
- Oyunda: YAPILACAKLAR > P2.4.
