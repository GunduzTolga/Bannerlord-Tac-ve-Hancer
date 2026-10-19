# Yapılacaklar

Son güncelleme: 19 Ekim 2026 · Mod durumu: Faz 1, 2, 3A, 3B derlendi ve oyuna kuruldu.
Faz 1 ve 2 oyunda denendi ve çalışıyor; **3A ve 3B henüz denenmedi.**
Mod adı artık **Taç ve Hançer** (Launcher'da bu adla görünür). **Kademeye göre asker ekleme**, **yoldaş sınırını kaldır**,
**parti sayısı sınırı**, **Entrika E0, E1 ve E2** (casus başı, casusluk, sırlar, suikast, şantaj/koz, kaçırma, gölge ağı, yapay zekâ entrikaları) kodlandı; bilgisayarda `build.ps1` ile derlenip denenmeli.
**P1:** Entrika ekranı (Gauntlet), resimli olay kartları ve haritadaki Entrika düğmesi kodlandı. Oyunda ilk kez denenecek; ekran görüntüsü çok işe yarar.

> ⚠️ Telefondan kod yazılıp GitHub'a gönderilebilir, ama **derleyip oyuna kurmak bilgisayar ister**
> (Visual Studio + oyunun DLL'leri). Telefonda yapılanlar, bilgisayarda `build.ps1` çalışınca oyuna gelir.

## 1. Oyunda test et (bilgisayarda, yedekli bir kayıtta)

### Önce: kurulum ve derleme (P0 altyapısı — yeni)
- [ ] Ya derle (**.NET SDK 8+** gerekir: `dotnet --version`, yoksa `winget install Microsoft.DotNet.SDK.8`) ya da **derlemeden kur**:
      GitHub → Actions → son yeşil çalıştırma → **TacVeHancer** paketi → içindeki `CheatMenu` klasörünü `Modules`'e kopyala
- [ ] Nexus/Workshop'tan **Harmony, ButterLib, UIExtenderEx, MCM** kurulu; Launcher'da sıra Harmony → ButterLib → UIExtenderEx → MCM → … → Taç ve Hançer
- [ ] `build.ps1` çalışıyor (ilk seferde NuGet paketleri iner), testler geçiyor, mod oyun klasörüne kopyalanıyor
- [ ] Ana menüde kırmızı "başlatılamayan bağımlılık" uyarısı **yok**
- [ ] **Esc → Mod Seçenekleri → Taç ve Hançer** açılıyor: Kurallar, Entrika, Harita grupları; eski ayarların (CheatMenu.txt) değerleri gelmiş
- [ ] MCM'de bir ayarı değiştir → Insert → Kurallar'da da değişmiş; Kurallar'da değiştir → MCM'de de değişmiş

### P1 — Entrika ekranı, olay kartları, harita düğmesi (yeni; en çok ekran görüntüsü lazım)
- [ ] Ana menüde açılış mesajı: "Insert: menü, Home: entrika ekranı"
- [ ] Haritanın üst ortasında **ENTRİKA** düğmesi var; tıklayınca ekran açılıyor ve oyun duruyor
- [ ] **Home** ekranı açıp kapatıyor; **Esc** ve **Kapat** kapatıyor; Insert → **Entrika ekranı** da açıyor
- [ ] Görünüm: koyu pencere, altın kenarlık, Türkçe harfler düzgün, yazılar okunur, taşan/üst üste binen yazı yok (ekran görüntüsü at)
- [ ] Sol sütun: casus başının **portresi**, Roguery, durum, sınırlar; **Değiştir** ile seçim listesi açılıyor
- [ ] Sekmeler tıklanınca değişiyor: Süren entrikalar, Yeni entrika, Kişiler, Gölge ağları, Sana karşı, Kronik
- [ ] Yeni entrika: tür ve filtre düğmeleri, sayfalar; bir hedef seç → sağda özet → **Başlat** → altın düştü, "Süren entrikalar"a geçti
- [ ] Süren entrika kartı: portre, aşama çubuğu (en az aşama çizgisiyle), kalan gün, başarı/yakalanma; **Harekete geç** → rüşvet kartı; **İptal et** → onay
- [ ] Kişiler: rapor/sır/koz olan herkes; dosyada rapor, **İfşa et**, koz düğmeleri, **Entrika başlat** (Yeni entrika'yı o hedefle açar)
- [ ] Gölge ağları ve Sana karşı sekmeleri; karşı hamle düğmeleri çalışıyor
- [ ] Kronik: olaylar en yeni üstte; **Dosyaya aktar** → Belgeler\Mount and Blade II Bannerlord\Configs\TacVeHancer\Kronik_….txt
- [ ] Olaylar, kararlar ve sonuçlar artık **resimli kart**: resim yoksa kişinin portresi; seçenekler çalışıyor; bildirimler Esc ile de kapanıyor
- [ ] Modun `Images` klasörüne bir resim koy (ör. `olay_rusvetci_hizmetkar.png`) → o olayın kartında görünüyor
- [ ] Ekran açıkken klavye haritaya gitmiyor (C / I / P karakter, envanter, parti ekranını açmıyor)
- [ ] Ekran ya da kart açılamazsa çökme yerine kırmızı mesaj + liste menüsü / düz pencere geliyor. Olursa
      `C:\ProgramData\Mount and Blade II Bannerlord\logs` içindeki en yeni `rgl_log` dosyasını gönder
- [ ] MCM → **Arayüz**: kısayol, düğme yeri (Gizli / Sol üst / Üst orta / Sağ üst), resimli kartlar aç/kapa
- [ ] Haritadaki düğme oyunun kendi arayüzünün üstüne biniyor mu? Biniyorsa MCM'den yerini değiştir ve hangisinin iyi olduğunu yaz
- [ ] **Bana gönder (ansiklopedi bağlantısı için):** oyun klasöründe `Modules` altında `EncyclopediaHeroPage.xml` ve `MapBar.xml`
      dosyalarını ara (Windows aramasıyla) ve ikisini gönder. Bunlarla lord sayfasına "bildiğin sırlar / koz / entrika başlat" bölümünü
      ve oyunun harita çubuğuna düğmeyi doğru yere ekleyebilirim

### P2.2 — Haberci ve saray istihbaratı (yeni)
- [ ] Entrika ekranı → **Saray**: « Önceki / Sonraki » ile krallıklar; hükümdar portresi, güç karşılaştırması, bilgi kaynağı yazısı
- [ ] Raporun olan lordda "Hükümdarıyla: hoşnutsuz (-24)" gibi tam sayı; gölge ağın olan krallıkta "(söylenti)"; diğerlerinde "bilinmiyor"
- [ ] Hoşnutsuzlar kırmızı ve listenin başında; "Casusluk (tam bilgi için)..." Yeni entrika'yı o lordla açıyor
- [ ] **Davet mektubu** (bir krallıktayken, 200 etkin varken): şans, ücret ve gün yazıyor → gönder → altın düştü,
      "Süren" sekmesinde **Yoldaki haberciler** altında görünüyor
- [ ] Günler geçince kart: kabul (klan krallığına geçti, etki düştü) ya da ret (ilişki -5; bazen mektup hükümdarına gitti)
- [ ] Kişiler → kozun olduğu uzak bir lord: koz düğmelerinde "(haberciyle N gün, … dinar)"; seç → haberci yola çıktı → varınca sonuç kartı
- [ ] Aynı lorda yan yana: koz düğmeleri haberci yazısız, hemen çalışıyor
- [ ] Kişi dosyasında "Hükümdarı X ile: ..." satırı ve (başka krallıktaki klan liderinde) davet düğmesi
- [ ] Haberci yoldayken kaydet → yükle → haberci hâlâ yolda, aynı gün varıyor

### P2.1 — Esir sorgulama (yeni)
- [ ] Bir lordu esir al → Entrika ekranı → **Esirler** sekmesi: portre, "Partinde", ilişki/Hesapçı/Cesaret/Onur, üç yöntemin olasılığı
- [ ] **Sorgula...** → kart: Konuştur / Baskı yap / Anlaşma teklif et / Vazgeç (her birinde başarı ve bedel yazıyor)
- [ ] Konuştur başarılı → sonuç kartında rapor; bazen "SIR BULUNDU" ve "Efendisi ... hakkında da konuştu"; Kişiler sekmesinde görünüyor
- [ ] Baskı yap → Merhamet düştü, esirle ilişki -15; bazen "klanı duydu"
- [ ] Anlaşma → esir serbest kaldı (parti ekranında yok), ilişki +10
- [ ] Aynı esiri hemen tekrar sorgulamak: "yakında sorgulandı; N gün sonra"
- [ ] Bir kalendeki/şehirdeki zindanda tutulan esir: uzaktayken "oraya git", o yerleşimdeyken sorgulanabiliyor
- [ ] Efendisi hakkında rapor alınan, tanışmadığın lord **Yeni entrika > Suikast** listesinde çıkıyor
- [ ] Insert → Entrika (liste menüsü) → **Esirleri sorgula** da çalışıyor
- [ ] Kart açıkken oyun duruyor (gün ilerlemiyor); Esc ile kapanan kartta oyunun kaçış menüsü açılmıyor

### Entrika E0 — yeni ad, menü düzeni, kayıt testi (önemli: E1 buna dayanıyor)
- [ ] Launcher'da mod **Taç ve Hançer** adıyla görünüyor, Türkçe harfler düzgün
- [ ] Insert → ana menü: Entrika, Hileler, Editörler, Klan & krallık, Bilgi, Kurallar; alt menülerde **Geri** doğru gruba dönüyor
- [ ] Entrika → **Kayıt testi: işaret bırak** → kaydet → kaydı yeniden yükle → Entrika menüsünde "Şu anki işaret: gün …" duruyor
- [ ] ⚠️ Yedek kayıtta: Launcher'da modu **kapat** → aynı kaydı aç → kayıt açılıyor (sadece "modül eksik" uyarısı normal) → modu geri aç

### Entrika E1 — casus başı, casusluk, suikast (yedekli bir kayıtta)
- [ ] Entrika → **Casus başı ata** → yoldaşlar portreli listede; birini seç → menüde "Casus başı: X - Roguery N"
- [ ] Yeni entrika → **Casusluk** → bir lord seç → onay ekranında maliyet, başarı, risk → başlat → altın düştü
- [ ] Birkaç gün bekle → "ilk bilgiler geldi" popup'ı → **Raporu al** → rapor popup'ı (konum, parti, altın, ilişkiler, kişilik, sır satırı)
- [ ] Entrika → **İstihbarat raporları** → rapor görünüyor; sır varsa **İfşa et** → sonuç popup'ı, ilişkiler değişti
- [ ] Yeni entrika → **Suikast** → tanıştığın ya da raporlu bir hedef → sol altta "aşama x/5 tamamlandı" mesajları
- [ ] Aşamalar arasında **olay popup'ları** çıkıyor (rüşvetçi hizmetkâr, zehir mi hançer mi…), seçimler uygulanıyor
- [ ] Süren entrikalar → **Harekete geç** → rüşvet seçenekleri → portreli sonuç popup'ı → hedef gerçekten öldü ya da yaralı kurtuldu
- [ ] Başarılı suikasttan sonra oyunun kendi ölüm bildirimi geliyor, hedefin klanına yeni lider geçiyor
- [ ] Yakalanınca hafif/ağır ifşa sonuçları; kendi krallığından birini öldürüp yakalanınca **"Hükümdar hesap soruyor"**
- [ ] Entrika sürerken **kaydet → yükle** → entrika aynı aşamada; bekleyen popup yeniden soruluyor
- [ ] Kurallar → **Entrika zorluğu** Kolay/Zor → onay ekranındaki yüzdeler değişiyor
- [ ] Hileler → **Entrika hileleri**: aşamaları tamamla, asla fark edilme, her zaman başarılı
- [ ] Popup'lar kapanıyor mu (portreli bildirimde **Tamam** çalışmazsa **Kapat** var)? Türkçe ekler doğru mu (Caladog'un, Sargot'ta)?

### Entrika E2 — şantaj, kaçırma, gölge ağı, yapay zekâ entrikaları
- [ ] Casusluktan sır bulunan bir lorda Yeni entrika → **Şantaj** → "şimdi tehdit et / delil topla" popup'ı → koz kazanıldı
- [ ] Entrika → **Kozlar** → altın / etki / esir bıraktırma / krallık değiştirtme seçenekleri; birini kullan → sonuç popup'ı
- [ ] Kozun olduğu lordla **konuş** → "Seninle özel bir konuyu konuşmamız gerek..." satırı görünüyor, seçenekler çalışıyor
- [ ] Yeni entrika → **Kaçırma** → harekete geç → başarılıysa hedef partinde **esir** (parti ekranında görünüyor)
- [ ] Çete lideri olan bir şehirde kasaba menüsünde **"Gölgelerde iş çevir"** → ağ kur → Entrika > Gölge ağları'nda görünüyor
- [ ] O şehirdeki bir hedefe entrika onay ekranında "Gölge ağı desteği" satırı çıkıyor
- [ ] Bir hafta geçince ağ bakımı altından düşüyor
- [ ] (Uzun oyun) Senden nefret eden bir lord ailene entrika kurdu → **"Sana karşı entrika sezildi"** popup'ı → ifşa / koruma / karşı suikast
- [ ] Kurallar → **Yapay zekâ sana entrika kursun** Kapalı/Az/Normal; **kendine de suikast** varsayılan kapalı
- [ ] Ortaya çıkan bir cinayetten sonra **kan davası** notu sonuç popup'ında görünüyor

### Asker ekle (kademeye göre) — yeni
- [ ] Ordu / Parti → **Asker ekle (kademeye göre)** → kademeler (T1, T2 …) ve her birindeki asker türü sayısı görünüyor
- [ ] Bir kademeye gir → kendi kültürün dışındaki askerler, paralı askerler ve haydutlar da listede
- [ ] Kademe listesinde her askerin **resmi** görünüyor; liste açılırken uzun takılma veya çökme yok
- [ ] Asker ve kültür adları **Türkçe** (oyunun diliyle aynı)
- [ ] Bir asker seç → adet yaz → partiye ekleniyor, liste aynı kademeye geri dönüyor
- [ ] **Tüm kademeler** listesinde isimle arama çalışıyor
- [ ] Listede olmaması gereken tuhaf karakter (turnuva, görev, eğitim karakteri vb.) var mı? Varsa adını yaz, filtreyi daraltırım

### Yoldaş sınırını kaldır — yeni
- [ ] Kurallar → **Yoldaş sınırını kaldır** AÇIK → klan ekranında yoldaş sınırı 999 görünüyor
- [ ] Sınır dolduktan sonra tavernada yeni bir yoldaş **işe alınabiliyor** (eskiden "daha fazla yoldaş alamazsın" deniyordu)
- [ ] KAPALI → sınır klan kademesine göre normale dönüyor (fazla yoldaşlar gitmez, sadece yenisi alınamaz)

### Parti sayısı sınırı — yeni
- [ ] Kurallar → **Parti sayısı sınırına ekle** → 3 yaz → klan ekranının Partiler sekmesinde sınır 3 artmış
- [ ] Sınır dolduktan sonra yeni parti **kurulabiliyor**
- [ ] **Parti sayısı sınırını kaldır** AÇIK → sınır 999; KAPALI → yine "normal + ekleme"

### Faz 3A — Klan & krallık, Bilgi
- [ ] Bağımsızken bir krallığa **vasal olarak katıl**, sonra **krallığından ayrıl**
- [ ] **Paralı asker** olarak katıl → aynı krallığa vasal olarak geç
- [ ] **Kendi krallığını kur** (bağımsızken) → ansiklopedide "X Krallığı"
- [ ] Bir krallıktayken **hükümdar ol**
- [ ] **Klan kademesi** 5 yap → klan ekranında 5
- [ ] İki AI krallığı arasında **savaş ilan ettir**, sonra **barış yaptır**
- [ ] Bir AI klanını başka krallığa **taşı**; bir klanın **liderini** ve bir krallığın **hükümdarını** değiştir
- [ ] Bir AI krallığını **yok et**
- [ ] Bilgi → **Bir lordu bul** → kamera lorda kayıyor mu, konum doğru mu
- [ ] **Harita sisini kaldır** AÇIK → uzaktaki partiler görünüyor; kaydet-yükle → hâlâ açık; KAPALI → normal

### Faz 3B — Kurallar
- [ ] **Yetenek XP 5x** → bir savaş → yetenek çubukları belirgin hızlı doluyor
- [ ] **Asker XP 5x** → savaş sonrası terfiye hazır asker sayısı artıyor
- [ ] **Parti boyutu +100** → parti ekranında sınır +100
- [ ] **Öğrenme sınırını kaldır** → karakter ekranında öğrenme sınırı ~1000+ görünüyor ⚠️ *(görünmüyorsa söyle, en riskli kısım bu)*
- [ ] Oyunu kapat-aç → Kurallar ayarları duruyor
- [ ] Savaş içi → **(Ordu) + müttefikleri de kat** → ortak savaşta müttefik askerler de ölümsüz
- [ ] War Sails: gemideyken parti ekranı ve bir deniz savaşı normal

### Önceki fazlardan açık kalanlar
- [ ] Şehir adını değiştirince haritadaki yazı **hemen** değişiyor mu (yoksa kaydet-yükle sonrası mı)
- [ ] **Berber ekranı** haritadan açılınca düzgün mü
- [ ] Yaş değiştirdikten birkaç gün sonra yaş hâlâ doğru mu (Doğum ve Yaşlanma açıkken)

Bir şey bozuksa: ekrandaki kırmızı `[Taç ve Hançer]` mesajını aynen yaz. Çökme olursa log:
`C:\ProgramData\Mount and Blade II Bannerlord\logs\` içindeki en yeni `rgl_log_*.txt`.

## 2. Sıradaki büyük iş — Entrika sistemi (CK3 tarzı)

Mod artık sadece hile menüsü değil, "her şeyi içeren" mod olacak. Tasarım taslağı:
`docs/superpowers/specs/2026-10-19-entrika-design.md` (casus başı, casusluk, sırlar, suikast, kozlar, yapay zekâ entrikaları).
- [x] Tasarımdaki 5 açık soru cevaplandı (hepsi önerildiği gibi, ad: Taç ve Hançer)
- [x] **E0** — menü düzeni, görünen ad, kayıt testi (kodlandı; oyunda test bekliyor)
- [x] **E1** — casus başı, casusluk + sırlar, suikast (aşamalar, olaylar, sonuçlar) (kodlandı; oyunda test bekliyor)
- [x] **E2** — şantaj/koz, kaçırma, gölge ağı, yapay zekâ entrikaları, karşı casusluk (kodlandı; oyunda test bekliyor)
- [ ] **E3** — sabotaj, kışkırtma, itibar zedeleme, resimli olay kartları

## 3. Sonraki büyük iş — Faz 4

Rapor bunları v1.5 kararlı sürüme çıkana kadar bekletmeyi öneriyordu (v1.5 şu an beta). İstersen beklemeden başlarız.
- [ ] **Yoldaş / başka kahraman görünümü** (yüz editörü) — orta-zor
- [ ] **Kişiye özel yaş kilidi** (seçtiğin kişiler yaşlanmasın)
- [ ] **Zorunlu gizlilik görevlerini atlama** seçeneği
- [ ] **War Sails araçları** — gemi ve figürhed verme
- [ ] **Tek panelli editör merkezi** — popup yerine sekmeli tek ekran (en büyük iş)

## 4. Ertelenen küçük işler
- [ ] Savaş hilelerini **belirli klan/krallığa** uygulama (şu an: ben / ordum / müttefikler)
- [ ] **İttifak kurma** (önce oyunun ittifak API'si doğrulanmalı)
- [ ] **Tek perk silme** (şu an sadece ekleme var)
- [ ] **Toplu ilişki** değişikliği (ör. bir klanın tamamıyla)
- [ ] Askerlerin **günlük eğitim XP'si** çarpanı

## 5. Repo ile ilgili kararlar
- [ ] **Lisans** eklensin mi? (şu an yok = tüm hakları saklı; açık yapacaksan MIT önerilir)
- [ ] Repo **private** kalsın mı, herkese açık mı olsun?

## 6. v1.5 kararlı sürüm çıkınca
- [ ] Tüm fazları yeniden derle ve test et (oyun güncellemesi API'leri değiştirebilir)
- [ ] v1.5'in yerli olarak getirdiği özelliklerle çakışma var mı bak (başlangıç seçenekleri, klan partisi anahtarları)

## Nasıl devam edilir
Claude'a şunlardan birini yazman yeterli:
- "3A/3B testi: şunlar çalıştı, şunlar bozuk: …"
- "Faz 4'e geçelim" (ya da listeden tek bir madde)
- "Lisans ekle" / "Repoyu public yap"
