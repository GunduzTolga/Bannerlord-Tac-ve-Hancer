# Olay kartı görselleri — liste ve prompt'lar

Bu görseller Faz P1'deki resimli olay kartlarında kullanılacak (yol haritası bölüm 3). Her kartta solda görsel, sağda metin ve seçenekler var.

## Teknik

- **Boyut:** 1600×900 (16:9), PNG. Daha büyük üretilirse sorun yok; oyuna koyarken küçültülür.
- **Dosya adı:** tablodaki ad (küçük harf, Türkçe karakter yok), örn. `olay_rusvetci_hizmetkar.png`.
- **Nereye:** oyunda doğrudan denemek için modun klasöründe `Modules\CheatMenu\Images\` içine koy; kart bir sonraki açılışta resmi gösterir
  (sprite paketine çevirmek gerekmez). Depoya eklenmesi için bana gönder ya da `Module/Images/` klasörüne koy.
- **Resim yoksa:** kartta ilgili kişinin portresi görünür; hiçbir şey bozulmaz. Bu yüzden resimler tek tek, istenen sırayla eklenebilir.

## Ortak stil (her prompt'un sonuna ekle)

```
, digital oil painting in the style of Mount & Blade II: Bannerlord loading screens, medieval fictional land of Calradia,
muted earthy palette, dramatic chiaroscuro candle or torch light, painterly brush strokes, cinematic composition, 16:9,
no text, no letters, no logos, no watermark, no gore, no nudity
```

- Yüzler genel ve isimsiz olsun (belirli bir oyun karakterine benzemesin): kart her lord için kullanılacak.
- Çok kişi varsa en fazla 3–4 figür; sahne okunur kalsın.
- Kan, yara, çıplaklık yok. Ölüm ima edilir (devrik kadeh, söndürülmüş mum), gösterilmez.

## Liste

### Entrika türleri ve sonuçlar

| # | Dosya | Ne anlatıyor | Prompt |
|---|---|---|---|
| 1 | `tur_casusluk.png` | Casusluk başlarken / rapor | A hooded spymaster reading a sealed letter by a single candle in a cramped stone room, maps and wax seals on the table |
| 2 | `sonuc_sir_bulundu.png` | Bir sır bulundu | Close-up of gloved hands opening a hidden compartment in a noble's writing desk, revealing a bundle of secret letters tied with ribbon |
| 3 | `tur_santaj.png` | Şantaj | A nervous medieval lord in rich robes reading a letter in his hall while a calm cloaked figure watches him from the doorway |
| 4 | `tur_suikast.png` | Suikast planı | A dark castle corridor at night, a shadowy figure waiting behind a pillar, a dagger's edge catching the torchlight |
| 5 | `sonuc_suikast_zehir.png` | Zehirle başarılı | An empty banquet table after a feast, a single overturned silver goblet, spilled wine, a guttering candle, no people |
| 6 | `sonuc_suikast_hancer.png` | Hançerle başarılı | A cloaked figure slipping away down a rain-soaked night alley in a walled town, a bell tower in the background |
| 7 | `sonuc_suikast_kaza.png` | Kaza süsü | A broken noble carriage on a misty mountain road at dawn, a riderless horse standing nearby, crows in the sky |
| 8 | `tur_kacirma.png` | Kaçırma | Masked riders galloping through a moonlit forest, one carrying a bound captive across his saddle |
| 9 | `sonuc_yakalandi.png` | Ajan yakalandı / girişim başarısız | Torch-bearing guards seizing a struggling hooded agent in a castle courtyard at night |
| 10 | `sonuc_ifsa.png` | İfşa / skandal | Nobles whispering behind their hands in a grand hall, one lord pointing accusingly, faces lit by braziers |
| 11 | `sonuc_hukumdar_hesap.png` | Hükümdar hesap soruyor | An angry ruler on a throne in a great hall, a kneeling noble before him, armed guards at the sides |
| 12 | `sonuc_entrika_sezildi.png` | Sana karşı entrika sezildi | A loyal advisor leaning in to whisper urgent news to a lord at night, an open window and a dark city behind them |
| 13 | `yer_golge_agi.png` | Gölgelerde iş çevir (şehir menüsü) | A tavern back room, a scarred gang leader counting coins across the table from a hooded visitor, smoke and lantern light |
| 14 | `sonuc_koz.png` | Koz kullanıldı | A proud lord reluctantly handing a heavy coin purse to a cloaked visitor in his private study |

### Olaylar (aşamalar arasında)

| # | Dosya | Olay | Prompt |
|---|---|---|---|
| 15 | `olay_rusvetci_hizmetkar.png` | Rüşvetçi hizmetkâr | A castle servant in a kitchen doorway at night holding out an open palm for a coin, glancing over his shoulder |
| 16 | `olay_ajan_yakalandi.png` | Ajan yakalandı | A bound man sitting in a damp dungeon cell, a jailer's lantern at the bars, chains on the wall |
| 17 | `olay_zehir_hancer.png` | Zehir mi, hançer mi? | A table with a small glass vial of poison and an unsheathed dagger side by side, a hand hovering between them, candlelight |
| 18 | `olay_hedef_yola_cikti.png` | Hedef yer değiştirdi | A lord's retinue riding out of a town gate at dawn, banners raised, a watcher in the shadows of the wall |
| 19 | `olay_rakip_suikastci.png` | Rakip suikastçı | Two hooded figures meeting warily on a moonlit rooftop of a medieval town, each with a hand on a hidden blade |
| 20 | `olay_muhafiz_degisimi.png` | Muhafız değişimi | Night guards changing shifts at a castle gate, one yawning, torches flickering, an unguarded side door ajar |
| 21 | `olay_vicdan_azabi.png` | Vicdan azabı | A troubled companion sitting alone by a campfire, staring at a dagger in his hands, the camp asleep behind him |
| 22 | `olay_supheli_tuccar.png` | Şüpheli tüccar | A busy town market, a merchant pretending to inspect cloth while secretly watching two cloaked men in the crowd |
| 23 | `olay_ziyafet.png` | Ziyafet daveti | A lively medieval feast in a long hall, musicians, roasting meat, guards distracted, a servant carrying a covered dish |
| 24 | `olay_cete_teklifi.png` | Çete liderinin teklifi | A gang leader leaning against an alley wall offering a handshake, his men lurking in the shadows behind him |

**Öncelik:** 1, 3, 4, 8, 9, 10, 12 ve 15–24. Bunlar en sık görülecek kartlar. Diğerleri sonra gelebilir.
