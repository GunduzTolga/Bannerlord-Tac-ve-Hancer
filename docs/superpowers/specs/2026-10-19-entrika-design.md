# Entrika Sistemi — Tasarım

**Tarih:** 2026-10-19 · **Oyun:** v1.4.8 + War Sails v1.2.8
**Durum:** Onaylandı (2026-10-19). Bölüm 13'teki sorular cevaplandı. E0, E1 ve E2 kodlandı; oyunda test bekliyor.
Uygulama sırasında verilen küçük kararlar bölüm 14'te (E1) ve 15'te (E2).

**Kullanıcı kararları (bu konuşmada verildi):**
- Ayrı mod yok. CheatMenu "her şeyi içeren" moda dönüşür: hileler + entrika (+ ileride başka sistemler).
- Entrika sistemi CK3'ten esinlenir ama Bannerlord'a uyarlanır. Ayrıntılı olacak ve oyun dengesini bozmayacak.

Bannerlord'da bir yıl 84 gün (4 mevsim × 21 gün). Aşağıdaki süreler buna göre seçildi.

---

## 1. Hedefler ve denge ilkeleri

1. **Her entrikanın bedeli var:** para, zaman, risk ve kişilik (Onur/Merhamet). Bedava ya da risksiz entrika yok.
2. **Erken oyunda zor, geç oyunda güçlü ama asla garanti değil.** Casus başının Roguery yeteneği büyüdükçe entrika açılır.
   Bir hükümdarı öldürmek en iyi koşulda bile garanti değil (hükümdar için tavan %70).
3. **Karar anları:** Oyuncu "şimdi mi vursam, bekleyip şansımı mı artırsam (ama yakalanma riskini büyütsem)" diye düşünmeli.
4. **Dünyayı yıkmasın:** Eşzamanlı entrika sınırı, aynı hedefe bekleme süresi, çocuklara dokunulmaz.
   Yapay zekâ entrikaları nadir ve ayarlanabilir (E2).
5. **Oyunun kendi sistemlerine bağlı:** Roguery XP'si, suç puanı, ilişki, kişilik, klan değiştirme, esir alma, oyunun kendi ölüm bildirimi.
   Ayrı bir "puan" icat etmek yerine oyunun mevcut değerleri kullanılır.
6. **Sırlar gerçek veriden gelir:** Bir lordun sırrı onun gerçek kişiliğinden ve durumundan üretilir.
   Onurlu bir lord temiz çıkabilir; şantaj herkese işlemez.
7. **Kaydet-yükle hilesi işe yaramaz:** Zarlar entrika başlarken belirlenen bir tohumdan gelir.
   Aynı seçimleri yaparsan aynı sonuç çıkar (bkz. 11.4). İsteyen zaten hile menüsünü kullanır.
8. **Hileler ayrı ve isteğe bağlı:** "Her zaman başarılı" gibi şeyler yalnızca Hileler menüsünde. Normal oyunda denge korunur.

---

## 2. Modun yeni yapısı

### 2.1 Ad
- Launcher'da görünen ad değişir (bkz. soru 1). İç kimlik (`Id=CheatMenu`) **aynı kalır**. Böylece:
  oyun klasörü, ayar dosyası (`CheatMenu.txt`) ve eski kayıtların modül listesi bozulmaz, yeniden kurulum gerekmez.

### 2.2 Ana menü (Insert) — öneri
Şu an kökte 13 satır var. Entrikayla birlikte gruplamak okunurluğu artırır:

```
Ana menü
├─ Entrika ▸            (yeni)
├─ Hileler ▸            Para & Ekonomi, Karakter, Ordu / Parti, Savaş içi, Harita, Entrika hileleri
├─ Editörler ▸          Adlandır, Kahraman editörü, İlişkiler, Yerleşim editörü, Eşya ekle
├─ Klan & krallık ▸
├─ Bilgi ▸
└─ Kurallar ▸           (mevcut kurallar + Entrika ayarları)
```

Bedeli, bazı hilelere bir tık daha fazla gerekmesi (bkz. soru 2).

---

## 3. Temel kavramlar

| Kavram | Açıklama |
|---|---|
| **Casus başı** | Oyuncu klanından bir kahraman (yoldaş ya da aile). Entrikaların gücü onun **Roguery** yeteneğinden ve **Hesapçı (Calculating)** kişiliğinden gelir. Entrikalardan Roguery XP'si kazanır. |
| **Entrika** | Bir hedefe karşı başlatılan, günler içinde ilerleyen plan. Türü, hedefi, aşaması, başarı ve keşif değerleri vardır. |
| **Aşama** | Entrika aşama aşama ilerler. Her türün **en az** ve **en fazla** aşama sayısı vardır. En az sayıya ulaşınca harekete geçilebilir. |
| **Fırsat** | Zorunlu aşamalardan sonra beklenen her aşama başarıya eklenir. Ama her aşamada yeniden keşif zarı atılır. |
| **Başarı şansı** | Harekete geçince atılan zar. |
| **Keşif şansı** | Her aşama sonunda entrikanın ortaya çıkma olasılığı. |
| **Olay** | Aşamalar sırasında çıkan, seçimli küçük hikâyeler (rüşvet isteyen hizmetkâr, yakalanan ajan…). Başarıyı, keşfi ve maliyeti etkiler. |
| **İstihbarat raporu** | Casusluk entrikasının ürünü. Hedefin konumu, gücü, kişiliği. 60 gün geçerli; o hedefe karşı entrikalara bonus verir. |
| **Sır** | Casuslukla bulunan, hedefin gerçek kişiliğinden ve durumundan türeyen bilgi. E1'de ifşa edilebilir, E2'de şantajla **koza** dönüşür. |
| **Koz** (E2) | Bir kahraman üzerindeki baskı aracı (CK3'teki "hook"). Altın, destek, esir serbest bırakma, klan değiştirme gibi taleplerde harcanır. |
| **Gölge ağı** (E2) | Bir şehirde çete lideri üzerinden kurulan muhbir ağı. O şehirdeki ve krallıktaki hedeflere bonus verir, bakım ister, açığa çıkabilir. |
| **Kan davası** (E2) | Cinayetin ortaya çıkarsa kurbanın yakınları bir süre sana karşı entrika adayı olur. |

---

## 4. Entrika türleri

| Tür | Faz | Amaç | Aşama (en az–en fazla) | Risk |
|---|---|---|---|---|
| Casusluk | E1 | Rapor + sır arama | 1–2 | Düşük |
| Suikast | E1 | Hedef "öldürüldü" diye ölür | 2–5 | Çok yüksek |
| Şantaj | E2 | Sırrı koza çevirir | 1–2 | Orta |
| Kaçırma | E2 | Hedef senin esirin olur (fidye vs.) | 2–4 | Yüksek |
| Sabotaj | E3 | Kuşattığın şehrin yiyeceğini/garnizonunu düşürür | 1–3 | Orta |
| Kışkırtma | E3 | Şehrin sadakatini düşürür, isyana iter | 2–4 | Orta |
| İtibar zedeleme | E3 | Hedefin hükümdarıyla ilişkisini bozar, klan değiştirmeye iter | 1–3 | Düşük-orta |

Lordu kendi krallığına çekmek için ayrı bir "ayartma" entrikası **yok**. Oyunun kendi ikna sistemi zaten var.
Bunun yerine koz (E2) ve itibar zedeleme (E3) o sistemi besler.

**Durum (20 Ekim):** sabotaj ve kışkırtma kodlandı, ayrıntılar `2026-10-20-e3-sabotaj-kiskirtma.md`'de (sabotajda aşama 1–3, kışkırtmada 2–4). İtibar zedeleme ayrı bir entrika olmadı: casus başının "Söylenti yay" görevi bu işi yapıyor.

---

## 5. Suikast (E1) — ayrıntılı

### 5.1 Kimler hedef olabilir
- Yaşayan, **yetişkin** (çocuk değil) bir kahraman: lord, yoldaş ya da eşraf.
- Oyuncunun kendisi ve **oyuncu klanının üyeleri olamaz** (E1; bkz. soru 3).
- Tanıştığın biri (`Hero.HasMet`) **ya da** hakkında geçerli istihbarat raporun olan biri olmalı.
- Aynı hedefe aynı anda tek entrika olabilir. İfşa olmuş ya da başarısız bir girişimden sonra o hedefe **120 gün** bekleme süresi var.
- Hikâye modunun kritik karakterleri hariç tutulur (doğrulanacak).

### 5.2 Akış
1. **Başlat:** Entrika → Yeni entrika → Suikast → hedef seç. Listede her hedefin yanında tahmini başarı ve maliyet yazar.
   Ardından bir özet görünür: maliyet, aşama süresi, başarı, aşama başına keşif. Onaylayınca maliyet ödenir.
2. **İlerleme:** Her gün sayaç ilerler. Aşama bitince:
   keşif zarı atılır → (yakalanmadıysan) "Aşama 2/5 tamamlandı" bilgisi gelir → %50 olasılıkla bir **olay** popup'ı çıkar.
3. **Harekete geç:** En az 2 aşamadan sonra entrika detayında "Harekete geç" açılır. İstersen o anda rüşvet eklersin.
   5. aşamaya gelince entrika kendiliğinden harekete geçmez, oyuncuya sorar.
4. **Hedef savaşın ya da kuşatmanın içindeyse** harekete geçilemez; bitince tekrar denenir.
5. **Sonuç:** Başarı zarı atılır → sonuç popup'ı gelir (hedefin portresiyle) → bölüm 5.5'teki sonuçlar uygulanır.

### 5.3 Başarı şansı

| Etken | Değer |
|---|---|
| Taban | %10 |
| Casus başının Roguery'si | + Roguery ÷ 10 (en fazla +30) |
| Casus başının Hesapçı seviyesi | + seviye × 3 (−6 … +6) |
| Güncel istihbarat raporu (≤60 gün) | +10 |
| Fırsat: 2. aşamadan sonraki her aşama | +6 (en fazla +18) |
| Olay seçimleri (toplam) | −15 … +15 |
| Harekete geçerken rüşvet | Küçük +5 / Orta +10 / Büyük +15 |
| Gölge ağı (E2) | 0 … +15 |
| Hedefin Roguery'si | − Roguery ÷ 20 (en fazla −15) |
| Hedefin Hesapçı seviyesi (yalnızca artı) | − seviye × 3 |
| Hedefin klan kademesi | − kademe × 2 |
| Hedef klan lideriyse | −5 |
| Hedef hükümdarsa | −15 |
| Hedef kendi klanının yerleşimindeyse | −10 |
| Hedef bir ordunun içindeyse | −10 |
| Hedef senin esirinse ya da senin yerleşimindeyse | +20 (ama iz bırakma ×2, bkz. 5.5) |
| Hedef tetikteyse (son 180 günde girişim sezdi) | −15 |
| Zorluk ayarı | Kolay +10 / Normal 0 / Zor −10 |
| **Sınır** | **%5 … %85** (hükümdar için en fazla %70) |

### 5.4 Keşif şansı (her aşama sonunda)

| Etken | Değer |
|---|---|
| Taban | %12 |
| Casus başının Roguery'si | − Roguery ÷ 25 (en fazla −12) |
| Gölge ağı (E2) | 0 … −6 |
| Hedefin Roguery'si | + Roguery ÷ 30 (en fazla +10) |
| Hedefin Hesapçı seviyesi (yalnızca artı) | + seviye × 2 |
| Olay seçimleri | ± |
| Zorluk ayarı | Kolay −3 / Zor +3 |
| **Sınır** | **%2 … %40** |

**Süre ve maliyet:**
- Aşama süresi: 7 gün − Roguery ÷ 60, en az 4 gün.
- Başlatma maliyeti: 1.000 + 750 × hedefin klan kademesi. Klan lideriyse +1.000, hükümdarsa toplam ×2.
- Rüşvet: Küçük = başlatma × 0,5 · Orta = × 1 · Büyük = × 2.
- İptal edersen para geri gelmez.

**Örnek hesaplar** (E1, gölge ağı yok):

| Senaryo | Başarı (2 aşama → 5 aşama) | Keşif / aşama (5 aşamada toplam) | Maliyet |
|---|---|---|---|
| **Erken oyun:** casus başı Roguery 40; hedef kademe-2 klanın sıradan üyesi, yolda, Roguery 30 | %9 → %27; raporla %37 | %11 (toplam ~%45) | 2.500 |
| **Orta oyun:** Roguery 120, Hesapçı +1, raporlu; hedef kademe-4 klan lideri, kendi kalesinde, Roguery 60 | %9 → %27; orta rüşvetle %37 | %9 (toplam ~%38) | 5.000 + 5.000 rüşvet |
| **Geç oyun:** Roguery 250, Hesapçı +2, raporlu; hedef kademe-6 hükümdar, başkentinde, Roguery 100, Hesapçı +1 | %5 (alt sınır; ham değer %1) → %19; büyük rüşvetle %34 | %7 (toplam ~%32) | 13.000 + 26.000 rüşvet |

Yorum:
- Erken oyunda suikast çoğunlukla başarısız olur ya da ortaya çıkar. Erken oyun casuslukla Roguery kasma dönemi.
- Geç oyunda bir hükümdarı öldürmek pahalı ve üçte bir şanslı. E2'de gölge ağı bunu yaklaşık yarıya çıkarır.
- Bu sayılar oyunda denenince ayarlanır. Formüller saf kodda olduğu için ayarlamak kolay (bkz. 11.5).

### 5.5 Sonuçlar

**Başarılı, iz bırakmadı:**
- Hedef oyunun kendi "öldürüldü" ölümüyle ölür: `KillCharacterAction.ApplyByMurder(hedef, katil: yok)`.
  Klan lideriyse yeni lideri, hükümdarsa yeni hükümdarı oyun kendisi belirler.
- Casus başı Roguery XP'si kazanır.
- Oyuncuya küçük bir **Merhamet −** kişilik XP'si (vicdan, −40) verilir.

**Başarılı ama iz bıraktı** (iz şansı = o anki keşif şansı × 2; hedef senin esirinse × 4) → **Ağır ifşa:**
- Kurbanın klanındaki yetişkinlerle ilişki −50.
- Kurbanın krallığındaki diğer klan liderleriyle −20.
- Senin krallığındaki onurlu (Onur ≥ 1) lordlarla −10.
- Kurbanın krallığında suç puanı +60.
- Ağır kişilik cezası: Onur −150, Merhamet −100 XP.
- Kurban hükümdarsa ve sen bağımsızsan ya da hükümdarsan: savaş (zaten savaşta değilseniz).
- Kurbanla aynı krallıktaysan **"Hükümdar hesap soruyor"** olayı çıkar:
  - Kan bedeli öde: altın (5.000 × kurbanın kademesi) + 100 etki.
  - İnkâr et: Charm zarı. Tutmazsa krallıktan sürülürsün.
  - Krallıktan ayrıl.
- E2'de kurbanın yetişkin yakınlarıyla **kan davası** başlar.

**Başarısız** → hedef yaralı kurtulur. Yakalanma şansı: %50 − Roguery ÷ 10, en az %20.
- Yakalanırsan → **Hafif ifşa (girişim):** hedef ve klanıyla −30, hedefin hükümdarıyla −10, suç puanı +25, küçük Onur −.
- Yakalanmazsan → hedef 180 gün tetikte kalır (−15 başarı, bkz. 5.3).

**Aşama sırasında keşif** → entrika iptal olur. Hafif ifşa uygulanır, ama ilişki cezası −30 yerine −20 olur.

### 5.6 Olay kataloğu (E1, ilk 10)
Her aşamada %50 olasılıkla, aynı entrikada tekrar etmeden çıkar. Her olayın bir varsayılan seçeneği var: popup kapatılırsa o seçilir.

| # | Olay | Seçenekler |
|---|---|---|
| 1 | **Rüşvetçi hizmetkâr** — hedefin hizmetkârı para istiyor | Öde (maliyet ×0,3, +8 başarı) · Tehdit et (+5 başarı, +4 keşif) · Reddet |
| 2 | **Ajan yakalandı** | Kurtar (maliyet ×0,5) · Sustur (Merhamet −, risk yok) · Bırak (%35 olasılıkla hafif ifşa) |
| 3 | **Zehir mi, hançer mi?** — yöntem seçimi | Zehir (−3 keşif, −5 başarı) · Hançer (+8 başarı, +4 keşif) · Kaza süsü (yalnızca hedef yoldaysa: +3 başarı, iz şansı yarıya iner) |
| 4 | **Hedef yer değiştirdi** | Takip et (maliyet ×0,2) · Bekle (süren aşama iki kat uzun sürer) |
| 5 | **Rakip suikastçı** — başkası da hedefin peşinde | İşbirliği (+10 başarı, +5 keşif) · İhbar et (hedefle ilişki +10, entrika biter, para geri gelmez) · Görmezden gel |
| 6 | **Muhafız değişimi** — kısa bir fırsat penceresi | Hemen harekete geç (aşama sınırı yok sayılır, +10 başarı) · Bekle |
| 7 | **Vicdan azabı** — yalnızca casus başı Merhamet ≥ 1 ise | İkna et (Charm zarı) · Zorla (casus başıyla ilişki −15) · Vazgeç (entrika biter) |
| 8 | **Şüpheli tüccar** — biri entrikayı sezmiş | Sustur (maliyet ×0,3) · Yok say (+5 keşif) |
| 9 | **Ziyafet daveti** — hedef bir ziyafete katılacak | Ziyafette vur (+12 başarı, ama ifşa olursa ilişki cezaları ×1,5) · Geç |
| 10 | **Çete liderinin teklifi** — hedefin şehrinde bir çete lideriyle ilişkin ≥ 10 ise | Kabul et (+10 başarı; E2'de çete liderinin sende bir kozu olur) · Reddet |

---

## 6. Casusluk ve sırlar (E1)

### 6.1 Casusluk
- **Hedef:** herhangi bir yetişkin lord ya da gezgin (eşraf yok: sırları ve gezisi yok). Tanışmış olman gerekmez; tanışmadıysan −10 başarı.
  Bu, v1.3'ten beri lordlara başka lordların yerini soramamak şikâyetine de cevap verir.
- **Aşamalar:** 1–2. Aşama süresi: 5 gün − Roguery ÷ 100, en az 3 gün.
  2. aşama yalnızca sır bulma şansını artırır.
- **Başarı:** %50 + Roguery ÷ 5 − hedefin Roguery'si ÷ 10 − kademe × 2; hükümdar −10. Sınır %20 … %95.
- **Keşif / aşama:** %6 − Roguery ÷ 50 + hedefin Roguery'si ÷ 40. Sınır %1 … %25.
  Keşfedilirse hedefle ilişki −10 olur ve o hedefe 120 gün bekleme başlar.
- **Maliyet:** 500 + 250 × kademe.
- **Rapor (60 gün geçerli):**
  - hedefin şu anki gerçek konumu ve partisinin gücü
  - yaklaşık altını
  - hükümdarıyla ve seninle ilişkisi
  - kişilik özellikleri
  - o hedefe karşı entrikalara +10
- **Sır bulma şansı:** %30 + Roguery ÷ 6 (en fazla %80); 2. aşamada +20.

### 6.2 Sır türleri (gerçek veriden)

| Sır | Ne zaman var olabilir | İfşa edilince (E1) |
|---|---|---|
| Zimmet | Cömertlik ≤ −1 | Hükümdarla −20 |
| İhanet planı | Klan lideri ve hükümdarıyla ilişkisi ≤ −10 | Hükümdarla −30 |
| Rüşvet alma | Onur ≤ −1 | Hükümdarla −15 |
| Esirlere zulüm | Merhamet ≤ −1 | Hükümdarla −10, onurlu lordlarla −5 |
| Korkaklık | Cesaret ≤ −1 | Hükümdarla −15 |
| Ağır borç | Altını çok düşük | Hükümdarla −10 |
| Kaçakçılık bağlantısı | Roguery ≥ 100 | Hükümdarla −15 |
| Gizli ilişki | Evli; rastgele (%15) | Eşiyle −30, eşinin klanıyla −15 |

- Koşulu tutan sır yoksa hedef **temiz** çıkar. Onurlu lordlar zor şantaj edilir.
- **İfşa et** (E1): yukarıdaki cezalar uygulanır. %30 olasılıkla kaynağın sen olduğun anlaşılır; o zaman hedefle −20.
  İfşa, bir lordu krallığından koparmanın ya da bir hükümdarın tabanını zayıflatmanın aracı.
- **Şantaj** (E2): sır, hedef üzerinde bir koza dönüşür (bkz. 8.1).

---

## 7. Sınırlar ve kötüye kullanım önlemleri

- **Eşzamanlı düşmanca entrika:** 1. Casus başının Roguery'si 125 olunca 2, 225 olunca 3.
- **Casus başı** başka bir partinin lideri ya da vali olamaz.
  Esir düşerse entrikalar durur; ölürse iptal olur ve para geri gelmez.
- Aynı hedefe tek entrika; başarısız ya da ifşa olmuş girişimden sonra aynı hedefe 120 gün bekleme.
- Hedef entrika sırasında ölürse (savaş vb.) entrika kendiliğinden kapanır; para geri gelmez.
- Çocuklar, oyuncu ve oyuncu klanı (E1) hedef olamaz.
- **Kaydet-yükle:** zarlar entrikanın tohumundan gelir; yeniden yüklemek sonucu değiştirmez.

---

## 8. E2 — kozlar, ağlar, yapay zekâ entrikaları (özet tasarım)

### 8.1 Şantaj ve koz
- Bir sır, şantajla kozun olur: 1–2 aşama, keşif riski orta. Koz bir yıl (84 gün) geçerli.
- **Koz harcanarak:**
  - altın (hedefin altınının %20'si)
  - etki
  - esir serbest bıraktırma
  - kısa süreli ilişki zorlama
  - klan lideriyse krallık değiştirtme: 2 koz ve ek koşullar; çok pahalı
- Koz harcamak hedefle ilişkiyi −10 düşürür.
- Kozun olduğu hedefe suikastta +10.
- Lord diyaloğuna yeni satır: *"Seninle özel bir konuyu konuşmalıyız…"*

### 8.2 Kaçırma
- Başarıda hedef ana partinin esir listesine geçer (`TakePrisonerAction`). Fidye gibi şeyleri oyunun kendi sistemi halleder.
- Keşif riski suikasttan yüksek.

### 8.3 Gölge ağı
- Çete lideri olan şehirlerde kasaba menüsüne **"Gölgelerde iş çevir"** seçeneği eklenir. Kurmak için çete lideriyle ilişki ≥ 0 ve altın gerekir.
- Seviye 1–3. Haftalık bakım ister.
- Etkisi:
  - o şehirdeki hedeflere +5 / +10 / +15 başarı, −2 / −4 / −6 keşif
  - aynı krallıktaki diğer hedeflere bunların yarısı
- En fazla ağ sayısı: 1 + Roguery ÷ 75.
- Ağlar yapay zekânın karşı casusluğuyla açığa çıkabilir.

### 8.4 Yapay zekâ entrikaları ve karşı casusluk
- **Kim entrika kurar:** sana karşı ilişkisi ≤ −40 olan klan liderleri ve kan davası olanlar.
  Hesapçı yüksek ya da Onur düşük olanlar daha olası.
- **Sıklık:** haftalık küçük bir şans. Aynı anda en fazla 1 (Az) / 2 (Normal) entrika. Ayardan kapatılabilir.
- **Hedefler:** oyuncu klanının yetişkin üyeleri ve yoldaşlar.
  **Oyuncunun kendisi yalnızca ayar açıksa** hedef olur (bkz. soru 4).
- **Formül:** oyuncununkiyle aynı. Yapay zekânın casus başı, klanındaki en yüksek Roguery'li kahramandır.
- **Karşı casusluk:** casus başın her hafta %20 + Roguery ÷ 5 (en fazla %90) olasılıkla sana karşı kurulan entrikayı sezer. Sezince seçeneklerin:
  - **İfşa et:** entrikacıyla ilişki −, entrika biter.
  - **Koruma tut:** altın karşılığı hedefe ek savunma.
  - **Karşı suikast:** bonuslu bir suikast entrikası başlar.
  - **Kendi kozuna çevir:** entrikacı hakkında otomatik "suikast planı" sırrı olur.
- **Kan davası:** ağır ifşadan sonra kurbanın yetişkin eşi, çocukları ve kardeşleri iki yıl boyunca nefret eşiğine bakılmadan entrika adayıdır.

---

## 9. Arayüz

### 9.1 Entrika menüsü
```
Entrika
├─ Casus başı: <ad> (Roguery 85)  ▸ değiştir
├─ Aktif entrikalar (1/1) ▸
│    └─ Suikast: <hedef> — Aşama 3/5, sonraki aşamaya 4 gün, başarı %27, keşif %11/aşama
│         ├─ Harekete geç (rüşvet: yok / küçük / orta / büyük)
│         ├─ Beklemeye devam
│         └─ İptal et
├─ Yeni entrika ▸ tür ▸ hedef (portreli; satırda tahmini başarı ve maliyet) ▸ özet ve onay
├─ İstihbarat raporları ▸ hedef ▸ rapor + sırlar ▸ ifşa et (E2: şantaj)
├─ Geçmiş (son 20 olay)
└─ (E2) Gölge ağları · Kozlar · Sezilen entrikalar
```

### 9.2 Popup kuralları
- Entrika popup'ları yalnızca **harita ekranındayken** ve başka popup açık değilken gösterilir.
  Savaşta, konuşmada ya da kasaba sahnesindeyken kuyrukta bekler. Popup açıkken oyun durur.
- Aşama tamamlandı gibi bilgiler popup değil, sol alttaki renkli mesajlardır.
  Popup yalnızca karar ve sonuç anlarında çıkar.
- Sonuç popup'larında hedefin portresi görünür (askerlerdeki yöntemle aynı).

---

## 10. Ayarlar ve hileler

**Kurallar → Entrika ayarları** (kalıcı, config dosyasında):
- Entrika zorluğu: Kolay / Normal / Zor
- (E2) Yapay zekâ entrikaları: Kapalı / Az / Normal
- (E2) Oyuncuya suikast: Kapalı (varsayılan) / Açık

**Hileler → Entrika** (oturumluk):
- Aktif entrikanın aşamalarını tamamla
- Asla fark edilme
- Suikast her zaman başarılı

---

## 11. Teknik mimari

### 11.1 Dosyalar
```
src/Intrigue/Core/          oyundan bağımsız, birim testli (TaleWorlds referansı yok)
  SchemeMath.cs             türler, başarı/keşif/maliyet/süre formülleri, tohumlu zar (SchemeRandom)
  SecretRules.cs            sır türleri, adları, ifşa cezaları
  IntrigueState.cs          entrikalar, raporlar, sırlar, bekleme süreleri, bildirimler, geçmiş + metin biçimi
  TextRecords.cs            kaçışlı satır biçimi (kayıttaki metin)
  Tr.cs                     isimlere Türkçe hâl eki (Caladog'un, Sargot'ta)
src/Intrigue/               oyun katmanı
  IntrigueBehavior.cs       CampaignBehaviorBase: günlük olay, SyncData
  SchemeFactors.cs          casus başı, hedef kuralları, SchemeMath girdileri, etiketler
  SchemeEngine.cs           başlat, günlük ilerleme, aşama, harekete geç, iptal, hile
  SchemeOutcomes.cs         sonuçları oyunun eylemleriyle uygular; rapor, sır bulma ve ifşa
  IntrigueEvents.cs         olay kataloğu
  IntriguePopups.cs         durumdan popup üretir ("yalnızca haritadayken")
src/Menu/Categories.Intrigue.cs   Entrika menüsü ve entrika hileleri
```

### 11.2 Kayıt güvenliği
- Tüm entrika durumu **tek bir metin** olarak kaydedilir: `SyncData("CheatMenu_Intrigue", ref string)`.
  İçinde bir sürüm satırı bulunur. Modun "kayda kendi veri türünü yazmaz" kuralı korunur.
- Kahramanlar `StringId` ile tutulur, yüklemede `Hero.Find(id)` ile bulunur.
  Bulunamayan kahramanın kaydı düşer ve oyuncuya bilgi verilir.
- **Doğrulanacak (E0):** mod kaldırılınca bu verinin sessizce yok sayıldığı.
  Test: kaydet → modu kapat → kaydı aç.

### 11.3 Kayıt
`SubModule.OnGameStart` içinde: `starter.AddBehavior(new IntrigueBehavior())`. Harmony yok.

### 11.4 Rastgelelik
- Her entrika başlarken bir tohum alır; her zar bu tohumdan çekilir ve zar sırası kayda yazılır.
- Kaydet-yükle aynı sonucu verir. Farklı seçimler farklı zarlar demektir.

### 11.5 Testler
`tests/CheatMenu.Tests` projesine Core dosyaları eklenir. Testler şunları kontrol eder:
- bölüm 5.4'teki örnek hesaplar (sayılar değişirse test bunu gösterir)
- tüm sınırlar
- yazma/okuma gidiş-dönüşü ve bozuk metne dayanıklılık

### 11.6 Doğrulanmış oyun parçaları (1.4.8 referans DLL'lerinde var)
- Ölüm ve eylemler:
  - `KillCharacterAction.ApplyByMurder(Hero, Hero, bool)`
  - `ChangeRelationAction`, `ChangeCrimeRatingAction.Apply`
  - `ChangeKingdomAction` (sürgün ve klan değiştirme), `DeclareWarAction.ApplyByPlayerHostility`
  - `TakePrisonerAction.Apply`
- Kahraman ve kişilik:
  - `Hero.Find(string)`, `Hero.HasMet`, `Hero.IsChild`, `Hero.LastKnownClosestSettlement`
  - `DefaultTraits.Calculating/Honor/Mercy/Valor/Generosity`, `Hero.SetTraitLevel`
  - `TraitLevelingHelper.OnIncidentResolved(trait, xp)` (oyunun rastgele olaylarının kişilik yolu)
- Altyapı:
  - `CampaignEvents.DailyTickEvent`, kahraman öldü olayları
  - `IDataStore.SyncData<T>`
  - `CampaignGameStarter.AddPlayerLine/AddDialogLine/AddGameMenuOption` (E2 diyalog ve kasaba menüsü)

### 11.7 Oyunda doğrulanacaklar
- `ApplyByMurder`'ın katil verilmezse kendiliğinden ilişki ya da suç cezası uygulayıp uygulamadığı.
- Kişilik XP değerlerinin (−40 … −150) oyunda hissedilir ama aşırı olmadığı.
- Portreli bildirimde "Tamam" düğmesinin seçim olmadan çalıştığı (çalışmazsa "Kapat" düğmesi var).
- Suç puanı eşikleri (kaç puanda şehirler düşmanlaşır).
- Bir vasalın `DeclareWarAction.ApplyByPlayerHostility` ile tüm krallığını savaşa sokup sokmadığı.
- Hikâye modu kritik karakterlerinin nasıl tanınacağı.

---

## 12. Aşamalar ve kabul ölçütleri

| Faz | İçerik | Bitti sayılması için |
|---|---|---|
| **E0** | Ana menü düzeni, görünen ad, `IntrigueBehavior` iskeleti ve kayıt testi | Kaydet-yükle sonrası test verisi duruyor; mod kapalıyken kayıt açılıyor |
| **E1** | Casus başı, casusluk + sırlar (ifşa), suikast (aşamalar, 10 olay, sonuçlar), sınırlar, zorluk ayarı, entrika hileleri | Bölüm 5–7'deki her kural oyunda bir kez görüldü; örnek hesaplar testlerde |
| **E2** | Şantaj/koz, kaçırma, gölge ağı + kasaba menüsü, yapay zekâ entrikaları + karşı casusluk + kan davası, diyalog satırları | Yapay zekâ bir entrika kurdu, sezildi, karşılık verildi |
| **E3** | Sabotaj, kışkırtma, itibar zedeleme, yapay zekâlar arası entrikalar (nadir), ün/korku, resimli olay kartları, isteğe bağlı gizlilik görevi | Ayrı tasarım belgesi |

**Resimler:** E1 ve E2 oyunun kendi portrelerini ve sancaklarını kullanır, özel resim gerekmez.
Resimli olay kartları E3'te. Özel bir ekran ve bilgisayarda Bannerlord Modding Kit gerekir.
O aşamada her kart için hazır prompt listesi verilecek (boyut, stil: oyunun yükleme ekranlarındaki mat yağlı boya hissi, sahne).

---

## 13. Kararlar (2026-10-19, kullanıcı: "hepsi önerdiğin gibi, adı Taç ve Hançer olsun")

| # | Soru | Karar |
|---|---|---|
| 1 | Görünen ad | **Taç ve Hançer** (iç kimlik `CheatMenu` kalır) |
| 2 | Ana menü gruplansın mı | Evet (bölüm 2.2) |
| 3 | Kendi klanından birine suikast | E1'de yok |
| 4 | Yapay zekâ oyuncuya suikast yapabilsin mi | Varsayılan kapalı, ayardan açılır (E2) |
| 5 | Tohumlu zar (kaydet-yükle sonucu değiştirmez) | Evet |

Orijinal sorular:

1. **Modun görünen adı ne olsun?** Öneriler: *Taç ve Hançer*, *Gölge Divanı*, *Kılıç ve Hançer*.
   İç kimlik `CheatMenu` kalır.
2. **Ana menü bölüm 2.2'deki gibi gruplansın mı?** Öneri: evet.
3. **Kendi klanından birine suikast olsun mu?** (CK3'teki akraba katli.) Öneri: E1'de hayır, ileride ağır cezalarla düşünülür.
4. **Yapay zekâ oyuncunun kendisine suikast yapabilsin mi?** Öneri: varsayılan kapalı, ayardan açılır.
   Oyuncu ölürse oyun varise geçer; varis yoksa kampanya biter.
5. **Kaydet-yükle sonucu değiştirmesin mi (tohumlu zar)?** Öneri: evet.

---

## 14. Uygulama notları (E1, 2026-10-19)

- **Olaylar:** her aşama sonunda %50; bir olay aynı entrikada bir kez çıkar. Casuslukta yalnızca 1, 2 ve 8 numaralı olaylar çıkar.
- **Son aşama:** 5. aşamada bir kez "plan hazır" popup'ı çıkar. "Sonra karar ver" denirse entrika menüden harekete geçmeyi bekler, sayaç durur.
- **Casusluk 1. aşaması:** "raporu al" ya da "bir aşama daha sır ara" diye sorulur.
- **Bildirimler** kayıtta tutulur: savaşta ya da konuşmadayken olan bir sonuç, haritaya dönünce gösterilir.
  Kaydedip çıkarsan bir sonraki yüklemede gösterilir.
- **Roguery XP'si:** aşama başına 200; başarılı casusluk +400; başarılı suikast +1.500; başarısız suikast +300.
  Oyuncunun öğrenme hızı ve Kurallar'daki XP çarpanı uygulanır.
- **Rapordan kısayol:** İstihbarat raporu ekranından doğrudan "Bu hedefe suikast başlat".
- **Hükümdar hedefin sırrı:** hükümdarın hükümdarı yok. İfşa cezası, krallığındaki klan liderleriyle ilişkisine yarım olarak uygulanır.
  Krallığı olmayan bir lordda bu ceza klanındakilere uygulanır.
- **Kayıt testi işareti** (E0) oyunda doğrulanana kadar Entrika menüsünde duruyor.

---

## 15. E2 uygulaması (2026-10-19)

Harmony gerekmedi:
- Kasaba menüsü `CampaignGameStarter.AddGameMenuOption("town", …)` ile eklendi.
- Lord diyaloğu `AddPlayerLine/AddDialogLine` ile eklendi (bağlantı: "hero_main_options" → bizim dallar → "lord_pretalk").

### 15.1 Şantaj ve koz
- **Koşul:** ifşa edilmemiş bir sırrını bildiğin, üzerinde kozun olmayan bir lord.
- **Aşamalar:** 1–2. 1. aşamadan sonra "şimdi tehdit et / bir aşama daha delil topla" sorulur.
- **Başarı:** 40 + Roguery ÷ 5 − hedefin Roguery'si ÷ 10 − kademe × 2 − (hükümdar 10) + sırrın ağırlığı ÷ 2 + (2. aşama 15)
  + olaylar + ağ ÷ 2. Sınır %15–90.
- **Keşif / aşama:** 8 − Roguery ÷ 40 + hedefin Roguery'si ÷ 40 − ağ ÷ 2. Sınır %2–30.
- **Maliyet:** 300 + 200 × kademe.
- **Başarı:** 84 gün geçerli koz. Hedefle ilişki −10. Varsa sana karşı entrikasından vazgeçer.
- **Başarısızlık:** sır yanar (listeden silinir), ilişki −20, 120 gün bekleme.
- **Koz kullanımları** (Entrika > Kozlar ya da diyalog): kullanmak kozu harcar ve ilişkiyi −10 düşürür.

  | Kullanım | Koşul |
  |---|---|
  | Altın | Hedefin altınının %20'si, en az 500 |
  | Etki | 50; yalnızca klan liderinden |
  | Esirleri bıraktırma | Hedefin partisinde ya da klan yerleşimlerinde tutulan, senin klanından/krallığından kahramanlar |
  | Krallığını bırakıp benimkine katılma | Sen bir krallıktaysan; 200 etki |
  | Krallığından ayrılma | — |
  | Kozu bırakma | İlişki +5 |
- Kozun olan hedefe suikast ve kaçırmada +10.

### 15.2 Kaçırma
- **Hedefler:** suikastla aynı (eşraf ve esirler hariç). Aşamalar 2–4.
- **Başarı:** suikast formülü −5; tavan %80, hükümdar için %60. Fırsat bonusu en fazla 2 aşama (+12).
- **Keşif:** suikast +3. **Maliyet:** suikastın ×0,8.
- **Başarı:** hedef ana partinin esir listesine geçer (`TakePrisonerAction`). Fidyeyi oyunun kendi sistemi halleder. Onur −20 XP.
- **İz bırakırsa:**
  - klanla −35, krallığıyla −15, onurlu lordlarla −5
  - suç puanı +40, Onur −80
  - hükümdarsa savaş; aynı krallıktaysan hükümdar yarım kan bedeli ister
  - kan davası yok
- **Olaylar:** "zehir mi hançer mi" ve "rakip suikastçı" yalnızca suikastta çıkar; diğerleri kaçırmada da çıkar.

### 15.3 Kan davası
- Ortaya çıkan bir cinayetten sonra kurbanın yaşayan yetişkin eşi, çocukları, annesi, babası ve kardeşleri 168 gün (2 yıl) boyunca kan davalıdır.
- Kan davalılar sana olan ilişkilerine bakılmadan entrika adayıdır ve seçilme ağırlıkları +4'tür.

### 15.4 Yapay zekâ entrikaları ve karşı casusluk
- **Ayarlar** (Kurallar): "Yapay zekâ sana entrika kursun" Kapalı / **Az** (varsayılan) / Normal; "Yapay zekâ senin kendine de suikast kurabilsin" (varsayılan kapalı).
- **Adaylar:**
  - seninle ilişkisi ≤ −40 olan klan liderleri ve kan davalılar
  - esir olanlar, üzerinde kozun olanlar ve zaten entrika kuranlar hariç
- **Haftalık yeni entrika olasılığı:** aday başına Az %3, Normal %6 (en fazla %25). Aynı anda Az 1, Normal 2 entrika.
- **Seçim ağırlığı:** 2, kan davası +4, Hesapçı +2, Onurlu −1 (en az 1).
- **Hedef:** klanının yetişkinleri (ayar açıksa sen de). Entrikacının partisi varsa %30 kaçırma, değilse suikast.
- **Süre:** 14–35 gün.
- **Sonuç zarı:** oyuncunun formülü, yapay zekânın gözünden: tüm aşamalar, raporlu, ajan klanındaki en yüksek Roguery'li kahraman.
  Oyuncunun zorluk ayarı tersine işler (Kolay'da yapay zekâ −10). Koruma tutulduysa −25.
- **Sezme:** haftalık %20 + casus başının Roguery'si ÷ 5 (en fazla %90; casus başı yoksa %5).
  - Sezilince entrikacı hakkında rapor ve "suikast planı" sırrı (ağırlık 40) eline geçer; bir popup sorar.
  - Seçenekler: ifşa et (plan biter, entrikacının hükümdarıyla −20, seninle −10) / koruma tut (2.000 dinar, −25) / karşı suikast (+10) / bekle.
- **Sonuçtan sonra kim yaptığı:** sezilmediyse başarıda %40, başarısızlıkta %50 olasılıkla öğrenilir (rapor + sır).

### 15.5 Gölge ağı
- **Nerede:** çete lideri olan şehirlerde kasaba menüsündeki "Gölgelerde iş çevir".
- **Kurma ve güçlendirme:**

  | Seviye | Maliyet | Çete lideriyle ilişki |
  |---|---|---|
  | 1 | 3.000 | ≥ 0 |
  | 2 | 6.000 | ≥ 10 |
  | 3 | 12.000 | ≥ 20 |

  Çete liderine hediye: 1.000 dinar karşılığı ilişki +5.
- **En fazla ağ:** 1 + casus başının Roguery'si ÷ 75.
- **Bonus** (en iyi ağ):
  - aynı şehirdeki hedefe başarı +5/+10/+15, keşif −2/−4/−6
  - aynı krallıktaki diğer hedeflere yarısı
- **Haftalık:**
  - bakım 150 × seviye; ödenemezse her ağ bir seviye düşer
  - seviye × %2 olasılıkla yapay zekâ ağı bulup dağıtır (şehrin sahibiyle −10)

