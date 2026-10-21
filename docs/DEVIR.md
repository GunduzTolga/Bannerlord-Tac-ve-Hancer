# Devir notu

## PC'ye geçince yapılacaklar (kullanıcı: "PC'ye geçince yaparız")
**1. Kur ve oyunda bak (önce bunlar)**
- `git checkout main; git pull origin ccr-561e2390-s83s0r; .\build.ps1` → temiz mi? Özellikle oyuna karşı arayüz denetimi (`--game`):
  yeni pencerelerin (Lordlar, İsyanlar, Savaş Raporu, Barış Masası) oyun fırçaları ve sprite'ları. Savaş Raporu'nun üst kısmı
  Diplomacy'nin prefab'ı olduğu için denetlenmiyor; oyunda göz ile bak.
- Oyunda test listesi: YAPILACAKLAR'ın en üstündeki 21 Ekim bölümleri (Barış Masası, Savaş Raporu, Lordlar, İsyanlar, gizli infaz,
  düzeltmeler, genelev). Bozuk görünen her pencerenin ekran görüntüsü + `rgl_log` + varsa `cokme_*.txt`.
- Eski açık sorunlar (aşağıda "Açık sorunlar"): Klan ekranındaki Entrika sekmesi boş mu (kırmızı `CheckBound` mesajı), Companion
  Autonomy'nin Klan > Birlikler'de üst üste binmesi, haritadaki "ENTRİKA" afişi, harita çubuğundaki Entrika simgesi.

**2. Oyunun kodunu açarak (ILSpy/dnSpy: `<oyun>\bin\Win64_Shipping_Client\TaleWorlds.CampaignSystem.dll`; bulutta yalnızca imzalar var)**
- **Oylamada destek sözü** (Lordlar penceresine "oylamada destek iste"): `KingdomElection` (`DetermineSupport`, `UpdateSupport`,
  `GetAiChoice`, `DetermineOfficialSupport`) ve `KingdomDecision.DetermineSupport` alt sınıfları; söz veren lordun oyunu senin
  istediğin sonuca bağlayacak güvenli kanca.
- **Barış Masası doğrulamaları:** `MakePeaceAction.ApplyByKingdomDecision`'da haracın yönü (1'den 2'ye mi) ve kimin kesesinden ödendiği;
  barışta esir lordlar serbest kalıyor mu (`EndCaptivityAction.ApplyByPeace` nereden çağrılıyor); `ChangeOwnerOfSettlementAction.ApplyByBarter`
  garnizonu ve bağlı köyleri nasıl devrediyor. `DiplomacyModel.GetWarProgressScore`'un ölçeği (Savaş Raporu'ndaki "üstünlük" satırı).
- **Krallık ekranına bağlamak:** Lordlar'ı Krallık ekranında sekme yapmak, Savaş Raporu'nu Krallık > Diplomasi'deki savaş listesinden de
  açmak (oyunun `KingdomManagement` prefab'ları: `<oyun>\Modules\SandBox\GUI\Prefabs`).
- Harita çubuğu Entrika simgesi: oyunun sprite sayfalarından uygun gri simge (yoksa kendi simgemizi çizeriz).

**3. Kullanıcının kendisi**
- Kart resimleri: 15 eski + 3 genelev + 2 barış (elçi, antlaşma). Prompt'lar `docs/gorseller/olay-kartlari.md`; `tools\resim-aktar.ps1`.
- Mod kararları: Convivium, Diplomacy & Intrigue (YAPILACAKLAR > Karar bekleyen modlar).

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
- **Uyum denetimindeki sorunların hepsi düzeltildi (21 Ekim, bulut; oyunda denenmedi):** varis değişince eski karakterin romantik
  kayıtları ve koltukları temizleniyor, buluşma/gebelik akrabalık ve yaş denetliyor; şantajda sır harcanıyor; tek "meşgul" tanımı
  (rehine, genelev dahil), misafir rehineye entrika yok; "Kur ve sevgililer" ayrı anahtar, Regard and Standing'in eş ilişkileri her
  zaman izleniyor; sekme bağlanmazsa ayrı Entrika ekranına geçiliyor; ajan bonusu formüllerde ayrı ve tam; kâhya yalnızca valisiz
  yerleşimlerde; "Saray günü düzenle" adı; Entrika > Hanedan menüsü; kayıt testi düğmesi kalktı.
- **Diplomacy bağlantıları (21 Ekim):** rehine anlaşması süresince Diplomacy saldırmazlık anlaşması (erken bitince o da biter);
  "Muhalefeti izle" raporunda isyancı gruplar (önde kim, kaç klan, istek, güç); Savaş Raporu'nda mareşalin satırları (güç tahmini,
  sahadaki ordular, topraklarına yürüyenler, tavsiye); ulak kartında yüz yüze görüşme Diplomacy habercisiyle, bizim ulak yalnızca
  hediye taşır. Hazinedar artık gerçek gelir veriyor: atölye ve kervan gelirine Trade/10 yüzde (en fazla %20), klan gelirinde
  "Hazinedar" satırı (`Cheats/Models/TreasurerFinanceModel`). API notları: Diplomacy kaynakları ilspycmd ile açıldı (yalnızca bulutta).
- **Esiri gizlice öldürt (21 Ekim, kullanıcı isteği):** açık idam yerine hastalık/kaza/firar süsü; şüphe ve ifşa Roguery'ye, esirin
  önemine ve yakalanalı kaç gün olduğuna bağlı (`PrisonDeathRules`, `PrisonDeaths`, tasarım `2026-10-21-gizli-infaz.md`).
- **İsyanlar ve iç savaş penceresi (21 Ekim):** Diplomacy'nin `RebelFactionsInterface.ShowInterface`'i Harmony ön yamasıyla bizim
  `UI.RebellionsScreen`'e yönleniyor (açılamazsa onunki açılır). Gruplar ve düğmeler Diplomacy'nin kendi ViewModel'leriyle
  (`RebelFactionItemVM`, `RebelFactionsVM`, reflection) çalışıyor; görünüş, Türkçe metinler, güç çubuğu, "en gönülsüz üye" notu ve
  hoşnutsuzlar bölümü (Saray kartları) bizim. Prefab'lar `TvhRebellions`, `TvhRebelFactionCard`. Oyunda henüz denenmedi.
- **Kullanıcı eşini genelevde çalışan olarak sordu:** yapılmadı ("eşini pazarlamak" çizgisi); eş yalnızca kendi kararıyla yönetici olabilir.
- **Lordlar penceresi (21 Ekim):** `UI.LordsScreen` (prefab `TvhLords`, `TvhLordCard`): krallığın ve tanıdığın lordlar tek yerde;
  ulak ve hediye (Saray'daki düğme), **yanına çağır** (nüfuz ya da altın; kabul ederse partisi oyunun eşlik emriyle sana yürür, her
  saat `LordContacts.Hourly`), **ziyafet** (kendi şehrinde ya da kalende, en fazla 6 davetli, 30 günde bir; ilişki, nüfuz, bazen rapor
  ve sır). Kurallar `LordContactRules` (testli), tasarım `2026-10-21-lordlar.md`. Açılış: Saray > Lordlar..., Insert > Entrika.
  **Oylamada destek sözü yapılmadı:** oyunun seçim kodunun gövdesi bulutta yok; PC'de oyunun DLL'lerini açıp oy ağırlığına kanca aranmalı.
- **Savaş Raporu artık bizim pencere (21 Ekim):** Diplomacy'nin `DetailWarViewInterface.ShowInterface`'i ön yamayla `UI.WarReportScreen`'e
  yönleniyor (açılamazsa onunki). Üst kısım Diplomacy'nin prefab'ından aynen ve onun `DetailWarVM`'ine bağlı (`WarReportVM.Report`); altı
  bizim iki sütun: gidişat (StanceLink, oyunun savaş puanı), kuvvetler, Diplomacy'nin yorgunluk dökümü, mareşal, barış (oyunun barış puanı,
  Diplomacy'nin barış koşulları), esirler, savaş günlüğü (`IWarLog`). `Intrigue.WarReport`, `WarReportRules` (testli), tasarım
  `2026-10-21-savas-raporu.md`. GuiCheck artık `ViewModel` türündeki veri kaynağının alt ağacını (başka modun iç türü) denetlemiyor.
  Insert > Entrika > Savaş raporları da açar. Oyunda henüz denenmedi.
- **Divan koltuğu "Serdar" artık "Mareşal" (kullanıcı kararı):** "Serdar" erkek adı gibi okunuyordu. Bütün metinler ve belgeler; resim
  dosyaları (`sonuc_serdar*.png`) ve kodda `Marshal` adı aynı. Kronik'teki eski satırlarda "Serdar" kalır.
- **Barış Masası (21 Ekim, kullanıcı isteği):** savaştığın krallıktan haraç, savaş tazminatı, garnizonlarından asker, şehir ve kale iste;
  ya da altın/haraç ver. Kabul = %50 + pazarlık gücü (yorgunluk farkı, güç oranı, başka cepheler, oyunun barış puanı) - şartların
  ağırlığı. Kaybeden taraf bazen elçi gönderip kendisi barış ister. Yalnızca hükümdar; Diplomacy'nin barış koşullarına uyar.
  `PeaceTreatyRules` (testli), `PeaceTreaties`, `UI.PeaceTableScreen`, tasarım `2026-10-21-baris-masasi.md`. Oyunda henüz denenmedi.
- Sıradaki işler: yukarıdaki "PC'ye geçince yapılacaklar". Bulutta yapılabilecekler bitince kullanıcıya sor.

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
- ~~Savaş Raporu (Diplomacy) ekranını geliştir~~ (21 Ekim yapıldı: `UI.WarReportScreen`; barış için Barış Masası düğmesi).
- ~~İsyanlar ve İç Savaş penceresi~~ (21 Ekim yapıldı: `UI.RebellionsScreen`).
- ~~Krallığın lordlarıyla iletişim sayfası~~ (21 Ekim yapıldı: `UI.LordsScreen`; ulak/hediye, yanına çağır, ziyafet). Kalan: **oylamada
  destek sözü** (PC'de oyunun seçim kodu açılmalı) ve isteğe bağlı Krallık ekranına sekme (oyunun Krallık prefab'ı PC'de görülmeli).
- Diplomacy ve diğer modların özelliklerini kendi ekranlarımıza taşıma (kullanıcının uzun vadeli isteği: tek mod, her şey bizim arayüzde).
- Karar bekleyenler: Convivium, Diplomacy & Intrigue (YAPILACAKLAR).
