# Bannerlord'un boşluklarını tek modla kapatmak

Mount & Blade II: Bannerlord, Ekim 2026'da hâlâ sevilen bir oyun; Steam'de son 30 günde **%89 olumlu**. Yine de en çok oy toplayan şikâyetler yapısal. Oyuncular TaleWorlds'ün yavaşlığından ve her yamada kırılan modlardan, çökme ve kayıt kayıplarından yakınıyor. Bunlara yaklaşık 80 saatten sonra boşalan geç oyun, sığ diplomasi ve kişiliksiz lordlar ekleniyor. Bu boşlukların çoğunu bakımı 2026'da da süren dev modlar dolduruyor: Diplomacy, Improved Garrisons, RBM ve Better Time. Bu modlarla yarışmak yerine onlarla uyumlu kalmak gerekiyor. Asıl açık alan hile ve editör tarafında:

- En çok indirilen hile modu **Temmuz 2023'ten beri güncellenmiyor**.
- Yerine geçen çatal (fork) sürümler v1.4.8'de çökme raporları alıyor.
- Bakımlı ve popüler bir ışınlanma modu yok; en büyüğünün yalnızca **70 beğenisi** var.
- **Parti adını değiştiren tek bir mod bile bulunamadı**.

Oyunun kendisi de sanıldığından fazlasını sunuyor: yaklaşık 80 kampanya konsol hilesi, gamepad'e gizlenmiş 15 maddelik bir hile menüsü, kendi klanını, klan üyelerini ve krallığını adlandırma. Ama yerleşime ışınlanma, yerleşim ve parti adlandırma, başka kahramanların yaşını ayarlama gibi işler yok. Haritada tıklayarak ışınlanma ise yalnızca kaydı kalıcı olarak işaretleyen motor hile modunda çalışıyor. Kurulu v1.4.8 dosyalarının incelenmesi, ilk iki özelliğin (**ışınlanma ve yeniden adlandırma**) **Harmony gerektirmeden, oyunun kendi kayıt alanlarıyla** yapılabildiğini gösteriyor. Mod sonradan kaldırılsa bile kayıt bozulmuyor. Önerilen plan şöyle:

1. Bu iki özelliği ortak, aranabilir bir "seçici" üzerine kurmak.
2. Aynı altyapıyla kahraman, ilişki, yerleşim, eşya ve zaman editörlerini eklemek.
3. Diplomacy tarzı sistemlerden, garnizon otomasyonundan, savaş yapay zekâsından ve v1.5 betasının getirdiği özelliklerden uzak durmak.

Mod resmî olmayan bir modül olduğu için, yüklü olduğu kampanyada başarımlar zaten kapanıyor. Mod bunu hiçbir koşulda gizlememeli ya da atlatmamalı.

## Oyuncular oyunu seviyor, ama kararlılık ve derinlik şikâyetleri 2026'da da sürüyor

Rakamlar bir çelişkiyi gösteriyor. Ana oyun Steam'de "Çok Olumlu":

- Son 30 günde **1.608 incelemenin %89'u** olumlu.
- Tüm İngilizce incelemelerde **98.954 incelemenin %87'si** olumlu ([Steam mağazası][s-store]).

Buna karşılık son bir yılın en "faydalı" bulunan olumsuz incelemelerinde (okunabilen yaklaşık 50 inceleme) en çok oy alan kategori **geliştirici ihmali ve "asıl işi modderlar yapıyor" algısı: 26 inceleme, toplam 3.108 oy**. Ardından gelen kategoriler (inceleme sayısı / toplam oy):

- hatalar ve çökmeler: 15 / 1.214
- sıkıcı geç oyun: 5 / 1.112
- diplomasi ve yapay zekâ krallıklar: 14 / 968
- yamalarla kırılan modlar: 14 / 884
- kişiliksiz lordlar: 9 / 878

Kaynak: [Steam inceleme API'si, en faydalı olumsuzlar, 365 gün][s-neg365]. Bu sayılar, bir özetleme modelinin kesik veriler üzerinde yaptığı yaklaşık bir sayım; istatistik değil, yön göstergesi. Yine de listenin tepesindeki tek inceleme (705 oy) tabloyu özetliyor: birkaç kale ve şehir aldıktan sonra tek oyunculu kampanya sıkıcılaşıyor.

v1.4.8 dönemine denk gelen son 100 olumsuz incelemede (12 Eylül–17 Ekim 2026) tablo daha da teknik. Yaklaşık **30'u çökme, hata, açılmama veya kayıt sorunlarından**, yaklaşık 18'i mod gerekliliğinden ya da güncellemelerle bozulan modlardan söz ediyor. Geç oyun (~11), içerik eksikliği (~10) ve zorunlu gizlilik görevleri (~9) arkadan geliyor ([Steam inceleme API'si, son olumsuzlar][s-negrec]). Steam forumunun en hareketli başlığı, 652 yanıtlı ve Ekim 2026'da hâlâ aktif olan sabitlenmiş "Launcher Issues" konusu ([Steam tartışmaları][s-disc]). v1.4.6 ve v1.4.7 yama notları bile bir uyarı taşıyor: ana oyun ile War Sails ikisi birden güncellenmeden oyunu açmak çökmeye yol açıyor ([GameDropDaily][gdd]).

Derinlik şikâyetleri somut mekaniklere dayanıyor:

- Oyuncular 80 saati geçtikten sonra döngünün (savaş, kuşatma, asker topla) yenilenmediğini yazıyor.
- Yenilen krallıklar ordularını bir anda yeniden kuruyor.
- Yapay zekâ krallıklar hiç teslim olmuyor. Haziran 2026'da 295 oy alan bir inceleme bunu "The AI will NEVER surrender to you" diye özetliyor ([Steam inceleme API'si][s-neg365]).
- Aralık 2025'te 198 oy alan bir inceleme, kendi krallığını kurmayı neredeyse kesin bir yenilgiye benzetiyor.
- Ekim 2026 şikâyetlerine göre vasal lordlar ordu kuramıyor, savunma yapamıyor ve en zayıf krallığa geçiyor.

2026 tarihli ayrıntılı bir Steam başlığı diplomasideki sorunları sıralıyor ([Steam "My Honest Review"][s-honest]):

- İttifakların ve ticaret anlaşmalarının süresi dolunca savaş yeniden başlıyor.
- Müttefikler koordine olmuyor ve kuşatmaları kırmaya gelmiyor.
- Yapay zekâ politikaları nadiren benimsiyor; diplomasi stratejik olmaktan çok sembolik kalıyor.

Arayüz de yardım etmiyor. Vanilla diplomasi sekmesi yalnızca oyuncunun kendi savaşlarını düz satırlar halinde gösteriyor; ittifaklar ansiklopedi sayfalarına dağılmış durumda, ticaret anlaşmaları içinse hiç arayüz yok (v1.3.15, Temmuz 2026) ([GitHub mod tasarım belgesi][gh-diplo]).

| Alan | Oyuncuların somut şikâyeti | TaleWorlds'ün yanıtı (sürüm) | Ekim 2026 durumu |
|---|---|---|---|
| Kararlılık ve modlar | Çökme, kayıt kaybı, her yamada kırılan modlar | Sürüm eşleşme uyarıları; v1.4'te açılışta mod çakışma uyarısı ([Steam: v1.4.0][an-140]) | Sürüyor; en gürültülü şikâyet |
| Geç oyun | 80 saatten sonra tekrar, bitmeyen savaşlar | v1.3'te "savaş ilerleme puanı"; v1.4'te uzun ve durgun savaşlar azaltıldı ([Steam: v1.3.4][an-134]) | Sürüyor; yeni geç oyun içeriği yok |
| Diplomasi | Rastgele savaşlar, teslim olmayan yapay zekâ, özet ekranı yok | v1.3'te ittifak, ticaret anlaşması, parley; v1.4'te müttefike saldırı cezaları; v1.5β'da kan davası ([Steam: v1.5.0 beta][an-150]) | Kısmen; teslim, saldırmazlık paktı ve elçi yok |
| Lordlar, yoldaşlar, görevler | Kişiliksiz karakterler, az diyalog | v1.3'te ~100 rastgele olay; v1.5β'da kişilik özelliği etkileri | Sürüyor |
| Oyuncu krallığı ve vasallar | Kuruluşta savaş yağmuru, beceriksiz vasallar | v1.5β'da klan partisi davranış anahtarları ([CalradiaWar][cw150]) | Kısmen, beta |
| Ekonomi ve yerleşimler | Mikro yönetim, işe yaramayan takas | v1.3'te kervan kontrolü, proje yenilemesi, devriyeler | Kısmen |
| Karakter gelişimi | Yavaş seviye, öğrenme sınırı | v1.3'te Fast Mode; v1.5β'da pasif XP | Kısmen |
| Kuşatma ve savaş yapay zekâsı | Merdivene çıkmayan, boş duran askerler ([Steam son olumsuzlar][s-negrec]) | v1.2'de itilebilir merdiven; v1.3'te bayılınca başka askeri yönetme | Sürüyor |
| Zorunlu gizlilik (v1.3 ile yeni) | Fark edilince görevi kaybetmek | v1.3.13'te gündüz baskın seçeneği geri geldi ([Steam: v1.3.13][an-1313]); v1.4'te asker gönderme başarı şansı | Kısmen |
| Arayüz ve QoL | Çok tık, yanlış "son görülme" konumu | v1.3.13'te envanter araması, parti ekranı kısayolları; v1.4'te çeşitli düzeltmeler | Kısmen |

Tablonun bir mod için anlamı şu: TaleWorlds'ün 2025–26'daki düzeltmelerinin çoğu, oyuncunun doğrudan göremediği yapay zekâ puanlama ayarları. TheGamer de ordu yapay zekâsı yenilemesinin genelde iyi karşılandığını, ama ittifak değişikliklerinin topluluğun beklediği büyük diplomasi elden geçirmesi olmadığını yazıyor ([TheGamer][thegamer]). Kalıcı şikâyetler ise **eksik oyuncu eylemleriyle** ilgili: teslim olmayı talep etmek, elçi göndermek, tımar vermek ya da geri almak, vasala emir vermek, diplomatik ağı tek ekranda görmek. Bir QoL + hile modu geç oyunun derinliğini tek başına çözemez. Ama iki şeyi doğrudan yapabilir: oyuncuya dünyayı istediği gibi biçimlendirme gücü vermek (ışınlanma, adlandırma, editörler) ve en gürültülü şikâyete, yani yamalarla kırılan modlara, dayanıklı bir tasarımla yanıt vermek.

### War Sails en çok fiyatı ve kampanyadan kopukluğu yüzünden eleştiriliyor

26 Kasım 2025'te çıkan War Sails ek paketi bir yıl sonra İngilizce incelemelerde **"Karışık" (2.524 incelemenin %54'ü olumlu)**, son 30 günde ise **"Çoğunlukla Olumsuz" (87 incelemenin %32'si olumlu)** ([Steam mağazası, War Sails][s-ws-store]). En faydalı bulunan olumsuz incelemelerde öne çıkan kategoriler (inceleme sayısı / toplam oy):

- fiyat/değer: 9 / 2.607
- içerik azlığı: 6 / 2.363
- deniz oyununun kampanyaya bağlanmaması: 6 / 1.959
- hatalar: 5 / 1.895
- birbirinin aynısı deniz savaşları: 4 / 1.598

Kaynak: [Steam inceleme API'si, War Sails][s-ws-neg]. TheSixthAxis 5 puan verip savaşların hep aynı kalıbı izlediğini yazdı ([TheSixthAxis][tsa]). JumpDashRoll ise kara ordusu daha ucuz olduğu için filoların geniş oyunda neredeyse anlamsız kaldığını belirtti ([JumpDashRoll][jdr]). TaleWorlds Nisan 2026'da sorunu kendisi kabul etti: lordlar yeterince denize açılmıyordu ve soylu filolarını bulmak zordu ([Steam: v1.4.0 geliştirici güncellemesi][an-140]). Şimdiye kadar gelen düzeltmeler:

- v1.4.5: denizden köy yağmaları ([Steam: v1.4.5][an-145]).
- v1.5 beta: limanda demirleme, feribotlar, kıyı kalesi limanları ([Steam: v1.5.0 beta][an-150]).

Mod açısından sonuç net: deniz sistemleri hâlâ hareket halinde. Deniz odaklı büyük işler v1.5 kararlı sürüme kadar beklemeli. Şimdilik yalnızca ucuz sarmalayıcılar mantıklı; örneğin oyunun konsolda zaten var olan gemi verme komutu.

## Büyük modlar boşlukları dolduruyor, hile ve editör alanı ise parçalı ve kırılgan

Nexus'ta listelenen 9.198 Bannerlord modunun en üstünü, neredeyse her kod modunun dayandığı kütüphaneler tutuyor: MCM, ButterLib, UIExtenderEx. Kütüphane olmayan ilk sıralar şöyle:

- **Improved Garrisons**: 31,1 bin beğeni, 2,2 milyon indirme
- **Diplomacy**: 28,8 bin beğeni, 2,1 milyon indirme
- **Detailed Character Creation**: 22,4 bin beğeni, 1,4 milyon indirme
- **Realistic Battle Mod**: 21,7 bin beğeni, 2,3 milyon indirme

Kaynak: [Nexus, beğeniye göre sıralı liste][nx-top]. Steam Atölyesi'nde aynı tablo tekrar ediyor: Harmony, UIExtenderEx ve ButterLib'in hemen ardından **Diplomacy 4., Improved Garrisons 5., RBM 7., Better Time 8.** sırada ([Steam Atölyesi, en çok abone][ws-top]). Son 12 ayda yüklenen modların zirvesinde ise yapay zekâyla diyalog üreten AI Influence var: 790 beğeni, 92 bin indirme ([Nexus, son 12 ay][nx-year]). Bu tablo ana mesajı doğruluyor: diplomasi derinliği ve tımar yönetimi en kalıcı talep, ve bu talebi karşılayan modlar 2026'da güncel.

| Mod | Doldurduğu boşluk | Ekim 2026 durumu | CheatMenu için anlamı |
|---|---|---|---|
| Diplomacy | Elçiler, saldırmazlık paktı, tımar bağışlama, savaş yorgunluğu, iç savaşlar | v1.5.4, 9 Eki 2026; v1.4.7'ye uyarlandı, War Sails desteği içinde ([Nexus 832][nx-832]) | Kopyalamayın, uyumlu kalın |
| Improved Garrisons | Tımarları uzaktan yönetme, otomatik asker toplama ve terfi | v4.3.0.3, 30 Ağu 2026 ([Nexus 688][nx-688]) | Kopyalamayın |
| Realistic Battle Mod | Savaş yapay zekâsı ve gerçekçilik | v4.5.4, 6 Eki 2026 ([Nexus 791][nx-791]) | Kapsam dışı; savaş hilelerini RBM ile birlikte test edin |
| Better Time | 4x/8x/16x hızlı ileri sarma | v1.1.3, 14 Eki 2026; yeni bakımcıya geçti ([Nexus 2849][nx-2849]) | Basit bir hız ayarı eklenebilir |
| Detailed Character Creation | Yaş, görünüm, NPC adları | v1.8.1, Oca 2026; v1.3.13'te kaldı, bakımcı ara verdi ([Nexus 118][nx-118]) | Yaş ve görünüm editörü için boşluk |
| Party AI Controls → Reforged | Klan partilerine emir ve kısıtlama | Orijinali 28 Kas 2025'te durdu; Reforged Eyl 2026 ([Nexus 6218][nx-6218]) | v1.5β yerli anahtar getiriyor; yapmayın |
| Governors Handle Issues → Updated | Valilerin köy ve şehir görevlerini çözmesi | Reloaded 28 Kas 2025, Updated 1 Oca 2026 ([Nexus 4526][nx-4526]) | Kapsam dışı |
| Party Screen Enhancements | Toplu terfi, toplu esir toplama | Ekim 2022'den beri ölü ([Nexus 492][nx-492]) | Bir kısmı yerli oldu; kalanı oyunda doğrulanmalı |
| Bannerlord Tweaks / Kaoses Tweaks | Onlarca denge ve grind ayarı tek pakette | 2020 ve Ocak 2023'te öldü ([Nexus 49][nx-49]) | Bakımlı halefi yok: fırsat |
| Surrender Tweaks | Lordların ve yerleşimlerin teslim olması | v1.8.0, Kas 2024, 1.3 öncesi ([Nexus 2797][nx-2797]) | Talep var, teknik analiz yapılmadı |

Topluluğun belirgin bir "devam ettirme" kültürü var. Büyük bir yazar bıraktığında, birkaç hafta içinde adında "Updated", "Reloaded", "Reforged" ya da "1.3.X" geçen bir yeniden yükleme ortaya çıkıyor. Bu da özelliklere duyulan talebin modların kendisinden daha kalıcı olduğunu gösteriyor. War Sails (v1.3) toplu bir kırılma anı oldu: popüler bir yazarın mod paketinin tamamı 28 Kasım 2025'teki son sürümde kaldı. Bağımlılık katmanı bile yamaya muhtaç kaldı; resmî olmayan bir "Harmony Updated v1.4.6" yeniden derlemesi geçen yıl 197 beğeni topladı ([Nexus, son 12 ay][nx-year]). En çarpıcı belirti ise bir rehber: 11 Ekim 2026'da güncellenen "Vanilla 1.4.8 Plus" rehberi, 160'tan fazla modu katı bir yükleme sırasıyla ve elle XML düzeltmeleriyle bir arada tutmayı anlatıyor ([Nexus 11354][nx-11354]). Bu, bakımlı tek bir "vanilla artı QoL" modülünün eksikliğinin kendisi.

### Hile ve editör modlarının en büyükleri ya terk edilmiş ya da 1.4.8'de sendeliyor

| Mod | Ne sunuyor | Son durum | Not |
|---|---|---|---|
| Bannerlord Cheats (NicoWaves) | 18 grupta 116 MCM ayarı, ekranlara özel kısayollar | 12 Tem 2023'ten beri güncellenmiyor; 11.528 beğeni, Steam'de 92.064 abone ([Nexus 1839][nx-1839], [Steam][st-cheats]) | MIT lisanslı kaynak; güvenle referans alınabilir ([GitHub][gh-cheats]) |
| Bannerlord Cheats Reload | Aynı set, her oyun sürümü için yeniden derleme | v1.3.15.0, 1 Mar 2026; 1.4.8'de çökme, bozuk eşya ekleme ve "module mismatch" döngüsü raporları ([Nexus 6446][nx-6446], [gönderiler][nx-6446p]) | Kullanıcılar güncelleme bekliyor |
| UFO's Cheat Mods Bundle | Reload'un yanında 7 başka modun parçaları ve War Sails hileleri | 1.4.8 için derlenmiş, 19 Ağu 2026; 29.111 abone ([Steam][st-ufo], [GitHub][gh-ufo]) | Başka modların dekompile edilmiş kodu: lisans riski |
| BannerWand Cheats | MCM içinde hile eğitmeni; her hileyi oyuncuya, klana veya belirli krallıklara uygulama | 24 Ağu 2026; 7.064 tekil indirme ([Nexus 9151][nx-9151]) | Hedefleme modeli kopyalanmaya değer |
| Bannerlord.GameMaster (BLGM) | 115'ten fazla konsol komutu; kahraman, klan, krallık ve yerleşim adlandırma dahil | 17 Eki 2026; 1.4.x ve 1.5.5 beta ([Nexus 9448][nx-9448]) | Lisans belirtilmemiş |
| Commander | BLGM üstüne grafik editör, NPC partilerine emir, kahramanın gerçek konumu | 17 Eki 2026 ([Nexus 10127][nx-10127]) | En yakın rakip; hile ayarları, ışınlanma ve zaman kontrolü yok |
| Character Reload Fix | Ansiklopediye butonlar: ad, görünüm, ilişki, sahiplik, binalar | e1.4.5.0, 16 May 2026; 1.4.8'de ilişki yöneticisinin çökmesi ve yaş kaydırıcısının çalışmaması raporlanıyor ([Nexus 3700][nx-3700], [gönderiler][nx-3700p]) | Kaynağı açık mı, doğrulanmadı |
| Işınlanma modları | Tıkla-ışınlan | En büyüğü 70 beğeni; "Teleport" (10005) şehirlere doğru ışınlamıyor ([Nexus arama][nx-teleport], [Nexus 10005][nx-10005]) | Güçlü bir sahibi yok |
| Adlandırma modları | Yerleşim ve yoldaş adları | Settlement Rename 427 beğeni (2020); Rename Any Settlement 315 (2022); Oh The Settlements You'll Rename yalnızca sahip olunan yerleşimler için, Ağu 2026 ([Nexus arama][nx-rename], [Nexus 12507][nx-12507]) | Parti adlandıran mod yok |
| Faster Time / No Aging Extended | 2x–20x hız / grup bazlı yaş sınırı | 1.4.8, 27 Ağu 2026 / 1.3.15, 9 Ağu 2026 ([Nexus 11712][nx-11712], [Nexus 12601][nx-12601]) | Tek amaçlı, küçük |

Bu alan tek bir döngüyle tanımlanıyor: yazar bırakıyor, bir çatal her yeni oyun sürümü için yamayı sürdürüyor, kullanıcılar her TaleWorlds yamasından sonra "lütfen güncelle" başlıklarında bekliyor. Bugün tipik bir oyuncu iki üç mod kullanmak zorunda: bir hile modu, bir editör, bir zaman ya da ışınlanma modu. Mevcut modlar üç farklı katmana dağılmış durumda:

1. **Kural çarpanları**: Cheats ve BannerWand kaydırıcıları.
2. **Doğrudan nesne editörleri**: Character Reload, BLGM, Commander.
3. **Harita ve zaman araçları**: ışınlanma, hız, ara noktalar.

Hiçbir mod bu üçünü birlikte kapsamıyor. Editörlerde en yakın rakip Commander, ama onda da hile kaydırıcıları, ışınlanma ve zaman kontrolü yok ([Nexus 10127][nx-10127]). Işınlanma ve adlandırma özellikle sahipsiz:

- Işınlanma modlarının çoğu var olma gerekçesini vanilla hile modunun yan etkisine dayandırıyor; hile modu açıkken eşya ve asker bölümleri yavaşlıyor ([Nexus arama][nx-teleport]).
- Yerleşim adlandıran güncel mod, dünya haritasındaki isim etiketini güncellemek için kaydedip yeniden yüklemeyi istiyor ([Nexus 12507][nx-12507]).
- Krallık adlandırma yalnızca BLGM'nin konsol komutunda var.

Mevcut modlar arayüz konusunda da yol gösteriyor. Popüler modlar kalıcı kuralları MCM sayfalarına, tek seferlik düzenlemeleri ise oyunun kendi ekranlarına eklenen butonlara koyuyor. BLGM'nin yazarı bile kullanıcılarına konsol yerine Commander'ın grafik arayüzünü öneriyor ([Nexus 9448][nx-9448]). Sorun raporları da birer tasarım dersi:

- Cheats Reload, RBM'nin sağlığı etkileyen hileleri bozduğunu ve zaman zaman çökmeye yol açtığını belirtiyor ([Nexus 6446 gönderiler][nx-6446p]).
- Faster Time, bir oyun güncellemesi hedefini bozarsa çökmek yerine kırmızı bir uyarıyla vanilla davranışa dönüyor ([Nexus 11712][nx-11712]).
- BannerWand, tehlikeli ayarları kendiliğinden sıfırlayan bir mekanizma ekledi ([Nexus 9151][nx-9151]).
- Character Reload'un yazarı, ilişki yöneticisini kullanmadan önce kaydetmeyi öğütlüyor ([Nexus 3700][nx-3700]).

Lisans tarafında da durum net. Bannerlord Cheats MIT lisanslı, yani kodundan güvenle yararlanılabilir. UFO paketi ise başka yazarların dekompile edilmiş kodundan oluşuyor; bu yüzden referans olarak bile uzak durulmalı ([GitHub UFO-Bannerlord][gh-ufo]).

## Oyun sanıldığından fazlasını yapıyor; açık kalan yer ışınlanma ve adlandırma

Ekim 2022'deki tam sürümden bu yana TaleWorlds, eskiden mod alanı sayılan pek çok şeyi oyuna ekledi:

- **v1.1 (Mart 2023):** parti ekranında sıralama, emeklilik ve varisle devam, sokak çeteleri, yağma yenilemesi. Kampanya hızı cheati de en çok 15'le sınırlanıp yeniden açıldı ([Steam: v1.1.0][an-110]).
- **v1.2 (Aralık 2023):** hava durumu, formasyon hedefleme, ambarlar, tüm tımarlarını kaybeden krallığın yok olması. Konsollar için 15 maddelik bir hile menüsü de geldi ([Steam: Geliştirme Güncellemesi #12][an-dev12]).
- **v1.3 (Kasım 2025, War Sails ile birlikte):** ittifaklar, ticaret anlaşmaları, savaşa çağrı, gizlilik ve kılık değiştirme, kaçak asker çeteleri, devriyeler, yaklaşık 100 rastgele olay, parley, kervan kontrolü, Fast Mode ve kahraman dışa/içe aktarma hileleri ([Steam: v1.3.0 beta][an-130], [Steam: v1.3.4][an-134]).
- **v1.4 (Mayıs–Ağustos 2026):** diplomasi ve ordu yapay zekâsı yenilemesi, klan üyesi başına iki parti rolü, oylamada çekimser kalma, kayıtlarda başarım işareti ([Steam: v1.4.0][an-140]).

Canlı sürüm **10 Ağustos 2026'da çıkan v1.4.8**, ve bu bilgisayardaki kurulumla aynı ([Steam: v1.4.8][an-148], [yerel: Version.xml][l-ver]). v1.5, 19 Ağustos 2026'dan beri betada ([Steam: v1.5.0 beta][an-150]).

| Özellik | Oyunda nerede | Geldiği sürüm | Mod için karar |
|---|---|---|---|
| Haritada Ctrl+Sol tık ile ışınlanma | Motor hile modu (engine_config.txt) | Eski; v1.4.8 kodunda doğrulandı ([yerel: SandBox.View.dll][l-sbv]) | Tamamla: hile modu istemeyen, yerleşim listeli sürüm |
| Kendi klanını, klan üyelerini ve krallığını adlandırma | Klan ve krallık ekranları | v1.4.8 kodunda mevcut ([yerel: ViewModelCollection.dll][l-vmc]) | Tamamla: yerleşim, parti ve yapay zekâya ait varlıklar |
| Kahraman dışa/içe aktarma (ad, görünüm, yetenek, altın) | Konsol | v1.3 ([Steam: v1.3.0 beta][an-130]) | Daha rahat bir editörle aşın |
| 15 maddelik Oyun İçi Hile Menüsü | Kampanya Seçenekleri, yalnızca gamepad etkinken | v1.2 ([yerel: SandBox.dll][l-sb]) | CheatMenu zaten kapsıyor; yeniden yazmayın |
| Yaklaşık 80 kampanya konsol hilesi | Geliştirici konsolu (hile modu ister) | v1.3'te düzenlendi ([yerel: CampaignSystem.dll][l-cs]) | Aynı eylemleri arayüzle sunun |
| Kampanya hızı çarpanı (en çok 15) | Konsol | v1.1 | Hile modu olmadan tamamla |
| Doğum ve yaşlanmayı kapatma; Fast Mode | Resmî isteğe bağlı modüller | v1.0 / v1.3; v1.5β'da başlangıç seçeneğine taşınıyor | Yapmayın |
| Parti ekranında sıralama; envanter araması; terfi/toplama kısayolları | Parti ve envanter ekranları | v1.1 / v1.3.13 ([Steam: v1.3.13][an-1313]) | Yapmayın |
| İttifak, ticaret anlaşması, savaşa çağrı, parley | Diplomasi | v1.3 | Yapmayın |
| Kervan yönlendirme, klan üyesi ekipmanı, destekçi eşraf | Diyalog | v1.3 | Yapmayın |
| Klan üyesine iki parti rolü; oylamada çekimser kalma | Klan ve krallık ekranları | v1.4 | Yapmayın |
| Haydut inine asker gönderme, gündüz baskını | Harita menüsü | v1.3 / v1.3.13 / v1.4 | Gizlilik atlama düşük öncelikli |
| Kral, vasal veya tüccar olarak başlama; senaryolar; tohum | Yeni oyun (Gelişmiş Başlangıç Seçenekleri) | v1.5β | Yapmayın |
| Klan partisi davranış anahtarları; pasif XP; kişilik etkileri | Klan ekranı | v1.5β ([CalradiaWar][cw150]) | Yapmayın |

Konsol, çoğu oyuncunun bildiğinden geniş. v1.4.8 dosyalarından **155 konsol fonksiyonu** çıkarıldı. Bunların yaklaşık 80'i kampanya hilesi ([yerel: CampaignSystem.dll][l-cs]) ve şunları kapsıyor:

- altın, etki ve şöhret
- yetenek, nitelik ve odak puanı
- herhangi iki kahraman arasındaki ilişki
- asker ve eşya ekleme
- yerleşim sahipliği ve refah, sadakat, güvenlik, yiyecek, milis ve hane değerleri
- bina seviyesi
- krallığa katılma ve krallık kurma
- savaş, barış ve ticaret anlaşması
- evlilik ve çocuk sahibi olma
- kişilik özelliği ve kültür
- War Sails için gemi ekleme

Eksik olanlar ise mod için doğrudan bir hedef listesi:

- ana partiyi ışınlama
- herhangi bir şeyi yeniden adlandırma
- başka bir kahramanın yaşını ayarlama (yalnızca oyuncuya yaş ekleniyor)
- krallık ya da klan yok etme
- ittifak kurma
- görev tamamlama (yalnızca iptal var)
- suç puanı
- klan liderini değiştirme

İnternette dolaşan 2020 tarihli hile listelerine güvenmemek gerekiyor. `campaign.teleport_hero_to_settlement` ya da `campaign.start_world_war` gibi komutlar v1.4.8'de artık yok ([gameplay.tips, 2020][gtips]). TaleWorlds v1.3'te hileleri "düzenlediğini ve gereksiz girdileri kaldırdığını" duyurmuştu ([Steam: v1.3.0 beta][an-130]).

### Hile modu, konsol ve başarımlar: üç katman, tek kalıcı işaret

Oyunda hileler üç katmanda çalışıyor:

1. **Motor hile modu**: Belgelerim klasöründeki `engine_config.txt` dosyasında `cheat_mode = 1` yapılarak açılıyor. Konsol hilelerini, haritada Ctrl+Sol tık ışınlanmasını, Ctrl+Alt+T ile tüm partileri görmeyi ve savaştaki Ctrl+F4 gibi kısayolları etkinleştiriyor ([yerel: SandBox.View.dll][l-sbv], [gameplay.tips][gtips]). Araştırma sırasında bu bilgisayardaki dosyada değer 0 görüldü.
2. **Oyun İçi Hile Menüsü**: +1000 altın, +100 etki, +100 şöhret, 5 asker, 10 tahıl, 10 savaş atı, partiyi iyileştir, harita sisini kaldır, bulunduğun şehirdeki tüm binaları bitir gibi maddeler içeriyor. Kampanya Seçenekleri'nde ancak bir gamepad etkinse görünüyor ([yerel: SandBox.dll][l-sb]).
3. **Başarım kontrolü**: Oyun bir kampanyayı üç gerekçeyle başarıma kapatıyor: hile kullanımı, resmî olmayan modüller ve sürüm düşürme. Hile modu açıkken kampanya çalıştığı anda, tek bir hile yazılmasa bile, kayda kalıcı bir "hile kullanıldı" işareti düşüyor. v1.4'ten beri kayıtlar başarımların açık mı kapalı mı olduğunu da gösteriyor ([yerel: SandBox.dll][l-sb], [Steam: v1.4.0][an-140]).

Bu yapı CheatMenu'nün konumunu belirliyor. CheatMenu resmî olmayan bir modül olduğu için, yüklü olduğu kampanyada başarımlar zaten kapanıyor; mod hile modunu açmaya ihtiyaç duymuyor ve başarım kontrolünü atlatmıyor. Mevcut "temiz kayıt" koruması yalnızca uyarı veriyor: başarımları hâlâ açık bir kayıt yüklendiğinde, kaydetmeden çıkıp modu kapatmayı öneriyor ([yerel: CleanSaveGuard.cs][l-csg]). Steam'in en çok abone olunan 30 modu arasında başarımları yeniden açan bir mod bulunması, bu konuda gerçek bir talep olduğunu gösteriyor ([Steam Atölyesi][ws-top]). Ama bu mod o yola bilerek girmemeli.

Yerli özelliklerle ilgili iki ayrıntı, yol haritasının ilk iki maddesinin gerekçesini oluşturuyor. Birincisi, yerli ışınlanma var ama yalnızca hile modunda çalışıyor, yerleşim seçmeye izin vermiyor ve kaydı kalıcı olarak işaretliyor. Bir mod yazarına göre hile modu ayrıca eşya ve asker ekranlarını yavaşlatıyor ([Nexus arama][nx-teleport]). İkincisi, yerli adlandırma da var ama yalnızca oyuncunun kendi klanı, klan üyeleri ve (hükümdarsa) krallığı için. Yerleşimler, partiler ve yapay zekâya ait kahraman, klan ve krallıklar dışarıda kalıyor. Bu ayrıntı, oyunun klan ve krallık ekranı kodunda bulunan adlandırma işlevlerine dayanıyor ([yerel: ViewModelCollection.dll][l-vmc]). Oyun içinde ayrıca denenmedi.

Son bir uyarı v1.5 için. 18 Ekim 2026 itibarıyla v1.5 hâlâ beta, ve kararlı sürüme geçtiğinde bazı alanları gereksiz kılacak:

- başlangıç seçenekleri
- klan partisi anahtarları
- kişilik etkileri
- tüm kahramanlara günlük pasif XP
- harita hızına +%15
- farklı kültürden vali atamanın sadakat cezasının kaldırılması

Aynı beta klan sekmesindeki finans ekranını kaldırıyor; bu da ileride yeni bir QoL boşluğu açabilir ([Steam: v1.5.0 beta][an-150]). Rakipler şimdiden hazırlanıyor: BLGM, Commander ve yeni bir ışınlanma modu 1.5.5 beta derlemeleri yayımlıyor ([Nexus 9448][nx-9448], [Nexus arama][nx-teleport]).

## İlk iki özellik v1.4.8'de kolay ve kayıt dostu

Bu bölümdeki değerlendirme, bu bilgisayarda kurulu v1.4.8 (War Sails v1.2.8) oyun dosyalarının incelenmesine dayanıyor: DLL'lerin üye listeleri ve bayt kodları okundu. Hiçbir şey oyun içinde çalıştırılarak test edilmedi ([yerel: Version.xml][l-ver]). Ana bulgu sade: incelenen sekiz alanın tamamı oyunun herkese açık fonksiyonlarıyla, **Harmony olmadan** yapılabiliyor. Bu alanlar ışınlanma, adlandırma, kahraman düzenleme, ilişkiler, yerleşimler, klan/krallık, zaman ve eşyalar. Harmony, oyunun kendi kodunu çalışırken değiştiren ve yamalarla en sık kırılan ek kütüphane. Bugünkü CheatMenu ona bağımlı değil. En gürültülü şikâyet "modlar her yamada kırılıyor" olduğuna göre, bu başlı başına bir rekabet avantajı. Daha da önemlisi, bu özelliklerin hepsi değişiklikleri oyunun zaten kaydettiği alanlara yazıyor. Mod kayda kendi veri türünü eklemediği için, kaldırıldığında kayıt açılmaya devam ediyor. Resmî modlama belgeleri, özel veri türü kaydeden bir mod kaldırılınca kayıtların açılamayabileceği konusunda uyarıyor ([docs.bannerlordmodding.com][docs-save]).

Işınlanmada oyunun kendisi yol gösteriyor. Hile modundaki Ctrl+tık ışınlanması v1.4.8'de şunları yapıyor ([yerel: SandBox.View.dll][l-sbv]):

- partiyi tıklanan noktaya koyuyor ve durduruyor;
- hedef karaya ya da denize düşüyorsa War Sails'in kara/deniz durumunu çeviriyor;
- ordudaki diğer partileri de taşıyor;
- harita görünürlüğünü güncelliyor.

War Sails'in hikâye görevi de oyuncuyu üsse geri gönderirken gemisi varsa limana, yoksa şehir kapısına koyuyor ([yerel: NavalDLC.dll][l-naval]). Mod bu adımları birebir izleyebilir. **Yerleşime ışınlanma kolay**, çünkü her yerleşimin kapı konumu ve (varsa) liman konumu hazır duruyor. Bu, "Teleport" modunun şehirlere doğru ışınlayamama sorununu doğrudan çözüyor. **İmlecin altına ışınlanma orta zorlukta.** Ayrıca ele alınması gerekenler: gemisi olmayan bir partinin denize düşmesi, sal durumu, devam eden karşılaşmalar ve ordular. Oyunun kendi korumaları modda da kopyalanmalı: sal durumundayken ışınlanmayı reddetmek ve geçersiz bir zemine hiç yazmamak. Geçersiz bir konum doğrudan kayda yazılır. Kısayol seçimi de önemli. Düz Ctrl+tık, vanilla hile moduna ait. Alt basılı tutup tıklamak, v1.3 ile gelen ve uzaktan lordla konuşmayı sağlayan Parley'nin hareketi ([Steam: v1.3.0 beta][an-130]). Bu yüzden varsayılan olarak Ctrl+Alt+Sol tık gibi bir kombinasyon daha güvenli.

Adlandırmada beş türün beşi de kalıcı:

- Kahraman, klan ve krallık adları, oyunun klan ve krallık ekranlarının kullandığı fonksiyonlarla değiştirilebiliyor ([yerel: ViewModelCollection.dll][l-vmc]).
- Yerleşim ve parti adları oyunun "özel ad" alanına yazılıyor, ve bu alan kaydediliyor ([yerel: CampaignSystem.dll][l-cs]).

Burada bir tuzak var: yerleşimin asıl adı kayda yazılmıyor, her yüklemede oyunun XML dosyasından yeniden okunuyor. O alanı doğrudan değiştiren bir mod, yüklemeden sonra adın geri döndüğünü görür. Doğru yol ise "orijinal ada dön" düğmesini de bedavaya getiriyor: özel adı boşaltmak asıl adı geri getiriyor. İki uyarı kalıyor. Birincisi, oyun bazı durumlarda parti adını kendisi yeniden yazıyor: parti dağılırken, bir kahraman parti liderine ışınlanırken ve bazı görev partilerinde. İkincisi, verilen adlar düz metin olduğu için oyunun dili değiştirildiğinde çevrilmiyor.

| Özellik | Zorluk | Harmony gerekir mi | Kayıt ve modu kaldırma | Ana risk |
|---|---|---|---|---|
| Yerleşime ışınlanma (kapı/liman) | Kolay | Hayır | Oyunun kendi alanları; güvenli | Şehirdeyken önce karşılaşmayı kapatmak |
| İmleç konumuna ışınlanma | Orta | Hayır | Güvenli | Kara/deniz geçişi, sal durumu, ordu, geçersiz zemin |
| Kahraman, klan, krallık adı | Kolay | Hayır | Kalıcı | Yok denecek kadar az |
| Yerleşim adı | Kolay | Hayır | Kalıcı ("özel ad" alanıyla) | Yanlış alan seçilirse yüklemede sıfırlanır |
| Parti adı | Kolay | Hayır | Kalıcı | Oyun bazı akışlarda adı yeniden yazar |
| Herhangi bir kahramanın yaşı | Kolay | Hayır | Kalıcı | Doğum/yaşlanma modülüyle etkileşimi denenmedi |
| Yetenek, nitelik, odak, kişilik özelliği | Kolay | Hayır | Kalıcı | Kişilik XP'si seviyeyi geri itebilir; düzeltme adımı gerekli |
| Perk ekleme / tek perk silme | Kolay / Orta | Hayır | Kalıcı | Tek perk silmek oyunun iç fonksiyonuna erişim ister |
| Ana kahramanın görünümü | Kolay | Hayır | Kalıcı | Haritadan açılınca davranışı denenmedi |
| Yoldaşın veya başka kahramanın görünümü | Orta-zor | İsteğe bağlı | Kalıcı | Oyunun ekranı sonucu yalnızca oyuncu için kaydediyor |
| İlişkiyi tam değere ayarlama | Kolay | Hayır | Kalıcı | Oyun bazı ilişkileri klan liderine yönlendirir |
| Yerleşim sahibi, istatistikleri, binaları bitirme | Kolay | Hayır | Kalıcı | Kuşatma veya yağma sırasında yapılmamalı |
| Klan kademesi, etki, krallığa katılma/ayrılma, savaş/barış | Kolay | Hayır | Kalıcı | Az |
| Krallık kurma | Orta | Hayır | Kalıcı | Vasal veya paralı askerken kenar durumlar denenmedi |
| Zaman hızı 4x üstü | Kolay | Hayır | Kaydedilmez | Her yüklemede yeniden uygulanmalı; ~16x üstü simülasyon kabalaşabilir |
| Aranabilir, ikonlu eşya ekleme | Kolay | Hayır | Kalıcı | Görev eşyaları filtrelenmeli |
| Sekmeli tek panel (Gauntlet) | Orta-yüksek | Hayır | Etkisiz | Arayüz API'si 1.3/1.4'te değişti; bakım yükü |
| MCM ayar sayfası | Kolay-orta | Evet (bağımlılık zinciri) | Etkisiz | Dört kütüphanelik bağımlılık |

Bu araştırma daha büyük sistemleri teknik olarak incelemedi:

- saldırmazlık paktı ve teslim olma gibi diplomasi katmanları
- garnizon otomasyonu
- kişi bazlı yaş kilidi
- zorunlu gizlilik görevlerini atlama
- XP veya parti boyutu gibi kural çarpanları

Bunların çoğu ya oyunun "model" sınıflarını zincirlemeyi gerektiriyor (CheatMenu'nün parti hızı için yaptığı gibi) ya da Harmony yamalarını. Ayrıca TaleWorlds'ün her büyük yamada yeniden yazdığı sistemlere dokunuyorlar. Yol haritasında bu yüzden sona konuldular.

Arayüzde mevcut açılır pencere yaklaşımını korumak mantıklı. v1.4.8'in çoklu seçim penceresi artık yerleşik bir arama kutusunu ve eşya ikonlarını destekliyor. Bu, binlerce kahraman, yerleşim veya eşya arasından seçim yapmayı mümkün kılıyor ve yamalarla neredeyse hiç kırılmıyor ([yerel: TaleWorlds.Core.dll][l-core]). Sekmeli, kaydırıcılı özel bir panel (Gauntlet) yapılabilir. Ama tam da 1.3/1.4'te değişen ve eski eğitimleri geçersiz kılan arayüz API'sine bağımlı ([yerel: Engine.GauntletUI.dll][l-eg]). MCM ise kalıcı ayarlar için topluluk standardı. Ancak Harmony, ButterLib ve UIExtenderEx ile birlikte dört parçalı bir bağımlılık zinciri getiriyor ([MCM sürümleri][mcm]).

## Yol haritası: önce ışınlanma ve adlandırma, sonra aynı altyapı üstünde editörler

Sıralama dört ölçüte göre yapıldı:

- **Talep:** şikâyetler ve mod popülerliği.
- **Boşluk:** oyunda yerli karşılığı ya da bakımlı bir mod karşılığı yok.
- **Uygulanabilirlik:** kolay, Harmony'siz ve kayıt dostu.
- **v1.5 riski:** beta özellikleri tarafından gereksiz kılınmamak.

İlk iki madde zaten belirlenmişti, ve bu dört ölçütün hepsinde birden en yüksek puanı alıyorlar; bu nadir bir örtüşme. Mimari açıdan en önemli karar ilk adımda veriliyor: kahraman, klan, krallık, yerleşim ve parti için **ortak bir aranabilir seçici**. Bu seçici, ışınlanma ve adlandırmanın ihtiyaç duyduğu parça olarak hemen yazılıyor ve ikinci fazdaki editörlerin neredeyse hepsini ucuzlatıyor.

| Sıra | Özellik | Neden şimdi | Zorluk |
|---|---|---|---|
| 1 | Işınlanma: yerleşime (kapı, içeri gir, liman), imlecin altına, bir kahramanın yanına; ortak seçiciyle birlikte | Yerli olanı yalnızca hile modunda; en büyük mod 70 beğeni ve şehirlerde hatalı | Kolay / Orta |
| 2 | Yeniden adlandırma: kahraman, klan, krallık, yerleşim, parti | Parti adlandıran mod yok; yerleşim modları küçük; yapay zekâ krallığını yalnızca BLGM konsolu adlandırıyor | Kolay |
| 3 | Kahraman editörü: yaş, yetenek, nitelik, odak, perk, kişilik; ana kahramanın görünümü | DCC 1,45 milyon indirmeye rağmen v1.3.13'te kaldı; Character Reload 1.4.8'de sorunlu; başka kahramanın yaşı konsolda yok | Kolay |
| 4 | İlişki editörü: herhangi bir kahramanla tam değer | Character Reload'un ilişki yöneticisi 1.4.5+ sürümünde çöküyor | Kolay |
| 5 | Yerleşim editörü: sahip, refah, sadakat, güvenlik, yiyecek, milis, hane, tüm projeleri bitir | Konsolda hile modu ister; yerli menü yalnızca bulunulan şehir için | Kolay |
| 6 | Aranabilir, ikonlu eşya ekleyici | Konsol eşya kimliği ister; Cheats Reload'da 1.4.8'de bozuk | Kolay |
| 7 | Zaman hızı (4x üstü) | Better Time Steam'de 8. sırada; konsol 15'le sınırlı ve hile modu ister | Kolay |
| 8 | Klan ve krallık araçları: katıl/ayrıl, savaş/barış, krallık kur, hükümdar ol, kademe, klan lideri değiştir, krallık yok et | Son iki madde konsolda yok | Kolay / Orta |
| 9 | Bilgi araçları: lordun gerçek konumu ve kamerayı oraya götürme, harita sisini kaldırma | "Son görülme yanlış" şikâyeti; Commander'da var | Kolay (çıkarım) |
| 10 | Kural çarpanları ve hedefli hileler: XP, öğrenme, parti boyutu; mevcut savaş hilelerine "kime uygulansın" seçeneği | Cheats mirası; BannerWand'ın hedefleme modeli | Orta (doğrulanmadı) |
| 11 | Yoldaş ve başka kahraman görünümü | DCC talebi büyük | Orta-zor |
| 12 | Kişiye özel yaş kilidi | No Aging Extended, BannerWand talebi | Doğrulanmadı |
| 13 | Zorunlu gizlilik görevlerini atlama seçeneği | v1.3 ile doğan yeni şikâyet | Doğrulanmadı |
| 14 | War Sails araçları: gemi ve figürhed verme | Konsolda var; UFO paketi kapsıyor | Kolay (çıkarım) |
| 15 | Tek panelli "editör merkezi" ve isteğe bağlı MCM ayarları | Özellik sayısı artınca açılır pencereler yetersiz kalır | Orta-yüksek |

### Faz 1: ışınlanma ve adlandırma (şimdi)

Kök menüye mevcut dört kategorinin (Para & Ekonomi, Karakter, Ordu / Parti, Savaş içi) yanına iki yeni kategori eklenir: **"Harita"** ve **"Adlandır"**.

**Harita** kategorisinde üç işlev bulunur:

- **Yerleşime ışınlan:** arama kutulu bir liste (şehir, kale, köy) açar. Kullanıcı kapıda durmayı ya da içeri girmeyi seçer. Liman seçeneği yalnızca yerleşimin limanı ve partinin gemisi varsa çıkar.
- **İmlecin olduğu yere ışınlan:** Ctrl+Alt+Sol tık gibi değiştirilebilir bir kısayolla çalışır.
- **Bir kahramanın yanına git:** önce kamerayı kahramana götürür, sonra isteğe bağlı olarak ışınlar.

Işınlanma şu durumlarda kapalı kalmalı: savaş, kuşatma, sal durumu, açık bir menü. Gemisi olmayan bir parti denize düşecekse en yakın karaya yerleştirilmeli. Bir ordu yönetiliyorsa ordunun tüm partileri birlikte taşınmalı. Bir düşman partisinin dibine ışınlanmanın hemen karşılaşma başlatıp başlatmadığı denenmedi; ilk testlerde bakılması gereken konu bu.

**Adlandır** kategorisinde akış şöyle: tür seçilir, seçicide aranır, mevcut adın hazır yazılı geldiği metin kutusunda düzenlenip onaylanır. Kısayollar da olmalı: "kendimi", "partimi", "bulunduğum yerleşimi" adlandır. Yerleşimlerde **"orijinal ada dön"** seçeneği bulunur. Parti adlandırmada, oyunun bazı olaylarda adı sıfırlayabileceğine dair kısa bir uyarı gösterilir.

### Faz 2: aynı seçiciyle editörler

Bu fazdaki beş maddenin her biri, Faz 1'de yazılan seçicinin bir varyasyonu ve oyunun kendi eylemlerinin çağrılmasından ibaret:

1. kahraman editörü
2. ilişki editörü
3. yerleşim editörü
4. eşya ekleyici
5. zaman hızı

Talep tarafında en güçlü kanıt kahraman editöründe. Yaş ve görünüm düzenleyen DCC 22,5 bin beğeni ve 1,45 milyon indirmeyle en popüler editör ([Nexus 118][nx-118]). Character Reload'un 2026 gönderileri ise tam da ilişki yöneticisi, diriltme, yaş ve sancak özelliklerindeki çökmeleri konu alıyor; yani kullanıcılar bu özelliklere bağımlı ([Nexus 3700 gönderiler][nx-3700p]). Bir 2026 Steam gönderisi de yoldaşları daha uzun tutabilmek için yaş değiştiren konsol komutlarının geri gelmesini istiyor ([Steam "My Honest Review"][s-honest]). Riskli işlemler önce onay istemeli ve "kaydetmeden önce yedek al" uyarısı göstermeli: sahiplik devri, krallık kurma, toplu ilişki değişikliği. Zaman hızı için önerilen sınır varsayılan olarak 16x; daha yükseği bir uyarıyla açılabilir.

### Faz 3: güç kullanıcısı ve bilgi araçları

Klan ve krallık araçları, oyunun konsolda bile sunmadığı işleri kapsamalı: klan liderini değiştirmek ve bir krallığı yok etmek. İttifak kurma için oyunda bir hile yok. Ancak bu araştırmada ittifak API'si incelenmediği için bu madde doğrulama bekliyor. Bilgi araçları, 2026 incelemelerinde geçen iki şikâyete doğrudan yanıt veriyor: "son görülme" konumunun yanlış olması ve bir lorda başka bir lordun yerini soramamak ([Steam son olumlular][s-posrec]). Bu faza ait kural çarpanları (XP, öğrenme hızı, parti boyutu), Cheats'in 116 ayarlık mirasından seçilmeli ([GitHub Cheats ayarları][gh-cheats-settings]). Hangilerinin Harmony'siz, yalnızca oyun modellerini zincirleyerek yapılabileceği ayrıca doğrulanmalı. Mevcut savaş hilelerine (ölümsüzlük, tek vuruş, sınırsız cephane) BannerWand'daki gibi bir hedef seçimi eklenebilir: oyuncu, klan, belirli krallıklar ([Nexus 9151][nx-9151]).

### Faz 4: v1.5 kararlı sürüme geçtikten sonra yeniden değerlendirin

Yoldaş görünümü, kişiye özel yaş kilidi, gizlilik atlama, War Sails araçları ve tek panelli editör merkezi bu fazda. Her biri ya daha yüksek teknik risk taşıyor ya da TaleWorlds'ün hâlâ değiştirdiği bir alana dokunuyor. v1.5 kararlı sürüme geçtiğinde ilk iş, Faz 1–3'teki her özelliği yeni sürümde yeniden denemek olmalı. War Sails'e kadar uzanan kıran API değişiklikleri (Ek A5), TaleWorlds'ün iç yapıyı bir sonraki büyük sürümde de değiştirebileceğini gösteriyor.

### Bilerek dışarıda bırakılanlar

Altı alana bilerek girilmiyor. Bazıları başka modların sağlam biçimde elinde, bazıları oyuna giriyor, biri de ilke gereği yasak:

- **Diplomacy'nin sistemleri:** elçi, saldırmazlık paktı, savaş yorgunluğu, iç savaş. Diplomacy hem Nexus'ta hem Steam'de ilk beşte ve Ekim 2026'da güncel.
- **Garnizon ve tımar otomasyonu:** Improved Garrisons'ın alanı.
- **Savaş yapay zekâsı:** RBM'nin alanı.
- **Klan partisi kontrolü ve başlangıç seçenekleri:** v1.5 betası yerli olarak getiriyor.
- **Küresel doğum/yaşlanma ve Fast Mode anahtarları:** resmî modüller zaten var.
- **Başarımları yeniden açan ya da başarım kontrolünü kandıran her türlü özellik:** ilke gereği yasak.

Bu sonuncusu yalnızca bir kural değil, modun ne olduğunun açık beyanı: CheatMenu dürüst bir kum havuzu aracı.

### Bütün fazlar için tasarım kuralları

Bu modun en değerli özelliği belki de görünmeyen bir şey: dayanıklılık. Bunun için dört kural:

1. **Harmony'siz ve yalnızca oyunun kendi kayıt alanlarına yazarak kalın.** Kayda özel veri türü eklemeyin. Kalıcı ayarlar gerekiyorsa bir yapılandırma dosyası kullanın. Kampanyaya özel basit bir sayı saklamak muhtemelen güvenli, ama bu denenmedi.
2. **Konsol hilelerini çağırmayın; aynı işi yapan eylemleri doğrudan kullanın.** Oyunun konsol hileleri hile modu kapalıyken çalışmıyor ve çalıştıkları kampanyaya "hile kullanıldı" işaretini kalıcı olarak koyuyor ([yerel: CampaignSystem.dll][l-cs]).
3. **Bir yama bir özelliği bozarsa çökmeyin, o özelliği kapatın.** Faster Time'ın kırmızı uyarıyla vanilla davranışa dönmesi gibi davranın. Tehlikeli değerleri sınırlayın.
4. **Yayımlamadan önce test edin.** Savaş hilelerini RBM ile, zaman hızını Better Time ile birlikte deneyin. Mevcut "temiz kayıt" uyarısını her sürümde koruyun.

## Sonuç

Bu araştırmanın en beklenmedik sonucu, talep ile kolaylığın aynı yerde buluşması. Hile ve editör pazarı, terk edilen modlar ve sürüm sürüm yamanan çatallarla tanımlanıyor. Işınlanma ve adlandırma bu pazarın en sahipsiz iki köşesi, ve v1.4.8'de yapılması en ucuz iki özellik de onlar. İkisi de oyunun kendi fonksiyonlarıyla, ek kütüphane olmadan ve kayda iz bırakmadan yapılabiliyor. Ortak seçici bir kez yazıldığında, ikinci fazdaki editörlerin çoğu yeni bir arayüz değil, yeni bir menü maddesi oluyor. Bu nedenle CheatMenu'nün rakiplerinden ayrışacağı nokta, özellik sayısından çok **her TaleWorlds yamasından sonra çalışmaya devam etmesi**. Oyuncuların en çok şikâyet ettiği şey zaten tam olarak bu.

Zamanlama da bir karar unsuru. v1.5 betası, oynanış modlarının birkaç nişini (klan partisi kontrolü, başlangıç seçenekleri, kişilik etkileri) yerli hale getirirken hile ve editör nişlerine dokunmuyor. Hatta finans ekranını kaldırarak yeni bir QoL boşluğu açabilir. Bu yüzden şimdi v1.4.8 üzerinde Faz 1 ve Faz 2'yi tamamlamak, ardından v1.5 kararlı sürümde yeniden doğrulamak doğru sıralama. Başarım kuralı da bir kısıt olmaktan çok bir konumlanma. Başarımları kandıran modlara Steam'de gerçek bir talep var. CheatMenu'nün açıkça "bu kampanyada başarımlar kapanır" demesi ve temiz bir kaydı yanlışlıkla işaretlemeden önce uyarması, onu güvenilir bir kum havuzu aracı yapan özellik.

## Ek A: Geliştirici için teknik ayrıntılar

Bu ek, ana metindeki kararların teknik dayanağı. Tüm API'ler kurulu v1.4.8 DLL'lerinde bu imzalarla bulundu, ama çalışma anında test edilmedi. "Çıkarım" diye işaretlenen yerler kod okumasından yapılan yorumlardır.

### A1. Işınlanma

| Konu | v1.4.8 API'si | Not |
|---|---|---|
| Parti konumu | `MobileParty.Position` (`CampaignVec2`, public set) | Setter yalnızca `_position` alanını ve konum bulucuyu günceller; kara/deniz durumuna, yapay zekâya ve görsellere dokunmaz ([yerel: CampaignSystem.dll][l-cs]) |
| Konum türü | `CampaignVec2(Vec2 pos, bool isOnLand)`; `IsOnLand`, `Face`, `IsValid()` | `Vec2 Position2D` kaldırıldı; yalnızca `GetPosition2D` okunabiliyor |
| Yerleşim konumları | `Settlement.Position`, `GatePosition`, `PortPosition`, `HasPort` | Hepsi `CampaignVec2` |
| Deniz durumu | `IsCurrentlyAtSea` (public set; bağlı partilere yayılır, olay tetikler), `ChangeIsCurrentlyAtSeaCheat()`, `IsInRaftState`, `HasNavalNavigationCapability`, `Anchor` | `SetSailAtPosition` ve `DisembarkToPosition` da var |
| Hareket emri | `MobileParty.SetMoveModeHold()`, `SetMoveGoToPoint(CampaignVec2, NavigationType)` | Emirler `MobilePartyAi`'den `MobileParty`'ye taşındı |
| Zemin doğrulama | `NavigationHelper.IsPositionValidForNavigationType(...)`, `FindReachablePointAroundPosition(pos, NavigationType, maxDistance, …)` | Gemisi olmayan partiyi karaya yaslamak için |
| İmleç | `MapScreen.Instance`, `SceneLayer.SceneView.TranslateMouse(ref near, ref far)`, `GetCursorIntersectionPoint(ref near, ref far, out float, out Vec3, ref PathFaceRecord, out bool isOnLand, …)` | ([yerel: SandBox.View.dll][l-sbv], [yerel: TaleWorlds.Engine.dll][l-eng]) |
| Kamera | `MapScreen.TeleportCameraToMainParty()`, `FastMoveCameraToPosition(CampaignVec2)` | Lord bulucu için de kullanılabilir |
| Kayıt | `_position` `SaveableField(225)`, `_isCurrentlyAtSea` `SaveableField(1110)` | Mod kaldırılınca güvenli |

Vanilla'nın `MapScreen.HandleLeftMouseButtonClick` içindeki Ctrl+tık akışı sırasıyla şunları yapıyor:

1. `Input.IsControlDown()`, `Game.Current.CheatMode` ve `!MapState.AtMenu` koşullarını denetler.
2. `PathFaceRecord.IsValid()` ve sal durumunu denetler.
3. `Position`'ı atar, ardından `SetMoveModeHold()` çağırır.
4. Konum geçerli değilse `ChangeIsCurrentlyAtSeaCheat()` çağırır.
5. Ordudaki tüm partileri taşır.
6. Tüm parti ve yerleşimler için `PartyBase.UpdateVisibilityAndInspected` çağırır.

War Sails'in `NavalStorylineData.TeleportMainPartyBackToBase` akışı ise şöyle ([yerel: NavalDLC.dll][l-naval]):

1. Parti gemiliyse `PortPosition`'a, değilse `GatePosition`'a konur.
2. `IsCurrentlyAtSea` atanır.
3. `PlayerEncounter.Finish(true)` çağrılır.
4. Sal durumu kapatılır.
5. Çapa sıfırlanır.
6. `EncounterManager.StartSettlementEncounter` çağrılır.
7. `SetMoveModeHold()` ve kamera ışınlaması yapılır.

Yerleşimden ayrılırken oyun `PlayerEncounter.LeaveSettlement()` ve `PlayerEncounter.Finish(true)` çağırıyor.

```csharp
// Yerleşime ışınlanma: taslak (çalışma anında test edilmedi)
var party = MobileParty.MainParty;
// 1) Aktif bir karşılaşma/yerleşim ziyareti varsa önce kapat:
//    PlayerEncounter.LeaveSettlement(); PlayerEncounter.Finish(true);
if (party.IsInRaftState) return;                       // vanilla da reddediyor
bool toPort = wantsPort && s.HasPort && party.HasNavalNavigationCapability;
CampaignVec2 pos = toPort ? s.PortPosition : s.GatePosition;
party.Position = pos;
party.IsCurrentlyAtSea = toPort;
party.SetMoveModeHold();
// 2) Ana parti bir ordu yönetiyorsa ordudaki her partinin Position'ını da pos yap (vanilla böyle yapıyor).
MapScreen.Instance?.TeleportCameraToMainParty();
// 3) İsteğe bağlı "içeri gir": EncounterManager.StartSettlementEncounter(party, s);

// İmleç konumu: taslak
Vec3 near = Vec3.Zero, far = Vec3.Zero;
var ms = MapScreen.Instance;
ms.SceneLayer.SceneView.TranslateMouse(ref near, ref far);
PathFaceRecord face = PathFaceRecord.NullFaceRecord;
ms.GetCursorIntersectionPoint(ref near, ref far, out float _, out Vec3 hit, ref face, out bool isOnLand);
if (!face.IsValid()) return;                            // geçersiz zemini asla kayda yazma
var target = new CampaignVec2(hit.AsVec2, isOnLand);
// Gemisi yok ve hedef deniz ise: NavigationHelper.FindReachablePointAroundPosition(target, NavigationType.Default, r)
party.Position = target;
party.SetMoveModeHold();
// Hedef mevcut kara/deniz moduyla uyumsuzsa: party.ChangeIsCurrentlyAtSeaCheat();
// Ordu partilerini taşı; PartyBase.UpdateVisibilityAndInspected(...) ile görünürlüğü yenile.
```

Kısayol `OnApplicationTick` içinde `Input.IsKeyDown`/`IsKeyPressed` ile yoklanabilir; CheatMenu Insert tuşunu zaten böyle okuyor. Koşul olarak `ScreenManager.TopScreen is MapScreen` ve `!MapState.AtMenu` kullanılmalı. Harmony olmadan tıklamayı yakalayan bir yöntem, vanilla'nın aynı tıklamayı bir "buraya git" emri olarak işlemesini engellemez. Hedef aynı nokta olduğu için bu zararsız olmalı, ama ışınlamadan sonra `SetMoveModeHold()` çağırmak güvenli tarafta kalmayı sağlar (çıkarım). Modun `SandBox.View.dll`, `TaleWorlds.Engine.dll` ve `TaleWorlds.Library.dll`'ye referans vermesi gerekir.

### A2. Yeniden adlandırma

| Tür | API | Kaydedilen alan | Not |
|---|---|---|---|
| Kahraman | `Hero.SetName(TextObject fullName, TextObject firstName)` | `_name` (181), `_firstName` (180) | Klan ekranı adı `GameTexts.FindText("str_generic_character_name")` ile sarıyor, `CampaignUIHelper.IsStringApplicableForHeroName` ile doğruluyor ve kahraman bir ordu yönetiyorsa `Army.UpdateName()` çağırıyor ([yerel: ViewModelCollection.dll][l-vmc]) |
| Klan | `Clan.ChangeClanName(name, informalName)` | `Name`/`InformalName` `SaveableProperty(51/52)` | Klan ekranı da aynısını kullanıyor |
| Krallık | `Kingdom.ChangeKingdomName(name, informalName)` | `SaveableProperty(1/2)` | Krallık ekranı da aynısını kullanıyor |
| Yerleşim | `settlement.Party.SetCustomName(TextObject)` | `PartyBase.CustomName` `SaveableProperty(216)` | `Settlement._name` kaydedilmiyor ve XML'den yeniden okunuyor; yansımayla değiştirmeyin. Boş ad, asıl adı geri getirir (çıkarım). Setter `SetSettlementProperties` ve `SetVisualAsDirty` çağırıyor |
| Parti | `mobileParty.Party.SetCustomName(TextObject)` | Aynı alan | `DisbandPartyAction.StartDisband`/`CancelDisband`, `TeleportHeroAction.ApplyInternal`, `MobileParty.OnLateLoad` ve bazı görev oluşturucular adı yeniden yazıyor ([yerel: CampaignSystem.dll][l-cs]) |
| Metin girişi | `InformationManager.ShowTextInquiry(new TextInquiryData(..., textCondition, ..., defaultInputText))` | Yok | Mevcut adla önceden doldurulabilir ([yerel: TaleWorlds.Library.dll][l-lib]) |

### A3. Diğer editörler, zaman ve eşya

| Özellik | v1.4.8 API'si | Not |
|---|---|---|
| Yaş | `Hero.SetBirthDay(CampaignTime)`, `HeroHelper.GetRandomBirthDayForAge(float)` | `_birthDay` (440); konsol yalnızca ana kahramana uyguluyor; `AgeModel.HeroComesOfAge`/`MaxAge` sınırları gözetilmeli |
| Yetenek | `HeroDeveloper.SetInitialSkillLevel(skill, v)` + `InitializeSkillXp(skill)` | Konsol hilesi `set_skill_player` de bunu kullanıyor |
| Nitelik / odak | `HeroDeveloper.AddAttribute(attr, delta, checkUnspentPoints: false)`, `AddFocus(skill, delta, false)`; `UnspentAttributePoints`/`UnspentFocusPoints` public set | `DefaultCharacterAttributes.*` statik |
| Perk | `HeroDeveloper.AddPerk(perk)`, `Hero.ClearPerks()` | Tek perk silmek internal `SetPerkValueInternal(perk, false)` ister (yansıma) |
| Kişilik | `Hero.SetTraitLevel(trait, v)`; ana kahraman için ardından `TraitLevelingHelper.UpdateTraitXPAccordingToTraitLevels()` ve `CampaignEventDispatcher.Instance.OnPlayerTraitChanged(trait, eski)` | Aksi halde kişilik XP'si seviyeyi geri itebilir (çıkarım) |
| Ana kahraman görünümü | `GameStateManager.Current.PushState(Game.Current.GameStateManager.CreateState<BarberState>(Hero.MainHero.CharacterObject, CharacterHelper.GetFaceGeneratorFilter()))` | `StaticBodyProperties` (`SaveableProperty(100)`) üzerinden kalıcı ([yerel: SandBox.dll][l-sb]) |
| Başka kahramanın görünümü | `BodyGeneratorView`'ı `hero.CharacterObject` ile barındıran küçük bir ekran; onayda `BodyGen.CurrentBodyProperties` değerini `hero.StaticBodyProperties`, `Weight` ve `Build` alanlarına kopyalamak | `CharacterObject.UpdatePlayerCharacterBodyProperties` yalnızca `IsPlayerCharacter && IsHero` durumunda yazıyor; berber ekranı `Hero.MainHero`'ya sabitlenmiş. Alternatif: Harmony postfix |
| İlişki (tam değer) | `Hero.SetPersonalRelation(other, v)` veya `CharacterRelationManager.SetHeroRelation(h1, h2, v)` | `ChangeRelationAction.Apply*` fark uygular, çarpana ve "etkin kahraman" yönlendirmesine tabidir |
| Yerleşim sahibi | `ChangeOwnerOfSettlementAction.ApplyByDefault(hero, settlement)` | Konsol köyleri ve paralı asker hizmetini reddediyor; `Town.OwnerClan`'ı doğrudan atamak olayları atlar |
| Yerleşim istatistikleri | `Town.Prosperity`/`Loyalty`/`Security` (public set), `Fief.FoodStocks`, `Settlement.Militia`, `Village.Hearth` | Sınırların tam değeri doğrulanmadı |
| Binaları bitir | `while (q.Count > 0) { var b = q.Peek(); b.BuildingProgress = b.GetConstructionCost(); BuildingHelper.CheckIfBuildingIsComplete(b); }` (`q = town.BuildingsInProgress`) | Yardımcı fonksiyon kuyruğun başını çıkarıyor; mutlaka `Peek` kullanın. Kuşatma, yağma veya harita olayı sırasında yapmayın |
| Şöhret ve kademe | `Clan.AddRenown(v)` veya `GainRenownAction.Apply`; tam kademe için `clan.Renown = ClanTierModel.GetRequiredRenownForTier(t); clan.AddRenown(0f);` | `Clan.Renown` setter'ı kademeyi güncellemiyor (çıkarım) |
| Etki | `ChangeClanInfluenceAction.Apply(clan, v)` | |
| Krallık üyeliği | `ChangeKingdomAction.ApplyByJoinToKingdom`, `ApplyByLeaveKingdom`, `ApplyByJoinFactionAsMercenary` | Konsol, oyuncu hükümdarken katılmayı reddediyor |
| Savaş / barış | `DeclareWarAction.ApplyByDefault(f1, f2)`, `MakePeaceAction.Apply(f1, f2)` | Barıştan önce kalıcı savaş (haydutlar) denetlenmeli |
| Krallık kur / yönet | `Campaign.Current.KingdomManager.CreateKingdom(name, informal, culture, founderClan, …)`, `ChangeRulingClanAction.Apply`, `DestroyKingdomAction.Apply`, `ChangeClanLeaderAction.ApplyWithSelectedNewLeader` | Vasalken önce `ApplyByLeaveKingdom` çağırın (çıkarım); kurucu klanın `HomeSettlement` değeri geçerli olmalı |
| Zaman | `Campaign.Current.SpeedUpMultiplier` (float, sınırsız setter) | Kaydedilmiyor; `Campaign` kurucusu ve `OnLoad` değeri 4'e sıfırlıyor, her yüklemede yeniden uygulanmalı. Yalnızca hızlı ileri sarma modunu etkiler (`dt = 0.25 · realDt · çarpan`). Konsol sınırı 15 (geliştirici modunda 30) |
| Eşya | `MBObjectManager.Instance.GetObjectTypeList<ItemObject>()`, `MobileParty.MainParty.ItemRoster.AddToCounts(item, n)` | Filtre alanları `ItemType`, `Tier`, `Value`, `Culture`, `NotMerchandise`, `IsCraftedByPlayer` ([yerel: TaleWorlds.Core.dll][l-core]) |

### A4. Arayüz seçenekleri

Açılır pencerede arama ve ikon için v1.4.8 kurucusu şu: `MultiSelectionInquiryData(title, desc, elements, isExitShown, min, max, affirmativeText, negativeText, affirmativeAction, negativeAction, soundEventPath = "", bool isSeachAvailable = false)`. Parametre adındaki yazım hatası TaleWorlds'ün kendi yazımı. Liste öğeleri `new InquiryElement(obj, title, new ItemImageIdentifier(item))` şeklinde oluşturulur; diğer seçenekler `CharacterImageIdentifier` ve `BannerImageIdentifier` ([yerel: TaleWorlds.Core.dll][l-core]). CheatMenu bugün ikon yerine `null` geçtiği için ImageIdentifier değişikliğinden etkilenmiyor. 10 argümanlı kurucu çağrısı da hâlâ uyumlu.

Özel bir Gauntlet paneli için v1.4.8 imzaları ([yerel: Engine.GauntletUI.dll][l-eg], [yerel: ScreenSystem.dll][l-ss]):

- `new GauntletLayer(string name, int localOrder, bool shouldClear = false)`
- `GauntletMovieIdentifier LoadMovie(string movieName, ViewModel dataSource)`
- `ReleaseMovie(GauntletMovieIdentifier)`
- Prefab dosyası: `Modules/CheatMenu/GUI/Prefabs/<Ad>.xml`
- Katmanı kurmak için `layer.InputRestrictions.SetInputRestrictions()`, `layer.IsFocusLayer = true`, `ScreenManager.TopScreen.AddLayer(layer)`, `ScreenManager.TrySetFocus(layer)`
- Kapatırken bunların tersi uygulanır.

v1.2.12'deki `GauntletLayer(int localOrder, ...)` ve `IGauntletMovie` imzaları artık geçersiz. Topluluk belgelerindeki örnek de 1.3 öncesine ait. Oyunun kendi modülleri artık otomatik üretilmiş prefab DLL'leri taşıyor; moddan XML prefab yüklemenin 1.4'te değişmeden çalıştığı doğrulanmadı. MCM 5.13.1, v1.0'dan v1.5'e kadar tüm sürümleri desteklediğini belirtiyor ([MCM sürümleri][mcm]). Bannerlord.Harmony bu bilgisayarda kurulu değil.

### A5. 1.2'den 1.4'e kıran API değişiklikleri (port kontrol listesi)

War Sails, deniz navigasyonunu kampanya çekirdeğine taşıdı. 1.2 döneminden kalan ışınlanma ve hile kodu şöyle güncellenmeli ([apidoc 1.2.12 MobileParty][api1212-mp], [yerel: CampaignSystem.dll][l-cs]):

| v1.2 | v1.4.8 |
|---|---|
| `party.Position2D = v` | `party.Position = new CampaignVec2(v, isOnLand)` |
| `party.Ai.SetMoveModeHold()` | `party.SetMoveModeHold()` |
| `party.Ai.SetMoveGoToPoint(v)` | `party.SetMoveGoToPoint(cv, NavigationType.Default)` |
| `new ImageIdentifier(item)` | `new ItemImageIdentifier(item)` |
| `new GauntletLayer(order)` | `new GauntletLayer("Ad", order)` |
| `IGauntletMovie` | `GauntletMovieIdentifier` döndüren `LoadMovie` sonucunu `ReleaseMovie`'ye verin |

Parti taşıyan her kod `IsCurrentlyAtSea` durumunu da ele almalı. Eski DLL'ler bu değişikliklere çarptığında yükleme anında ya da ilk çağrıda `MissingMethodException` veya `TypeLoadException` alıyor.

### A6. Başarım bütünlüğü: mod neye dokunmamalı

`SandBox.CampaignBehaviors.DumpIntegrityCampaignBehavior.IsGameIntegrityAchieved` üç durumda başarısız oluyor:

1. hile kullanımı (`Campaign.EnabledCheatsBefore`)
2. resmî olmayan modüller (`Campaign.PreviouslyUsedModules`)
3. sürüm düşürme

`CampaignCheats.CheckCheatUsage`, `Game.Current.CheatMode` açıksa `EnabledCheatsBefore` bayrağını bir kez ve kalıcı olarak atıyor ([yerel: SandBox.dll][l-sb], [yerel: CampaignSystem.dll][l-cs]).

Mod şunları **asla** yapmamalı:

- `EnabledCheatsBefore` veya `PreviouslyUsedModules` değerini değiştirmek
- `DumpIntegrityCampaignBehavior`'ı yamalamak ya da kaldırmak
- `Game.Current.CheatMode` değerini programdan açmak
- `CampaignCheats.*` statik fonksiyonlarını çağırmak (hile modu kapısı ve kalıcı işaret nedeniyle)

Mevcut `CleanSaveGuard`, oyunun `CheckIfModulesAreDefault` mantığını yalnızca okuyarak taklit ediyor ve uyarı gösteriyor ([yerel: CleanSaveGuard.cs][l-csg]). Mod kendisi resmî olmayan bir modül olduğu için kontrol, beklendiği gibi, "resmî olmayan modüller" gerekçesiyle başarısız olmaya devam ediyor.

### A7. v1.4.8'deki yerli konsol komutları (özet)

Bu liste, modda "zaten var, yeniden yazma ama arayüzle sunabilirsin" kontrolü için ([yerel: CampaignSystem.dll][l-cs], [yerel: SandBox.View.dll][l-sbv]):

| Grup | Komutlar |
|---|---|
| Ekonomi | `add_gold_to_hero`, `add_influence`, `add_renown_to_clan`, `add_morale_to_party`, `boost_cohesion_of_army` |
| Kahraman | `add_skill_xp_to_hero`, `add_attribute_points_to_hero`, `add_focus_points_to_hero`, `set_skill_player`, `set_skills_of_hero`, `set_all_heroes_skills`, `set_player_trait`, `set_hero_trait`, `set_hero_culture`, `set_clan_culture`, `add_player_age`, `kill_hero` |
| İlişki ve aile | `add_hero_relation` (`all` destekli), `show_relation`, `marry_player_with_hero`, `marry_hero_to_hero`, `conceive_child` |
| Parti | `add_troops`, `add_prisoner_to_party`, `add_xp_to_player_party_troops`, `heal_player_party`, `kick_companion`, `add_item_to_player_party` |
| Yerleşim | `give_settlement_to_player` (`Calradia` = hepsi), `give_settlement_to_kingdom`, `give_workshop_to_player`, `add_building_level`, `set_prosperity/security/food/hearth/militia_of_settlement`, `set_loyalty_of_settlement`, `add_power_to_notable`, `add_supporters_for_main_hero` |
| Krallık | `join_kingdom`, `join_kingdom_as_mercenary`, `create_player_kingdom`, `create_clan`, `lead_kingdom`, `rule_your_faction`, `activate_all_policies_for_player_kingdom`, `declare_war`, `declare_peace`, `make_trade_agreement`, `make_clan_mercenary_of_kingdom`, `rebellion_enabled` |
| Harita ve zaman | `set_parties_visible`, `show_hideouts`, `show_settlements`, `toggle_information_restrictions`, `set_main_party_attackable`, `set_campaign_speed_multiplier` (en çok 15), `focus_hero`, `focus_mobile_party` |
| Dışa/içe aktarma | `export_main_hero`, `import_main_hero`, `export_hero` |
| War Sails (`naval.*`) | `add_ship_to_player`, `unlock_figurehead`, `list_all_ship_names`, `damage_player_ships` |
| Yok | Parti ışınlama, herhangi bir adlandırma, başka kahramanın yaşı, krallık/klan yok etme, ittifak kurma, görev tamamlama, suç puanı, klan lideri değiştirme |

## Ek B: Yöntem ve sınırlamalar

Bu rapor beş araştırma notunun sentezi. Kapsam ve sınırlar şöyle:

- **Erişilemeyen kaynaklar:** Reddit ile TaleWorlds forumları ve sitesi otomatik erişimi engelledi. Oyuncu şikâyetlerinin "ne kadar gürültülü" olduğu bu yüzden ağırlıkla Steam inceleme API'si, Steam tartışmaları, Atölye sıralamaları ve basından ölçüldü.
- **Sayımların niteliği:** Kategori sayıları, bir özetleme modelinin kesik JSON verisi üzerinde yaptığı yaklaşık etiketlemeler. Yön gösterir, kesin istatistik değildir.
- **Eski şikâyetler:** Tımar oylaması, saf değiştirme ve evlilikle ilgili şikâyetlerin bir kısmı 2020–24 başlıklarından geliyor; v1.4.8'de aynı davranıp davranmadıkları doğrulanmadı.
- **Yama notları:** Resmî notlar Steam haber API'sinden okundu.
- **v1.5:** Özellikleri beta notlarına dayanıyor ve kararlı sürümde değişebilir.
- **Mod verileri:** Nexus ve Steam istatistikleri 18 Ekim 2026'da okundu. Bazı Steam abone sayılarına ulaşılamadı. BLGM ve UFO paketinin lisansları doğrulanmadı.
- **Teknik değerlendirme:** Kurulu oyun dosyalarının incelemesine dayanıyor ve hiçbir şey oyunda çalıştırılarak test edilmedi. Özellikle düşman partisinin yanına ışınlanma, yerleşim içindeyken ışınlanma, yüksek zaman çarpanlarında kararlılık ve parti adının oyun akışlarında sıfırlanması ilk testlerde bakılması gereken konular.
- **Doğrulanmamış yerli özellikler:** Vanilla v1.4.8'in "hepsini terfi ettir", "hepsini topla" ve eşya kilitleme özelliklerine yerli olarak sahip olup olmadığı doğrulanmadı.

[s-store]: https://store.steampowered.com/app/261550/
[s-neg365]: https://store.steampowered.com/appreviews/261550?json=1&filter=all&day_range=365&language=english&num_per_page=100&review_type=negative&purchase_type=all
[s-negrec]: https://store.steampowered.com/appreviews/261550?json=1&filter=recent&language=english&num_per_page=100&review_type=negative&purchase_type=all
[s-posrec]: https://store.steampowered.com/appreviews/261550?json=1&filter=recent&language=english&num_per_page=100&review_type=positive&purchase_type=all
[s-disc]: https://steamcommunity.com/app/261550/discussions/0/
[s-honest]: https://steamcommunity.com/app/261550/discussions/0/800092529560144927/
[s-ws-store]: https://store.steampowered.com/app/2927200/
[s-ws-neg]: https://store.steampowered.com/appreviews/2927200?json=1&filter=all&day_range=365&language=english&num_per_page=100&review_type=negative&purchase_type=all
[gdd]: https://gamedropdaily.com/updates/mount-and-blade-ii-bannerlord/
[gh-diplo]: https://github.com/sum117/bannerlord-diplomacy-overview/issues/1
[thegamer]: https://www.thegamer.com/mount-and-blade-2-dlc-war-sails-mixed-reviews-bugs/
[tsa]: https://www.thesixthaxis.com/2026/01/13/mount-blade-2-bannerlord-war-sails-review/
[jdr]: https://jumpdashroll.com/article/mount-and-blade-ii-bannerlord---war-sails-review
[cw150]: https://calradiawar.com/news/bannerlord-v150-beta-update
[an-110]: https://store.steampowered.com/news/app/261550/view/5070528176557709918
[an-dev12]: https://store.steampowered.com/news/app/261550/view/5124582150127345736
[an-130]: https://store.steampowered.com/news/app/261550/view/1810503566552360
[an-134]: https://store.steampowered.com/news/app/261550/view/1817483467032776
[an-1313]: https://store.steampowered.com/news/app/261550/view/1819386365106738
[an-140]: https://store.steampowered.com/news/app/261550/view/1829528821306653
[an-145]: https://store.steampowered.com/news/app/261550/view/1833334318569123
[an-148]: https://store.steampowered.com/news/app/261550/view/1840310314351373
[an-150]: https://store.steampowered.com/news/app/261550/view/1841579228662390
[gtips]: https://gameplay.tips/guides/7215-bannerlord-cheats.html
[nx-top]: https://www.nexusmods.com/games/mountandblade2bannerlord/mods?sort=endorsements
[nx-year]: https://www.nexusmods.com/games/mountandblade2bannerlord/mods?sort=endorsements&timeRange=365
[ws-top]: https://steamcommunity.com/workshop/browse/?appid=261550&browsesort=totaluniquesubscribers&section=readytouseitems&actualsort=totaluniquesubscribers&p=1
[nx-832]: https://www.nexusmods.com/mountandblade2bannerlord/mods/832
[nx-688]: https://www.nexusmods.com/mountandblade2bannerlord/mods/688
[nx-791]: https://www.nexusmods.com/mountandblade2bannerlord/mods/791
[nx-2849]: https://www.nexusmods.com/mountandblade2bannerlord/mods/2849
[nx-118]: https://www.nexusmods.com/mountandblade2bannerlord/mods/118
[nx-6218]: https://www.nexusmods.com/mountandblade2bannerlord/mods/6218
[nx-4526]: https://www.nexusmods.com/mountandblade2bannerlord/mods/4526
[nx-492]: https://www.nexusmods.com/mountandblade2bannerlord/mods/492
[nx-49]: https://www.nexusmods.com/mountandblade2bannerlord/mods/49
[nx-2797]: https://www.nexusmods.com/mountandblade2bannerlord/mods/2797
[nx-11354]: https://www.nexusmods.com/mountandblade2bannerlord/mods/11354
[nx-1839]: https://www.nexusmods.com/mountandblade2bannerlord/mods/1839
[st-cheats]: https://steamcommunity.com/sharedfiles/filedetails/?id=2882183897
[gh-cheats]: https://github.com/welles/BannerlordCheats
[gh-cheats-settings]: https://raw.githubusercontent.com/welles/BannerlordCheats/stable/Settings/BannerlordCheatsPerCampaignSettings.cs
[nx-6446]: https://www.nexusmods.com/mountandblade2bannerlord/mods/6446
[nx-6446p]: https://www.nexusmods.com/mountandblade2bannerlord/mods/6446?tab=posts
[st-ufo]: https://steamcommunity.com/sharedfiles/filedetails/?id=3583201039
[gh-ufo]: https://github.com/UFOdestiny/UFO-Bannerlord
[nx-9151]: https://www.nexusmods.com/mountandblade2bannerlord/mods/9151
[nx-9448]: https://www.nexusmods.com/mountandblade2bannerlord/mods/9448
[nx-10127]: https://www.nexusmods.com/mountandblade2bannerlord/mods/10127
[nx-3700]: https://www.nexusmods.com/mountandblade2bannerlord/mods/3700
[nx-3700p]: https://www.nexusmods.com/mountandblade2bannerlord/mods/3700?tab=posts
[nx-teleport]: https://www.nexusmods.com/games/mountandblade2bannerlord/mods?keyword=teleport&sort=endorsements
[nx-10005]: https://www.nexusmods.com/mountandblade2bannerlord/mods/10005
[nx-rename]: https://www.nexusmods.com/games/mountandblade2bannerlord/mods?keyword=rename&sort=endorsements
[nx-12507]: https://www.nexusmods.com/mountandblade2bannerlord/mods/12507
[nx-11712]: https://www.nexusmods.com/mountandblade2bannerlord/mods/11712
[nx-12601]: https://www.nexusmods.com/mountandblade2bannerlord/mods/12601
[docs-save]: https://docs.bannerlordmodding.com/_csharp-api/savesystem/
[mcm]: https://github.com/Aragas/Bannerlord.MBOptionScreen/releases
[api1212-mp]: https://apidoc.bannerlord.com/v/1.2.12/class_tale_worlds_1_1_campaign_system_1_1_party_1_1_mobile_party.html
[l-ver]: file:///C:/Program%20Files%20%28x86%29/Steam/steamapps/common/Mount%20%26%20Blade%20II%20Bannerlord/bin/Win64_Shipping_Client/Version.xml
[l-cs]: file:///C:/Program%20Files%20%28x86%29/Steam/steamapps/common/Mount%20%26%20Blade%20II%20Bannerlord/bin/Win64_Shipping_Client/TaleWorlds.CampaignSystem.dll
[l-vmc]: file:///C:/Program%20Files%20%28x86%29/Steam/steamapps/common/Mount%20%26%20Blade%20II%20Bannerlord/bin/Win64_Shipping_Client/TaleWorlds.CampaignSystem.ViewModelCollection.dll
[l-core]: file:///C:/Program%20Files%20%28x86%29/Steam/steamapps/common/Mount%20%26%20Blade%20II%20Bannerlord/bin/Win64_Shipping_Client/TaleWorlds.Core.dll
[l-lib]: file:///C:/Program%20Files%20%28x86%29/Steam/steamapps/common/Mount%20%26%20Blade%20II%20Bannerlord/bin/Win64_Shipping_Client/TaleWorlds.Library.dll
[l-eng]: file:///C:/Program%20Files%20%28x86%29/Steam/steamapps/common/Mount%20%26%20Blade%20II%20Bannerlord/bin/Win64_Shipping_Client/TaleWorlds.Engine.dll
[l-eg]: file:///C:/Program%20Files%20%28x86%29/Steam/steamapps/common/Mount%20%26%20Blade%20II%20Bannerlord/bin/Win64_Shipping_Client/TaleWorlds.Engine.GauntletUI.dll
[l-ss]: file:///C:/Program%20Files%20%28x86%29/Steam/steamapps/common/Mount%20%26%20Blade%20II%20Bannerlord/bin/Win64_Shipping_Client/TaleWorlds.ScreenSystem.dll
[l-sbv]: file:///C:/Program%20Files%20%28x86%29/Steam/steamapps/common/Mount%20%26%20Blade%20II%20Bannerlord/Modules/SandBox/bin/Win64_Shipping_Client/SandBox.View.dll
[l-sb]: file:///C:/Program%20Files%20%28x86%29/Steam/steamapps/common/Mount%20%26%20Blade%20II%20Bannerlord/Modules/SandBox/bin/Win64_Shipping_Client/SandBox.dll
[l-naval]: file:///C:/Program%20Files%20%28x86%29/Steam/steamapps/common/Mount%20%26%20Blade%20II%20Bannerlord/Modules/NavalDLC/bin/Win64_Shipping_Client/NavalDLC.dll
[l-csg]: file:///C:/Users/tolgaa/Desktop/Bannerlord/src/CleanSaveGuard.cs
