# Görsel listesi: bütün resimler, dosya adları ve prompt'lar

Modda üretilmesi gereken resimlerin **hepsi** bu dosyada: **64 olay kartı resmi**, her biri için iki sürüm daha (en sonda: "Sürümler"). Başka resim gerekmiyor. Entrika ekranı, harita
düğmesi ve sekmeler oyunun kendi çizimlerini kullanıyor. Yeni bir özellik resim isterse buraya eklenir; birim testi koddaki her resim
adının burada olduğunu denetler.

## Teknik

- **Boyut:** 1600×900 (16:9), PNG. Daha büyük üretilirse sorun yok; oyuna koyarken küçültülür.
- **Dosya adı:** tablodaki ad (küçük harf, Türkçe karakter yok), örn. `olay_rusvetci_hizmetkar.png`.
- **Nereye (en kolay):** resimleri `Masaüstü\TVH Resimler` klasörüne koy ve depoda
  `powershell -ExecutionPolicy Bypass -File tools\resim-aktar.ps1` çalıştır: adı düzeltir (büyük harf, Türkçe harf, boşluk), yanlış adda en
  yakınını önerir, 16:9'a kırpıp 1600×900 JPG yapar, hem `Modules\CheatMenu\Images\` (oyun) hem `Module/Images/` (depo) içine koyar,
  sonunda kaç kartın bittiğini ve eksik ★ kartları yazar. Elle koyarsan: modun klasöründe `Modules\CheatMenu\Images\`; kart bir sonraki
  açılışta resmi gösterir. Aynı adla hem .png hem .jpg varsa oyun .png'yi gösterir.
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
- Genelev resimlerinde (32, 50–54) herkes tamamen giyinik; kapı, koridor, perde, meyhane, baskın ve sabah anlatılır. İçerisi ve
  yakınlık gösterilmez (oyundaki kartlar da "perde iner" diye anlatır).

Ortak stil (yeni bir prompt yazarsan sonuna ekle):

```
, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

## Kontrol listesi

`tools\resim-aktar.ps1` her çalışmada Durum sütununu ve bu sayıyı oyundaki resimlere göre kendisi günceller (**Durum: 49/70 tamamlandı** — 29 adet `★` öncelikli + 20 adet normal kart). **Öncelik** sütunu: ★ en sık görülen kartlar, önce bunlar.

| # | Dosya | Nerede görünür | Öncelik | Durum |
|---|---|---|---|---|
| 1 | `tur_casusluk.png` | Casusluk başlarken ve casusluk raporu | ★ | ✅ |
| 2 | `sonuc_sir_bulundu.png` | Casuslukta bir sır bulundu | | ✅ |
| 3 | `tur_santaj.png` | Şantaj aşamaları ve şantaj tuttu | ★ | ✅ |
| 4 | `tur_suikast.png` | Suikast aşamaları | ★ | ✅ |
| 5 | `sonuc_suikast_zehir.png` | Suikast zehirle başarılı | | ✅ |
| 6 | `sonuc_suikast_hancer.png` | Suikast hançerle başarılı | | ✅ |
| 7 | `sonuc_suikast_kaza.png` | Suikast kaza süsüyle başarılı | | ✅ |
| 8 | `tur_kacirma.png` | Kaçırma aşamaları ve sonucu | ★ | ✅ |
| 9 | `sonuc_yakalandi.png` | Ajan yakalandı, girişim başarısız | ★ | ✅ |
| 10 | `sonuc_ifsa.png` | Sır ifşası, şantaj tutmadı | ★ | ✅ |
| 11 | `sonuc_hukumdar_hesap.png` | Hükümdar hesap soruyor | | ✅ |
| 12 | `sonuc_entrika_sezildi.png` | Sana karşı entrika sezildi, korundun | ★ | ✅ |
| 13 | `yer_golge_agi.png` | Şehir menüsü > Gölgelerde iş çevir | | ✅ |
| 14 | `sonuc_koz.png` | Koz kullanıldı | | ✅ |
| 15 | `olay_rusvetci_hizmetkar.png` | Olay: rüşvetçi hizmetkâr | ★ | ✅ |
| 16 | `olay_ajan_yakalandi.png` | Olay: ajan yakalandı | ★ | ✅ |
| 17 | `olay_zehir_hancer.png` | Olay: zehir mi, hançer mi? | ★ | ✅ |
| 18 | `olay_hedef_yola_cikti.png` | Olay: hedef yer değiştirdi | ★ | ✅ |
| 19 | `olay_rakip_suikastci.png` | Olay: rakip suikastçı | ★ | ✅ |
| 20 | `olay_muhafiz_degisimi.png` | Olay: muhafız değişimi | ★ | ✅ |
| 21 | `olay_vicdan_azabi.png` | Olay: vicdan azabı | ★ | ✅ |
| 22 | `olay_supheli_tuccar.png` | Olay: şüpheli tüccar | ★ | ✅ |
| 23 | `olay_ziyafet.png` | Olay: ziyafet daveti | ★ | ✅ |
| 24 | `olay_cete_teklifi.png` | Olay: çete liderinin teklifi | ★ | ✅ |
| 25 | `sonuc_sorgu.png` | Esir sorgusu (P2.1) | ★ | ✅ |
| 26 | `sonuc_haberci.png` | Haberci sonucu, ulak döndü, görüşme (P2.2) | ★ | ✅ |
| 27 | `sonuc_gizli_bulusma.png` | Gizli ilişki ya da gayrimeşru çocuk sırrı bulundu (P2.4) | | ✅ |
| 28 | `sonuc_skandal.png` | Gizli ilişki ya da gayrimeşru çocuk ifşa edildi (P2.4) | | ✅ |
| 29 | `sonuc_fisilti.png` | Aldatılan eşe söylendi (P2.4) | | ✅ |
| 30 | `sonuc_diplomatik_kriz.png` | Yabancı krallıkta ağın çökertildi, elçi hesap soruyor (P2.5) | | ✅ |
| 31 | `sonuc_savas_meclisi.png` | Casus skandalları yüzünden savaş meclisi toplandı (P2.5) | | ⏳ |
| 32 | `yer_genelev.png` | Genelev: kapı ve dış cephe; işletmen, ayrılan çalışanlar, genelev kartı | | ✅ |
| 33 | `sonuc_hizmet_yemini.png` | Esir hizmetine katıldı (bağlılık yemini) | | ✅ |
| 34 | `sonuc_kur.png` | Kur: başlangıç, ret, eşin reddi, eşinin bilgi görevi (P2.6) | | ✅ |
| 35 | `sonuc_dogum.png` | Kurdan ya da sevgiliden doğan çocuk (P2.6) | | ✅ |
| 36 | `sonuc_aldatma.png` | Eşinin gizli ilişkisi ortaya çıktı, yüzleşme | | ✅ |
| 37 | `sonuc_bosanma.png` | Boşanma | | ✅ |
| 38 | `sonuc_duello.png` | Düello: meydan okuma, ret, sonuç | ★ | ✅ |
| 39 | `sonuc_mesrulastirma.png` | Gayrimeşru çocuk meşrulaştırıldı (P2.8) | | ✅ |
| 40 | `yer_saray_gunu.png` | Saray günü: divan ve dilekçeler (P2.9) | ★ | ✅ |
| 41 | `sonuc_rehine.png` | Rehine anlaşması: gönderme, değişim, dönüş, bozulan barış (P2.10) | | ✅ |
| 42 | `olay_takip.png` | Kendin takip et: gündüz sahneleri ve takip raporu (P2.11) | ★ | ✅ |
| 43 | `olay_takip_gece.png` | Kendin takip et: gece sahneleri (P2.11) | ★ | ✅ |
| 44 | `sonuc_hazine.png` | Divan: hazinedarın atölye önerisi, uzaktan üretim emri (P2.12) | ★ | ✅ |
| 45 | `sonuc_zimmet.png` | Divan: hazinedarın zimmeti ortaya çıktı, hesap sor (P2.12) | | ✅ |
| 46 | `sonuc_hekim.png` | Divan: saray hekiminin uyarısı, sağlık (P2.13) | | ⏳ |
| 47 | `sonuc_panzehir.png` | Hekim suikast kurbanını kurtardı (P2.13) | | ⏳ |
| 48 | `sonuc_serdar.png` | Divan: mareşalin erken uyarısı, düşman ordusu yürüyor (P2.14) | ★ | ✅ |
| 49 | `sonuc_kahya.png` | Divan: kâhyanın uyarısı (isyan, kıtlık) ve uzaktan inşaat emri (P2.15) | ★ | ✅ |
| 50 | `olay_genelev_gece.png` | Genelev: müşteri olarak bir gece (perde iner) | | ⏳ |
| 51 | `olay_genelev_askerler.png` | Genelev: askerlere bir gece ısmarlamak (moral) | | ⏳ |
| 52 | `sonuc_genelev_dedikodu.png` | Genelevde duyulanlar; kendi genelevinin dedikodusu | | ⏳ |
| 53 | `olay_genelev_baskin.png` | Genelev baskını (başka birinin şehrinde) | | ⏳ |
| 54 | `sonuc_hastalik.png` | Hastalık ve tedavisi (şehrin hekimi, saray hekimi) | | ⏳ |
| 55 | `tur_sabotaj.png` | Sabotaj aşamaları, hazırlık ve "harekete geç" (E3) | ★ | ✅ |
| 56 | `tur_kiskirtma.png` | Kışkırtma aşamaları ve "harekete geç" (E3) | ★ | ✅ |
| 57 | `sonuc_sabotaj_ambar.png` | Sabotaj tuttu: ambar yandı, erzak gitti (E3) | | ⏳ |
| 58 | `sonuc_sabotaj_garnizon.png` | Sabotaj tuttu: kuyular bozuldu, garnizon hasta (E3) | | ⏳ |
| 59 | `sonuc_sabotaj_sur.png` | Sabotaj tuttu: sur zayıfladı ya da çöktü, gedik (E3) | | ⏳ |
| 60 | `sonuc_isyan.png` | Kışkırtma tuttu: halk ayaklandı, belki oyunun kendi isyanı (E3) | ★ | ✅ |
| 61 | `olay_rusvetci_nobetci.png` | Olay: rüşvetçi nöbetçi (sabotaj ve kışkırtma, E3) | | ⏳ |
| 62 | `olay_elebasi_yakalandi.png` | Olay: elebaşı yakalandı (kışkırtma, E3) | | ⏳ |
| 63 | `olay_kacakci_tuneli.png` | Olay: kaçakçı tüneli (sabotaj, E3) | | ⏳ |
| 64 | `olay_ekmek_kuyrugu.png` | Olay: ekmek kuyruğu (kışkırtma, E3) | | ⏳ |
| 65 | `olay_genelev_adaylar.png` | Genelev: çalışan al, kapıya gelen adaylar | | ⏳ |
| 66 | `sonuc_genelev_yonetici.png` | Genelev: evi işleten yoldaş (yönetici) | | ⏳ |
| 67 | `sonuc_genelev_yoldas.png` | Genelev: evdeki yoldaşlar; kendi isteğiyle çalışan ya da ayrılan yoldaş | | ⏳ |
| 68 | `sonuc_baris_elcisi.png` | Barış Masası: köşeye sıkışan krallığın elçisi barış istiyor; reddedilen teklif | | ⏳ |
| 69 | `sonuc_baris_antlasmasi.png` | Barış Masası: antlaşma imzalandı (tazminat, kale, şehir, asker) | ★ | ⏳ |
| 70 | `sonuc_diplomat.png` | Divan: diplomatın uyarısı (savaşa hazırlanan krallık) ve ilişki görevinin başarısı | | ⏳ |

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

**32 · `yer_genelev.png`** — genelev (dış cephe; içerisi gösterilmez)

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

**40 · `yer_saray_gunu.png`** — saray günü (divan)

```
A lord seated on a carved wooden chair at the end of a torch-lit great hall holding court, a line of petitioners waiting, a kneeling merchant presenting a scroll, a scribe at a small desk taking notes, guards by the pillars, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**41 · `sonuc_rehine.png`** — rehine anlaşması (misafir soylu)

```
A young noble in travel clothes being formally welcomed at the gate of a foreign castle by its ruler, two escorts with banners of different colors behind them, servants carrying a small chest, respectful and ceremonial mood, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**42 · `olay_takip.png`** — kendin takip et (gündüz)

```
A hooded figure in a plain wool cloak following a richly dressed lord and two guards through a crowded town market at midday, keeping a careful distance behind a fruit stall, townsfolk bustling between them, the lord unaware, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**43 · `olay_takip_gece.png`** — kendin takip et (gece)

```
A cloaked figure pressed against a stone wall in the shadow of a narrow torch-lit alley at night, watching a noble and a lantern-bearing servant enter a tavern door further down the street, moonlight on wet cobblestones, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**44 · `sonuc_hazine.png`** — hazinedar (divan)

```
A treasurer in a fur-trimmed robe at a heavy oak table in a candlelit counting room, open ledgers, a balance scale, stacks of silver coins and small sample sacks of wool, iron ore and grain, a lord standing beside him reading a report, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**45 · `sonuc_zimmet.png`** — hazinedarın zimmeti

```
A spymaster laying two different ledgers side by side before a stern lord in a dim treasury vault, an open strongbox half empty, the guilty treasurer standing pale by the wall under guard, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**46 · `sonuc_hekim.png`** — saray hekimi

```
An elderly court physician in a dark wool robe examining an aging lord seated by a window in a castle chamber, a table with herb bundles, clay jars, a mortar and an open medical codex, morning light, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**47 · `sonuc_panzehir.png`** — hekim suikast kurbanını kurtardı

```
A court physician kneeling beside a pale wounded noble on a bed in a candlelit bedchamber, pouring an antidote from a small vial into a cup, a bandaged arm, a knocked-over goblet on the floor, a worried family member at the door, no blood, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**48 · `sonuc_serdar.png`** — mareşalin uyarısı (savaş meclisi)

```
A grizzled marshal in mail and a fur-lined cloak leaning over a large map table in a torch-lit war room, wooden army tokens pushed toward a castle, a dusty scout delivering news at his side, the lord listening grimly, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**49 · `sonuc_kahya.png`** — kâhya (şehir yönetimi)

```
A steward in a modest wool robe with a ring of keys at his belt presenting a wax tablet of accounts to a lord on a castle balcony overlooking a busy town, masons raising a new stone building below, grain carts at the gate, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

### Genelev (yetişkin teması; Kurallar'da varsayılan kapalı)

**50 · `olay_genelev_gece.png`** — genelevde bir gece (perde iner; içerisi gösterilmez)

```
An upstairs corridor of a discreet medieval townhouse at night, a single candle in a wall niche, a heavy curtain half drawn across a doorway with warm light behind it, a cloaked nobleman pausing at the threshold, an older hostess in a plain dress holding a lantern further down the hall, everyone fully clothed, intimate secretive mood, nothing shown beyond the curtain, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**51 · `olay_genelev_askerler.png`** — askerlerin gecesi

```
A crowded tavern hall at night full of off-duty soldiers in padded gambesons drinking, singing and laughing at long tables, a fiddler on a bench, serving women carrying jugs of ale, a smiling captain paying a purse of coins to the innkeeper, everyone fully clothed, rowdy festive mood, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**52 · `sonuc_genelev_dedikodu.png`** — genelevde duyulanlar, dedikodu

```
Two noblemen in fur-trimmed cloaks whispering over cups of wine in a dim curtained alcove of a townhouse at night, while a woman in a plain dress pouring the wine quietly listens, candlelight, everyone fully clothed, conspiratorial mood, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**53 · `olay_genelev_baskin.png`** — genelev baskını

```
Town guards with torches and spears bursting through the door of a lantern-lit townhouse at night, startled patrons in cloaks fleeing out the back, an overturned table and spilled wine, a stern guard captain pointing, everyone fully clothed, chaotic tense mood, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**54 · `sonuc_hastalik.png`** — hastalık ve tedavi

```
A feverish nobleman lying in bed under heavy blankets in a stone chamber, pale and sweating, while an elderly physician in dark robes examines him and a servant holds a bowl of steaming herbs, candlelight, everyone clothed, somber mood, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**65 · `olay_genelev_adaylar.png`** — kapıya gelen adaylar (çalışan almak)

```
Three adults in their twenties waiting at the lantern-lit side door of a timber-framed townhouse in the evening, two women and a man from different Calradian cultures in plain wool dresses, tunics and travel cloaks with small bundles, a stern older hostess with a ledger and a quill looking them over from the doorway, everyone fully clothed, quiet businesslike mood, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**66 · `sonuc_genelev_yonetici.png`** — evi işleten yoldaş (yönetici)

```
A confident adult house manager in a fur-trimmed wool coat sitting at a heavy desk by the entrance hall of a candlelit townhouse, an open ledger, a ring of iron keys and a purse of coins in front of them, a burly doorman with a cudgel standing behind, curtains drawn over the inner doorways, everyone fully clothed, calm authoritative mood, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**67 · `sonuc_genelev_yoldas.png`** — evdeki yoldaş (kendi isteğiyle)

```
A cloaked adult traveler, a seasoned companion with a sword at the hip, standing in the warm doorway of a lantern-lit townhouse at night with a red cloth lantern above the door, looking back over the shoulder toward the dark street with a calm and decided expression, nothing shown inside beyond a curtain, fully clothed, quiet independent mood, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

### E3: sabotaj ve kışkırtma

**55 · `tur_sabotaj.png`** — sabotaj

```
A cloaked saboteur crouching in the shadow of a besieged town's inner wall at night, a coil of rope and a small clay oil flask at his feet, distant siege campfires glowing beyond the battlements, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**56 · `tur_kiskirtma.png`** — kışkırtma

```
A fiery agitator in a patched cloak standing on a market cart addressing an angry crowd of townsfolk in a medieval town square at dusk, raised fists and torches, a few guards watching warily from a distance, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**57 · `sonuc_sabotaj_ambar.png`** — ambar yandı

```
A large timber granary burning at night inside town walls, townsfolk forming a bucket chain, sacks of grain spilling into the mud, orange flames lighting the rising smoke, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**58 · `sonuc_sabotaj_garnizon.png`** — garnizon hasta

```
Soldiers in padded gambesons slumped and sick around a stone well in a castle courtyard at dawn, a physician sniffing a bucket of murky water, an officer covering his mouth with his sleeve, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**59 · `sonuc_sabotaj_sur.png`** — sur çöktü

```
A section of a town's stone curtain wall collapsing into a cloud of dust at dawn, besieging soldiers in mail cheering in the distance, defenders scrambling along the parapet, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**60 · `sonuc_isyan.png`** — halk ayaklandı

```
An uprising in a medieval town square, townsfolk with clubs and pitchforks pushing back a thin line of guards before the keep gate, the lord's banner torn down, torches and smoke, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**61 · `olay_rusvetci_nobetci.png`** — rüşvetçi nöbetçi

```
A town gate guard in a padded jacket holding out an open palm in a torchlit gatehouse at night, a cloaked stranger dropping a coin into it, the heavy gate half open behind them, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**62 · `olay_elebasi_yakalandi.png`** — elebaşı yakalandı

```
Town guards hauling a shouting agitator down from a market cart in a crowded square, townsfolk stepping back, a fallen torch on the cobblestones, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**63 · `olay_kacakci_tuneli.png`** — kaçakçı tüneli

```
A weathered smuggler holding a lantern at the mouth of a hidden stone tunnel beneath a town wall, beckoning a cloaked figure inside, crates and wine barrels stacked nearby, damp and dark, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**64 · `olay_ekmek_kuyrugu.png`** — ekmek kuyruğu

```
A long angry line of poor townsfolk with empty baskets outside a bakery at dawn, the baker shaking his head in the doorway, empty bread racks behind him, cold mist, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

### Barış Masası

**68 · `sonuc_baris_elcisi.png`** — barış elçisi

```
A weary envoy in a travel-stained cloak kneeling before a ruler's high seat in a torchlit hall, holding out a sealed letter, a small chest of coins at his side, guards watching from the shadows, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**69 · `sonuc_baris_antlasmasi.png`** — barış antlaşması

```
Two rival rulers seated across a heavy oak table inside a large tent at dusk, a scribe pressing a wax seal onto a parchment treaty, a chest of silver and a ring of castle keys laid on the table, their banners on either side, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**70 · `sonuc_diplomat.png`** — diplomat

```
A well-dressed court diplomat at a writing desk by candlelight, sealing a letter with wax, an open map of rival kingdoms and a small chest of gifts beside him, a messenger waiting at the door, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
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

**`yer_genelev_2.png`** · `yer_genelev_3.png` — genelev (dış cephe)

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

**`yer_saray_gunu_2.png`** · `yer_saray_gunu_3.png` — saray günü (divan)

```
An open-air court under a large tree in a castle courtyard, a lady on a wooden dais listening to two arguing villagers holding a boundary stone, onlookers gathered around, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

```
An Aserai ruler on cushions in a shaded arcade hearing petitions, a veiled widow with two children before him, a guard and a scribe nearby, sunlight through carved screens, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**`sonuc_rehine_2.png`** · `sonuc_rehine_3.png` — rehine anlaşması

```
Two noble youths crossing paths on a bridge between two armed escorts during a formal exchange at dawn, envoys of two realms watching from either bank, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

```
A young guest noble learning swordsmanship in the courtyard of a foreign castle under the eye of the host lord, children of the house watching, banners on the walls, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**`olay_takip_2.png`** · `olay_takip_3.png` — kendin takip et (gündüz)

```
An Aserai spy in a plain robe pretending to examine pottery at a sunlit souk stall while glancing sideways at a passing emir and his guards, faded cloth awnings overhead, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

```
A Vlandian townsman half hidden behind a pillar of a church porch, watching a knight in a fur-trimmed cloak whisper to a priest in the town square, pigeons on the cobbles, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**`olay_takip_gece_2.png`** · `olay_takip_gece_3.png` — kendin takip et (gece)

```
A Sturgian spy crouching on a snowy wooden rooftop at night, watching through a lit window as a boyar counts coins with his steward, smoke rising from the chimneys, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

```
An Imperial agent listening at the closed back-room door of a candlelit inn at night, the silhouette of a hooded stranger visible through the gap, the innkeeper pointedly looking away, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**`sonuc_hazine_2.png`** · `sonuc_hazine_3.png` — hazinedar (divan)

```
A merchant caravan of camels and mules being unloaded in a busy Aserai caravanserai courtyard while a clerk with a wax tablet counts the bales, a treasurer watching from a shaded arcade, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

```
A Vlandian workshop foreman reading a sealed letter in a dye works, vats of colored cloth behind him, workers pausing to listen, a courier with a muddy cloak waiting at the door, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**`sonuc_zimmet_2.png`** · `sonuc_zimmet_3.png` — hazinedarın zimmeti

```
A Sturgian treasurer caught at night in a wooden storehouse slipping silver coins into his own purse by lantern light, a hooded figure watching unseen from the doorway, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

```
A treasurer kneeling and holding out a heavy purse to a seated lord in a castle hall, courtiers watching in silence, an open ledger on the floor between them, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**`sonuc_hekim_2.png`** · `sonuc_hekim_3.png` — saray hekimi

```
An Aserai physician in a turban and long robe teaching a young apprentice over an open herbal manuscript in a courtyard infirmary, shelves of glass and clay vessels, sunlight through arches, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

```
A Battanian healer woman tending a pregnant noblewoman resting by a hearth in a timber hall, steaming herbal brew, a midwife preparing linens, calm and warm atmosphere, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**`sonuc_panzehir_2.png`** · `sonuc_panzehir_3.png` — hekim suikast kurbanını kurtardı

```
A physician sniffing a poisoned cup at a feast table while guards seize a fleeing servant in the background, a noble clutching his chest but alive, nobles rising in alarm, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

```
A Khuzait shaman-physician sewing a wound on a sleeping chieftain inside a felt yurt by lamplight, an empty antidote flask and a broken dagger beside the bed, no blood, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**`sonuc_serdar_2.png`** · `sonuc_serdar_3.png` — mareşalin uyarısı

```
A lone Khuzait rider galloping at dusk toward a hilltop castle with beacon fires being lit along its walls, a dust cloud of a distant army on the horizon, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

```
A Vlandian marshal on the battlements at dawn pointing out over the plain toward the campfires of a besieging host, the garrison captain and archers beside him, smoke rising, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**`sonuc_kahya_2.png`** · `sonuc_kahya_3.png` — kâhya

```
An angry crowd of townsfolk with torches and tools gathering before a town hall at dusk, a worried steward on the steps trying to calm them, guards forming a line, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

```
An Imperial steward inspecting half-empty granaries with a scribe counting sacks by lamplight, a few thin townsfolk waiting outside the door with baskets, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**`olay_genelev_gece_2.png`** · `olay_genelev_gece_3.png` — genelevde bir gece

```
Dawn over a sleeping Imperial town, a nobleman in a dark cloak quietly stepping out of a back door into a narrow alley, a lantern guttering on the wall behind him, a cat watching from a barrel, everyone fully clothed, quiet and discreet mood, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

```
A warmly lit common room in an Aserai house of pleasure, cushions and low brass tables, a musician playing an oud in the corner, a hooded guest being led up a staircase by a hostess holding a candle, everyone fully clothed in robes and veils, discreet atmosphere, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**`olay_genelev_askerler_2.png`** · `olay_genelev_askerler_3.png` — askerlerin gecesi

```
Sturgian warriors in a smoky wooden longhouse feasting by the hearth, horns of mead raised, a skald singing, a bearded commander tossing a purse to the host, everyone fully clothed, boisterous celebration, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

```
The morning after a night of revelry, Vlandian soldiers stumbling back to their camp outside the town walls at dawn, some grinning, one carrying a friend over his shoulder, a sergeant shaking his head, everyone fully clothed, humorous tired mood, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**`sonuc_genelev_dedikodu_2.png`** · `sonuc_genelev_dedikodu_3.png` — genelevde duyulanlar

```
A hostess in a shawl handing a folded note to a hooded spymaster at a back door at night, lantern light, a watchful bodyguard waiting in the alley, everyone fully clothed, secretive mood, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

```
Court ladies and lords gossiping behind their hands in an Imperial palace corridor, glancing at a flustered lord walking past, morning light through tall windows, everyone fully clothed, scandal and whispers, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**`olay_genelev_baskin_2.png`** · `olay_genelev_baskin_3.png` — genelev baskını

```
A guard captain barring the door of a shuttered townhouse with a wooden plank while a small crowd of townsfolk watches in the morning, the red lantern above the door taken down and lying in the mud, everyone fully clothed, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

```
Aserai city watchmen escorting a defiant hostess and a few cloaked customers out of a house at night under torchlight, neighbors peering from latticed windows, everyone fully clothed, tense mood, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**`olay_genelev_adaylar_2.png`** · `olay_genelev_adaylar_3.png` — kapıya gelen adaylar

```
An Aserai courtyard at dusk behind latticed screens, a veiled older hostess seated on cushions interviewing two adult women in modest robes and headscarves while a servant pours tea, everyone fully clothed, calm and businesslike, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

```
A snowy Sturgian street at night, two adults in fur hoods knocking at the back door of a log townhouse with a red lantern, a burly doorkeeper opening it with a candle, everyone fully clothed, quiet hopeful mood, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**`sonuc_genelev_yonetici_2.png`** · `sonuc_genelev_yonetici_3.png` — evi işleten yoldaş

```
An adult house manager in a Vlandian wool doublet counting coins into small stacks at a back-room table by candlelight, a ledger and a stern doorman in the background, everyone fully clothed, shrewd businesslike mood, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

```
An adult Khuzait house manager in a quilted deel standing at the top of a townhouse staircase with a lantern, overseeing the quiet entrance hall below where a cloaked guest is greeted, everyone fully clothed, watchful mood, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**`sonuc_genelev_yoldas_2.png`** · `sonuc_genelev_yoldas_3.png` — evdeki yoldaş

```
An adult Battanian companion with braided hair and a plaid cloak sitting on the windowsill of a lantern-lit upstairs room, counting a small purse of coins and smiling to themself, the street visible below, fully clothed, independent mood, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

```
An adult Imperial companion in a travel cloak leaving a townhouse at dawn with a pack over the shoulder, the red lantern above the door being extinguished by the hostess, fully clothed, calm farewell mood, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**`sonuc_hastalik_2.png`** · `sonuc_hastalik_3.png` — hastalık ve tedavi

```
A court physician in a cluttered apothecary grinding herbs with a mortar and pestle, jars and bundles of dried plants hanging from the beams, a worried squire waiting by the door, warm lamplight, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

```
A nobleman recovering in a sunny courtyard wrapped in a cloak, sipping a steaming cup handed to him by a physician, color returning to his face, morning light, everyone clothed, hopeful mood, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**`tur_sabotaj_2.png`** · `tur_sabotaj_3.png` — sabotaj

```
Two hooded figures in Sturgian wool cloaks slipping through a narrow postern door in a timber palisade at night, one carrying a shuttered lantern, snow on the ground, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

```
An Aserai saboteur in dark robes lowering himself by rope into a stone cistern inside a desert fortress at night, a guard's torch passing far above on the walls, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**`tur_kiskirtma_2.png`** · `tur_kiskirtma_3.png` — kışkırtma

```
Imperial townsfolk gathered in a dim tavern cellar around a speaker holding up a crudely painted plain cloth banner, candles and clay cups on the barrels, conspiratorial mood, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

```
A Battanian crowd of farmers and townswomen around a village well listening to a hooded agitator, pitchforks and staves in hand, overcast sky, tense murmuring mood, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**`sonuc_sabotaj_ambar_2.png`** · `sonuc_sabotaj_ambar_3.png` — ambar yandı

```
A Khuzait steppe fortress storehouse with felt-covered grain bales on fire at night, guards running with water skins, embers swirling in the wind, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

```
A Vlandian castle undercroft filled with smoke, a quartermaster staring in despair at charred grain sacks and broken barrels by lantern light, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**`sonuc_sabotaj_garnizon_2.png`** · `sonuc_sabotaj_garnizon_3.png` — garnizon hasta

```
An Imperial garrison barracks at night, several soldiers lying ill on straw pallets under blankets while a worried sergeant holds up a lantern, a water jug knocked over on the floor, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

```
A Sturgian fortress kitchen, a cook pouring a cauldron of spoiled water into a drain while pale soldiers wait in line with empty cups, grey morning light, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**`sonuc_sabotaj_sur_2.png`** · `sonuc_sabotaj_sur_3.png` — sur çöktü

```
A sappers' tunnel beneath a castle wall lit by torches, timber props cracking and smoldering, rubble falling from the ceiling, a single fleeing silhouette in the distance, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

```
An Aserai desert fortress with a fresh breach in its mudbrick wall, dust settling, defenders with spears rushing to fill the gap, the sun rising behind the tents of a siege camp, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**`sonuc_isyan_2.png`** · `sonuc_isyan_3.png` — halk ayaklandı

```
Battanian townsfolk throwing open a town's wooden gate at night to a cheering crowd, the lord's banner trampled in the mud, torchlight, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

```
Angry Imperial citizens storming a town council hall, overturned benches, a frightened magistrate slipping out through a side door, candlelight, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**`olay_rusvetci_nobetci_2.png`** · `olay_rusvetci_nobetci_3.png` — rüşvetçi nöbetçi

```
A Khuzait fortress sentry on a wooden watchtower leaning down to take a small purse from a hooded rider below, moonlight, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

```
A Vlandian castle postern, a bored guard pretending to look away while two cloaked figures slip past with a covered basket, lantern light, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**`olay_elebasi_yakalandi_2.png`** · `olay_elebasi_yakalandi_3.png` — elebaşı yakalandı

```
A bound agitator sitting on a bench in a town guardhouse at night, a stern guard captain questioning him by candlelight, a confiscated cudgel on the table, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

```
Aserai market guards leading a defiant prisoner in chains through a bazaar while merchants watch in silence, harsh afternoon light, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**`olay_kacakci_tuneli_2.png`** · `olay_kacakci_tuneli_3.png` — kaçakçı tüneli

```
Sturgian river smugglers under a wooden pier at night showing a hooded buyer a dark culvert that leads under the fortress walls, fog on the water, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

```
Inside an old stone water conduit lit by a single torch, a smuggler pointing the way forward to two cloaked men, water trickling along the channel, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**`olay_ekmek_kuyrugu_2.png`** · `olay_ekmek_kuyrugu_3.png` — ekmek kuyruğu

```
A Battanian millhouse where farmers argue with a steward over a nearly empty grain sack, overcast sky, tense mood, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

```
An Aserai bazaar, an angry crowd haggling at a nearly empty bread stall, a merchant raising his hands, dust and harsh light, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**`sonuc_baris_elcisi_2.png`** · `sonuc_baris_elcisi_3.png` — barış elçisi

```
A Vlandian herald carrying a white flag of truce riding slowly into an enemy army camp at dawn, soldiers lowering their spears as he passes between the tents, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

```
An Aserai emissary bowing in a sunlit palace courtyard, servants behind him carrying gift chests and rolled carpets, a ruler listening from a shaded dais, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**`sonuc_baris_antlasmasi_2.png`** · `sonuc_baris_antlasmasi_3.png` — barış antlaşması

```
Sturgian and Imperial nobles clasping forearms on a wooden river bridge between two waiting armies, a priest holding up the treaty scroll, banners lowered, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

```
A defeated lord handing the keys of a castle gate to a victorious ruler before the castle walls, the old garrison filing out under an overcast sky, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

**`sonuc_diplomat_2.png`** · `sonuc_diplomat_3.png` — diplomat

```
An Imperial envoy in an embroidered tunic whispering to a ruler behind a carved screen in a palace corridor, courtiers watching from a distance, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```

```
A Khuzait diplomat presenting a falcon as a gift to a foreign khan inside a large felt tent, both sides' retainers looking on, painterly digital concept art in the style of Mount & Blade II: Bannerlord loading screens, grounded realistic early medieval world of Calradia around the 11th-12th century, period-accurate clothing and armor such as mail hauberks, lamellar, padded gambesons, wool tunics and fur-trimmed cloaks, no full plate armor, no fantasy elements, muted earthy palette of browns, ochres and desaturated blues, warm candle or torch light with deep shadows, visible brush strokes, cinematic 16:9 composition, no text, no letters, no logos, no watermark, no gore, no nudity
```
