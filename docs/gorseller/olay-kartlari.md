# Görsel listesi: bütün resimler, dosya adları ve prompt'lar

Modda üretilmesi gereken resimlerin **hepsi** bu dosyada: **39 olay kartı resmi**, her biri için iki sürüm daha (en sonda: "Sürümler"). Başka resim gerekmiyor. Entrika ekranı, harita
düğmesi ve sekmeler oyunun kendi çizimlerini kullanıyor. Yeni bir özellik resim isterse buraya eklenir; birim testi koddaki her resim
adının burada olduğunu denetler.

## Teknik

- **Boyut:** 1600×900 (16:9), PNG. Daha büyük üretilirse sorun yok; oyuna koyarken küçültülür.
- **Dosya adı:** tablodaki ad (küçük harf, Türkçe karakter yok), örn. `olay_rusvetci_hizmetkar.png`.
- **Nereye:** oyunda doğrudan denemek için modun klasöründe `Modules\CheatMenu\Images\` içine koy; kart bir sonraki açılışta resmi gösterir
  (sprite paketine çevirmek gerekmez). Depoya eklenmesi için bana gönder ya da `Module/Images/` klasörüne koy.
- **Resim yoksa:** kartta ilgili kişinin portresi görünür; hiçbir şey bozulmaz. Bu yüzden resimler tek tek, istenen sırayla eklenebilir.

## Oyuna uygunluk (önemli)

Kartlar oyunun yükleme ekranlarıyla yan yana duracak; aynı dünyadan görünmeliler.

- **Dönem:** erken orta çağ, kabaca 11.–12. yüzyıl. Zincir zırh, lamel, kapitone, yün tunik, kürklü pelerin. **Tam plaka zırh, rönesans
  kıyafeti, büyü, ejderha, parlak fantastik zırh yok.**
- **Kültürler** (bir kart belli bir kültürü göstermiyorsa karışık ya da nötr bırak): İmparatorluk = Bizans, Vlandia = Norman/Frank,
  Sturgia = Rus/Viking, Battania = Kelt/Gal, Khuzait = bozkır (Moğol/Türk), Aserai = Arap/Endülüs.
- **Görünüş:** boyalı konsept sanatı; fotoğraf gerçekçiliği, anime, çizgi film ya da 3B oyuncak görünümü değil. Renkler toprak tonu
  ve soluk; ışık mum ya da meşale, gölgeler derin.
- **Stil referansı (en etkili yol):** oyun açılırken çıkan yükleme ekranlarından birinin ekran görüntüsünü al ve üreticinin "stil
  referansı / image prompt" özelliğine ver (Midjourney'de `--sref`, diğerlerinde "referans görsel"). Prompt sahneyi anlatır, referans
  stili oyuna kilitler. Bütün kartlarda **aynı** referansı kullan ki seri tutarlı olsun.
- **Kontrol:** bir yükleme ekranının yanına koyduğunda yabancı duruyor mu? Modern nesne, yazı ya da fantastik bir şey var mı?
  İlk birkaç resmi bana gönder; oyunla karşılaştırıp gerekirse prompt'ları ayarlarım.

## Kurallar

- Aşağıdaki prompt'lar **tam hâliyle** yazıldı (ortak stil sonlarına eklendi): olduğu gibi kopyala.
- Yüzler genel ve isimsiz olsun (belirli bir oyun karakterine benzemesin): kart her lord için kullanılacak.
- Çok kişi varsa en fazla 3–4 figür; sahne okunur kalsın.
- Kan, yara, çıplaklık yok. Ölüm ima edilir (devrik kadeh, söndürülmüş mum), gösterilmez.
- Gizli ilişki ve kur resimlerinde (27–29, 34) figürler tamamen giyinik; yalnızca gizlilik, kur ve skandal anlatılır, yakınlık gösterilmez.

Ortak stil (yeni bir prompt yazarsan sonuna ekle):

```
, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

## Kontrol listesi

Ürettikçe işaretle. **Öncelik** sütunu: ★ en sık görülen kartlar, önce bunlar.

| # | Dosya | Nerede görünür | Öncelik |
|---|---|---|---|
| 1 | `tur_casusluk.png` | Casusluk başlarken ve casusluk raporu | ★ |
| 2 | `sonuc_sir_bulundu.png` | Casuslukta bir sır bulundu | |
| 3 | `tur_santaj.png` | Şantaj aşamaları ve şantaj tuttu | ★ |
| 4 | `tur_suikast.png` | Suikast aşamaları | ★ |
| 5 | `sonuc_suikast_zehir.png` | Suikast zehirle başarılı | |
| 6 | `sonuc_suikast_hancer.png` | Suikast hançerle başarılı | |
| 7 | `sonuc_suikast_kaza.png` | Suikast kaza süsüyle başarılı | |
| 8 | `tur_kacirma.png` | Kaçırma aşamaları ve sonucu | ★ |
| 9 | `sonuc_yakalandi.png` | Ajan yakalandı, girişim başarısız | ★ |
| 10 | `sonuc_ifsa.png` | Sır ifşası, şantaj tutmadı | ★ |
| 11 | `sonuc_hukumdar_hesap.png` | Hükümdar hesap soruyor | |
| 12 | `sonuc_entrika_sezildi.png` | Sana karşı entrika sezildi, korundun | ★ |
| 13 | `yer_golge_agi.png` | Şehir menüsü > Gölgelerde iş çevir | |
| 14 | `sonuc_koz.png` | Koz kullanıldı | |
| 15 | `olay_rusvetci_hizmetkar.png` | Olay: rüşvetçi hizmetkâr | ★ |
| 16 | `olay_ajan_yakalandi.png` | Olay: ajan yakalandı | ★ |
| 17 | `olay_zehir_hancer.png` | Olay: zehir mi, hançer mi? | ★ |
| 18 | `olay_hedef_yola_cikti.png` | Olay: hedef yer değiştirdi | ★ |
| 19 | `olay_rakip_suikastci.png` | Olay: rakip suikastçı | ★ |
| 20 | `olay_muhafiz_degisimi.png` | Olay: muhafız değişimi | ★ |
| 21 | `olay_vicdan_azabi.png` | Olay: vicdan azabı | ★ |
| 22 | `olay_supheli_tuccar.png` | Olay: şüpheli tüccar | ★ |
| 23 | `olay_ziyafet.png` | Olay: ziyafet daveti | ★ |
| 24 | `olay_cete_teklifi.png` | Olay: çete liderinin teklifi | ★ |
| 25 | `sonuc_sorgu.png` | Esir sorgusu (P2.1) | ★ |
| 26 | `sonuc_haberci.png` | Haberci sonucu, ulak döndü, görüşme (P2.2) | ★ |
| 27 | `sonuc_gizli_bulusma.png` | Gizli ilişki ya da gayrimeşru çocuk sırrı bulundu (P2.4) | |
| 28 | `sonuc_skandal.png` | Gizli ilişki ya da gayrimeşru çocuk ifşa edildi (P2.4) | |
| 29 | `sonuc_fisilti.png` | Aldatılan eşe söylendi (P2.4) | |
| 30 | `sonuc_diplomatik_kriz.png` | Yabancı krallıkta ağın çökertildi, elçi hesap soruyor (P2.5) | |
| 31 | `sonuc_savas_meclisi.png` | Casus skandalları yüzünden savaş meclisi toplandı (P2.5) | |
| 32 | `yer_gece_evi.png` | Gece evi: müşterilerden duyulanlar, ayrılanlar, baskın, dedikodu | |
| 33 | `sonuc_hizmet_yemini.png` | Esir hizmetine katıldı (bağlılık yemini) | |
| 34 | `sonuc_kur.png` | Kur: başlangıç, ret, eşin reddi, eşinin bilgi görevi (P2.6) | |
| 35 | `sonuc_dogum.png` | Kurdan ya da sevgiliden doğan çocuk (P2.6) | |
| 36 | `sonuc_aldatma.png` | Eşinin gizli ilişkisi ortaya çıktı, yüzleşme | |
| 37 | `sonuc_bosanma.png` | Boşanma | |
| 38 | `sonuc_duello.png` | Düello: meydan okuma, ret, sonuç | ★ |
| 39 | `sonuc_mesrulastirma.png` | Gayrimeşru çocuk meşrulaştırıldı (P2.8) | |

## Prompt'lar (kopyalamaya hazır)

### Entrika türleri

**1 · `tur_casusluk.png`** — casusluk

```
A hooded spymaster reading a sealed letter by a single candle in a cramped stone room, maps and wax seals on the table, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**3 · `tur_santaj.png`** — şantaj

```
A nervous medieval lord in rich robes reading a letter in his hall while a calm cloaked figure watches him from the doorway, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**4 · `tur_suikast.png`** — suikast planı

```
A dark castle corridor at night, a shadowy figure waiting behind a pillar, a dagger's edge catching the torchlight, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**8 · `tur_kacirma.png`** — kaçırma

```
Masked riders galloping through a moonlit forest, one carrying a bound captive across his saddle, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

### Entrika sonuçları

**2 · `sonuc_sir_bulundu.png`** — bir sır bulundu

```
Close-up of gloved hands opening a hidden compartment in a noble's writing desk, revealing a bundle of secret letters tied with ribbon, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**5 · `sonuc_suikast_zehir.png`** — zehirle başarılı

```
An empty banquet table after a feast, a single overturned silver goblet, spilled wine, a guttering candle, no people, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**6 · `sonuc_suikast_hancer.png`** — hançerle başarılı

```
A cloaked figure slipping away down a rain-soaked night alley in a walled town, a bell tower in the background, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**7 · `sonuc_suikast_kaza.png`** — kaza süsü

```
A broken noble carriage on a misty mountain road at dawn, a riderless horse standing nearby, crows in the sky, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**9 · `sonuc_yakalandi.png`** — ajan yakalandı

```
Torch-bearing guards seizing a struggling hooded agent in a castle courtyard at night, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**10 · `sonuc_ifsa.png`** — ifşa

```
Nobles whispering behind their hands in a grand hall, one lord pointing accusingly, faces lit by braziers, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**14 · `sonuc_koz.png`** — koz kullanıldı

```
A proud lord reluctantly handing a heavy coin purse to a cloaked visitor in his private study, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

### Saray ve karşı casusluk

**11 · `sonuc_hukumdar_hesap.png`** — hükümdar hesap soruyor

```
An angry ruler on a throne in a great hall, a kneeling noble before him, armed guards at the sides, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**12 · `sonuc_entrika_sezildi.png`** — sana karşı entrika sezildi

```
A loyal advisor leaning in to whisper urgent news to a lord at night, an open window and a dark city behind them, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**13 · `yer_golge_agi.png`** — gölgelerde iş çevir (şehir menüsü)

```
A tavern back room, a scarred gang leader counting coins across the table from a hooded visitor, smoke and lantern light, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

### P2: esir sorgusu, haberci, gizli ilişki, diplomatik kriz

**25 · `sonuc_sorgu.png`** — esir sorgusu

```
A captive noble seated at a rough wooden table in a dim stone cell, a calm hooded questioner across from him, a single candle and an open ledger, no violence, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**26 · `sonuc_haberci.png`** — haberci sonucu, ulak döndü, görüşme

```
A lord breaking the wax seal of a letter by a window in his solar, a travel-worn courier waiting at the door, dusk light, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**27 · `sonuc_gizli_bulusma.png`** — gizli ilişki ya da gayrimeşru çocuk sırrı bulundu

```
Two fully clothed cloaked figures, a man and a woman, meeting secretly at night beneath a stone arch in a walled castle garden, seen from a distance through ivy, a single shuttered lantern between them, discreet and mysterious mood, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**28 · `sonuc_skandal.png`** — gizli ilişki ifşa edildi, saray skandalı

```
A noble feast hall falling silent, richly dressed courtiers turning to stare and whisper at a shamed noble standing alone, a dropped love letter with a broken wax seal on the stone floor, a wronged spouse walking out of the hall, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**29 · `sonuc_fisilti.png`** — aldatılan eşe söylendi

```
A grim noblewoman seated in a candlelit private chapel listening as a hooded confidant leans in to whisper, her hand clenched on a folded letter, stained glass dark behind them, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**30 · `sonuc_diplomatik_kriz.png`** — elçi hesap soruyor

```
A stern foreign envoy in rich embroidered robes standing before a seated lord in a stone hall, slapping a sealed parchment with a dangling wax seal onto the table, two armed retainers watching tensely, banners on the walls, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**31 · `sonuc_savas_meclisi.png`** — savaş meclisi

```
A ruler standing at the head of a long table in a torch-lit great hall, leaning over a large painted map of Calradia with carved wooden markers, armored nobles around the table arguing and raising their fists, a war banner behind the throne, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**32 · `yer_gece_evi.png`** — gece evi (dış cephe; içerisi gösterilmez)

```
A two-story timber-framed house on a narrow medieval town street at night, warm lantern light behind shuttered upper windows, a red cloth lantern hanging above the door, a cloaked visitor slipping inside while a stern older woman in a plain dress and shawl counts coins at a small table by the entrance, everyone fully clothed, discreet and secretive mood, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**33 · `sonuc_hizmet_yemini.png`** — esir hizmetine katıldı

```
A formerly captive young noble in travel-worn clothes kneeling in a torch-lit hall and placing their hands between the hands of a seated lord in a gesture of fealty, unlocked iron shackles lying on the stone floor beside them, a few retainers watching, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**34 · `sonuc_kur.png`** — kur (hediye, şiir, mektup)

```
A fully clothed young noblewoman in an embroidered dress reading a folded letter beside a stone window in a castle garden, a small carved wooden gift box and a single red rose resting on the sill, a richly dressed nobleman waiting respectfully at a distance under an arch, warm late afternoon light, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**35 · `sonuc_dogum.png`** — doğum

```
A tired but smiling noble mother resting in a canopied bed in a warm hearth-lit chamber, a midwife placing a swaddled newborn in her arms, a wooden cradle and a basin nearby, tapestries on the stone walls, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**36 · `sonuc_aldatma.png`** — eşin seni aldatıyor

```
A grim lord standing alone in a dark castle chamber at night, holding a crumpled intercepted love letter with a broken wax seal, a hooded spymaster waiting silently by the door with a candle, an empty chair by the cold hearth, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**37 · `sonuc_bosanma.png`** — boşanma

```
A noble spouse in a travel cloak leaving a castle courtyard at dawn with a small retinue and a loaded packhorse, looking back once at the keep, a lord watching from a high window, a family banner being taken down, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**38 · `sonuc_duello.png`** — düello

```
Two noblemen in mail and padded gambesons facing each other with swords and round shields on the sand of a town arena, a crowd of townsfolk and nobles watching from wooden stands, banners hanging from the railings, dust in the late afternoon light, no blood, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**39 · `sonuc_mesrulastirma.png`** — meşrulaştırma

```
A lord standing in a crowded great hall with his hand on the shoulder of a young boy in new noble clothes, a scribe reading aloud from a sealed charter, courtiers whispering, the clan banner behind them, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

### Olaylar (entrika aşamaları arasında)

**15 · `olay_rusvetci_hizmetkar.png`** — rüşvetçi hizmetkâr

```
A castle servant in a kitchen doorway at night holding out an open palm for a coin, glancing over his shoulder, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**16 · `olay_ajan_yakalandi.png`** — ajan yakalandı

```
A bound man sitting in a damp dungeon cell, a jailer's lantern at the bars, chains on the wall, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**17 · `olay_zehir_hancer.png`** — zehir mi, hançer mi?

```
A table with a small glass vial of poison and an unsheathed dagger side by side, a hand hovering between them, candlelight, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**18 · `olay_hedef_yola_cikti.png`** — hedef yer değiştirdi

```
A lord's retinue riding out of a town gate at dawn, banners raised, a watcher in the shadows of the wall, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**19 · `olay_rakip_suikastci.png`** — rakip suikastçı

```
Two hooded figures meeting warily on a moonlit rooftop of a medieval town, each with a hand on a hidden blade, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**20 · `olay_muhafiz_degisimi.png`** — muhafız değişimi

```
Night guards changing shifts at a castle gate, one yawning, torches flickering, an unguarded side door ajar, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**21 · `olay_vicdan_azabi.png`** — vicdan azabı

```
A troubled companion sitting alone by a campfire, staring at a dagger in his hands, the camp asleep behind him, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**22 · `olay_supheli_tuccar.png`** — şüpheli tüccar

```
A busy town market, a merchant pretending to inspect cloth while secretly watching two cloaked men in the crowd, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**23 · `olay_ziyafet.png`** — ziyafet daveti

```
A lively medieval feast in a long hall, musicians, roasting meat, guards distracted, a servant carrying a covered dish, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**24 · `olay_cete_teklifi.png`** — çete liderinin teklifi

```
A gang leader leaning against an alley wall offering a handshake, his men lurking in the shadows behind him, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

## Sürümler: aynı karta farklı resimler

Her kartın birden çok resmi olabilir: asıl resim (ör. `sonuc_kur.png`), sonra `sonuc_kur_2.png`, `sonuc_kur_3.png`, en fazla `sonuc_kur_5.png`. Oyun kartı her açışta
klasördekilerden **rastgele birini** gösterir; böylece aynı olay her seferinde başka bir resimle gelir. Hepsini yapmak zorunda değilsin:
yalnızca asıl resim de yeter.

Aşağıda her kart için iki sürüm daha var (`_2` ve `_3`). Asılları gibi tam hâliyle yazıldılar; kopyalayıp kullan. Sürümler farklı
kültürlerden ve farklı sahnelerden seçildi (Aserai, Khuzait, Sturgia, Battania, İmparatorluk, Vlandia); stil referansı aynı kaldığı
sürece seri tutarlı görünür. `_4` ve `_5` için kendi fikrini yaz, sonuna ortak stili ekle.

**`tur_casusluk_2.png`** · `tur_casusluk_3.png` — casusluk

```
A Khuzait steppe scout in a felt hat lying in tall grass on a hillside at dusk, watching a distant walled town, a folded message tucked in his belt, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

```
An Aserai informant in a dim desert-town bazaar alcove passing a tiny rolled note to a hooded buyer under a hanging oil lamp, spice sacks around them, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**`sonuc_sir_bulundu_2.png`** · `sonuc_sir_bulundu_3.png` — bir sır bulundu

```
A Battanian scribe in a candlelit stone hut deciphering a coded letter, wax tablets and a broken seal on the table, a raven on the windowsill, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

```
A Sturgian clerk in a wooden longhouse counting-room uncovering a second ledger hidden beneath loose floorboards by lantern light, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**`tur_santaj_2.png`** · `tur_santaj_3.png` — şantaj

```
An Imperial senator in rich robes in a marble atrium staring at a small locket placed on the table by a smiling masked visitor, oil lamps flickering, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

```
A Vlandian baron at a castle window at night reading a short anonymous letter with a trembling hand, a single candle and an untouched goblet of wine, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**`tur_suikast_2.png`** · `tur_suikast_3.png` — suikast planı

```
A veiled figure crouched on a flat rooftop above a lantern-lit courtyard in an Aserai desert city at night, a curved dagger at the belt, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

```
A cloaked figure climbing a rope over the wooden palisade of a snowy Sturgian fortress at night while a guard dozes by a brazier, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**`sonuc_suikast_zehir_2.png`** · `sonuc_suikast_zehir_3.png` — zehirle başarılı

```
A noble's bedside table at dawn with a small empty clay vial beside a half-finished cup of wine, morning light through shutters, the bed curtains drawn, no body visible, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

```
An Imperial kitchen at night, a cook's assistant secretly tipping a few drops from a tiny vial into a silver soup tureen while other cooks are busy in the background, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**`sonuc_suikast_hancer_2.png`** · `sonuc_suikast_hancer_3.png` — hançerle başarılı

```
A crowded Aserai market at noon, a figure in a dusty cloak walking calmly away while people turn toward a commotion behind him, colorful awnings, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

```
A misty Battanian forest path at dawn, a riderless noble horse with an empty saddle, a dark figure disappearing between the trees, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**`sonuc_suikast_kaza_2.png`** · `sonuc_suikast_kaza_3.png` — kaza süsü

```
A collapsed wooden bridge over a rushing river in a Vlandian valley, broken planks and an overturned cart in the water, onlookers on the bank, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

```
A hunting party in an autumn forest gathered at the edge of a ravine, hunting dogs barking, a broken spear on the ground, no body visible, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**`tur_kacirma_2.png`** · `tur_kacirma_3.png` — kaçırma

```
Khuzait horsemen galloping across a moonlit steppe with a bound captive on a spare horse, dust rising behind them, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

```
A small rowing boat slipping away from a riverside castle at night, two hooded men rowing and a bound noble seated between them, lanterns on the walls behind, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**`sonuc_yakalandi_2.png`** · `sonuc_yakalandi_3.png` — ajan yakalandı

```
An Aserai city watch with spears dragging a hooded agent out of a narrow alley into torchlight, onlookers on balconies, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

```
A captured spy kneeling in the snow before Sturgian guards in fur cloaks, his hidden letters scattered on the ground, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**`sonuc_ifsa_2.png`** · `sonuc_ifsa_3.png` — ifşa

```
A town crier on the steps of a Vlandian market cross reading a proclamation to a crowd, nobles in the crowd exchanging shocked glances, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

```
An Imperial council chamber, a speaker holding up a damning letter as senators rise from their marble benches in uproar, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**`sonuc_hukumdar_hesap_2.png`** · `sonuc_hukumdar_hesap_3.png` — hükümdar hesap soruyor

```
A Khuzait khan seated on cushions in a large felt tent glaring at a noble standing before him, guards with bows at the entrance, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

```
An Aserai ruler in a sunlit arched hall pointing at a kneeling lord, courtiers watching from behind carved wooden screens, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**`sonuc_entrika_sezildi_2.png`** · `sonuc_entrika_sezildi_3.png` — sana karşı entrika sezildi

```
A spymaster laying a captured dagger and a coded note on a map table in front of a startled lord, candles burning low, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

```
A servant girl whispering urgently to a noblewoman in a castle corridor at night, pointing back toward a shadowy doorway, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**`yer_golge_agi_2.png`** · `yer_golge_agi_3.png` — gölgelerde iş çevir

```
A smoky Aserai backstreet tea house, a gang leader reclining on cushions while a hooded visitor slides a purse across a low table, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

```
A dockside warehouse at night in a Vlandian port town, smugglers stacking crates while their boss shakes hands with a cloaked stranger, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**`sonuc_koz_2.png`** · `sonuc_koz_3.png` — koz kullanıldı

```
A defeated-looking noble signing a parchment at a desk while a calm cloaked figure holds the candle for him, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

```
A Battanian chieftain reluctantly releasing chained prisoners in a muddy courtyard on the orders of a hooded envoy, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**`olay_rusvetci_hizmetkar_2.png`** · `olay_rusvetci_hizmetkar_3.png` — rüşvetçi hizmetkâr

```
A stable boy in a castle stable at night accepting a coin from a hooded stranger, horses watching from their stalls, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

```
A maidservant in a corridor of an Imperial villa slipping a silver coin into her apron while glancing at a closed door, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**`olay_ajan_yakalandi_2.png`** · `olay_ajan_yakalandi_3.png` — ajan yakalandı

```
A captured agent tied to a chair in a candlelit cellar, two guards questioning him, a ring of keys hanging on the wall, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

```
A prisoner in a wooden cage in a snowy Sturgian courtyard, a guard warming his hands at a fire nearby, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**`olay_zehir_hancer_2.png`** · `olay_zehir_hancer_3.png` — zehir mi, hançer mi?

```
A hooded figure's hands resting on a velvet cloth between a curved dagger and a small blue glass vial, an oil lamp casting long shadows, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

```
A cluttered apothecary workshop with shelves of jars, a cloaked customer pointing at a small dark bottle while the apothecary hesitates, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**`olay_hedef_yola_cikti_2.png`** · `olay_hedef_yola_cikti_3.png` — hedef yer değiştirdi

```
A noble caravan with camels leaving an Aserai city gate at sunrise, a lone watcher on a dune in the foreground, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

```
A river barge flying a lord's banner leaving a Vlandian town at dawn, a figure on the riverbank following on foot, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**`olay_rakip_suikastci_2.png`** · `olay_rakip_suikastci_3.png` — rakip suikastçı

```
Two masked figures facing each other in a narrow candlelit corridor, each hiding a dagger behind their back, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

```
Two hooded rivals glaring at each other across a tavern table, a coin purse between them, other patrons oblivious, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**`olay_muhafiz_degisimi_2.png`** · `olay_muhafiz_degisimi_3.png` — muhafız değişimi

```
Imperial guards in scale armor marching along a fortress wall at sunset, an empty watchtower in the foreground, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

```
A drowsy Sturgian guard asleep against a wooden gate at night, snow falling, the small door behind him slightly open, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**`olay_vicdan_azabi_2.png`** · `olay_vicdan_azabi_3.png` — vicdan azabı

```
A companion kneeling alone in a small roadside chapel at night, a dagger laid on the altar step before him, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

```
A troubled young warrior staring into a basin of water in a castle chamber, his reflection blurred, a dagger on the table, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**`olay_supheli_tuccar_2.png`** · `olay_supheli_tuccar_3.png` — şüpheli tüccar

```
A merchant in an Aserai souk inspecting a carpet while his eyes follow a veiled figure, colorful stalls around them, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

```
A fur trader at a Sturgian winter market watching two cloaked men whispering by a frozen well, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**`olay_ziyafet_2.png`** · `olay_ziyafet_3.png` — ziyafet daveti

```
A Khuzait feast inside a great felt tent, roasted mutton on platters, musicians with horsehead fiddles, guards drinking from cups, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

```
An Imperial banquet in a columned hall with reclining guests, servants carrying amphorae, a cupbearer glancing nervously aside, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**`olay_cete_teklifi_2.png`** · `olay_cete_teklifi_3.png` — çete liderinin teklifi

```
A scarred gang leader sitting on barrels in a dim warehouse with his thugs behind him, offering a deal with an open hand, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

```
A smuggler chief in a small boat on a misty riverbank at night offering a sealed pouch to a hooded agent on the shore, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**`sonuc_sorgu_2.png`** · `sonuc_sorgu_3.png` — esir sorgusu

```
A captive Khuzait noble seated on a rug inside a tent with hands bound in front, a calm questioner sitting opposite with a cup of tea, no violence, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

```
A captured knight in a castle tower room by a window, a scribe taking notes as an officer asks questions, no violence, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**`sonuc_haberci_2.png`** · `sonuc_haberci_3.png` — haberci, ulak, görüşme

```
A courier on a tired horse arriving at a castle gate at dusk, holding up a sealed letter to the guards, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

```
A noblewoman receiving a letter from a kneeling messenger in a garden courtyard, a fountain behind them, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**`sonuc_gizli_bulusma_2.png`** · `sonuc_gizli_bulusma_3.png` — gizli ilişki sırrı

```
Two fully clothed cloaked figures meeting by a candlelit side door of a chapel at night, seen from across a dark courtyard, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

```
A fully clothed couple in cloaks talking quietly under a willow by a moonlit river, two horses tied nearby, seen from a distance, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**`sonuc_skandal_2.png`** · `sonuc_skandal_3.png` — skandal

```
A market square crowd gossiping and pointing at a noble's carriage passing by, townswomen whispering behind their hands, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

```
A noblewoman leaving a chapel while courtiers turn their backs on her, a torn letter lying on the steps, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**`sonuc_fisilti_2.png`** · `sonuc_fisilti_3.png` — eşine söylendi

```
A grim lord standing by a fireplace while a hooded confidant whispers to him, his fist clenched on the mantelpiece, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

```
A noble spouse sitting in a dim garden at night as a maid whispers to them, a single lantern between them, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**`sonuc_diplomatik_kriz_2.png`** · `sonuc_diplomatik_kriz_3.png` — elçi hesap soruyor

```
Two delegations facing each other across a long table in a tent under a truce banner, one envoy throwing down a sealed letter, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

```
A foreign herald on horseback delivering an ultimatum before a castle gate, nobles watching from the walls, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**`sonuc_savas_meclisi_2.png`** · `sonuc_savas_meclisi_3.png` — savaş meclisi

```
Khuzait chieftains gathered around a fire in a great tent, one raising a bow above his head as the others shout, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

```
Imperial generals around a table in a marble hall, a ruler pointing at a painted map, scrolls and helmets on the table, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**`yer_gece_evi_2.png`** · `yer_gece_evi_3.png` — gece evi (dış cephe)

```
A lantern-lit doorway of a discreet townhouse in an Aserai city at night, colorful glass lamps, a veiled doorwoman greeting a hooded noble, everyone fully clothed, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

```
A timber house on a muddy Vlandian street at night with warm windows, a burly doorman taking a coin from a cloaked customer, everyone fully clothed, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**`sonuc_hizmet_yemini_2.png`** · `sonuc_hizmet_yemini_3.png` — esir hizmetine katıldı

```
A former captive Khuzait noble bowing inside a tent before a seated lord, offering their sword with both hands, guards watching, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

```
A young noblewoman in plain travel clothes swearing an oath on a relic held by a priest before a seated lord in a chapel, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**`sonuc_kur_2.png`** · `sonuc_kur_3.png` — kur

```
A nobleman reading a poem from a scroll to a smiling noblewoman in a sunny castle garden, both fully clothed, a lute resting on a bench, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

```
A noblewoman giving a small embroidered handkerchief to a knight before a tournament, crowds and banners in the background, both fully clothed, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**`sonuc_dogum_2.png`** · `sonuc_dogum_3.png` — doğum

```
A proud father holding a swaddled newborn up to the morning light by a castle window, a nursemaid smiling behind him, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

```
A wooden cradle by a warm hearth in a Sturgian longhouse, a mother rocking it while relatives gather to look, furs on the benches, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**`sonuc_aldatma_2.png`** · `sonuc_aldatma_3.png` — eşin seni aldatıyor

```
A noblewoman in a dark corridor watching through a half-open door as her husband meets a cloaked lady in the garden beyond, everyone fully clothed, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

```
A spymaster laying out evidence on a table before a stunned lord: a lover's ring, a ribbon and intercepted letters, candlelight, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**`sonuc_bosanma_2.png`** · `sonuc_bosanma_3.png` — boşanma

```
A noble couple standing apart in a chapel before a priest, a broken ring on a velvet cushion between them, witnesses watching, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

```
A noblewoman riding away from a castle at dusk with her servants and a packhorse, the drawbridge rising behind her, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**`sonuc_duello_2.png`** · `sonuc_duello_3.png` — düello

```
A nobleman throwing down a leather glove at the feet of a rival lord in a crowded castle hall, courtiers stepping back in shock, torches on the walls, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

```
A victorious lord lowering his sword over a fallen rival kneeling on the sand of a Vlandian arena, the rival raising an open hand in yield, spectators cheering from the stands, no blood, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**`sonuc_mesrulastirma_2.png`** · `sonuc_mesrulastirma_3.png` — meşrulaştırma

```
A priest writing a child's name into a large family register in a candlelit chapel while a noble father and a young girl watch, witnesses standing behind, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

```
A young noble kneeling to receive a family signet ring from a seated lord in a castle solar, an older noblewoman looking away in shame by the window, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```
