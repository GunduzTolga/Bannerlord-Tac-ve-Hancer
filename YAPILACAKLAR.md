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
- [x] **.NET SDK 8** kuruldu (8.0.426, 19 Ekim). Alternatif: GitHub → Actions → **TacVeHancer** paketi → `CheatMenu` klasörünü `Modules`'e kopyala
- [x] Workshop'tan **Harmony 2.4.2, ButterLib 2.12.0, UIExtenderEx 3.0.3, MCM 5.12.3** kurulu ve Launcher'da işaretli
      (sıra: Harmony → ButterLib → UIExtenderEx → MCM → … → Taç ve Hançer). MCM şartı 5.12.3'e indirildi; kullanılan API o sürümde var
- [x] `build.ps1` çalışıyor: 418 test geçti, arayüz denetimi geçti (24 prefab), mod oyun klasörüne kopyalandı; 634 tip bağımlılıklarla yükleniyor
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
- [ ] MCM → **Arayüz**: kısayol, düğme yeri (Gizli / Sol üst / Üst orta / Sağ üst / Harita çubuğunda), resimli kartlar aç/kapa
- [ ] Haritadaki düğme oyunun kendi arayüzünün üstüne biniyor mu? Biniyorsa MCM'den yerini değiştir ve hangisinin iyi olduğunu yaz
- [x] **Ansiklopedi bağlantısı ve harita çubuğu düğmesi** PC'de yapıldı (19 Ekim); denemesi aşağıda

### Harita çubuğu düğmesi ve ansiklopedideki Entrika bölümü (yeni, PC'de yapıldı)
- [ ] MCM → Arayüz → düğme yeri: **Harita çubuğunda** seç → kaydı yeniden yükle → oyunun sol alttaki harita çubuğunda
      (Klan ile Krallık arasında) hançer simgeli Entrika düğmesi var; üst ortadaki düğme görünmüyor
- [ ] Çubuktaki düğmenin üstüne gelince "Entrika" ipucu; harekete hazır suikast/kaçırma ya da sezilen entrika varsa rozet
- [ ] Tıklayınca Entrika ekranı açılıyor; konuşma sırasında çubuktaki düğme soluk ve nedeni yazıyor
- [ ] Simge boş/bozuk görünüyorsa ekran görüntüsü at (çubuğa eklenemezse eski düğme üst ortada çıkar, mod bozulmaz)
- [ ] Ansiklopedide bir lordun sayfası: "Müttefikler" başlığının üstünde **Entrika** bölümü; bildiklerin
      (rapor kaç gün önce, sırlar, koz, süren entrika, hükümdarıyla arası). Başlığa tıklayınca diğerleri gibi katlanıyor
- [ ] **Dosyasını aç** (yalnızca hakkında bir şey biliyorsan açık) → ansiklopedi kapanır, Entrika ekranı Kişiler'de o kişiyle açılır
- [ ] **Entrika başlat...** → Yeni entrika sekmesi tür ve hedef seçili açılır; başlatılamıyorsa altında kırmızı neden
- [ ] Kendi sayfanda bölüm yok; ansiklopediyi Klan/Krallık ekranından açınca düğmeler soluk, "yalnızca haritadayken" yazıyor
- [ ] Bölüm hiç görünmüyorsa: rgl_log'da `[CheatMenu]` ya da `UIExtender` satırlarını gönder

### P2.3 — Casus başı görevleri (yeni)
- [ ] Sol sütunda "Görev: Boşta" ve **Görev...** düğmesi → kart: Karşı casusluk / Muhalefeti izle / Söylenti yay... / Boşta
- [ ] Karşı casusluk seç → kart metninde sezme yüzdesi arttı; (uzun oyunda) sana karşı entrikalar daha sık seziliyor
- [ ] Muhalefeti izle (bir krallıktayken) → bir hafta sonra "Muhalefet raporu" kartı; adı geçenler Kişiler sekmesinde raporlu
- [ ] Söylenti yay... → hedef listesi → seç → sol sütunda "Görev: Söylenti yay: X"; haftada 100 dinar düşüyor
- [ ] Birkaç hafta sonra Saray'da X'in hükümdarıyla ilişkisi düştü (raporun varsa sayıyla görürsün)
- [ ] Yakalanırsa "Söylentilerin kaynağı ortaya çıktı" kartı, ilişkiler düştü, görev Boşta'ya döndü
- [ ] Saray ve kişi dosyasında "Hakkında söylenti yay" düğmesi de görevi o lordla başlatıyor
- [ ] Insert → Entrika (liste menüsü) → **Casus başı görevi** aynı kartı açıyor
- [ ] Görev sürerken kaydet → yükle → görev ve hedef duruyor

### P2.15 — Divan: kâhya (yeni)
- [ ] Divan'da dört koltuk yan yana sığıyor mu (SERDAR, KÂHYA, HAZİNEDAR, SARAY HEKİMİ)? Küçük düğme yazıları okunuyor mu? Ekran görüntüsü at
- [ ] Kâhya ata (Steward, yolsuz mu, sadakat katkısı) → kart: "Şehir ve kalelerinde sadakat günde +0,5"
- [ ] Bir şehrinin sadakatine bak (oyunun şehir ekranında açıklama): "Kâhya (ad)" satırı var mı?
- [ ] ŞEHİR VE KALELER: sadakat, güvenlik, refah, inşaat (kalan gün), günlük proje, günlük gelir; sorunlu olan kırmızı
- [ ] "Yönet..." → kart: durum ve öneri → "İnşaatı değiştir..." → bir yapı seç → 100 dinar düştü, şehre gitmeden inşaat değişti
- [ ] "Günlük projeyi değiştir..." → seç → şehir ekranında yeni günlük proje
- [ ] Sadakati düşen bir yerde (eşiğe 10 gün kala) "Kâhyan uyarıyor" kartı; erzak 5 günden azsa kıtlık uyarısı
- [ ] Insert > Entrika > Divan > Kâhya: rapor metni, "Yönet: X"
- [ ] Kaydet → yükle: kâhya duruyor

### P2.14 — Divan: serdar (yeni)
- [ ] Divan'da üç koltuk yan yana sığıyor mu (SERDAR, HAZİNEDAR, SARAY HEKİMİ)? Yazılar taşıyorsa ekran görüntüsü at
- [ ] Serdar ata (Tactics, Scouting, tahmin payı) → kart: görüş menzili, erken uyarı yüzdesi; parti başındaki biri de olabiliyor
- [ ] Savaştayken SAVAŞ MECLİSİ: krallık gücü (onlar / biz), oran, tavsiye
- [ ] SAVUNMAN: şehir ve kalelerin garnizon, milis, sur, erzak; kuşatma altındaysa kırmızı
- [ ] DÜŞMAN ORDULARI: kaç kişi, nerede, hedef ("Pravend'i kuşatmaya gidiyor"), sana kaç gün; "Haritada izle"
- [ ] ZAYIF HEDEFLER: en zayıf beş düşman kalesi; birine git ve oyundaki garnizonla karşılaştır (Tactics düşükse sapmalı)
- [ ] Bir ordu şehrine ya da köyüne yürürken (çoğu zaman) kuşatmadan önce "Serdarın uyarıyor" kartı geliyor mu, süre doğru mu?
- [ ] Insert > Entrika > Divan > Serdar: rapor metni
- [ ] Kaydet → yükle: serdar duruyor

### P2.13 — Divan: saray hekimi (yeni)
- [ ] Divan sekmesinin üstünde iki koltuk kartı yan yana (HAZİNEDAR, SARAY HEKİMİ); boşsa açıklama ve "Ata"
- [ ] Hekim ata → liste (Medicine, şarlatan, ömür etkisi) → kart: Medicine, koruma özeti ("Doğumda ölüm -%30, yaşlılık -%18, ...")
- [ ] Hazinedarı ya da casus başını hekim yapmaya çalış → "zaten ...; biri iki koltukta oturamaz"
- [ ] SAĞLIK listesi: klanındakiler, yaş, can, yaralı/gebe, yıllık risk; yaşlılarda kırmızı. Gebe olan için "doğuma ~N gün" doğru mu?
- [ ] (Editörler'den yaşını 70+ yap) birkaç hafta sonra "Hekimin uyarıyor" kartı; bir yıl tekrar gelmiyor
- [ ] Yapay zekâ suikastı (ayarlardan "Yapay zekâ sana entrika kursun: Normal") başarılı olursa bazen "Suikastten kurtuldu" kartı, kurban yaralı
- [ ] Eşin gizli ilişkiden gebeyse (P2.6b) hekim haftalar içinde fark ediyor → yüzleş / görmezden gel / boşa kartı
- [ ] Klanından bir kadın doğum yapınca ölüm daha seyrek mi (uzun oyunda gözlem; olursa rgl_log yeter)
- [ ] Insert > Entrika > Divan > Saray hekimi: rapor metni, ata / görevden al
- [ ] Kaydet → yükle: hekim duruyor

### P2.12 — Divan: hazinedar (yeni)
- [ ] Entrika ekranında sekmeler sığıyor mu (9 sekme: Süren ... Divan ... Kronik)? Taşan yazı varsa ekran görüntüsü at
- [ ] Divan sekmesi → "Hazinedar ata" → liste (Trade, Hesapçı, tahmin payı) → seç; koltuk kartı: portre, Trade, tahmin payı
- [ ] Üstte günlük gelir: atölyeler ve kervanlar; Klan ekranındaki gelirle aynı mı?
- [ ] ATÖLYELER: her atölye, günlük gelir, hazinedarın önerisi; "Üretimi değiştir..." → kart → bir tür seç → altın düştü, atölye değişti
      (şehre gitmeden); birkaç gün sonra gelir tahmine yakın mı?
- [ ] KERVANLAR: nerede / nereye gidiyor, günlük gelir; "Haritada izle" çalışıyor
- [ ] Birkaç gün geçince SEFER DEFTERİ doluyor: "Ortysia > Sargot · 2 sefer · ort. +310" (kazançlar makul mü?)
- [ ] TİCARET FIRSATLARI: "Demir: Varcheg'de al ~32, Sargot'ta sat ~55 (+%72)"; birine git ve fiyatlara bak: Trade'i yüksek hazinedarda
      yakın, düşükte sapmalı mı?
- [ ] KERVAN KURMAYA UYGUN ŞEHİRLER: üç şehir; "Haritada göster"
- [ ] Onuru düşük birini hazinedar yap, casus başı da olsun → haftalar içinde "Hazinede açık" kartı → Divan > "Hesap sor..." üç seçenek
- [ ] Hazinedarı casus başı yapmaya çalış → "zaten hazinedarın" hatası; tersi de
- [ ] Insert > Entrika > "Divan: hazinedar": rapor metni, atölye değiştirme, görevden alma
- [ ] Kaydet → yükle: hazinedar, defter, zimmet duruyor

### P2.11 — Casusluğu kendin yap (yeni)
- [ ] Bir lordun bulunduğu şehre gir → şehir menüsünde "Birini gizlice takip et" → lord listesi → seç
- [ ] Üç kart ("TAKİP 1/3"...): sahne metni, şüphe ve ipucu satırı; düğmelerde şüphe aralığı, ipucu, beceri, varsa bedel
- [ ] Gece dene (şehirde bekle): kart resmi gece resmi, "Gece: karanlık senden yana", şüphe aralıkları daha düşük
- [ ] Sonunda "Takip: X" kartı: rapor, sır ya da "sır yakalayamadın"; Kişiler'de rapor görünüyor
- [ ] Hep riskli seçeneği seç → "FARK EDİLDİN" kartı: kaç / yol sor / kese; yakalanınca ilişki -15 (yabancı şehirde suç puanı da)
- [ ] Aynı lorda karşı casusluk entrikası sürerken takip et (30+ ipucu) → sonuç kartında "bir aşama ilerledi"
- [ ] Entrika ekranı > Kişiler > lord dosyası > "Kendin gizlice takip et": başka şehirdeyken kapalı ve nerede olduğunu yazıyor
- [ ] Aynı lordu hemen tekrar takip et → "N gün sonra"; kaydet → yükle → bekleme süresi duruyor
- [ ] **Deneysel:** Kurallar > "Takibi sahnede dene" aç (önce kaydet!) → takip et → oyunun şehir sahnesi açılıyor mu, hedef orada mı,
      yürüyor mu? Çıkınca kart geliyor mu (başarı / izini kaybettin / fark edildin / görünmedi)? Ne olduğunu yaz ya da ekran görüntüsü at

### P2.10 — Rehine anlaşmaları (yeni; hükümdar ya da bağımsız klan lideri olmalısın)
- [ ] Entrika ekranı > Saray > barışta olduğun bir krallık > "Rehine anlaşması..." → kart
- [ ] "Rehine gönder" → klanından birini seç → kart; kişi o krallığın hanedan şehrinde; hükümdarla +15; Saray'da durum satırı
- [ ] "Rehine değişimi teklif et" (bir şehrin/kalen varken) → kabul ederse onların biri senin şehrinde; reddederse kart
- [ ] Haftalar geçtikçe misafirin ev sahibiyle ilişkisi artıyor; misafir başka yere gitmiyor
- [ ] "Süren" sekmesinde rehine satırı ve kalan gün
- [ ] Hile ile o krallığa savaş ilan et → "Yeminini bozdun" kartı; senin misafirin onların zindanında, onlarınki senin esirin
- [ ] Bir yıl sonra "Rehineler döndü"; erken bitirince hükümdarla -15
- [ ] Vasalken düğme kapalı ("Yalnızca hükümdar ya da bağımsız klan lideri")

### P2.9 — Saray günü (yeni)
- [ ] Kendi şehrinde → şehir menüsünde "Divan kur (saray günü)" → üç kart ("DİVAN 1/3"...); dilekçe sahibinin portresi
- [ ] Seçenek düğmelerinde sonuçlar yazıyor; parası yetmeyen seçenek kapalı
- [ ] Seçince şehir ekranında sadakat/güvenlik/refah/erzak değişti; kişilik özelliğine küçük puan (oyunun bildirimi)
- [ ] Sonda "Divan dağıldı" özeti
- [ ] "Yakalanan casus" → "Sorgula" → Kişiler'de bir yabancı lordun raporu
- [ ] Kalede de çalışıyor (şehir işleri çıkmıyor); başkasının şehrinde seçenek yok
- [ ] Hemen tekrar: düğme kapalı, "N gün sonra"

### P2.8 — Varis ve meşrulaştırma (yeni)
- [ ] Sevgilinden (annesi evli) doğan çocuk: Editörler > Aile editörü > [Ben] > "Gayrimeşru çocuğunu meşrulaştır" → bedel (etki ya da altın)
- [ ] Onay → çocuk klan ekranında; "Meşrulaştırma" kartı; resmî babası ve annesiyle ilişkiler düştü; sır "ifşa edildi"
- [ ] Kişiler'de annesinin dosyasında da "Çocuğunu meşrulaştır" var
- [ ] **Önce yedek kayıt al:** "Klanı devret" → varis listesi (oyunun sırasıyla) → seç → oyuna varis olarak devam ediyorsun; eski karakter emekli
- [ ] Ordudayken / savaşta / esirken "Klanı devret" kapalı

### Düello (yeni)
- [ ] İlişkin -10 ya da kötü bir lordla aynı şehirde: Kişiler > dosyası > "Düelloya davet et (onur)..." → kart (kabul %, sonuçlar)
- [ ] Kabul ederse şehrin arenası açılıyor, ikiniz dövüşüyorsunuz; kimse ölmüyor (ölürse ya da sahne yanlış açılırsa ekran görüntüsü)
- [ ] Kazanınca/kaybedince haritada sonuç kartı: ün +10 / -5
- [ ] Reddederse "Düello reddedildi" kartı, ün +5
- [ ] Aynı şehirde değilken düğme kapalı ve hedefin nerede olduğunu yazıyor
- [ ] Eşinin sevgilisi ortaya çıktıysa "Eşin seni aldatıyor" kartında "... ile düello"; kazanınca ilişki bitiyor
- [ ] Kan davalı (yakınını öldürdüğün) biriyle: kazanınca kan davası kapanıyor
- [ ] Husumet olmayan bir lordda düğme kapalı ("Aranızda bir husumet yok")

### P2.7 — Aile editörü (yeni)
- [ ] Insert → Editörler → **Aile editörü**: önce sen, sonra klanın, sonra diğerleri; satırlarda yaş ve eş
- [ ] Bekâr bir klan üyesi → "Evlendir..." → yalnızca uygun adaylar; seç → oyunun evlilik bildirimi, eş klana geldi
- [ ] Evli bir kadın → "Gebelik başlat" → klan ekranında hamile; oyunun süresinde doğum
- [ ] "Çocuk istemiyorlar" açık bir çiftte gebelik olmuyor; P2.6'daki buluşmalarda da gebelik seçeneği yok
- [ ] Klanındaki bir çocuk → "Bir yaş büyüt" / "Yetişkinlik yaşına getir" → ertesi gün yetişkin, oyunun yetişkinlik bildirimi
- [ ] "Yaşlanmayı dondur" açık bir kahraman bir yıl sonra aynı yaşta
- [ ] Başka bir çifti "Boşa" → evlilikle gelen taraf ailesine döndü; kendi eşinde entrika boşanması (sonuçlarıyla)
- [ ] Kaydet → yükle → "çocuk istemiyorlar" ve dondurma duruyor

### Eşinin gizli hayatı ve boşanma (yeni, ayar varsayılan açık)
- [ ] Kişiler sekmesinde eşin listenin sonunda; dosyasında "Eşini araştır", "Boşa..."
- [ ] Hile ile eşinle ilişkiyi düşür (ör. -30) ve uzun süre ayrı kal; birkaç ay sonra "Eşini araştır" (1.000, 14 gün) →
      "Süren"de araştırma satırı → "Eşin seni aldatıyor: X" kartı (sevgili, süre, varsa çocuklar)
- [ ] Kişiler'de sevgili X için "Gizli ilişki" sırrı; eşinden doğan çocuk varsa "Gayrimeşru çocuk ... resmî babası sensin"
- [ ] **Yüzleş:** bazen bitiriyor (ilişki -10), bazen bitirmiyor (-20)
- [ ] Hiçbir şey yokken araştırma: "kayda değer bir şey bulunamadı"; bazen eşin fark edip kırılıyor
- [ ] **Boşa:** onayda sonuçlar; eşin ailesinin klanına döndü, partinden ayrıldı; klan ekranında artık eşin yok; çocuklar kaldı;
      oyunun kendi evlilik seçenekleri yeniden açık
- [ ] Eşin hamileyken "Boşa" kapalı ("doğumdan sonra")
- [ ] Casus başı karşı casusluk görevindeyken ilişki kendiliğinden ortaya çıkabiliyor
- [ ] MCM > Entrika > "Eşin seni aldatabilir" kapalıyken araştırma seçeneği yok, ilişki başlamıyor; boşanma yine açık
- [ ] Kaydet → yükle → ilişki, araştırma ve gizli gebelik aynı

### P2.6 — Kur, sevgili, eşini bilgi için göndermek, gebelik (yeni)
- [ ] Kişiler ya da Saray'da başka klandan karşı cinsten bir yetişkin lord: "Kur yap..." → kart (başarı, yakalanma) → başla, 500 dinar
- [ ] "Süren" sekmesinde "KUR, SEVGİLİLER, GEBELİKLER": aşama adı, kalan gün; 21 gün sonra "Sevgilin oldu" ya da "Kur tutmadı" kartı
- [ ] Sevgili: "Gizli buluşma..." kartı (gebelik %15 / tedbirli %4, yakalanma); buluşunca ilişki +5; 14 gün bekleme
- [ ] Birkaç buluşma sonra (ya da şansla) "X hamile kaldı" → Süren'de doğuma kalan gün → "Doğum" kartı; çocuk klan ekranında
- [ ] Görülme: "herkes biliyor", eşin ve sevgilinin eşiyle ilişkiler düştü
- [ ] Evliysen: "Eşini gönder: bilgi için kur..." → kart (eşinin kabul ihtimali) → onurlu eş çoğu zaman reddediyor (ilişki -10)
- [ ] Eşin kabul ederse 21 gün sonra "Eşinin görevi bitti": rapor, bazen sır; bazen "yakınlık oldu" (hedef evliyse sırrı oluyor)
- [ ] Eşin hamile kalırsa doğumda resmî baba sen, gerçek baba hedef; hedef için gayrimeşru çocuk sırrı Kişiler'de
- [ ] Oyunun Doğum ve Ölüm ayarı kapalıyken gebelik seçenekleri yok
- [ ] Insert → Entrika → **Kur ve sevgililer**: kur yap / eşini gönder listesi, buluşma, bitir
- [ ] Kaydet → yükle → kur, sevgili, gebelik aynı

### Gece evi işletmesi (yeni, varsayılan kapalı)
- [ ] MCM > Entrika > **Gece evi işletmesi** aç
- [ ] Gölge ağın olan şehirde > Gölgelerde iş çevir: "Gece evi aç (4.000, 2 gönüllü çalışan)" → açıldı; menü açıklamasında gece evi satırı
- [ ] "Çalışan al" sınırı ağ seviyesine göre (4/6/8); pay seçenekleri (adil/standart/düşük) ayrılma oranlarıyla yazıyor
- [ ] Bir hafta sonra "Gece evlerinden haftalık gelir" mesajı; altın arttı; kronikte kayıt
- [ ] Bir süre sonra "Gece evinde duyulanlar: Lord" kartı; Kişiler'de o lordun güncel raporu (bazen sır)
- [ ] Düşük payda birkaç hafta: "ayrılanlar" kartı, çalışan sayısı düştü
- [ ] Entrika ekranı > Ağlar: şehrin kartında gece evi satırı
- [ ] Ağı dağıt → gece evi de kapandı
- [ ] Ayarı kapat → menüde gece evi seçenekleri yok, haftalık gelir gelmiyor
- [ ] Kaydet → yükle → ev, çalışan sayısı ve pay aynı

### P2.5 — Ağ ifşasından diplomatik kriz (yeni)
- [ ] Hile ya da sabırla barışta olduğun bir krallığın şehrinde gölge ağı kur; çökertilince tek kart: "Diplomatik kriz: Krallık"
      (ne oldu, suç puanı, gerilim, üç seçenek; resim: elçi)
- [ ] Kart sonrası o krallığın şehrinde oyunun kendi suç göstergesi yükselmiş (muhafızlar sert); **Tazminat öde** → suç siliniyor
- [ ] **İnkâr et** ve **Reddet** sonuç kartları; hükümdarla ilişki ve "gerilim a -> b" yazıyor
- [ ] Entrika ekranı > Saray > o krallık: "casus gerilimi: gergin (N/100)"; haftalar geçtikçe azalıyor
- [ ] Gerilim 60+ (birkaç reddedilmiş skandal): bir hafta "Savaş meclisi" kartı; o krallığın kararlarında savaş oylaması görünüyor;
      vasalsan hükümdarınla -20
- [ ] Kendi krallığının bir şehrinde ağ çökerse kriz yok, yalnızca bildirim ve hükümdarla ilişki düşüşü
- [ ] Savaşta olduğun krallıkta ağ çökerse kriz yok
- [ ] Kriz kartı açıkken kaydet → yükle → kart yine soruluyor

### P2.4 — Gizli ilişki ve gayrimeşru çocuk sırları (yeni)
- [ ] Evli lordlara casusluk/sorgu → bir süre sonra "SIR BULUNDU: Gizli ilişki - X ile evli olduğu hâlde Y ile..." (resim: gizli buluşma)
- [ ] Kişiler sekmesinde sevgili Y de görünüyor, onda da aynı sır var; ikisine de **Şantaj** başlatılabiliyor
- [ ] Sevgili gerçekçi: yetişkin, yaşları yakın, akrabası değil, çoğunlukla aynı krallıktan; senin klanından hiç kimse değil
- [ ] Aynı lorda tekrar casusluk → bazen "Gayrimeşru çocuk - Kızı/Oğlu Z'nin gerçek babası Y; resmî babası W bunu bilmiyor"
- [ ] **İfşa et** → "Skandal" kartı: eşler, kayın, (çocukta) resmî babayla ilişkiler düşüyor; iki kişide de sır "ifşa edildi"
- [ ] **Eşine söyle** → kart: aldatılan eş sana +10, aldatanla arası düştü; kayın ve hükümdar etkilenmedi; sır "eşine söylendi"
- [ ] Şantaj sürerken iki taraf için de İfşa et / Eşine söyle kapalı (kırmızı not)
- [ ] Ansiklopedide çocuğun aile ağacı değişmedi
- [ ] MCM > Entrika > **Gizli ilişki ve gayrimeşru çocuk sırları** kapat → yeni casuslukta bu sırlar çıkmıyor; bilinenler duruyor
- [ ] Insert → Entrika → İstihbarat raporları → lord → "Eşine söyle (X): ..." çalışıyor
- [ ] Sır bulunduktan sonra kaydet → yükle → sevgili ve çocuk aynı

### P2.2 — Haberci, saray istihbaratı, ikna bonusu (yeni)
- [ ] Entrika ekranı → **Saray**: « Önceki / Sonraki » ile krallıklar; hükümdar portresi, güç karşılaştırması, bilgi kaynağı yazısı
- [ ] Raporun olan lordda "Hükümdarıyla: hoşnutsuz (-24)" gibi tam sayı; gölge ağın olan krallıkta "(söylenti)"; diğerlerinde "bilinmiyor"
- [ ] Hoşnutsuzlar kırmızı ve listenin başında; "Casusluk (tam bilgi için)..." Yeni entrika'yı o lordla açıyor
- [ ] **Haritada izle** → lord haritada izleme işaretiyle görünüyor
- [ ] Raporun olan bir lordla konuşup **krallığına davet et** (oyunun ikna turları): Saray'da "ikna +10" (sırrı da varsa +15) yazıyor;
      ikna turlarındaki başarı yüzdeleri raporsuz bir lorda göre daha yüksek görünüyor
- [ ] Kişiler → kozun olduğu uzak bir lord: koz düğmelerinde "(haberciyle N gün, … dinar)"; seç → haberci yola çıktı,
      "Süren" sekmesinde **Yoldaki haberciler** altında; varınca sonuç kartı
- [ ] Aynı lorda yan yana: koz düğmeleri haberci yazısız, hemen çalışıyor
- [ ] Kişi dosyasında "Hükümdarı X ile: ..." satırı, Haritada izle ve Söylenti yay düğmeleri
- [ ] Haberci yoldayken kaydet → yükle → haberci hâlâ yolda, aynı gün varıyor
- [ ] **Ulak:** Saray'da uzak bir lord → "Ulak gönder: görüşme + hediye..." → kart: hediyesiz / 1.000 / 3.000 / 6.000 (ilişki ve toplam
      bedel yazıyor) → seç: altın düştü, "Yoldaki haberciler"de "Görüşme daveti (hediye …)"
- [ ] Ulak varınca kart: "Ulak döndü: X" (hediye ilişkisi yazıyor) → **Şimdi görüş** → lordla konuşma penceresi açılıyor, lord
      uzakta olduğu hâlde; krallığa davet (ikna turları) çalışıyor; konuşma bitince harita normal
- [ ] Aynı kartta **Sonra** → Saray'da ve kişi dosyasında "Görüş: konuşma penceresi (N gün)" düğmesi; 5 gün sonra kayboluyor
- [ ] Aynı lorda 30 gün içinde ikinci hediye: kart "bu kez hediye ilişki getirmez" diyor, ilişki +0
- [ ] İlişkisi −30'dan kötü bir lord: ulak geri çevriliyor, hediye geri geliyor
- [ ] ⚠️ Savaştığın krallığın bir lorduyla ulak görüşmesi: konuşmada saldırı/teslim seçenekleri çıkıyor mu, seçince çökme var mı?
      (varsa ekran görüntüsü)
- [ ] Insert → Entrika (liste menüsü) → **Ulak ve görüşme**: hazır görüşmeler ve "Ulak gönder..." listesi çalışıyor

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
- [ ] **Hizmetine davet:** evli olmayan, klan lideri olmayan bir esir → "Esirle konuş..." → "Hizmetine davet et (%N)"
- [ ] Kabul: kart (yemin); esir partinde ve klan ekranında, ona parti/vali görevi verilebiliyor; eski klan lideriyle ilişkin düştü
- [ ] Ret: esir partinde kalıyor; tekrar denemek "N gün sonra yeniden" diyor; oyunun fidye teklifleri gelmeye devam ediyor
- [ ] Klan lideri ve evli esirde seçenek kapalı, nedeni yazıyor

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
