# P2.9 — Saray günü (divan ve dilekçeler)

**Tarih:** 2026-10-19 · **Durum:** Kodlandı (oyunda test bekliyor) · **Yol haritası:** bölüm 4, sıra 9

## Neden

Barış zamanı yönetimin kendisi oyun olsun. Olay kartlarının doğal yeri burası. Yeni değerler icat edilmiyor: kararlar oyunun kendi
sadakat, güvenlik, refah ve erzak değerlerini, eşrafla ilişkini ve kişilik özelliklerini etkiliyor.

## Nasıl

- **Nerede:** kendi şehrin ya da kalende, yerleşim menüsünde "Saray günü düzenle (dilekçeler)". Aynı yerde 21 günde bir; beklerken düğme kapalı
  ve kaç gün kaldığını söyler.
- **Divanda üç dilekçe** gelir, her biri resimli bir kartta. Dilekçe sahibi o yerin gerçek bir eşrafıdır (tüccar, zanaatkâr, çete
  lideri, muhtar); kartta portresi görünür. Uygun eşraf yoksa portre çıkmaz.
- **Her seçeneğin sonucu düğmede yazar**, ör. "Vergiyi indir (600 dinar, refah +60, Cömertlik +)". Parası yetmeyen seçenek kapalıdır.
- **Sonunda özet kartı:** bugünkü kararlar.

## Dilekçeler (12; kalede şehir işleri çıkmaz)

| # | Dilekçe | Seçenekler (özet) |
|---|---|---|
| 1 | Toprak anlaşmazlığı | tapu (Onur) / işleyen yoksul (Merhamet, sadakat) / böl (altın, Hesapçılık) |
| 2 | Ekmek hırsızı (şehir) | kırbaç ve sürgün (güvenlik, Merhamet −) / angarya (Onur) / affet (Merhamet, sadakat) |
| 3 | Tüccarların vergi talebi (şehir) | indir (refah, Cömertlik) / muhafız koy (güvenlik, Hesapçılık) / reddet |
| 4 | Askerin dulu | maaş (sadakat, Cömertlik) / iş ver / reddet (Cömertlik −) |
| 5 | Çete şikâyeti (şehir) | çeteyi dağıt (güvenlik, Cesaret; çete lideri −) / haraçtan pay al (altın, Onur −) / görmezden gel |
| 6 | Salgın (şehir) | karantina (refah −) / hekim (altın, sadakat) / dua |
| 7 | Kıtlık | ambarı aç (erzak −, sadakat) / pahalıya sat (altın, sadakat −) / tahıl al (altın, erzak) |
| 8 | Zengin oğlanın suçu (şehir) | hapset (Onur; tüccar −, zanaatkâr +) / kan bedeli / affet (Onur −) |
| 9 | Tapınak bağışı | cömert / az / reddet |
| 10 | Yakalanan casus | **sorgula: efendisi hakkında rapor** (önce savaştığın krallıklardan) / as (güvenlik) / bırak |
| 11 | Lonca tekeli (şehir) | izin ver (altın, refah −) / reddet (refah) |
| 12 | Köylülerin koruma talebi | devriye (güvenlik) / kendileri silahlansın |

Kişilik etkileri oyunun görev ödülleri gibi verilir (`TraitLevelingHelper`): bir karar küçük bir puan verir, özellik zamanla birikir.

## Kayıt

Divan bekleme süreleri ("courtday") kampanya kaydına yazılır.

## Test

- Dilekçe verisi (benzersiz, 2–3 seçenek, sınırlar, kalede yeterli dilekçe), çekiliş, sonuç metni, kayıt: `tests/CheatMenu.Tests`
  (PetitionTests).
- Oyunda: YAPILACAKLAR > P2.9.
