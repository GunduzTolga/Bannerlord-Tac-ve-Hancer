# Yapılacaklar

Son güncelleme: 20 Ekim 2026 · Mod durumu: Faz 1, 2, 3A, 3B derlendi ve oyuna kuruldu.
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

### Tek mod: Diplomacy, DLX Hair Layers ve Women in Calradia içimizde (yeni, 20 Ekim)
Üçü ayrı mod değil, Taç ve Hançer'in klasörüne paketlendi (`third_party\BENIOKU.md`). Ayrı klasörleri oyundan silindi,
launcher listesinden çıkarıldı (yedek: `Configs\LauncherData.xml.yedek-20261020-000338`). Çevrimdışı doğrulandı: Diplomacy
yükleyicisi kendini buluyor, bütün DLL'ler yükleniyor, Women in Calradia ayar dosyası pakettekine yönleniyor.
- [ ] Launcher'da yalnızca **Taç ve Hançer** var (Diplomacy, DLX Hair Layers, Women in Calradia ayrıca görünmüyor); oyun açılıyor
- [ ] Ana menüde kırmızı uyarı ya da "modül yüklenemedi" penceresi yok
- [ ] Krallık ekranında Diplomacy'nin düğmeleri ve sekmeleri; **Mod Seçenekleri**'nde Diplomacy ayarları
- [ ] Karakter düzenleyicide DLX saç katmanları paneli; envanterde "Hair under helmet" düğmesi
- [ ] Yeni kampanyada Women in Calradia yoldaşları tavernalarda; **yüzleri düzgün** (doku bozuksa ekran görüntüsü at)
- [ ] Mod Seçenekleri'nde Women in Calradia ayarları
- [ ] **AutoTrader** (20 Ekim'de eklendi): kasaba/köy menüsünde "Automatically trade wares" seçeneği; **Alt+A** ayar ekranını açıyor;
      bir kasabada otomatik al-sat yapıyor. Açılışta "1.4.6 için" uyarısı çıkabilir, önemsiz
- [ ] **Regard and Standing** (20 Ekim'de eklendi; lordlar arası gizli ilişkilerin sahibi o, bizim casus başımız onun gerçek
      ilişkilerini buluyor): Mod Seçenekleri'nde "Regard and Standing" sayfası; turnuva davet mektupları; klanındaki bir evlilik onay
      istiyor. Gizli ilişki denemeleri: `docs\superpowers\specs\2026-10-20-regardstanding-iliskiler.md` sonundaki liste
- [ ] **Extra Complexion** (20 Ekim'de eklendi): karakter düzenleyicide yara, makyaj, dövme ve savaş boyası seçenekleri; DLX saç
      katmanları paneliyle **üst üste binmiyor mu** (ikisi de düzenleyiciye panel ekliyor; ekran görüntüsü at). Women in Calradia
      yoldaşlarının yüzleri hâlâ düzgün (iki modun ten ayarları tek dosyada birleşti)
- [ ] **Companion Autonomy** (20 Ekim'de eklendi): Klan > Birlikler'de yoldaş partisinin satırında harçlık ve ayarlar; bir hafta sonra
      yoldaş kendi kesesinden teçhizat ya da asker almış mı (mesajlar); Mod Seçenekleri'nde ayar sayfası

### Entrika ekranı yeni düzen: her şey tek yerde (21 Ekim, yeni; en çok ekran görüntüsü lazım)
Lordlar, İsyanlar ve Barış Masası artık ayrı pencere değil, Entrika ekranının sekmeleri. Tasarım: `docs\superpowers\specs\2026-10-21-arayuz-duzeni.md`.
- [ ] Üstte dört bölüm (Entrika / Kişiler / Saray / Savaş ve barış), altında o bölümün alt sekmeleri; bölüm değişince alt sekmeler değişiyor
- [ ] Casus başı sütunu yalnızca Entrika bölümünde; ötekilerde içerik tam genişlik. Yazılar ve kartlar taşıyor mu
- [ ] Kişiler > **Lordlar**, Savaş ve barış > **Savaşlar** (solda savaşlar, sağda Barış Masası) ve **İsyanlar** açılıyor
- [ ] Haritadaki savaş yorgunluğu göstergesi → Savaş Raporu (pencere) → "Barış Masası..." → rapor kapanıp Savaşlar sekmesi o savaşla açılıyor
- [ ] Krallık ekranındaki Diplomacy grup düğmesi → Krallık ekranı kapanıp İsyanlar sekmesi açılıyor (başka krallıkta "Kendi krallığın" düğmesi)
- [ ] Saray'da seçili krallığın "İsyanlar..." düğmesi; Divan'daki "Barış Masası" düğmesi; elçi kartındaki "Barış Masası'na otur"
- [ ] Insert > Entrika'daki Lordlar / İsyanlar / Barış Masası girişleri ilgili sekmeyi açıyor
- [ ] Klan ekranının Entrika sekmesinde de, ayrı Entrika ekranında da aynı

### Savaş ve barış > Krallıklar: ittifak, ticaret, saldırmazlık, savaş ilanı (21 Ekim, yeni; ekran görüntüsü çok işe yarar)
Bütün krallıklar ve bağların tek sekmede. Tasarım: `docs\superpowers\specs\2026-10-21-kralliklar-sekmesi.md`.
- [ ] Savaş ve barış bölümünde alt sekmeler: Krallıklar · Savaşlar · İsyanlar. Solda krallıklar (savaştıkların kırmızı, müttefik/saldırmazlık/ticaret altın)
- [ ] Sağda seçilinin kartı (sancak, hükümdar portresi, güç, yayılmacılık), DURUM satırları ve NE YAPABİLİRSİN düğmeleri; yazılar taşıyor mu
- [ ] Hükümdarken: İttifak öner (kabul etmiyorsa nedeni yanında), Ticaret anlaşması öner, Saldırmazlık öner, Savaş ilan et; bedeller (nüfuz/altın) düşüyor mu
- [ ] Müttefikle: "Savaşa çağır: X" → müttefik savaşa giriyor mu, **altın bir kez mi düşüyor** (iki kez ya da hiç düşmüyorsa yaz); "İttifakı boz"
- [ ] Kalan günler doğru mu (ittifak, ticaret, saldırmazlık); Krallık ekranı > Diplomasi'dekiyle karşılaştır
- [ ] Savaştığın krallıkta "Barış Masası'na otur" Savaşlar sekmesini o savaşla açıyor; "Savaş Raporu" açılıyor
- [ ] Hükümdar değilken yalnızca ulak ve "anlaşmaları hükümdarın yapar" notu; krallığın yokken de liste ve ulak
- [ ] Insert > Entrika > "Krallıklar" sekmeyi açıyor

### Saray > Krallık: bağış, toprak verme, iade (21 Ekim, yeni)
Diplomacy'nin bağış ve fief pencereleri artık Entrika ekranının Saray > Krallık sekmesi. Tasarım: `docs\superpowers\specs\2026-10-21-krallik-sekmesi.md`.
- [ ] Saray bölümünde alt sekmeler: Saray · Krallık · Divan · Kronik. Krallık: solda klanlar (sancak, lider, ilişki, yer, güç), sağda seçili klan
- [ ] Altın bağışla: miktarı +/- ile seç, "ilişki +N" önizlemesi Diplomacy'nin penceresindekiyle aynı mı; Bağışla → kese azalıyor, ilişki artıyor
- [ ] Hükümdarsan: Toprak ver listesinde şehir ve kalelerin; birine tıkla → onay → yer o klana geçiyor, ilişki artıyor; az yeri olanlar kıskanıyor
- [ ] Hükümdar değilsen: "Toprak vermek hükümdarın işi" notu; paralı klan seçince "Paralı klanlara toprak verilmez"
- [ ] İade: barıştaki bir krallıktan aldığın yer listede mi; iade et → eski sahibine dönüyor, ilişki artıyor
- [ ] Krallık ekranı > Klanlar'daki Diplomacy düğmeleri ("Altın Bağışla", "Tımar ver") ve ansiklopedideki "Tımar ver": Krallık ekranı kapanıp
      Krallık sekmesi o klan seçili açılıyor mu; Diplomasi'deki "Return Fief" düğmesi (çevirisi yok) Krallık sekmesini açıyor mu
- [ ] Krallıkta değilken bu düğmeler Diplomacy'nin kendi penceresini açıyor (ya da hiç görünmüyor)

### Divan: diplomat (21 Ekim, yeni)
- [ ] Entrika ekranı > Divan: beş kart yan yana sığıyor mu (yazılar taşıyor mu, "Azlet" düğmesi), DİPLOMAT kartı: Charm, katkılar, görev
- [ ] Ata (Charm'ı yüksek bir yoldaş) → raporda DİPLOMASİ: her krallık için "barış yanlısı / savaş konuşuluyor / sana savaş açmaya
      hazırlanıyor" ve hükümdarıyla ilişkin; savaştıkların için pazarlık gücün ve **Barış Masası** düğmesi
- [ ] Bütün krallıklar hep aynı basamakta mı görünüyor? (öyleyse yaz: eşik ayarlanacak)
- [ ] "İlişki görevi..." → bir hükümdar seç → her hafta ilişki artıyor (kartta "Görevi: X"); 60'a varınca bildirim
- [ ] Barış Masası'nda "diplomatın (+N)" görünüyor; Lordlar penceresinde çağır/davet olasılıkları diplomatla yükseliyor
- [ ] Bir krallık savaşa hazırlanınca "Diplomatın uyarıyor: X" kartı
- [ ] Insert > Entrika > Divan > Diplomat menüsü çalışıyor
- Tasarım: `docs\superpowers\specs\2026-10-21-divan-diplomat.md`

### Barış Masası: şartlı barış (21 Ekim, yeni; hükümdar olmalısın; ekran görüntüsü çok işe yarar)
- [ ] Entrika ekranı > Savaş ve barış > **Savaşlar**: solda savaşların (sancak, kaç gündür, güç, yorgunluk, şartsız barış %), sağda seçilinin
      masası. Savaş Raporu'ndaki **Barış Masası...** ve Insert > Entrika > **Barış Masası...** buraya getiriyor
- [ ] Masa: üstte pazarlık gücün ve nedenleri; solda bölümlü istediklerin (**Para**: haraç, tazminat · **Asker ve esirler** · **Toprak**:
      sınırına yakın şehir/kaleler · **Anlaşmalar**: rehine, saldırmazlık, ticaret, ittifak), sağda verdiklerin (altın, haraç, esirlerini
      bırak); altta özet, **Kabul olasılığı: %X**. Şart ekledikçe olasılık düşüyor mu
- [ ] Yeni şartlar: esir lordların serbest kaldı mı; rehine senin şehrine geldi mi (Saray'da rehine anlaşması görünüyor mu); saldırmazlık
      kuruldu mu; ticaret anlaşması ve ittifak kuruldu mu (kurulamazsa sonuç kartında oyunun nedeni)
- [ ] **Teklif et** → kabul: barış, kale/şehir senin (klanında), tazminat kesende, askerler partinde, haraç her gün geliyor (klan gelirinde);
      Savaş Raporu ve masa kapanıyor, sonuç kartı. Ret: kart, 7 gün yeniden teklif yok (masa nedenini yazıyor)
- [ ] Kaybederken: "Altın ver" ya da "Haraç öde" ile şartsız barışın olasılığı artıyor mu
- [ ] **Elçi:** çok öndeyken birkaç hafta içinde "BARIŞ ELÇİSİ" kartı: tazminat teklifi; Kabul et / Barış Masası'na otur (pazarlık gücüne
      +15) / Geri çevir
- [ ] Diplomacy'nin kısa savaş sınırı varken masa "savaş yeterince uzun sürmedi" diyor; iç savaşta masa kurulmuyor
- [ ] Devralınan kalenin garnizonu ve köyleri sana geçti mi; barıştan sonra esir lordların serbest kaldı mı (bak, yaz)
- Tasarım: `docs\superpowers\specs\2026-10-21-baris-masasi.md`

### Savaş Raporu: yeni pencere (21 Ekim, yeni; ekran görüntüsü çok işe yarar)
Haritadaki savaş yorgunluğu göstergesine tıklayınca Diplomacy'nin raporu yerine bizimki açılıyor; üst kısım aynı, altı yeni.
- [ ] Üst kısım eskisi gibi mi: iki büyük sancak, "Savaş Raporu" başlığı, iki tarafın savaş yorgunluğu, ilişki kutuları, tarih; yeni olarak
      "savaş N gündür sürüyor" satırı. Sağ üstte Kapat; Esc de kapatıyor
- [ ] Altta iki sütun (kaydırılıyor): **Savaşın gidişatı** (üstünlük puanı, kayıplar, fethedilen yerler, yağmalar, esir lordlar),
      **Kuvvetler** (güç, ordular, klanlar, lordlar, şehir/kale/köy, müttefikler, başka cepheler), **Savaş yorgunluğu** (Diplomacy'nin
      dökümü); sağda **Mareşalin raporu**, **Barış**, **Esirler**, **Savaş günlüğü**
- [ ] Satırların altındaki çubuklar iki krallığın renginde mi, oranlar mantıklı mı; yazılar taşıyor mu
- [ ] "Savaştaki üstünlük" ve yorgunluk satırlarındaki sayıların üstüne gelince ipucu çıkıyor mu
- [ ] Barış bölümü: iki tarafın tavrı ve "Barış teklif edebilirsin" ya da Diplomacy'nin nedeni (ör. savaş yeterince uzun sürmedi)
- [ ] Savaş günlüğünde bu savaşın olayları (savaş, kuşatma, esir alma) "N gün önce" ile
- [ ] Insert > Entrika > **Savaş raporları** → savaştığın krallığı seç → aynı pencere
- [ ] Pencere açılamazsa kırmızı mesaj ve Diplomacy'nin eski raporu açılıyor (olursa ekran görüntüsü ve `rgl_log`)
- Tasarım: `docs\superpowers\specs\2026-10-21-savas-raporu.md`

### Lordlar: yanına çağır ve ziyafet (21 Ekim, yeni; ekran görüntüsü çok işe yarar)
- [ ] Entrika ekranı > Kişiler > **Lordlar** (Insert > Entrika > "Lordlar" de buraya): süzgeçler (Krallığın / Tanıdıkların / Davetliler),
      sağ üstte Ziyafet ver ve koşulu
- [ ] Lord kartı: portre, ad, klan, nerede ve kaç gün yol, seninle ilişki, hükümdarıyla arası, son görüşme, etiketler; yazılar taşmıyor
- [ ] **Yanına çağır:** aynı krallıktan biri 20 nüfuz, başkası 800 altın; kabul ederse partisi haritada sana doğru geliyor (kartta
      "sana doğru yolda"), yakına gelince "yanına geldi" mesajı → üstüne tıklayıp konuş. Reddederse bedelin yarısı geri dönüyor
- [ ] Gelmeye çalışırken orduya katılır, esir düşer ya da 3 gün geçerse "Gelemedi" bildirimi; lord yeniden kendi işine dönüyor (takılı
      kalmıyor mu, bak)
- [ ] **Ziyafet:** kendi şehrinde ya da kalende, kartlardan en fazla 6 lord davet et → Ziyafet ver → olay kartı: gelenler, gelmeyenler,
      ilişki artışı, nüfuz, bazen "şarap dilleri çözdü" (rapor, belki sır). Para düştü; 30 gün yeniden verilemiyor (düğme gri, nedeni yazıyor)
- [ ] Kendi yerleşiminin dışında düğme gri ve nedeni yazıyor; davet listesi pencere kapanıp açılınca duruyor
- [ ] Kayıt al, yükle: yolda olan çağrılan lord yine geliyor
- Tasarım: `docs\superpowers\specs\2026-10-21-lordlar.md`. Oylamada destek sözü yapılmadı (oyunun seçim kodu bulutta görünmüyor).

### İsyanlar ve iç savaş (21 Ekim, yeni; ekran görüntüsü çok işe yarar)
Diplomacy'nin "Factions" penceresi yerine Entrika ekranı > Savaş ve barış > **İsyanlar** açılıyor; katıl/ayrıl/isyan/destek düğmeleri yine
Diplomacy'nin kuralıyla çalışıyor. (Aşağıdaki "pencere" maddeleri artık bu sekme için.)
- [ ] Krallık ekranında Diplomacy'nin grup (Factions) düğmesi → **İsyanlar ve İç Savaş** penceresi: başlık, krallık ve hükümdar,
      özet ("N grup · en güçlüsü X: %57, gereken %65"), sağ üstte **Grup kur...** ve Kapat. Insert > Entrika > "İsyanlar ve iç savaş" da açıyor
- [ ] Grup yoksa: grupların nasıl kurulduğunu anlatan metin (boş kutu değil)
- [ ] Grup kartı: önderin portresi, "X önderliğinde: ayrılık / tahttan indirme", istek, durum (destek topluyor / isyana hazır / iç savaş),
      katılanlar ve hükümdarla ilişkileri, güç çubuğu (kırmızı dolgu = grubun payı, açık çizgi = gereken pay), "en gönülsüz üye" notu
- [ ] Düğmeler: Gruba katıl (olmuyorsa nedeni yanında) / Gruptan ayrıl / İsyanı başlat (önder sensen; onay soruyor) / Destek topla
- [ ] **Hoşnutsuzlar** bölümü: Saray'daki lord kartları (hükümdarına küs olanlar), ulak, haritada izle, söylenti yay düğmeleriyle
- [ ] Grup kur... → Diplomacy'nin seçimi (ayrılık / tahttan indirme, nüfuz bedeli) → pencere yenileniyor, yeni grup kartı geliyor
- [ ] Esc ve Kapat pencereyi kapatıyor (Krallık ekranı açık kalıyor mu, bak); yazılar taşmıyor, kaydırma çalışıyor
- [ ] Ansiklopedide bir krallığın sayfasındaki grup düğmesi de bu pencereyi açıyor

### Esiri gizlice öldürt (21 Ekim, yeni)
- [ ] Klan > Entrika > Esirler: her esirde kırmızı **Gizlice öldürt...** düğmesi → kart: işi kim ayarlıyor (Roguery), üç yöntem
      (Zindan hastalığı 1.500 / Kaza 1.000 / Kaçarken öldürüldü 500), her birinde "şüphe %X, ifşa %Y"
- [ ] Esir kartı > **Kaderini belirle...** > Hizmetine davet et ve Gizlice öldürt aynı yerde
- [ ] Yöntem seç → para düştü, esir öldü (oyunun ölüm mesajı: hastalıkta doğal ölüm, ötekilerde yaralar); sonuç kartında temiz / şüphe /
      ifşa ve ilişki değişimleri; Merhamet düştü
- [ ] Roguery'si yüksek casus başıyla yüzdeler düşüyor; yeni yakalanan, hükümdar ya da partindeki esirde yükseliyor
- [ ] Uzaktaki bir zindandaki esirde de çalışıyor
- Tasarım: `docs\superpowers\specs\2026-10-21-gizli-infaz.md`

### Düzeltmeler ve Diplomacy bağlantıları (21 Ekim, yeni; bulutta derlendi ve test edildi)
- [ ] **Varis:** sevgilisi olan bir karakterle klanı devret (Entrika > **Hanedan**) → yeni karakterde eski sevgili ve kur yok, divan koltuğu
      varisin kendisiyse boşaldı; Kronik'te "Klanın başına X geçti"
- [ ] **Şantaj:** bir sırla şantaj tut → sır listesinde "kozda harcandı"; aynı lorda yeniden şantaj "kullanılmamış sırrı yok" diyor
- [ ] **Rehine:** divandaki birini rehine olarak gönderemiyorsun (listede yok); misafir rehineye suikast/kaçırma "yeminini bozar" diyor;
      rehine giderken partinden çıkıyor
- [ ] Krallığın varken rehine anlaşması → kartta "N günlük saldırmazlık anlaşması"; Krallık > Diplomasi'de anlaşma görünüyor;
      anlaşmayı erken bitirince o da bitiyor
- [ ] **Kur ve sevgililer** MCM'de ayrı anahtar; "Gizli ilişki sırları" kapalıyken kur yine çalışıyor
- [ ] "Eşin seni aldatabilir" kapalıyken eşinin (Regard and Standing'in kurduğu) ilişkisini araştırabiliyorsun
- [ ] **Savaş Raporu:** altta mareşalin satırları (güç tahmini sol/sağ, sahadaki ordular, topraklarına yürüyen, tavsiye); mareşal yoksa
      "Mareşalin yok" satırı; pencere satırlara göre uzuyor, kapat düğmesi yerinde
- [ ] **Muhalefeti izle** raporunda (krallıkta isyancı grup varsa) "Örgütlü muhalefet": önde kim, kaç klan, istek, güç
- [ ] **Ulak kartı:** ilk seçenek "Görüşme: Diplomacy habercisi (bedel, süre)" → Diplomacy'nin habercisi gidiyor, varınca konuşma;
      hediyeli ulak varınca "Hediyen ulaştı" kartı (artık bizim görüşme penceremiz açılmıyor)
- [ ] **Hazinedar:** Klan > gelir açıklamasında "Hazinedar X" satırı; atölye+kervan gelirinin Trade/10 yüzdesi kadar (en fazla %20)
- [ ] **Kâhya:** valisi olan şehirde sadakat açıklamasında kâhya satırı yok; valisizde var
- [ ] Entrika menüsünde "Kayıt testi" yok, "Hanedan" var; şehir menüsünde "Saray günü düzenle (dilekçeler)"
- [ ] Genelev > Yoldaşlar > Yönetici ata: listede **eşin** de var (isterse yönetir; çalışan olarak hiç sorulmaz)

### Casus başı: Ajan yetiştir (21 Ekim, yeni)
- [ ] Yoldaşı casus başı ata (krallık gerekmez), Görev → **Ajan yetiştir**; iki hafta sonra "yeni ajan" mesajı, haftada 200 altın düştü
- [ ] Görev satırında "· ajan N"; yeni entrikanın başarı yüzdesi ajan başına +2 arttı; bakım ajan başına haftada 50 altın
- [ ] Bir entrikan ortaya çıkınca sonuçta "Ajanlarından biri yakalandı (kalan N)"; kaydet-yükle sonrası ajan sayısı duruyor
- Tasarım: `docs\superpowers\specs\2026-10-21-ajan-yetistir.md`

### Karar bekleyen modlar (20 Ekim'de incelendi, eklenmedi)
- **Convivium: Living Feasts:** lordlar salonda gerçekten toplanıyor, yapay zekâ da ziyafet veriyor; ziyafette ikna, askerlere
  ziyafet, turnuva. Ama kur/özel sahne, gebelik/babalık ve onur düellosu kısımları bizim kur, gebelik ve düello sistemlerimizle
  aynı işi yapıyor (gizli ilişkiler de RegardStanding'de). Önerim: eklenirse bu dört kısım kapalı başlasın (onun Mod Seçenekleri'nde
  ayrı ayrı anahtarları var). İki teknik iş: kendi MCM kopyası (farklı sürüm) pakete alınmamalı; arayüz eklentisini
  `GetModuleFullPath("ConviviumFeasts")` ile arıyor, BundleShim'e genel bir "paketlenen modun klasörü = bizimki" yönlendirmesi gerekir.
- **Diplomacy & Intrigue (DiplomacyIntrigue 0.4.0):** büyük bir diplomasi + entrika sistemi (antlaşmalar, vasallık, barış masası,
  savaş yorgunluğu, iç savaş, saray, istihbarat, suikast). Hem Diplomacy'nin (paketli) hem bizim entrikamızın işini yapıyor; ikisiyle
  birlikte açılırsa aynı şeyi iki-üç sistem yönetir. Karar: Diplomacy mi kalsın, D&I mı gelsin (o zaman çakışan entrika kısımları
  bizde kalmalı), yoksa yalnızca iyi fikirleri (vasallık, barış masası) bizim koda mı alınsın?
- [ ] Bu modlar ayrıyken alınmış bir kayıt varsa, yüklerken "eksik modül" uyarısı çıkabilir; yine de yüklenir
- [ ] Yeni oyun: karakter oluşturma çökmeden geçiyor (eski çökmenin nedeni bulundu: lord sayfasının `Refresh` metodu; Diplomacy
      ve DLX Hair'in açılışta yamaladığı 37 metodun hiçbiri o tetiği taşımıyor)
- [ ] Hile menüsü → iki krallığa savaş ilan et; aralarında Diplomacy saldırmazlık anlaşması varsa mesajda
      "aradaki saldırmazlık anlaşması bozuldu" yazıyor ve Krallık → Diplomasi'de anlaşma artık görünmüyor

**Açılışta "Taç ve Hançer.RegardStanding submodule could not be loaded correctly due to a dependency conflict" (20 Ekim akşamı):**
neden bulundu (`tools\RefCheck`, Regard and Standing 1.5.0'ın DLL'iyle). Regard and Standing, isteğe bağlı ortağı **Calradia
Remembers**'la konuşan tek bir sınıf taşıyor (`PairRegardSource+Source`, o modun bir arayüzünü uyguluyor). Calradia Remembers kurulu
değilken oyun o sınıfı yükleyemiyor ve pencereyi gösteriyor. Mod bunu bekliyor: köprüyü yalnızca Calradia Remembers varsa açıyor,
geri kalan her şey çalışıyor. Yani **OK'e basınca oyun devam etmeli**; ayrı kurulumda da aynı pencere çıkardı.
- [x] Yalnızca sınıfın arayüzünü sökmek yetmedi (oyun DLL referanslarına da bakıyor) → **Calradia Remembers bağlantısı DLL'den
      tamamen söküldü** (`tools\RsPatch`: 16 üye silindi, 13 metot boşaltıldı, 3 DLL referansı kaldırıldı; kalan 20.945 çağrının hepsi
      çözülüyor). Repodaki `third_party\RegardStanding\bin\Win64_Shipping_Client\RegardStanding.dll` yamalı
- [ ] Yamalı `RegardStanding.dll`'i iki yere koy: oyunda `Modules\CheatMenu\bin\Win64_Shipping_Client\` (hemen etki eder) ve
      masaüstündeki repoda `third_party\RegardStanding\bin\Win64_Shipping_Client\` (derleme eskisini geri koymasın). Oyun açılıyor,
      pencere çıkmıyor
- [ ] Hâlâ çıkıyorsa: en yeni `rgl_log` (`C:\ProgramData\Mount and Blade II Bannerlord\logs`) ve pencerenin ekran görüntüsü

**Uyum raporu (yeni, 20 Ekim akşamı):** kampanya her açıldığında mod, bütün modların oyuna yaptığı Harmony yamalarını ve eklediği
oyun modellerini tarar; birden çok modun aynı yere dokunduğu her noktayı risk seviyesiyle
`Belgeler\Mount and Blade II Bannerlord\Configs\TacVeHancer\uyum_raporu.txt`'ye yazar. Ciddi ya da dikkat gerektiren çakışma
varsa ekranda mesaj çıkar. CİDDİ: iki mod aynı metodun kodunu değiştiriyor, biri öbürünün işini atlayabiliyor ya da bir modun
modeli öncekileri çağırmadığı için onların etkisi kayboluyor.
- [ ] Bir kayıt yükle → `uyum_raporu.txt` oluştu; dosyayı bana gönder (çakışmaları birlikte çözeriz)
- [ ] Ekranda "Mod uyumu: ... çakışma" mesajı çıktı mı, kaç ciddi var

Çakışma denetimi (20 Ekim): ikna modelini ikimiz de ekliyoruz, bizimki Diplomacy'ninkini çağırıp üstüne ekliyor. Ansiklopedi
lord sayfasında ikimiz de bölüm ekliyoruz, ayrı yerlere. Klan ekranı (biz) ile Krallık ekranı (Diplomacy) ayrı. DLX Hair yalnızca yüz
düzenleyici, envanter ve 3B görünüme dokunuyor. Women in Calradia oyunun kendi gebelik sistemini kullanıyor, yaş eşyası yaşı en az 19'da
tutuyor. Tek boşluk saldırmazlık anlaşmasıydı: savaş ilanında bitmiyordu, artık bizim taraf bitiriyor (`src\Bundle\DiplomacyCompat.cs`).

### E3 — Sabotaj ve kışkırtma (yeni, 20 Ekim)
İlk kez bir **yerleşimi** hedefleyen entrikalar (tasarım: `docs/superpowers/specs/2026-10-20-e3-sabotaj-kiskirtma.md`). Kolay deneme:
Entrika hilelerinden "Süren entrikaların aşamalarını tamamla" ve "Entrikalar her zaman başarılı" (bir yedek kayıtta).
- [ ] Entrika > Yeni entrika: **Sabotaj** ve **Kışkırtma** sekmeleri var; altı sekme tek satıra sığıyor, yazılar taşmıyor (ekran görüntüsü at)
- [ ] Sabotaj sekmesinde liste şehir ve kaleler: sahibinin portresi, başarı, bedel, etiketler (gölge ağı, sadakat, kuşatıyorsun, tetikte).
      Filtreler: Hepsi · Düşmanlar · Gölge ağı olanlar · Kuşattığın · Şehirler
- [ ] **Sabotaj hazırla...** → "neyi vuracaksın" kartı: Erzak ambarı / Garnizon / Surlar → altın düştü, "Süren"de kart (yerleşim + "Hedef: ...")
- [ ] Kuşatmadan **Harekete geç** gri: "Sabotaj ancak X kuşatılırken yapılır". O yeri kuşatınca açılıyor; plan hazırsa kuşatırken
      "plan hazır" kartı kendiliğinden çıkıyor
- [ ] Ambar: kuşatmada erzak %70 düştü (yerleşim ekranında). Garnizon: asker sayısı ~%20 azaldı. Surlar: kuşatma ekranında surlar yarıda,
      çok hasarlı bölüm çöktü
- [ ] Kışkırtma sekmesi yalnızca **gölge ağın olan** başka krallık şehirlerini listeliyor; ağ yoksa liste boş ve nedeni yazıyor
- [ ] Kışkırtmanın her aşamasında o şehrin sadakati 3 düşüyor (mesajda yazıyor)
- [ ] Şehrin içindeyken Harekete geç gri ("önce şehirden çık"); dışarıdayken vurunca sadakat -20, güvenlik -10; sadakat isyan eşiğinin
      altındaysa ve milis güçlüyse şehir **oyunun kendi isyanıyla** ayaklanıyor (harita: yeni isyancı klan)
- [ ] Olaylar: rüşvetçi nöbetçi, elebaşı yakalandı, kaçakçı tüneli (kuşatıyorsan "hemen vur"), ekmek kuyruğu
- [ ] Barıştaki bir krallıkta ortaya çıkınca: sahibiyle ilişki cezası, suç puanı ve Entrika > Saray'da casus gerilimi arttı
- [ ] Yerleşimi ele geçirince entrika "artık senin krallığının" diye kendiliğinden kapanıyor
- [ ] Insert → Entrika → Yeni entrika: Sabotaj ve Kışkırtma; süren entrika ayrıntısı ve Harekete geç çalışıyor
- [ ] Kaydet → yükle: süren sabotaj/kışkırtma (aşama, seçim, olay) duruyor

### P1 — Entrika ekranı, olay kartları, harita düğmesi (yeni; en çok ekran görüntüsü lazım)
- [ ] Ana menüde açılış mesajı: "Insert: menü, Home: entrika ekranı"
- [ ] Haritanın üstünde ayrı bir Entrika düğmesi **yok** (20 Ekim'de kaldırıldı); düğme yalnızca oyunun harita çubuğunda
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
- [ ] MCM → **Arayüz**: kısayol, "Harita çubuğunda Entrika düğmesi" (açık/kapalı, varsayılan açık), resimli kartlar aç/kapa
- [x] **Ansiklopedi bağlantısı ve harita çubuğu düğmesi** PC'de yapıldı (19 Ekim); denemesi aşağıda

### Entrika artık Klan ekranının bir sekmesi (yeni, 19 Ekim; en çok ekran görüntüsü lazım)
Entrika ayrı bir pencere değil: **Klan ekranında Birlikler ile Tımarlar arasında "Entrika" sekmesi** (Diplomacy modunun Krallık
ekranına sekme eklemesi gibi). Arka plan, üst şerit, sancak ve "Bitti" oyunun kendisi; düğmeler, sekmeler ve kartlar oyunun dokularıyla.
- [ ] Klan ekranı (L): sekmeler Üyeler · Birlikler · **Entrika** · Tımarlar · Diğer; beşi üst şeride sığıyor, yazılar taşmıyor
- [ ] Entrika sekmesine tıkla: sağdaki "Mali Durum" kutusu gizleniyor; solda casus başı kutusu (oyunun pencere çerçevesi), sağda alt sekmeler
- [ ] Başka sekmeye geç (Üyeler vb.) → bizim panel kapanıyor, oyunun paneli geliyor; geri dönünce kaldığın alt sekme duruyor
- [ ] Düğmeler oyunun pencere düğmeleri gibi; kartlar ve satırlar taş dokulu (harita olaylarının seçenekleri gibi)
- [ ] **Oyunun kendi fırçaları (20 Ekim):** alt sekmeler (Entrikalar, Yeni, Kişiler...) Klan ekranının üst sekmeleriyle aynı görünüyor
      (`Header.Tab.Center`), ana düğmeler oyunun "Tamam" düğmesi (`Popup.Done.Button`), casus başı kutusu Klan ekranının sol liste
      paneli gibi (`Frame1Brush`), kaydırma çubukları oyunun kendi çubuğu (`Standard.VerticalScrollbar`). `build.ps1` bu adları oyunun
      dosyalarında doğrular: "oyunda böyle bir fırça yok" derse o parça eski görünüşünde kalır (çökmez), hatayı bana gönder
- [ ] Türkçe harfler (ş, ğ, ı, İ) başlık yazı tipinde (Galahad) düzgün mü
- [ ] Sekmeden bir olay kartı açılınca (ör. Harekete geç → rüşvet kartı) kart Klan ekranının üstünde görünüyor
- [ ] Düello ya da sahnede takip seçilince Klan ekranı kapanıyor, sahne haritadan açılıyor
- [ ] "Bitti" ya da Esc Klan ekranını kapatıyor
- [ ] Casus başı / divan koltuğu seçerken listede **yoldaşların portreleri** var (eskiden yalnızca ad vardı)
- [ ] Ayarlar tek seferlik taşındı: eski düğme yeri "Gizli" idiyse harita çubuğu düğmesi kapalı, başka bir yerdeyse açık (MCM'de de öyle)
- [ ] Sekme görünmüyorsa ya da bozuksa: Home yine çalışır, ekran görüntüsü at

### Harita çubuğu düğmesi ve ansiklopedideki Entrika bölümü (PC'de yapıldı)
- [ ] Oyunun sol alttaki harita çubuğunda (Klan ile Krallık arasında) hançer simgeli Entrika düğmesi var (artık varsayılan);
      üst ortadaki eski düğme yok. Tıklayınca Klan ekranı **Entrika sekmesinde** açılıyor
- [ ] **Home** de Klan ekranını Entrika sekmesinde açıyor, tekrar basınca kapatıyor; Insert → Entrika ekranı da aynı
- [ ] Çubuktaki düğmenin üstüne gelince "Entrika" ipucu; harekete hazır suikast/kaçırma ya da sezilen entrika varsa rozet
- [ ] Konuşma sırasında çubuktaki düğme soluk ve nedeni yazıyor
- [ ] Simge boş/bozuk görünüyorsa ekran görüntüsü at (çubuğa eklenemezse düğme hiç çıkmaz; Home ve Klan sekmesi çalışır, mod bozulmaz)
- [ ] Görünüm (20 Ekim): olay kartı oyunun açılır pencere tuvali ve çerçevesiyle; Divan satırları, haberci ve kronik satırları, sırlar, Saray
      kutusu, kişi dosyası taş dokulu kartlarla; kaydırma çubukları oyunun tutamacıyla; düz renkli kutu kalmadı (portre çerçeveleri ve ilerleme
      çubukları hariç). Oyunun kendi ekranlarının yanında yabancı duran bir yer varsa ekran görüntüsü at
- [ ] Ansiklopedide bir lordun sayfası: "Müttefikler" başlığının üstünde **Entrika** bölümü; bildiklerin
      (rapor kaç gün önce, sırlar, koz, süren entrika, hükümdarıyla arası). Başlığa tıklayınca diğerleri gibi katlanıyor
- [ ] **Dosyasını aç** (yalnızca hakkında bir şey biliyorsan açık) → ansiklopedi kapanır, Klan ekranı Entrika sekmesinde, Kişiler'de o kişiyle
- [ ] **Entrika başlat...** → Yeni entrika alt sekmesi tür ve hedef seçili açılır; başlatılamıyorsa altında kırmızı neden
- [ ] Kendi sayfanda bölüm yok
- [ ] Bölüm hiç görünmüyorsa: rgl_log'da `[CheatMenu]` ya da `UIExtender` satırlarını gönder
- [x] **Yeni oyun: karakter oluşturma yüz ekranından sonra çökmeden geçiyor** (19 Ekim düzeltildi ve oyunda denendi: ansiklopedi bölümü oyun
      açılışında kaydediliyor ve oyunun `CampaignUIHelper` sınıfını bozuyordu; kayıt artık kampanya başlarken yapılıyor)
- [ ] Kampanyaya girdikten sonra ansiklopedideki Entrika bölümü hâlâ görünüyor (kayıt yeri değişti)

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
- [ ] Divan'da dört koltuk yan yana sığıyor mu (MAREŞAL, KÂHYA, HAZİNEDAR, SARAY HEKİMİ)? Küçük düğme yazıları okunuyor mu? Ekran görüntüsü at
- [ ] Kâhya ata (Steward, yolsuz mu, sadakat katkısı) → kart: "Şehir ve kalelerinde sadakat günde +0,5"
- [ ] Bir şehrinin sadakatine bak (oyunun şehir ekranında açıklama): "Kâhya (ad)" satırı var mı?
- [ ] ŞEHİR VE KALELER: sadakat, güvenlik, refah, inşaat (kalan gün), günlük proje, günlük gelir; sorunlu olan kırmızı
- [ ] "Yönet..." → kart: durum ve öneri → "İnşaatı değiştir..." → bir yapı seç → 100 dinar düştü, şehre gitmeden inşaat değişti
- [ ] "Günlük projeyi değiştir..." → seç → şehir ekranında yeni günlük proje
- [ ] Sadakati düşen bir yerde (eşiğe 10 gün kala) "Kâhyan uyarıyor" kartı; erzak 5 günden azsa kıtlık uyarısı
- [ ] Insert > Entrika > Divan > Kâhya: rapor metni, "Yönet: X"
- [ ] Kaydet → yükle: kâhya duruyor

### P2.14 — Divan: mareşal (yeni)
- [ ] Divan'da üç koltuk yan yana sığıyor mu (MAREŞAL, HAZİNEDAR, SARAY HEKİMİ)? Yazılar taşıyorsa ekran görüntüsü at
- [ ] Mareşal ata (Tactics, Scouting, tahmin payı) → kart: görüş menzili, erken uyarı yüzdesi; parti başındaki biri de olabiliyor
- [ ] Savaştayken SAVAŞ MECLİSİ: krallık gücü (onlar / biz), oran, tavsiye
- [ ] SAVUNMAN: şehir ve kalelerin garnizon, milis, sur, erzak; kuşatma altındaysa kırmızı
- [ ] DÜŞMAN ORDULARI: kaç kişi, nerede, hedef ("Pravend'i kuşatmaya gidiyor"), sana kaç gün; "Haritada izle"
- [ ] ZAYIF HEDEFLER: en zayıf beş düşman kalesi; birine git ve oyundaki garnizonla karşılaştır (Tactics düşükse sapmalı)
- [ ] Bir ordu şehrine ya da köyüne yürürken (çoğu zaman) kuşatmadan önce "Mareşalin uyarıyor" kartı geliyor mu, süre doğru mu?
- [ ] Insert > Entrika > Divan > Mareşal: rapor metni
- [ ] Kaydet → yükle: mareşal duruyor

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
- [ ] Kendi şehrinde → şehir menüsünde "Saray günü düzenle (dilekçeler)" → üç kart ("DİVAN 1/3"...); dilekçe sahibinin portresi
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

### Genelev (eski adı gece evi; varsayılan kapalı; 19 Ekim'de müşteri tarafı, 21 Ekim'de adlı çalışanlar, yoldaş yönetici ve yoldaş çalışan eklendi)
- [ ] MCM > Entrika > **Genelev (yetişkin teması)** aç
- [ ] Herhangi bir şehirde kasaba menüsü > **Genelevine uğra** → resimli kart: sade oda (150), en iyi oda (400), askerlerine bir gece, Ayrıl
- [ ] Geceyi geçir → "kapı ardından kapanıyor" kartı (içerisi anlatılmıyor); para düştü; aynı gün ikinci kez gri ("Bu gece zaten...")
- [ ] Birkaç gece sonra: bazen "yan odada X konuşuldu" (Kişiler'de X'in raporu), bazen hastalık, evliysen bazen "eşin duydu" (ilişki -10)
- [ ] Hastayken: can %70'in üstüne çıkmıyor; kartta "şehrin hekimine görün (500)" → 3 gün sonra "İyileştin" kartı
- [ ] Saray hekimin varsa hastalık daha çabuk geçiyor (kartta gün sayısı ve hekimin adı)
- [ ] Askerlerine bir gece ısmarla → "Askerlerin gecesi" kartı; parti morali +12; 7 gün gri
- [ ] Gölge ağın olan şehirde > Gölgelerde iş çevir: "Genelev aç (4.000, 2 gönüllü çalışan)" → açıldı, mesajda iki çalışanın **adı**;
      menü açıklamasında genelev satırı; o şehirde gece yarı fiyat
- [ ] **Yönetim kartı (21 Ekim, yeni):** o şehirde kasaba menüsü > **Genelevini yönet** → kart: yönetici, çalışanlar adlarıyla (yaş,
      kulağı delik / gözde / sadık, kaç haftadır), haftalık hesap. Gölgelerde iş çevir > "Genelevi yönet..." ve Klan > Entrika > Ağlar >
      şehrin kartındaki **Genelev** düğmesi de aynı kartı açıyor
- [ ] Çalışan al... → kapıda 3 aday (ad, yaş, özellik) → birini seç → 300 düştü, kartta adı var. Şehir dışındayken gri ve nedeni yazıyor.
      Sınır ağ seviyesine göre (4/6/8)
- [ ] Çalışanların payı... → adil/standart/düşük, ayrılma oranlarıyla; şu anki gri
- [ ] Kart > **Yoldaşlar...** → **Yönetici ata...** → yoldaş listesi (Charm, Roguery; istemeyenin yanında "[istemez: onurlu biri...]") →
      onurlu birini seç: "istemiyor" kartı, ilişki değişmedi. İsteyen birini seç → "evi işletecek" kartı; yoldaş partinden çıktı ve o
      şehirde (şehrin kişilerinde görünüyor)
- [ ] **Yoldaşlar... > Bir yoldaşına sor: evde çalışmak ister mi?** → listede "[isteyebilir]" ya da "[istemez: neden]". İstemeyene sor:
      "Hayır" kartı, ilişki değişmedi. İsteyene sor: kabul kartı; partinden ayrıldı; kartta "(yoldaşın)" diye çalışanlar arasında
- [ ] Bir hafta sonra yoldaşın altını arttı (kendi payı); pay "düşük"e çekilince bir süre sonra bırakıp sana dönüyor (kart)
- [ ] **Çalışan yoldaşı geri çağır...**: şehirdeysen hemen partinde, uzaktaysan yola çıkıp geliyor
- [ ] Ev kapanınca / baskında evdeki yoldaşlar sana dönüyor; evde çalışan yoldaşı casus başı yapamıyorsun ("zaten genelevde çalışıyor")
- [ ] Yönetim kartında yönetici satırı: kazanç +%, duyulma %, haftalık payı. Bir hafta sonra yoldaşın altını arttı (Klan ekranında ya da
      Editörler'de bak); senin gelirin kartta yazana yakın
- [ ] Eşini ya da divandaki birini (casus başı vb.) yönetici yapamıyorsun; yöneticiyi casus başı yapmaya çalışınca "zaten genelev yöneticin"
- [ ] Yöneticiyi geri çağır: şehirdeysen hemen partine katılıyor; uzaktaysan "yola çıktı" ve bir süre sonra partide
- [ ] Şehirde yöneticiyle konuşup partine alırsan: haftalık kartta "ev artık yöneticisiz"
- [ ] Eski bir kayıtta zaten açık genelev varsa: çalışanlara ad verildi (kartı aç ya da bir hafta bekle)
- [ ] Bir hafta sonra "Genelevlerden haftalık gelir" mesajı; altın arttı; kronikte kayıt
- [ ] Bir süre sonra "Genelevde duyulanlar: Lord" kartı (yeni resim); Kişiler'de o lordun güncel raporu (bazen sır)
- [ ] Düşük payda birkaç hafta: "ayrılanlar" kartı **adlarıyla** ("Lysa, Mira payını az bulup ayrıldı"), çalışan sayısı düştü; sadık olanlar daha geç gidiyor
- [ ] "Genelevde duyulanlar" kartında haberi getiren çalışanın adı (varsa kulağı delik olan)
- [ ] Entrika > Ağlar: şehrin kartında genelev satırı
- [ ] Ağı dağıt → genelev de kapandı
- [ ] Ayarı kapat → kasaba menüsünde "Genelevine uğra" ve işletme seçenekleri yok, haftalık gelir gelmiyor
- [ ] Kaydet → yükle → ev, çalışanların adları/yaşları/özellikleri, evdeki yoldaşlar, yönetici, pay, hastalık ve bekleme süreleri aynı
- [ ] Yeni resimler (65-67: adaylar, yönetici, yoldaş) yokken bu kartlarda genelevin dış cephesi görünüyor; eksik sabotaj/hastalık/olay
      kartlarında da aynı konudan bir resim (portre değil)
- [ ] Resimler (32, 50–54) `TVH Resimler`'e konup "aktar" denince kartlarda görünüyor

### Herkesi biç ve doğum sahnesi (yeni, 19 Ekim)
- [ ] MCM > Kurallar > **Herkesi biç**: varsayılan "Yalnızca ben"; savaşta düşmanı öldüren kılıç savuruşu arkadakine geçiyor
- [ ] Kalkanda, dost askerde ve öldürmeyen vuruşta kılıç duruyor; mızrak dürtmesi, tekme ve yumruk geçmiyor
- [ ] "Herkes" seçince düşmanlar da (ve askerlerin) biçebiliyor; "Kapalı" seçince oyun eskisi gibi
- [ ] Başka klandan sevgilin senden doğum yapınca oyunun 3B doğum sahnesi, ardından bizim doğum kartı (üst üste binmiyor)

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

Bir şey bozuksa: ekrandaki kırmızı `[Taç ve Hançer]` mesajını aynen yaz. Çökme olursa Claude'a "çöktü" demen yeter; mod
çökmenin tam çağrı zincirini `Belgeler\Mount and Blade II Bannerlord\Configs\TacVeHancer\cokme_<tarih>.txt` dosyasına yazar,
oyunun kendi günlüğü `C:\ProgramData\Mount and Blade II Bannerlord\logs\` içindeki en yeni `rgl_log_*.txt`.

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
