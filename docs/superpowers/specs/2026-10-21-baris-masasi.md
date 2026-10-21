# Barış Masası: şartlı barış antlaşması

21 Ekim 2026. Kullanıcı isteği: "Çok öndeyiz, adamlar barış teklif ediyor; haraç, kale hatta şehir alabilelim anlaşma ile. Para + kale,
para + asker gibi şeyler eklersek güzel olur. Oyunda sadece haraç alma var diye biliyorum."

## Oyunda zaten olan (bozulmuyor)

- Oyunun kendi barış teklifleri (haraçlı) ve Krallık > Diplomasi'deki barış önerisi (oylama).
- Diplomacy: bir taraf savaş yorgunluğunda %100'e varınca barış zorla gelir; kaybeden savaş tazminatı ve 5 yıl haraç öder, bu savaşta
  aldığı yerleri geri verir. Barış Masası bunun yerine geçmez; tükenmeden önce pazarlık etmeyi sağlar.

## Masa (`UI.PeaceTableScreen`, `PeaceTableVM`, prefab `TvhPeaceTable` + `TvhPeaceTerm`)

- Açılış: Savaş Raporu'nun sağ üstündeki **Barış Masası...**, Insert > Entrika > **Barış Masası...**, elçi kartındaki "Barış Masası'na otur".
- **İstediklerin** (her biri - / + ile): haraç (günde 100/250/500/1.000, 60-120 gün; oyunun haraç düzeniyle her gün ödenir), savaş
  tazminatı (hükümdarın kesesinin %5-75'i), garnizonlarından asker (20-150; partindeki boş yer ve onların sağlam askeri kadar, yüksek
  kademeliler önce), sınırına en yakın 8 şehir ve kale (her biri ayrı ayrı istenir).
- **Verdiklerin:** altın (keseden 1.000-50.000), haraç ödemek (kaybederken barış almak için). Hem haraç alıp hem verilemez.
- Altta: şartların özeti, **kabul olasılığı**, şartların ağırlığı ve pazarlık gücün, engel varsa nedeni; **Şartları sıfırla**, **Teklif et**.

## Kurallar (`PeaceTreatyRules`, saf, testli)

- **Pazarlık gücü** = onların savaş yorgunluğu - seninki (Diplomacy) + güç oranı ((oran-1)×40, en fazla ±40) + başka cepheleri (her biri
  +10, en fazla 3) + oyunun barış puanı (barışa yatkınlarsa +15, değilse -15) + elçi gönderdilerse +15 (5 gün).
- **Ağırlıklar:** tazminat = kesenin yüzdesi (kesenin tamamı 100; kese en az 20.000 sayılır); haraç = toplamın keseye oranının %60'ı;
  asker = her 4 asker 1; kale 30, şehir 55, hükümdarın kendi klanınınsa +15, bütün yerlerini istersen +50. Senin verdiğin altın
  (%80) ve haraç (%50) ağırlığı düşürür.
- **Kabul** = %50 + pazarlık gücü - ağırlık, %3-95. Örnek: çok öndeysen (yorgunluk 80'e 20, güç 1,5 kat, barışa yatkınlar: güç 95) şartsız barış
  %95, bir kale ve kesenin dörtte biri %90, iki şehir %35, iki şehir ve kesenin yarısı %3.
- **Ret:** 7 gün aynı krallığa teklif götürülmez; olasılık %30'un altındaysa şartlar hakaret sayılır (hükümdarlarıyla ilişki -3).
- **Kabul:** önce barış (`MakePeaceAction.ApplyByKingdomDecision`, haraçla), sonra yerler sana (`ChangeOwnerOfSettlementAction.ApplyByBarter`;
  garnizon ve bağlı köyler oyunun kuralıyla), tazminat hükümdarın kesesinden, asker garnizonlarından partine. Yerini kaybeden klanın
  lideri sana -10 kırgın; ağır şartlarda (ağırlık 40+) hükümdarlarıyla ilişki ağırlık/10 kadar düşer (en fazla -10).

## Elçi (`PeaceTreaties.Weekly`)

Haftada bir: hükümdarsan ve savaştığın bir krallığın pazarlık gücün 30'u geçtiyse %10-45 olasılıkla elçi gelir (krallık başına 21 günde
bir, haftada en fazla bir). Teklifleri, kabul edeceklerinden emin oldukları (%80+) en büyük tazminat; yoksa şartsız barış. Kart:
**Kabul et**, **Barış Masası'na otur: daha fazlasını iste** (elçi gönderdikleri için +15), **Geri çevir**.

## Sınırlar

- Yalnızca hükümdar antlaşma yapar (vassal: Krallık > Diplomasi'den barış önerisi, oylama).
- Diplomacy'nin barış koşullarına uyulur: savaşın en kısa süresi, görev bağı, oyuncunun kuşatması, isyancı krallık (iç savaş Diplomacy'nin
  kuralıyla biter). Diplomacy'nin barış bedeli (nüfuz/altın) alınmaz: bedel masadaki şartlar.
- Kayıt: `peacetalk` kayıtları (ret ve elçi bekleme günleri).

## Resimler

`sonuc_baris_elcisi` (yoksa haberci resmi), `sonuc_baris_antlasmasi` (yoksa elçi resmi). Prompt'lar `docs/gorseller/olay-kartlari.md` 68-69.

## PC'de doğrulanacak

`MakePeaceAction.ApplyByKingdomDecision`'da haracın yönü ve kimin kesesinden ödendiği; barışta esir lordların serbest kalıp kalmadığı;
`ApplyByBarter`'ın garnizonu ve köyleri nasıl devrettiği (oyunun DLL'leri açılarak).
