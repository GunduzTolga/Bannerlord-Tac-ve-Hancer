# Görsel listesi: bütün resimler, dosya adları ve prompt'lar

Modda üretilmesi gereken resimlerin **hepsi** bu dosyada: **29 olay kartı resmi**. Başka resim gerekmiyor. Entrika ekranı, harita
düğmesi ve sekmeler oyunun kendi çizimlerini kullanıyor. Yeni bir özellik resim isterse buraya eklenir; birim testi koddaki her resim
adının burada olduğunu denetler.

## Teknik

- **Boyut:** 1600×900 (16:9), PNG. Daha büyük üretilirse sorun yok; oyuna koyarken küçültülür.
- **Dosya adı:** tablodaki ad (küçük harf, Türkçe karakter yok), örn. `olay_rusvetci_hizmetkar.png`.
- **Nereye:** oyunda doğrudan denemek için modun klasöründe `Modules\CheatMenu\Images\` içine koy; kart bir sonraki açılışta resmi gösterir
  (sprite paketine çevirmek gerekmez). Depoya eklenmesi için bana gönder ya da `Module/Images/` klasörüne koy.
- **Resim yoksa:** kartta ilgili kişinin portresi görünür; hiçbir şey bozulmaz. Bu yüzden resimler tek tek, istenen sırayla eklenebilir.

## Kurallar

- Aşağıdaki prompt'lar **tam hâliyle** yazıldı (ortak stil sonlarına eklendi): olduğu gibi kopyala.
- Yüzler genel ve isimsiz olsun (belirli bir oyun karakterine benzemesin): kart her lord için kullanılacak.
- Çok kişi varsa en fazla 3–4 figür; sahne okunur kalsın.
- Kan, yara, çıplaklık yok. Ölüm ima edilir (devrik kadeh, söndürülmüş mum), gösterilmez.
- Gizli ilişki resimlerinde (27–29) figürler tamamen giyinik; yalnızca gizlilik ve skandal anlatılır, yakınlık gösterilmez.

Ortak stil (yeni bir prompt yazarsan sonuna ekle):

```
, digital oil painting in the style of Mount & Blade II: Bannerlord loading screens, medieval fictional land of Calradia, muted earthy palette, dramatic chiaroscuro candle or torch light, painterly brush strokes, cinematic composition, 16:9, no text, no letters, no logos, no watermark, no gore, no nudity
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

## Prompt'lar (kopyalamaya hazır)

### Entrika türleri

**1 · `tur_casusluk.png`** — casusluk

```
A hooded spymaster reading a sealed letter by a single candle in a cramped stone room, maps and wax seals on the table, digital oil painting in the style of Mount & Blade II: Bannerlord loading screens, medieval fictional land of Calradia, muted earthy palette, dramatic chiaroscuro candle or torch light, painterly brush strokes, cinematic composition, 16:9, no text, no letters, no logos, no watermark, no gore, no nudity
```

**3 · `tur_santaj.png`** — şantaj

```
A nervous medieval lord in rich robes reading a letter in his hall while a calm cloaked figure watches him from the doorway, digital oil painting in the style of Mount & Blade II: Bannerlord loading screens, medieval fictional land of Calradia, muted earthy palette, dramatic chiaroscuro candle or torch light, painterly brush strokes, cinematic composition, 16:9, no text, no letters, no logos, no watermark, no gore, no nudity
```

**4 · `tur_suikast.png`** — suikast planı

```
A dark castle corridor at night, a shadowy figure waiting behind a pillar, a dagger's edge catching the torchlight, digital oil painting in the style of Mount & Blade II: Bannerlord loading screens, medieval fictional land of Calradia, muted earthy palette, dramatic chiaroscuro candle or torch light, painterly brush strokes, cinematic composition, 16:9, no text, no letters, no logos, no watermark, no gore, no nudity
```

**8 · `tur_kacirma.png`** — kaçırma

```
Masked riders galloping through a moonlit forest, one carrying a bound captive across his saddle, digital oil painting in the style of Mount & Blade II: Bannerlord loading screens, medieval fictional land of Calradia, muted earthy palette, dramatic chiaroscuro candle or torch light, painterly brush strokes, cinematic composition, 16:9, no text, no letters, no logos, no watermark, no gore, no nudity
```

### Entrika sonuçları

**2 · `sonuc_sir_bulundu.png`** — bir sır bulundu

```
Close-up of gloved hands opening a hidden compartment in a noble's writing desk, revealing a bundle of secret letters tied with ribbon, digital oil painting in the style of Mount & Blade II: Bannerlord loading screens, medieval fictional land of Calradia, muted earthy palette, dramatic chiaroscuro candle or torch light, painterly brush strokes, cinematic composition, 16:9, no text, no letters, no logos, no watermark, no gore, no nudity
```

**5 · `sonuc_suikast_zehir.png`** — zehirle başarılı

```
An empty banquet table after a feast, a single overturned silver goblet, spilled wine, a guttering candle, no people, digital oil painting in the style of Mount & Blade II: Bannerlord loading screens, medieval fictional land of Calradia, muted earthy palette, dramatic chiaroscuro candle or torch light, painterly brush strokes, cinematic composition, 16:9, no text, no letters, no logos, no watermark, no gore, no nudity
```

**6 · `sonuc_suikast_hancer.png`** — hançerle başarılı

```
A cloaked figure slipping away down a rain-soaked night alley in a walled town, a bell tower in the background, digital oil painting in the style of Mount & Blade II: Bannerlord loading screens, medieval fictional land of Calradia, muted earthy palette, dramatic chiaroscuro candle or torch light, painterly brush strokes, cinematic composition, 16:9, no text, no letters, no logos, no watermark, no gore, no nudity
```

**7 · `sonuc_suikast_kaza.png`** — kaza süsü

```
A broken noble carriage on a misty mountain road at dawn, a riderless horse standing nearby, crows in the sky, digital oil painting in the style of Mount & Blade II: Bannerlord loading screens, medieval fictional land of Calradia, muted earthy palette, dramatic chiaroscuro candle or torch light, painterly brush strokes, cinematic composition, 16:9, no text, no letters, no logos, no watermark, no gore, no nudity
```

**9 · `sonuc_yakalandi.png`** — ajan yakalandı

```
Torch-bearing guards seizing a struggling hooded agent in a castle courtyard at night, digital oil painting in the style of Mount & Blade II: Bannerlord loading screens, medieval fictional land of Calradia, muted earthy palette, dramatic chiaroscuro candle or torch light, painterly brush strokes, cinematic composition, 16:9, no text, no letters, no logos, no watermark, no gore, no nudity
```

**10 · `sonuc_ifsa.png`** — ifşa

```
Nobles whispering behind their hands in a grand hall, one lord pointing accusingly, faces lit by braziers, digital oil painting in the style of Mount & Blade II: Bannerlord loading screens, medieval fictional land of Calradia, muted earthy palette, dramatic chiaroscuro candle or torch light, painterly brush strokes, cinematic composition, 16:9, no text, no letters, no logos, no watermark, no gore, no nudity
```

**14 · `sonuc_koz.png`** — koz kullanıldı

```
A proud lord reluctantly handing a heavy coin purse to a cloaked visitor in his private study, digital oil painting in the style of Mount & Blade II: Bannerlord loading screens, medieval fictional land of Calradia, muted earthy palette, dramatic chiaroscuro candle or torch light, painterly brush strokes, cinematic composition, 16:9, no text, no letters, no logos, no watermark, no gore, no nudity
```

### Saray ve karşı casusluk

**11 · `sonuc_hukumdar_hesap.png`** — hükümdar hesap soruyor

```
An angry ruler on a throne in a great hall, a kneeling noble before him, armed guards at the sides, digital oil painting in the style of Mount & Blade II: Bannerlord loading screens, medieval fictional land of Calradia, muted earthy palette, dramatic chiaroscuro candle or torch light, painterly brush strokes, cinematic composition, 16:9, no text, no letters, no logos, no watermark, no gore, no nudity
```

**12 · `sonuc_entrika_sezildi.png`** — sana karşı entrika sezildi

```
A loyal advisor leaning in to whisper urgent news to a lord at night, an open window and a dark city behind them, digital oil painting in the style of Mount & Blade II: Bannerlord loading screens, medieval fictional land of Calradia, muted earthy palette, dramatic chiaroscuro candle or torch light, painterly brush strokes, cinematic composition, 16:9, no text, no letters, no logos, no watermark, no gore, no nudity
```

**13 · `yer_golge_agi.png`** — gölgelerde iş çevir (şehir menüsü)

```
A tavern back room, a scarred gang leader counting coins across the table from a hooded visitor, smoke and lantern light, digital oil painting in the style of Mount & Blade II: Bannerlord loading screens, medieval fictional land of Calradia, muted earthy palette, dramatic chiaroscuro candle or torch light, painterly brush strokes, cinematic composition, 16:9, no text, no letters, no logos, no watermark, no gore, no nudity
```

### P2: esir sorgusu, haberci, gizli ilişki

**25 · `sonuc_sorgu.png`** — esir sorgusu

```
A captive noble seated at a rough wooden table in a dim stone cell, a calm hooded questioner across from him, a single candle and an open ledger, no violence, digital oil painting in the style of Mount & Blade II: Bannerlord loading screens, medieval fictional land of Calradia, muted earthy palette, dramatic chiaroscuro candle or torch light, painterly brush strokes, cinematic composition, 16:9, no text, no letters, no logos, no watermark, no gore, no nudity
```

**26 · `sonuc_haberci.png`** — haberci sonucu, ulak döndü, görüşme

```
A lord breaking the wax seal of a letter by a window in his solar, a travel-worn courier waiting at the door, dusk light, digital oil painting in the style of Mount & Blade II: Bannerlord loading screens, medieval fictional land of Calradia, muted earthy palette, dramatic chiaroscuro candle or torch light, painterly brush strokes, cinematic composition, 16:9, no text, no letters, no logos, no watermark, no gore, no nudity
```

**27 · `sonuc_gizli_bulusma.png`** — gizli ilişki ya da gayrimeşru çocuk sırrı bulundu

```
Two fully clothed cloaked figures, a man and a woman, meeting secretly at night beneath a stone arch in a walled castle garden, seen from a distance through ivy, a single shuttered lantern between them, discreet and mysterious mood, digital oil painting in the style of Mount & Blade II: Bannerlord loading screens, medieval fictional land of Calradia, muted earthy palette, dramatic chiaroscuro candle or torch light, painterly brush strokes, cinematic composition, 16:9, no text, no letters, no logos, no watermark, no gore, no nudity
```

**28 · `sonuc_skandal.png`** — gizli ilişki ifşa edildi, saray skandalı

```
A noble feast hall falling silent, richly dressed courtiers turning to stare and whisper at a shamed noble standing alone, a dropped love letter with a broken wax seal on the stone floor, a wronged spouse walking out of the hall, digital oil painting in the style of Mount & Blade II: Bannerlord loading screens, medieval fictional land of Calradia, muted earthy palette, dramatic chiaroscuro candle or torch light, painterly brush strokes, cinematic composition, 16:9, no text, no letters, no logos, no watermark, no gore, no nudity
```

**29 · `sonuc_fisilti.png`** — aldatılan eşe söylendi

```
A grim noblewoman seated in a candlelit private chapel listening as a hooded confidant leans in to whisper, her hand clenched on a folded letter, stained glass dark behind them, digital oil painting in the style of Mount & Blade II: Bannerlord loading screens, medieval fictional land of Calradia, muted earthy palette, dramatic chiaroscuro candle or torch light, painterly brush strokes, cinematic composition, 16:9, no text, no letters, no logos, no watermark, no gore, no nudity
```

### Olaylar (entrika aşamaları arasında)

**15 · `olay_rusvetci_hizmetkar.png`** — rüşvetçi hizmetkâr

```
A castle servant in a kitchen doorway at night holding out an open palm for a coin, glancing over his shoulder, digital oil painting in the style of Mount & Blade II: Bannerlord loading screens, medieval fictional land of Calradia, muted earthy palette, dramatic chiaroscuro candle or torch light, painterly brush strokes, cinematic composition, 16:9, no text, no letters, no logos, no watermark, no gore, no nudity
```

**16 · `olay_ajan_yakalandi.png`** — ajan yakalandı

```
A bound man sitting in a damp dungeon cell, a jailer's lantern at the bars, chains on the wall, digital oil painting in the style of Mount & Blade II: Bannerlord loading screens, medieval fictional land of Calradia, muted earthy palette, dramatic chiaroscuro candle or torch light, painterly brush strokes, cinematic composition, 16:9, no text, no letters, no logos, no watermark, no gore, no nudity
```

**17 · `olay_zehir_hancer.png`** — zehir mi, hançer mi?

```
A table with a small glass vial of poison and an unsheathed dagger side by side, a hand hovering between them, candlelight, digital oil painting in the style of Mount & Blade II: Bannerlord loading screens, medieval fictional land of Calradia, muted earthy palette, dramatic chiaroscuro candle or torch light, painterly brush strokes, cinematic composition, 16:9, no text, no letters, no logos, no watermark, no gore, no nudity
```

**18 · `olay_hedef_yola_cikti.png`** — hedef yer değiştirdi

```
A lord's retinue riding out of a town gate at dawn, banners raised, a watcher in the shadows of the wall, digital oil painting in the style of Mount & Blade II: Bannerlord loading screens, medieval fictional land of Calradia, muted earthy palette, dramatic chiaroscuro candle or torch light, painterly brush strokes, cinematic composition, 16:9, no text, no letters, no logos, no watermark, no gore, no nudity
```

**19 · `olay_rakip_suikastci.png`** — rakip suikastçı

```
Two hooded figures meeting warily on a moonlit rooftop of a medieval town, each with a hand on a hidden blade, digital oil painting in the style of Mount & Blade II: Bannerlord loading screens, medieval fictional land of Calradia, muted earthy palette, dramatic chiaroscuro candle or torch light, painterly brush strokes, cinematic composition, 16:9, no text, no letters, no logos, no watermark, no gore, no nudity
```

**20 · `olay_muhafiz_degisimi.png`** — muhafız değişimi

```
Night guards changing shifts at a castle gate, one yawning, torches flickering, an unguarded side door ajar, digital oil painting in the style of Mount & Blade II: Bannerlord loading screens, medieval fictional land of Calradia, muted earthy palette, dramatic chiaroscuro candle or torch light, painterly brush strokes, cinematic composition, 16:9, no text, no letters, no logos, no watermark, no gore, no nudity
```

**21 · `olay_vicdan_azabi.png`** — vicdan azabı

```
A troubled companion sitting alone by a campfire, staring at a dagger in his hands, the camp asleep behind him, digital oil painting in the style of Mount & Blade II: Bannerlord loading screens, medieval fictional land of Calradia, muted earthy palette, dramatic chiaroscuro candle or torch light, painterly brush strokes, cinematic composition, 16:9, no text, no letters, no logos, no watermark, no gore, no nudity
```

**22 · `olay_supheli_tuccar.png`** — şüpheli tüccar

```
A busy town market, a merchant pretending to inspect cloth while secretly watching two cloaked men in the crowd, digital oil painting in the style of Mount & Blade II: Bannerlord loading screens, medieval fictional land of Calradia, muted earthy palette, dramatic chiaroscuro candle or torch light, painterly brush strokes, cinematic composition, 16:9, no text, no letters, no logos, no watermark, no gore, no nudity
```

**23 · `olay_ziyafet.png`** — ziyafet daveti

```
A lively medieval feast in a long hall, musicians, roasting meat, guards distracted, a servant carrying a covered dish, digital oil painting in the style of Mount & Blade II: Bannerlord loading screens, medieval fictional land of Calradia, muted earthy palette, dramatic chiaroscuro candle or torch light, painterly brush strokes, cinematic composition, 16:9, no text, no letters, no logos, no watermark, no gore, no nudity
```

**24 · `olay_cete_teklifi.png`** — çete liderinin teklifi

```
A gang leader leaning against an alley wall offering a handshake, his men lurking in the shadows behind him, digital oil painting in the style of Mount & Blade II: Bannerlord loading screens, medieval fictional land of Calradia, muted earthy palette, dramatic chiaroscuro candle or torch light, painterly brush strokes, cinematic composition, 16:9, no text, no letters, no logos, no watermark, no gore, no nudity
```
