# Devir notu

## En son durum: 21 Ekim 2026 (bulut oturumu, telefondan)
- **Genelev: adlı çalışanlar, yoldaş yönetici ve yoldaş çalışan, yönetim kartı** (kullanıcı isteği; oyunda henüz denenmedi).
  Çalışanlar adlı (kültürün ad listesi, yaş 19+, özellik: kulağı delik / gözde / sadık). **Kural değişti (kullanıcı kararı):** yoldaşlar
  çalışan da olabilir, ama yalnızca kendi kararıyla (karakterine göre kabul/ret, her hafta yeniden bakılır, payı kendi kesesine;
  eş, sevgili, esir, aile, lord asla). Yönetici: Charm kazancı, Roguery haberi ve güvenliği artırır, %10 kendi kesesine.
  Yönetim kartı: kasaba menüsü > "Genelevini yönet", Gölgelerde iş çevir > "Genelevi yönet...", Entrika > Ağlar > "Genelev" düğmesi.
- **Resim yedekleri:** kartın resmi yoksa aynı konudan var olan bir resim gösteriliyor (`CardImages.Fallbacks`), portre en son.
  Yeni resim yerleri 65-67 (genelev adayları, yönetici, yoldaş); liste ve prompt'lar `docs/gorseller/olay-kartlari.md`.
  Tasarım: `docs/superpowers/specs/2026-10-19-p2-genelev.md`; test listesi: YAPILACAKLAR > Genelev.
- **Bulutta derleme artık var:** Ubuntu'nun `dotnet-sdk-8.0` paketi ve NuGet referans DLL'leriyle mod derleniyor, testler ve GuiCheck
  (oyunsuz) çalışıyor. Oyuna kurmak ve `--game` denetimi yine PC'de `build.ps1`.
- **Uyum denetimi (21 Ekim, kodda doğrulandı), düzeltilecekler:**
  1. Oyuncu karakteri değişince (varis, `Dynasty.Abdicate`) sevgili/kur kayıtları ve koltuklar kalıyor; `Courtships.MeetBlocked` ve
     `MayConceive` akrabalık ve yaş denetlemiyor (varis eski karakterin sevgilisini "devralıyor").
  2. Şantaj: başarı sırrı harcamıyor, bekleme koymuyor; koz kullanılınca aynı sırla yeniden şantaj yapılabiliyor (para/nüfuz döngüsü).
     Genelevdeki "duyulanlar" hedefin tetikte olmasına bakmıyor.
  3. Tek bir "meşgul mü" denetimi yok: rehine giden divan üyesi çalışmaya devam ediyor; serdar, kâhya, genelev ve rehinenin "meşgul"
     tanımları farklı. Tutulan rehineler entrikalarda "senin esirin" sayılıp +20 bonus veriyor.
  4. "Eşin seni aldatabilir" kapalıyken Regard and Standing eşin ilişkilerini yine yürütüyor, sen öğrenemiyorsun
     (`SpouseLife.Weekly` erken dönüyor); tek "romantik sırlar" anahtarı kendi kurlarını da kapatıyor.
  5. `ClanIntrigueTab.CheckBound` sekme bağlanmazsa yalnızca hata yazıyor; ayrı Entrika ekranına geri dönmüyor (Home da boş sekmeye gider).
  6. Çakışmalar: ulak/haberci Diplomacy'nin "Send Messenger"ıyla aynı işi yapıyor (planlanan Lordlar sekmesi üçüncüsü olur); ajan bonusu
     kartta "her yerde +2" diyor ama formüllerde yarıya iniyor ya da ağ üst sınırında kayboluyor; kâhyanın sadakati valilerin üstüne
     ekleniyor; "Divan" hem sekme hem saray günü adı. "Klanı devret" yalnızca hile editöründe; "Kayıt testi" düğmesi hâlâ menüde.
- Sıradaki işler (kullanıcı "sırayla yapalım" dedi): Savaş Raporu'nu geliştir, İsyanlar/İç Savaş penceresi, Krallık > Lordlar sekmesi,
  kalan 15 kart resmi, Convivium / Diplomacy & Intrigue kararı.

## 21 Ekim 2026 (PC oturumu, telefona devir)
- **Git:** her şey `main`'de ve GitHub'da. Bulut dalı `ccr-be29c96c-8tfbau` da `main`'e getirildi. Yeni işe `main`'den başla.
  Repo herkese açık kalıyor (kullanıcı kararı); paketli modların kodu (DLL, XML) artık repoda, büyük içerik (.tpac, resim, ses) dışarıda.
- **Bugün yapılanlar (oyunda henüz doğrulanmadı):**
  - Harita çubuğu: Entrika düğmesi `IsLockingNavigation = true` dönüyordu, bütün çubuğu kilitliyordu (yalnızca Entrika açılıyordu). Artık false.
  - Entrika simgesi: Roguery'nin küçük simgesi (`gui_skills_icon_roguery_small`), doygunluk -100 (diğer çubuk simgeleri gibi gri).
    Kullanıcı "diğerleri gibi" istedi; oyunda hâlâ uymazsa kendi gri simgemizi çizmek gerekecek (sprite sayfaları .tpac'ta, görülemiyor).
  - Savaş Raporu (Diplomacy `DetailWarView.xml`, third_party'de düzenlendi): 820 px genişlik, 20/24 punto, büyük sancaklar; BundleShim
    `DetailWarVM.ReportHeight`'i 1.25 ile büyütüyor (Diplomacy DLL'i bizden sonra yüklendiği için OnBeforeInitialModuleScreenSetAsRoot'ta).
    Kullanıcı ekranı çok beğendi; geliştirmeye devam edilebilir (yeni bilgi için Diplomacy VM'inden veri çekmek gerekir).
  - Casus başına "Ajan yetiştir" görevi (`AgentRules`, tasarım `2026-10-21-ajan-yetistir.md`). Casusluk krallık istemiyor; yoldaş casus başı olabilir.
  - Companion Autonomy rol etkileri en az 130 px (BundleShim). Uyum raporu: Diplomacy'nin null-koruma yaması "incelendi" (yanlış alarm gitti).
  - 27 yeni kart resmi (49/64).
- **Açık:** Klan ekranındaki Entrika sekmesi hâlâ boş olabilir. "Türetilmiş VM" teşhisi yanlış çıktı (aşağıda); kesin teşhis için oyundaki
  kırmızı mesaj (`ClanIntrigueTab.CheckBound`) bekleniyor. Karar bekleyen modlar: Convivium, Diplomacy & Intrigue (YAPILACAKLAR).
- **Bulutta oyun yok:** bulutta derleme ve testler çalışır, ama oyuna kurmak PC'de `build.ps1` ile olur.

## 20 Ekim akşamı (buluttaki oturumdan yerel oturuma) — tarihçe
- PC'de `third_party\RegardStanding` repodakiyle çakışırsa (git "would be overwritten" der): PC'deki klasörü sil, repodaki yamalı.

## Bu akşam yapılanlar (dalda, oyunda henüz doğrulanmadı)
1. **Arayüz oyunun kendi stilleriyle:** sekme/düğme/panel fırçaları oyunun fırçalarından kopyalanıyor (`TvhUi.GameLooks`:
   Header.Tab.Center, Clan.TabControl.Text, Popup.Done.Button, Popup.Button.Text, Frame1Brush), bütün kaydırma çubukları oyunun
   `Standard.VerticalScrollbar` prefab'ı. GuiCheck `--game` ile bu adları oyunda doğrular (build.ps1 oyun kuruluysa yapar).
2. **Uyum raporu** (`src/Bundle/CompatReport.cs` + `src/CompatRules.cs`, testli): `uyum_raporu.txt`.
3. **RefCheck** (`tools/RefCheck`): paketli DLL'lerde eksik DLL / yüklenemeyen tür; build.ps1 sonunda çalışır.
4. **Regard and Standing açılış penceresi** ("submodule could not be loaded correctly due to a dependency conflict"): neden,
   isteğe bağlı Calradia Remembers'ın DLL'lerine referans. `tools/RsPatch` bağlantıyı tamamen söktü; repodaki
   `third_party/RegardStanding/bin/Win64_Shipping_Client/RegardStanding.dll` yamalı. Arada indirilen DLL Windows'un "engelli"
   işaretiyle "Cannot load" verdi; `bundle.ps1` artık her kopyada `Unblock-File` yapıyor.
5. **Entrika sekmesi boş ve tıklanmıyor** (Klan ekranında üçüncü düğme): teşhis, başka bir modun Klan ekranını türetilmiş
   `ClanManagementVM` ile açması (`GauntletClanScreen.CreateDataSource`; muhtemelen Companion Autonomy) → UIExtenderEx mixin'i yalnızca
   tam türe bağlıyordu. Düzeltme: `[ViewModelMixin(..., handleDerived: true)]`. Bağlanmazsa Klan ekranında kırmızı mesaj ekran
   türünü ve DLL'ini yazar (`ClanIntrigueTab.CheckBound`).

## 21 Ekim, PC oturumu: bulgular
- Bulut dalı yerelde `main`'e birleşti; build.ps1 temiz (testler, arayüz denetimi, DLL denetimi: 8 alt modül DLL'i yükleniyor).
- **Entrika sekmesi:** "başka mod Klan ekranını türetilmiş VM ile açıyor" teşhisi tutmuyor: paketteki hiçbir DLL `ClanManagementVM`
  ya da `GauntletClanScreen`'den türemiyor, onları kurmuyor ya da yamalamıyor. UIExtenderEx 3.0.3'ün bağlama için aradığı oyun
  iç yapıları (`ViewModel._propertiesAndMethods`, `DataSourceTypeBindingPropertiesCollection` Properties/Methods/ctor) 1.4.8'de var.
  Sekme şeridinin DataSource'u ClanManagementVM'nin kendisi. Kesin teşhis için oyunda `CheckBound`'un kırmızı mesajı gerekli
  (son derleme oyunda hiç açılmadı).
- **Companion Autonomy rol etkileri:** hata değil, tasarımı: bütçe bloğu görünürken `CARoleEffectsHeight` 36 px. BundleShim en az 130 px yapıyor.
- **"ENTRİKA" afişi:** Diplomacy'nin göstergesinde yazı yok; büyük ihtimalle kaldırılan yüzen harita düğmemizdi (üst-orta yedeği). Oyunda bak.
- **Repo herkese açık (PUBLIC):** RegardStanding'in (yamalı) DLL'i yerel `main`'de ve bulut dalında. Push etmeden önce kullanıcı kararı:
  repo gizli mi olacak?

## Açık sorunlar — önce bunlar
1. **Entrika sekmesi** düzeldi mi? Dalı çek, build.ps1, oyunda L → Entrika. Hâlâ boşsa kırmızı mesajdaki türe bak; o modun kaynağında
   `CreateDataSource`'u bul. Gerekirse UIExtenderEx'siz yedek: mixin özelliklerini `ViewModel._propertiesAndMethods`'a kendimiz ekleriz.
2. **Regard and Standing penceresi** gitti mi? Gitmediyse `rgl_log`'daki ayrıntıya bak (oyun hangi referansı/türü şikâyet ediyor);
   `tools/RefCheck`'i dağıtılmış modüle karşı çalıştır.
3. **Companion Autonomy, Klan > Birlikler'de "Birlik Lideri Rolleri & Etkileri"nin üstüne biniyor** ("Yoldaşın kesesi / Harçlık /
   Kendi başına alışverişe gider / Ordu düzeni" bloğu). Onun prefab ekini (third_party\CompanionAutonomy\GUI) aşağı/yan kaydır ya da
   rol listesinin altına al. Oyunun ClanParties prefab'ına bakarak yerleştir.
4. **Haritadaki savaş yorgunluğu afişinde "ENTRİKA" yazıyor** (Diplomacy'nin 25 ⚑ 68 göstergesi). Olması gereken krallık adı ya da
   "Savaş". Bizim bir metnimiz/dil dosyamız ya da harita çubuğu düğmemiz (`src/UI/MapBarButton.cs`, `MapBar.Left.*` fırçalarına katman
   ekliyor) Diplomacy'ninkiyle çakışıyor olabilir — kontrol et.

## Kullanıcının istedikleri (sıradaki işler)
- **Savaş Raporu (Diplomacy) ekranını geliştir:** kullanıcı çok beğendi (üst kısım güzel). Daha fazla bilgi ve oyun görünümü.
- **İsyanlar ve İç Savaş penceresi (Diplomacy):** daha oyuna benzesin (şu an boş liste, düz kutu).
- **Krallığın lordlarıyla iletişim sayfası** — önerilen: Krallık ekranında Diplomacy sekmelerinin yanına "Lordlar" sekmesi
  (UIExtenderEx, Klan sekmemiz gibi). Liste: portre, klan, ilişki, konum, sana bakışı, son temas. Eylemler: haberci gönder (mevcut
  `Messengers` sistemi; hal hatır/teşekkür/özür, karaktere göre ilişki, haftalık sınır), hediye (altın/eşya, karaktere göre),
  ziyafete davet (şehrinde, birkaç lord), oylamada destek sözü iste, yanına çağır (lordun partisi gelir, konuşma oyunun diyaloğunda).
  Bizim "Saray" alt sekmesi (`TvhCourtPanel`) zaten krallık lordlarını gösteriyor; oradan yararlan.
- Diplomacy ve diğer modların özelliklerini kendi ekranlarımıza taşıma (kullanıcının uzun vadeli isteği: tek mod, her şey bizim arayüzde).
- Karar bekleyenler: Convivium, Diplomacy & Intrigue (YAPILACAKLAR).
