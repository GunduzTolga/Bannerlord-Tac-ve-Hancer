# Nexus Mods'tan "Taç ve Hançer" için özellik fikirleri (Bannerlord v1.4.8 + War Sails, Ekim 2026)

Hazırlanma: 19 Ekim 2026. Hedef mod: **Taç ve Hançer** (iç kimlik `CheatMenu`), tek oyunculu, şu an **Harmony/MCM bağımlılığı yok**, arayüz popup tabanlı ([tasarım belgesi](../docs/superpowers/specs/2026-10-18-bannerlord-cheat-menu-design.md), [entrika tasarımı](../docs/superpowers/specs/2026-10-19-entrika-design.md)).

**Yöntem ve sınırlar**
- Bu oturumda `nexusmods.com` ve `steamcommunity.com` doğrudan açılamadı (vekil sunucu 403 / DNS hatası). Nexus ve Steam bilgileri **arama motoru özetlerinden** ("arama özeti") ve önceki araştırmalardan alındı. Önceki araştırma bu sayfaları 18 Ekim 2026'da doğrudan okumuştu.
- GitHub doğrudan okunabildi. Bağımlılık ve sürüm bilgisi mümkün olduğunca GitHub README / `SubModule.xml` dosyalarından doğrulandı.
- Sayfasını göremediğim ya da kaynakların çeliştiği bilgiler **doğrulanmadı** diye işaretli.
- Önceki dosyalarda olanlar burada yinelenmedi; yalnız gerektiği yerde onlara bağlantı verildi:
  - [popular_gameplay_mods.md](Bannerlord%20eksiklikleri%20ve%20mod%20f%C4%B1rsatlar%C4%B1/popular_gameplay_mods.md): popüler modlar, sıralamalar, 1.3/1.4 durumu
  - [cheat_editor_mods.md](Bannerlord%20eksiklikleri%20ve%20mod%20f%C4%B1rsatlar%C4%B1/cheat_editor_mods.md): hile ve editör modları
  - [technical_feasibility.md](Bannerlord%20eksiklikleri%20ve%20mod%20f%C4%B1rsatlar%C4%B1/technical_feasibility.md): API ve arayüz uygulanabilirliği

**Kısaltmalar:** H = Harmony, BL = ButterLib, UIX = UIExtenderEx, MCM = Mod Configuration Menu v5, BLSE = Bannerlord Software Extender.
**Örtüşme sütunu:** *Var* = modda zaten var · *Planlı* = entrika yol haritasında (E2/E3) · *Kısmen* · *Yok*.

---

## Özet: Taç ve Hançer için en iyi 15 fikir (sıralı)

Önce önemli bir uyarı. **Fourberie (Cunning)**, entrika tarafında bize en yakın rakip. Bu modda şunlar var: "Scheme Room" içinde cinayet entrikası, kaçırma (yapay zekâ da kullanıyor), başarısız cinayetten sonra kan davası, şüphe çubuklu sızma görevleri, kale ve şehir menüsünden sabotaj, çete liderleri ([Nexus 2969](https://www.nexusmods.com/mountandblade2bannerlord/mods/2969), arama özeti; [Steam değişiklik günlüğü](https://steamcommunity.com/sharedfiles/filedetails/changelog/2875710877)).
- E1 ve planlı E2'nin önemli bir kısmı onunla örtüşüyor.
- Bizi ayıracak şeyler: CK3 tarzı sır ve koz ekonomisi, seçimli olay kartları, karşı casusluk kararları ve aile/diplomasi bağlantısı.
- İki mod birlikte kurulduğunda uyumlu çalışıp çalışmadığı test edilmeli.

| # | Fikir | Neden (tek satır) | İlham veren mod(lar) | Zorluk | Harmony / UIX? |
|---|---|---|---|---|---|
| 1 | **Esir sorgulama:** esir lorddan sır veya istihbarat raporu çıkar (grafik değil; "baskı" seçeneği Onur cezası verir) | Mevcut sır altyapısını hemen besler. Esir lordlar oyunda boşa duruyor. | Intrigues (kaçırılan hedefe işkence/sakatlama) [2693](https://www.nexusmods.com/mountandblade2bannerlord/mods/2693); Bellum Civile (esir kozu) [GitHub](https://github.com/OhMyMike57/BellumCivile) | kolay | Hayır |
| 2 | **Haberci/mektup:** herhangi bir lordla uzaktan, gecikmeli görüşme. Koz harcama ve tehdit mektupla yapılır. | Altı yıldır en çok yeniden yazılan QoL özelliği. Ayrıca E2'de koz harcamak için lordu kovalamak gerekmez. | Diplomacy (Messengers) [832](https://www.nexusmods.com/mountandblade2bannerlord/mods/832), Courier [9048](https://www.nexusmods.com/mountandblade2bannerlord/mods/9048), Telepathy [1203](https://www.nexusmods.com/mountandblade2bannerlord/mods/1203), Realistic Courier [12106](https://www.nexusmods.com/mountandblade2bannerlord/mods/12106) | orta | Büyük olasılıkla hayır (Telepathy yalnız MCM istiyor; uzaktan diyalog API'si doğrulanmadı) |
| 3 | **Konsey makamları** (Mareşal, Şansölye, Kâhya, Casus başı, Saray hekimi) **ve casus başına görev modu:** *Karşı casusluk / Muhalefeti ortaya çıkar / Söylenti yay* | BK Redux ve Bellum Civile'nin ortak çekirdeği. Mevcut casus başını genişletir; planlı karşı casusluğa bir "ayar düğmesi" verir. | Bellum Civile Privy Council (+15 savunma / +15 saldırı gibi değerler) [GitHub](https://github.com/OhMyMike57/BellumCivile); BK Redux [wiki](https://github.com/GIO443/bannerlord-banner-kings-redux/wiki) | kolay–orta | Hayır. Vergi/asker etkisi model zinciri ister (resmî API). |
| 4 | **Gizli ilişkiler ve dedikodu** (sahnesiz, yalnızca metin). Yeni sır türleri: *gizli sevgili*, *gayrimeşru çocuk*. Tanıklar ve yayılan söylenti. | 2026'nın en canlı romantizm/aile modlarının özü. Şantajın en doğal yakıtı. | Dramalord [6682](https://www.nexusmods.com/mountandblade2bannerlord/mods/6682); Regard and Standing [13882](https://www.nexusmods.com/mountandblade2bannerlord/mods/13882) | orta | Hayır (kendi kayıt durumumuzda; vanilla gebelik modeline dokunmadan) |
| 5 | **Rehine anlaşmaları:** barış veya ittifak teminatı olarak hanedan üyesi rehine. Anlaşma bozulursa rehinenin akıbeti oyuncunun seçimine kalır. | Yalnız Bellum Civile'de var. Aile, diplomasi ve entrikanın kesişimi; planlı kaçırma özelliğini tamamlar. | Bellum Civile (100 gün, korumalı gözetim) [GitHub](https://github.com/OhMyMike57/BellumCivile) | orta | Rehine esir yerine "misafir" işaretiyle tutulursa hayır. Gerçek esirlik + kaçış engeli Harmony ister. |
| 6 | **Hak iddiası uydurma** entrikası (sahte soy belgesi). Ödülü savaş gerekçesi ya da fief oylamasında ağırlık. | CK3'teki "fabricate claim"in karşılığı. Aşama ve keşif motorumuza birebir oturur. | Bellum Civile (3 yıl; %33 / %66 / %100 kontrolleri; başarısızlıkta sahtecilik ifşa olur) [GitHub](https://github.com/OhMyMike57/BellumCivile) | orta | Hayır |
| 7 | **Saray günü:** dilekçeler ve yargılamalar. Hüküm tarzı kişilik XP'si, altın, sadakat ve güvenliği etkiler. Resimli olay kartlarının ilk kullanım alanı. | Barış zamanında yapacak bir şey olmaması en eski şikâyetlerden. BannerCourt Ekim 2026'da hâlâ aktif. | BannerCourt [13940](https://www.nexusmods.com/mountandblade2bannerlord/mods/13940); Hold Court [5522](https://www.nexusmods.com/mountandblade2bannerlord/mods/5522) (terk edilmiş) | orta | Hayır (şehir menüsü seçeneği + popup) |
| 8 | **Varis ve meşruiyet:** klan varisini seç, gayrimeşru çocuğu meşrulaştır, varis sırası görünümü | 2026'da çok sayıda hanedan modu çıktı. Vanilla'da varis kontrolsüz. | Crown and Kin [12828](https://www.nexusmods.com/mountandblade2bannerlord/mods/12828), Magnum Opus [12035](https://www.nexusmods.com/mountandblade2bannerlord/mods/12035), RoT – Dynasty and Succession [13309](https://www.nexusmods.com/mountandblade2bannerlord/mods/13309), Dynasty Core [13193](https://www.nexusmods.com/mountandblade2bannerlord/mods/13193) | orta | Klan varisi için hayır (olay dinleyicisiyle; doğrulanmadı). Krallık ardıllık yasası Harmony ister. |
| 9 | **Aile editörü ekleri:** evlendir/boşa, hamileliği başlat ya da kapat (sahnesiz), çocuğu yetişkinliğe büyüt, yaşlanmayı dondur | Rakip editörlerin bizde olmayan parçaları. Resmî Action API'leriyle yapılır. | BLGM / Commander / Character Reload / No Aging Extended ([önceki dosya](Bannerlord%20eksiklikleri%20ve%20mod%20f%C4%B1rsatlar%C4%B1/cheat_editor_mods.md)) | kolay | Hayır |
| 10 | **Casus ağının ifşası diplomatik krize dönüşür:** krallıklar arası ilişki düşer, savaş baskısı doğar | Diplomacy & Intrigue'in ana fikri. Planlı karşı casusluğa sonuç katmanı ekler. | Diplomacy & Intrigue [13595](https://www.nexusmods.com/mountandblade2bannerlord/mods/13595), [GitHub](https://github.com/namphan19/Bannerlord-Diplomacy-Intrigue) | kolay | Hayır |
| 11 | **Ajan görevlendirme:** bir yoldaşı N gün gizli göreve gönder (casusluk, kurye, asker toplama) | Casus başı dışındaki yoldaşlara da iş verir. | Delegated Missions [13846](https://www.nexusmods.com/mountandblade2bannerlord/mods/13846), Companion Orders [12912](https://www.nexusmods.com/mountandblade2bannerlord/mods/12912), Send Companions Recruiting [3506](https://www.nexusmods.com/mountandblade2bannerlord/mods/3506) | orta (fiziksel yolculuk: zor) | Hayır |
| 12 | **NPC hırsları ve NPC'ler arası entrika, haber akışıyla.** Yapay zekâ yalnız oyuncuya değil, birbirine de entrika kurar. | "Yaşayan dünya" talebi en güçlü sinyallerden. Planlı yapay zekâ entrikasını genişletir. | Complex Characters (Bloc) [gry-online](https://www.gry-online.pl/download/mount-blade-ii-bannerlord-complex-characters-v105-mod/zf152cd); Bellum Civile; AI Influence [9711](https://www.nexusmods.com/mountandblade2bannerlord/mods/9711) | zor | Hayır |
| 13 | **Borç defteri:** lordlara faizli borç ver; ödenmeyen borç koza dönüşür | Banka modları ile koz sisteminin sentezi. Ekonomiyi entrikaya bağlar. | Settlement Finances [gamewatcher](https://www.gamewatcher.com/mount-and-blade-2-bannerlord-settlement-finances-mod); Xorberax's Legacy – Banks of Calradia [3462](https://www.nexusmods.com/mountandblade2bannerlord/mods/3462) | orta | Hayır |
| 14 | **War Sails kuralları ve liman sabotajı:** deniz XP çarpanı, deniz hızı, gemi fiyat çarpanı, gemi onar/ver. Planlı sabotaja "limandaki filoyu yak" türü. | War Sails hâlâ ince kapsanmış bir alan. Kural ve hile kategorimize doğal ek. | War Sails Tweaks [12697](https://www.nexusmods.com/mountandblade2bannerlord/mods/12697); BannerWand (sonsuz gemi canı) [9151](https://www.nexusmods.com/mountandblade2bannerlord/mods/9151); UFO Bundle (gemi verme) [GitHub](https://github.com/UFOdestiny/UFO-Bannerlord) | kolay–orta | Hız için hayır (zincirli model). Gemi API'leri doğrulanmadı. |
| 15 | **Entrika kroniği:** tüm entrika, olay ve ilişki değişimlerinin günlüğü; dosyaya aktarma | Test ve oyuncu takibi için ucuz. Rakiplerde de var. | Regard and Standing (günlük + konsol komutları) [13882](https://www.nexusmods.com/mountandblade2bannerlord/mods/13882); Diplomacy & Intrigue ("Write a report to file") [GitHub](https://github.com/namphan19/Bannerlord-Diplomacy-Intrigue); World Events Announcer [12730](https://www.nexusmods.com/mountandblade2bannerlord/mods/12730) | kolay | Hayır |

**Sıralamanın mantığı**
- Önce mevcut E1 altyapısını (sır, rapor, aşama/keşif motoru, olay popup'ları) doğrudan besleyen ve Harmony gerektirmeyen fikirler geliyor: 1, 3, 4, 6, 10, 15.
- Sonra boşluğu kanıtlanmış aile ve diplomasi fikirleri geliyor: 2, 5, 7, 8, 9.
- En sonda zor ya da riskli olanlar var: 11–14.

**Bağımlılıksızlık bir satış noktası**
- Bu raporda bağımlılığı bilinen modların büyük çoğunluğu en az H ya da MCM istiyor (bkz. bölüm B11).
- Her TaleWorlds yamasında bu zincir kırılıyor. Önceki araştırmaya göre 160+ modluk rehberler ve resmî olmayan Harmony yeniden derlemeleri buna kanıt.
- Taç ve Hançer'in "kendi başına çalışır" olması korunmalı.

---

## A. En popüler modlar ve 1.3 / 1.4 durumu (kısa güncelleme)

Ayrıntılı sıralamalar (tüm zamanlar, son 12 ay, Steam) önceki dosyada var: [popular_gameplay_mods.md, Q1–Q2](Bannerlord%20eksiklikleri%20ve%20mod%20f%C4%B1rsatlar%C4%B1/popular_gameplay_mods.md). Burada yalnız yeni bulunanlar var.

| Bulgu | Kaynak |
|---|---|
| Son iki haftanın Nexus "top files" listesinde MCM, UIExtenderEx, BLSE ve Diplomacy ilk dörtte. Kütüphane yığını hâlâ en çok indirilen içerik. | [Nexus top](https://www.nexusmods.com/mountandblade2bannerlord/mods/top) (arama özeti) |
| Ekim 2026 "Ayın modu" adayları: Eagle Rising – II Punic War, The Carrack (War Sails gemisi), Daughters of the Mountains, Cavalry Charge AI | [Nexus MOTM](https://www.nexusmods.com/mountandblade2bannerlord/mods/motm) (arama özeti) |
| Üçüncü taraf bir toplayıcıya göre Diplomacy yaklaşık 2,0 milyon, Captivity Events (18+) yaklaşık 1,6 milyon indirme aldı. Bu sayılar CE'yi en çok indirilen modlar arasına koyar. **Doğrulanmadı.** | [discovermods – Diplomacy](https://discovermods.com/bannerlord/diplomacy/), [discovermods – Captivity Events](https://discovermods.com/bannerlord/captivity-events/) (arama başlıkları) |
| **2026'nın yeni dalgası siyaset ve hanedan modları.** Hepsi 1.4.8 hedefli, çoğu düşük beğenili ve erken sürümde: Magnum Opus, Diplomacy & Intrigue (26 Eyl 2026, v0.4.0), Dynasty and Intrigue (Nis–May 2026), Crown and Kin (DEV.10), Houses And Heirs, Dynasty Core, Noble Household (v0.4.1), Regard and Standing (17 Eki 2026), BannerCourt (16 Eki 2026) | Her birinin bağlantısı aşağıda, B1–B3 tablolarında |
| Banner Kings'in aslı 26 Oca 2025'te durdu. Bakımını **Banner Kings Redux** fork'u sürdürüyor: v1.4 destekli, War Sails Nord içeriği var, GitHub sürümleri resmî kaynak. | [GitHub README](https://github.com/GIO443/bannerlord-banner-kings-redux/blob/main/README.md); [Nexus 10922](https://www.nexusmods.com/mountandblade2bannerlord/mods/10922?tab=files) |
| Çekirdek kütüphaneler 1.4.8 ve 1.5 beta için güncel: ButterLib 2.12.0 ("v1.4.8 ve v1.5.1'e güncellendi"), MCM 5.13.1 (17 Eki), UIExtenderEx 3.0.3 (7 Eki), Harmony 2.4.2.248 (21 Tem). | [ButterLib](https://github.com/BUTR/Bannerlord.ButterLib/releases), [MCM](https://github.com/Aragas/Bannerlord.MBOptionScreen/releases), [UIX](https://github.com/BUTR/Bannerlord.UIExtenderEx/releases), [Harmony](https://github.com/BUTR/Bannerlord.Harmony/releases) |

---

## B. Alanlara göre modlar

### B1. Entrika, suikast, casusluk, entrika planları

| Mod | Ne ekler | Bağımlılık | Son bilinen sürüm | Örtüşme | Değer | Zorluk | H/UIX |
|---|---|---|---|---|---|---|---|
| [Fourberie (Cunning)](https://www.nexusmods.com/mountandblade2bannerlord/mods/2969) | Scheme Room: cinayet, kaçırma (v1.2.9.4; yapay zekâ da kullanır), başarısız cinayet sonrası kan davası (v1.5.0.0), şüphe çubuklu sızma, sabotaj taktikleri, çete liderleri, yankesicilik, dövüş çukurları ([Steam günlüğü](https://steamcommunity.com/sharedfiles/filedetails/changelog/2875710877)) | Listelenmemiş ([önceki araştırma](Bannerlord%20eksiklikleri%20ve%20mod%20f%C4%B1rsatlar%C4%B1/popular_gameplay_mods.md)) | v1.4.8.3 (18 Eki 2026, önceki araştırma); v1.5.0.3 arama özetinde (sürüm numarası doğrulanmadı) | Yüksek (suikast var; kaçırma, kan davası, sabotaj, gölge ağı planlı) | Rakip olarak incelenmeli; birlikte çalışma testi yapılmalı | — | — |
| [Intrigues (Bloc)](https://www.nexusmods.com/mountandblade2bannerlord/mods/2693) | Suikast, kaçırma (hedef esirin olur; sat/idam et/sakatla), işkence, söylenti yayma, seçilen şehirde huzursuzluk çıkarma. Bekleme süresi var; hedef önce ölürse ücret iade edilir. Yakalanırsan kınanırsın; hükümdar hedefse savaş gerekçesi doğar. ([GamePressure](https://www.gamepressure.com/download.asp?ID=77322)) | Doğrulanmadı | v1.0.8, Tem 2021, e1.5.7 (ölü) | Kısmen (suikast var; kaçırma planlı) | Fikir kaynağı: söylenti → huzursuzluk zinciri, iade kuralı | — | — |
| [Diplomacy & Intrigue](https://www.nexusmods.com/mountandblade2bannerlord/mods/13595) | Krallıklar arası antlaşma, haraç, vasallık; saray şikâyetleri ve iç savaş; casus ağları, gizli görevler, karşı casusluk, diplomatik olaylar. Ctrl+D menüsü, `diplomacy.*` konsol komutları ([GitHub](https://github.com/namphan19/Bannerlord-Diplomacy-Intrigue)) | BLSE, H 2.4.2+, BL 2.10.4+, UIX 2.13.2+, MCM 5.11.4+ | v0.4.0 (5 Eki 2026), 1.4.8. README'ye göre yalnız "Faz 0" tamam. | Kısmen | Evet: "ifşa → diplomatik kriz" (Özet #10) | kolay | Hayır |
| [Dynasty and Intrigue](https://www.nexusmods.com/mountandblade2bannerlord/mods/10764) | Hane birlikleri, hanedan iddiaları, ardıllık krizleri, siyasi evlilikler, hane kan davaları; yoldaşlarla "Emissary" diplomasisi; "House Compacts". Her özellik MCM'de ayrı açılır. | H, UIX, BL, MCM | v2.x, son güncelleme 10 May 2026 (1.4.8 durumu doğrulanmadı) | Kısmen (kan davası planlı) | Fikir: yoldaş elçiler | orta | Hayır |
| [Bellum Civile](https://www.nexusmods.com/mountandblade2bannerlord/mods/10609) (intrigue parçası) | Privy Council'de Casus başı. Görev seçenekleri: Karşı casusluk (+15 savunma), Muhalefeti ortaya çıkar (isyan büyümesi %15 yavaş), Söylenti yay (+15 saldırı). Başarısızlık "tartışmalılık" puanı artırır. Yetkinlik Roguery, Scouting ve Cunning'den gelir. ([GitHub](https://github.com/OhMyMike57/BellumCivile)) | H, UIX, MCM v5 | 1.4.1 (README 7 Eki 2026); Nexus'ta v1.4.5 (önceki araştırma) | Kısmen (casus başı var) | Evet (Özet #3) | kolay | Hayır |
| [Magnum Opus](https://www.nexusmods.com/mountandblade2bannerlord/mods/12035) | "Alley Network": sokakları yükselt (günlük gelir iki katı ya da sıfır), paralı asker kariyeri, ardıllık yasaları (primogenitür + kadın varis seçeneği), anıt şehirler | Nexus etiketleri H/UIX/BL (tam liste doğrulanmadı) | 1.6.8, "BL 1.4.8 & Warsails" | Kısmen (gölge ağı planlı) | Fikir: sokak geliri seviyeleri | orta | Hayır |
| [BannerCourt](https://www.nexusmods.com/mountandblade2bannerlord/mods/13940) | Skandal sistemi: ifşa olan lord zamanla sadakat ve güvenlik kaybeder; mahkeme kararları bunu yumuşatır ya da ağırlaştırır | Doğrulanmadı | 16 Eki 2026 | Kısmen (ifşa var) | Evet (Özet #7) | orta | Hayır |
| [Complex Characters (Bloc)](https://www.gry-online.pl/download/mount-blade-ii-bannerlord-complex-characters-v105-mod/zf152cd) | 467 durumsal diyalog satırı; lordlara hırs ve kişilik hedefleri. Hırs türleri (Fear/Throne/Wealth/Fame) **doğrulanmadı**. | Doğrulanmadı | v1.0.5, Haz 2025 | Yok | Evet (Özet #12) | zor | Hayır |
| [Regard and Standing](https://www.nexusmods.com/mountandblade2bannerlord/mods/13882) | Kin ve kan davaları, intikam yeminleri (teçhizatı alınan esir intikam yemini eder), esaretten kurtarılınca "onur borcu", şiddetli ölüm haberinin haneye yayılması | Doğrulanmadı | 17 Eki 2026 | Planlı (kan davası) | Evet: "onur borcu" bir tür olumlu koz | orta | Hayır |
| Affairs of Calradia | Bir arama özetine göre "Blood Debt" ve "Personal Rival" sistemleri; [Bridges to Remember](https://www.nexusmods.com/mountandblade2bannerlord/mods/13562) adında anılıyor. **Kendi sayfası doğrulanmadı.** | Doğrulanmadı | Doğrulanmadı | Planlı | Belki | — | — |

**Dikkat çekici:** Yapay zekânın oyuncuya entrika kurması yalnız Fourberie'de (kaçırma) bulundu. Yapay zekânın zehirle ya da suikastçıyla oyuncuyu hedef aldığı ayrı bir mod bulunamadı ([arama özeti](https://www.gamepressure.com/download/mount-blade-ii-bannerlord-intrigues-v108-mod/z912e0a)). Planlı "yapay zekâ entrikaları ve karşı casusluk" (E2 §8.4) gerçek bir boşluk.

### B2. Diplomasi ve siyaset

| Mod | Ne ekler | Bağımlılık | Son bilinen sürüm | Örtüşme | Değer | Zorluk | H/UIX |
|---|---|---|---|---|---|---|---|
| [Diplomacy](https://www.nexusmods.com/mountandblade2bannerlord/mods/832) | Haberciler, saldırmazlık paktı, fief bağışı, savaş yorgunluğu, genişlemecilik, iç savaş grupları, ayrılma, tahttan indirme ([önceki araştırma](Bannerlord%20eksiklikleri%20ve%20mod%20f%C4%B1rsatlar%C4%B1/popular_gameplay_mods.md)) | H, BL, UIX, MCM | v1.5.4 (9 Eki 2026), War Sails dahil | Kısmen (savaş/barış ilanı var) | Haberci fikri (Özet #2) | orta | Hayır |
| [Bellum Civile](https://www.nexusmods.com/mountandblade2bannerlord/mods/10609) | Üç saray bloğu (Nobility / Glory / Liberty), hükümdara meydan okuma ve koalisyon, ihanet oylaması, iç savaş sonrası mahkemeler (af / el koyma / sürgün / idam), hak iddiası uydurma, özel savaşlar, naiplik, kişisel birlikler, rehine anlaşmaları, kadet haneler, paralı asker şirketleri ([GitHub](https://github.com/OhMyMike57/BellumCivile)) | H, UIX, MCM v5; isteğe bağlı Diplomacy ve HAP. Lisans CC BY-NC-SA 4.0. | 1.4.1–1.4.5, hedef 1.4.8 | Kısmen | Evet (Özet #5, #6) | orta–zor | Kendi sistemlerimiz için hayır; krallık arayüzü sekmesi UIX ister |
| [Banner Kings Redux](https://github.com/GIO443/bannerlord-banner-kings-redux) | Feodal unvan hiyerarşisi, sözleşmeler, konsey (Mareşal / Kâhya / Şansölye / Casus başı / Saray hekimi), din (7 inanç + Nord), eğitim, sınıflara göre vergi, malikâneler, köle emeği ([wiki](https://github.com/GIO443/bannerlord-banner-kings-redux/wiki)) | H, BL, UIX, MCM + **Bannerlord Living Economy (zorunlu)** | 1.9.x (v1.4). Uyum yamaları BK Redux 1.9.33.5–1.9.33.8'i 1.4.7/1.4.8 ile eşliyor ([7886](https://www.nexusmods.com/mountandblade2bannerlord/mods/7886), [13393](https://www.nexusmods.com/mountandblade2bannerlord/mods/13393)) | Kısmen | Konsey fikri evet; din ve unvanlar çok büyük | zor | Hayır (kendi sürümümüz için) |
| [Crown and Kin](https://www.nexusmods.com/mountandblade2bannerlord/mods/12828) | Hükümdara boyun eğme, hanedan evliliği, dinamik ardıllık. DEV.10 sürümüyle meşruiyet ve tanınma: haneler hükümdarı tanır, şartlı tanır, tarafsız kalır, reddeder ya da başka bir adayı destekler. | BL, H, MCM, UIX | DEV.10, 1.4.8 için | Yok | Evet: "tanınma" fikri, varis seçimiyle birlikte (Özet #8) | orta | Hayır |
| [Kingdom Politics Expanded 1.3.X](https://www.nexusmods.com/mountandblade2bannerlord/mods/9507) | İhtiyaca dayalı yapay zekâ politika ve fief oylaması ([önceki araştırma](Bannerlord%20eksiklikleri%20ve%20mod%20f%C4%B1rsatlar%C4%B1/popular_gameplay_mods.md)) | H | 20 Ara 2025 | Yok | Hayır (bizim kapsam dışı) | zor | Harmony |
| [AI Influence](https://www.nexusmods.com/mountandblade2bannerlord/mods/9711) | LLM ile diyalog, hafıza, dinamik olaylar; NPC yalanı hatırlar; `world_secrets.json` ile kendi irfan ve görevlerini yazma; MCM'de 70+ ayar ([smods](https://catalogue.smods.ru/archives/402946)) | BL, H, MCM, UIX | v6.0.2 BETA (Tem 2026, önceki araştırma) | Yok | Hayır (LLM ve API anahtarı gerekir). Ama "sırlar dosyası" fikri ilginç. | zor | — |
| [Courier](https://www.nexusmods.com/mountandblade2bannerlord/mods/9048) / [Telepathy](https://www.nexusmods.com/mountandblade2bannerlord/mods/1203) / [Realistic Courier](https://www.nexusmods.com/mountandblade2bannerlord/mods/12106) | Ansiklopediden haberci gönderme; anında ya da "güvercin postası" gecikmeli konuşma; yoldaş haberci fiziksel olarak gider ve döner | Courier: BL/H/MCM/UIX. Telepathy: MCM. Realistic Courier: doğrulanmadı. | Courier 1.4.X (Haz 2026); Telepathy 1.3.8 (Kas 2025) | Yok | Evet (Özet #2) | orta | Ansiklopedi düğmesi UIX ister; menüden açarsak hayır |
| Diplomacy Overhaul ([13617](https://www.nexusmods.com/mountandblade2bannerlord/mods/13617)) | İçerik doğrulanmadı | Doğrulanmadı | Doğrulanmadı | — | — | — | — |

### B3. Aile, evlilik, romantizm, çocuklar, hanedan

| Mod | Ne ekler | Bağımlılık | Son bilinen sürüm | Örtüşme | Değer | Zorluk | H/UIX |
|---|---|---|---|---|---|---|---|
| [Dramalord](https://www.nexusmods.com/mountandblade2bannerlord/mods/6682) | Flört, buluşma, ilişkiler, aşk evliliği, gizli sevgililer, dedikodu, tanıklar, şantaj, düello. Öfkelenen kahramanlar birbirini öldürebilir. MCM ile çok ayarlanabilir. ([smods](https://catalogue.smods.ru/archives/271407), [Steam günlüğü](https://steamcommunity.com/sharedfiles/filedetails/changelog/3233297311?p=2)) | H 2.4.2, BL 2.10.3, UIX 2.13.2, MCM 5.11.3 ([GitHub SubModule.xml](https://github.com/steffenkopany/Dramalord)) | Nexus 5.0.42 ([arama özeti](https://www.nexusmods.com/mountandblade2bannerlord/mods/6682?tab=files&file_id=47138&nmm=1)); GitHub master 1.3.15 hedefli; 1.4.8 doğrulanmadı | Kısmen (sırlar var) | Evet, sahnesiz biçimde (Özet #4) | orta | Hayır |
| [Regard and Standing](https://www.nexusmods.com/mountandblade2bannerlord/mods/13882) | İlişki ve yakınlık, kur ve düğün, ilişkiler, kin, dedikodu ("kim duydu"); `regardstanding.*` konsol komutları ve ayrıntılı günlük | Doğrulanmadı | 17 Eki 2026 | Kısmen | Evet (Özet #4, #15) | orta | Hayır |
| [Marry Anyone (Game 1.5.X)](https://www.nexusmods.com/mountandblade2bannerlord/mods/5240) / [Continued](https://www.nexusmods.com/mountandblade2bannerlord/mods/11949) | Herkesle evlilik, yönelim seçenekleri, çok eşlilik seçeneği; kuru atlama ve yeniden deneme ayarları | H, MCM | 3.6.0.2 (14 Eyl 2026, 1.4.x–1.5.x); Continued 6 Tem 2026 | Yok | Editörde "evlendir" (Özet #9). Çok eşlilik hayır (bkz. C). | kolay | Hayır |
| [Family Control (Pregnancy)](https://www.nexusmods.com/mountandblade2bannerlord/mods/2813) | Gebelik zamanlaması, gayrimeşru çocuklar, çocuğun kültürü, yetişkinlikte evlat edinme, savaşta düşük riski ([önceki araştırma](Bannerlord%20eksiklikleri%20ve%20mod%20f%C4%B1rsatlar%C4%B1/popular_gameplay_mods.md)) | H | 1.4.5 / War Sails (4 Haz 2026) | Yok | Evet: editör + "gayrimeşru çocuk" sırrı | orta | Vanilla gebelik kuralını değiştirmek H ya da model ister; editörden tek seferlik işlem için gerekmez |
| [Houses And Heirs](https://www.nexusmods.com/mountandblade2bannerlord/mods/13050) | Evli klan üyelerinden çocuk sahibi olmalarını istemek; evlilik, gebelik ve doğum geçmişi | MCM v5 | 1.4.8 referans | Yok | Belki | kolay | Hayır |
| [Dynasty Core](https://www.nexusmods.com/mountandblade2bannerlord/mods/13193) | Doğurganlık, gebelik hızı, otomatik çocuk eğitimi | Doğrulanmadı | 1.5.5 (1.4.8 için) | Yok | Belki: "doğurganlık çarpanı" kuralı | kolay | Model zinciri; H gerekmez (doğrulanmadı) |
| [Noble Household](https://www.nexusmods.com/mountandblade2bannerlord/mods/13938) | Çocuklar için öğretmen, eğitmen, bakıcı NPC'ler | Doğrulanmadı | v0.4.1, erken erişim, 1.4.8 | Yok | Belki | orta | Hayır |
| [Crown and Kin](https://www.nexusmods.com/mountandblade2bannerlord/mods/12828) / [Magnum Opus](https://www.nexusmods.com/mountandblade2bannerlord/mods/12035) / [Bellum Civile](https://github.com/OhMyMike57/BellumCivile) | Ardıllık yasaları, kadın varis seçeneği, naiplik, evlilikle iddia kazanma | Yukarıda | 1.4.8 | Yok | Evet (Özet #8) | orta | Krallık seçim kuralı için H |
| [RoT – Dynasty and Succession](https://www.nexusmods.com/mountandblade2bannerlord/mods/13309) | Klan sayfasında varis sırası; kahramanlar "meşru / piç / meşrulaştırılmış" diye etiketlenir (yalnız Realm of Thrones) | H, UIX, MCM v5 (arama özeti) | Doğrulanmadı | Yok | Evet: meşrulaştırma fikri | orta | Klan sayfasına yazmak UIX ister; popup'ta göstermek gerektirmez |
| [Romance](https://www.nexusmods.com/mountandblade2bannerlord/mods/9032) | Warband tarzı kur: şiir, turnuva adağı, hediye, sevgi göstergesi | Doğrulanmadı | 1.3.5 | Yok | Belki: "baştan çıkarma" entrikasının sahnesiz adımları | orta | Hayır |
| [Brut.FamilyTree](https://www.nexusmods.com/mountandblade2bannerlord/mods/9605) / Family Tree (Steam) | Soy ağacı görünümü | Doğrulanmadı | Doğrulanmadı | Yok | Belki (popup'ta metin ağacı) | orta | Görsel ağaç Gauntlet ister |
| [Female Lord Matrilineal Marriage](https://www.nexusmods.com/mountandblade2bannerlord/mods/12496) | Yapay zekâ kadın klan liderleri için anaerkil evlilik | Doğrulanmadı | 3 Ağu 2026 | Yok | Hayır | — | — |
| Children Grow Faster Redux, Education, Keep Your Clan Members After Marriage, Marriage Offers For Player | [Önceki araştırma](Bannerlord%20eksiklikleri%20ve%20mod%20f%C4%B1rsatlar%C4%B1/popular_gameplay_mods.md) | — | — | Yok | "Çocuğu büyüt" editörü (Özet #9) | kolay | Hayır |

### B4. Ekonomi ve yerleşim yönetimi

| Mod | Ne ekler | Bağımlılık | Son bilinen sürüm | Örtüşme | Değer | Zorluk | H/UIX |
|---|---|---|---|---|---|---|---|
| [Bannerlord Living Economy](https://www.nexusmods.com/mountandblade2bannerlord/mods/10796) | Toplumsal sınıflar, tedarik zincirleri, bölgesel talep, mevsimler, akıllı kervanlar, lord yatırımı. Modül kimliği hâlâ `BetterEconomy`. | Doğrulanmadı | 1.4.6 | Yok | Hayır (bizim kapsam dışı; büyük bir model değişimi) | zor | — |
| [Harvest And Production (HAP)](https://www.nexusmods.com/mountandblade2bannerlord/mods/9671) | İklim bölgeleri, mevsimler, 20+ yeni ürün, yiyecek ekonomisi ve bozulma, köy vergisi düzeltmesi. Aynı anda tek bir ekonomi modu önerilir. | Doğrulanmadı | Aktif (arama özeti) | Yok | Hayır | zor | — |
| [Bannerlord Economy Overhaul](https://www.nexusmods.com/mountandblade2bannerlord/mods/9558), [Improved Economy For AI Lords](https://www.nexusmods.com/mountandblade2bannerlord/mods/8698) | Fiyat ve yapay zekâ lord ekonomisi | Doğrulanmadı | Doğrulanmadı | Yok | Hayır | — | — |
| Settlement Finances (Snah79) ([tanıtım](https://www.gamewatcher.com/mount-and-blade-2-bannerlord-settlement-finances-mod)) | Büyük şehirlerde banka: yatırma, çekme, faiz, kredi; toprak sahibine ya da çete liderine yatırım | Doğrulanmadı | Eski (sürüm doğrulanmadı) | Yok | Evet (Özet #13) | orta | Hayır |
| Banks of Calradia ([Xorberax's Legacy](https://www.nexusmods.com/mountandblade2bannerlord/mods/3462) içinde) | Banka sistemi ([önceki araştırma](Bannerlord%20eksiklikleri%20ve%20mod%20f%C4%B1rsatlar%C4%B1/popular_gameplay_mods.md)) | H, MCM | 1.0.8.8 (1.4.8) | Yok | Evet (Özet #13) | orta | Hayır |
| Life in Calradia (OneDrop) ([röportaj](https://www.nexusmods.com/pine/news/14658)) | Ev ve han satın alma, kiralama, yükseltme (3 kalite). Köy evlerinde bahçe. Aile ve yoldaşlar taşınır, günlük rutin izler. | Doğrulanmadı | Doğrulanmadı | Yok | Belki: "güvenli ev" olarak gölge ağına bağlanabilir | zor (sahneler) | Hayır, ama sahne işi gerekir |
| [Magnum Opus](https://www.nexusmods.com/mountandblade2bannerlord/mods/12035) | Sokak (alley) gelir yükseltmeleri; anıt şehirlere milis ve garnizon bonusu | — | 1.6.8 | Planlı (gölge ağı) | Evet (gölge ağına gelir katmanı) | orta | Hayır |
| Player Settlements, Homesteads, Improved Garrisons, Governors Handle Issues, AutoTrader | [Önceki araştırma](Bannerlord%20eksiklikleri%20ve%20mod%20f%C4%B1rsatlar%C4%B1/popular_gameplay_mods.md) | — | — | Kısmen (yerleşim editörü var) | Hayır (büyük ayrı modlar) | — | — |

### B5. Yoldaşlar ve kişilikler

| Mod | Ne ekler | Bağımlılık | Son bilinen sürüm | Örtüşme | Değer | Zorluk | H/UIX |
|---|---|---|---|---|---|---|---|
| [Delegated Missions](https://www.nexusmods.com/mountandblade2bannerlord/mods/13846) | Gruplar fiziksel olarak gider, savaşır, döner: erzak, asker toplama, yağma, kuşatma, köy ele geçirme, savunma, devriye, **casusluk**. Etiket: "AI-Generated Content". | Doğrulanmadı | v0.9.0, 1.5.4 + War Sails | Yok | Evet (Özet #11) | zor (fiziksel) / orta (soyut) | Hayır |
| [Companion Orders](https://www.nexusmods.com/mountandblade2bannerlord/mods/12912) | Yoldaşı bütçeyle ileri gelenlerin desteğini toplamaya gönderme | Doğrulanmadı | Doğrulanmadı | Yok | Evet | orta | Hayır |
| [Send Companions Recruiting](https://www.nexusmods.com/mountandblade2bannerlord/mods/3506) | Kültür, kademe, bütçe ve süre seçerek asker toplamaya gönderme | Doğrulanmadı | v1.1.0 (eski) | Yok | Belki | orta | Hayır |
| [Companion Dynasties](https://www.nexusmods.com/mountandblade2bannerlord/mods/12933) / [Make Companions Vassals](https://www.nexusmods.com/mountandblade2bannerlord/mods/3150) | Yoldaşa soyluluk verme, yoldaştan klan kurma | Doğrulanmadı | Doğrulanmadı | Kısmen (klan araçları var) | Evet: "yoldaştan klan kur" editörü | kolay | Hayır |
| [They Married (Your Companions)](https://www.nexusmods.com/mountandblade2bannerlord/mods/8358) | Yoldaş evlilikleri (ayrıntı doğrulanmadı) | Doğrulanmadı | Doğrulanmadı | Yok | Belki | kolay | Hayır |
| [Deserving Companions](https://www.nexusmods.com/mountandblade2bannerlord/mods/3074) | Yoldaş görevlerinin beceri eşiğini düşürür, XP'yi artırır | Doğrulanmadı | Eski | Kısmen (XP çarpanları var) | Hayır | — | — |
| Complex Characters | Durumsal diyalog, NPC'lerin oyuncunun eylemlerine yorum yapması | Doğrulanmadı | Haz 2025 | Yok | Belki | zor | Hayır |
| Living Wanderers, Useful companions, Distinguished Service, More Tavern Companions | [Önceki araştırma](Bannerlord%20eksiklikleri%20ve%20mod%20f%C4%B1rsatlar%C4%B1/popular_gameplay_mods.md); [smods](https://catalogue.smods.ru/archives/411715) | — | — | Kısmen (yoldaş sınırı kaldırma var) | Hayır | — | — |

**Boşluk:** Yoldaşların onay, laf atışması ya da fikir sistemi için yayımlanmış bir mod bulunamadı. Yalnız bir Nexus forum önerisi var ([forum](https://forums.nexusmods.com/profile/41002870-dovah164/)). Kişilik özelliklerine dayalı "yoldaş tepkisi" (ör. Onurlu yoldaş suikasttan rahatsız olur) entrikaya doğal olarak bağlanır.

### B6. Esirler ve kölelik benzeri sistemler

| Mod | Ne ekler | Bağımlılık | Son bilinen sürüm | Örtüşme | Değer | Zorluk | H/UIX |
|---|---|---|---|---|---|---|---|
| [Bellum Civile](https://github.com/OhMyMike57/BellumCivile) | Rehine anlaşmaları (100 gün, çocuklar uygun, korumalı gözetim, kaçış ve fidye engelli); iç savaş sonrası mahkemeler | H, UIX, MCM | 1.4.x | Yok | Evet (Özet #5) | orta | Kaçış engeli için H |
| [Intrigues](https://www.nexusmods.com/mountandblade2bannerlord/mods/2693) | Kaçırılan hedef esir olur; sat, idam et ya da sakatla. İşkence seçeneği. | Doğrulanmadı | e1.5.7 | Planlı (kaçırma) | Evet: işkencenin yerine sahnesiz "sorgu" (Özet #1) | kolay | Hayır |
| [Banner Kings Redux](https://github.com/GIO443/bannerlord-banner-kings-redux) | Köle nüfus sınıfı, malikânelerde esir emeği (Steward perk'i), Nord "thrall" kanunu, köy baskınlarında esir alma ([Steam yorumları](https://steamcommunity.com/workshop/filedetails/discussion/2880158010/7048779603517444007)) | H, BL, UIX, MCM, Living Economy | 1.9.x | Yok | Hayır (ekonomi modeli ister; tema hassas) | zor | — |
| Lord Plunder ([ModDB](https://www.moddb.com/mods/lord-plunder)) | Savaş sonrası esir lordu yağmala ya da bağışla, ilişkiye etki. Fidye sistemi "yakında" yazıyor. | Doğrulanmadı | Doğrulanmadı | Yok | Evet: "esir lordun teçhizatını al" (+ Regard and Standing'deki intikam yemini) | kolay | Hayır |
| Prevent escape ([134](https://www.nexusmods.com/mountandblade2bannerlord/mods/134)), True Prisoners | Soylu esirlerin kaçışını ve pazarlığını değiştirir | Doğrulanmadı | Eski | Yok | Belki | orta | Muhtemelen H |
| [Prisoner Lords Expanded](https://www.nexusmods.com/mountandblade2bannerlord/mods/newrecently) | 18 Eki 2026 güncellemesi; içerik **doğrulanmadı** | — | — | — | — | — | — |
| Surrender Tweaks, Recruitable ve esir toplama modları | [Önceki araştırma](Bannerlord%20eksiklikleri%20ve%20mod%20f%C4%B1rsatlar%C4%B1/popular_gameplay_mods.md) | — | — | Kısmen | Hayır | — | — |
| Captivity Events | 18+; bkz. bölüm C | — | — | — | — | — | — |

### B7. Olaylar ve görevler

| Mod | Ne ekler | Bağımlılık | Son bilinen sürüm | Örtüşme | Değer | Zorluk | H/UIX |
|---|---|---|---|---|---|---|---|
| [BannerCourt](https://www.nexusmods.com/mountandblade2bannerlord/mods/13940) | Lord salonunda belli saatlerde yargılama, hüküm tarzları, Lordlar Konseyi'nde yasa tartışması, dilekçeler (haydut, asker kaçağı, kaçakçı). Warband'dan "Nervous Man" görevi. Görev açıkken mod kaldırılırsa kayıt bozulur. | Doğrulanmadı | 16 Eki 2026 | Planlı (olay kartları) | Evet (Özet #7) | orta | Hayır |
| [Banner Flavor Events](https://www.nexusmods.com/mountandblade2bannerlord/mods/9137) | Yolculukta rastgele olaylar ve sefer sistemi: popup, istatistik değişimi, atmosfer metni; War Sails için deniz güncellemesi planlı | H, MCM | v2.8, Kas 2025'te yüklendi, 1.3.10 test edildi | Kısmen (olay popup'ları var) | Evet: entrika dışı rastgele olay paketi | kolay | Hayır |
| [Random Events](https://www.nexusmods.com/mountandblade2bannerlord/mods/1410) | 59 olay; olay kuyruğu ve geçmişi için konsol komutları | Doğrulanmadı | 2020 (ölü) | Kısmen | Fikir: olay geçmişi | kolay | Hayır |
| [Random Event Expansion](https://www.nexusmods.com/mountandblade2bannerlord/mods/9907) | Captivity Events eklentisi: 700+ dallanan olay, 300+ özel çizim; şehir, kara, deniz, zindan ve esir senaryoları. CE'ye bağlı olduğu için **yetişkin içerik olasılığı yüksek, doğrulanmadı.** | Captivity Events | 1.3.13 | Planlı (resimli kartlar) | Yalnız ölçek ve format örneği; içerik alınmaz | — | — |
| [World Events Announcer](https://www.nexusmods.com/mountandblade2bannerlord/mods/12730) | Mevsimlik dünya olayları özeti popup'ı | Doğrulanmadı | Doğrulanmadı | Yok | Evet (Özet #15) | kolay | Hayır |
| AI Influence, Serve as Soldier, Enlisted | LLM olayları; 7 rastgele olay; 275+ anlatı parçası ([önceki araştırma](Bannerlord%20eksiklikleri%20ve%20mod%20f%C4%B1rsatlar%C4%B1/popular_gameplay_mods.md)) | — | — | — | — | — | — |

### B8. Arayüz ve QoL (ansiklopedi, parti ekranı, harita)

Ayrıntılar önceki dosyada: [popular_gameplay_mods.md §3g](Bannerlord%20eksiklikleri%20ve%20mod%20f%C4%B1rsatlar%C4%B1/popular_gameplay_mods.md). Bizim için öne çıkanlar:

| Mod | Ne ekler | Bağımlılık | Son bilinen sürüm | Örtüşme | Değer | Zorluk | H/UIX |
|---|---|---|---|---|---|---|---|
| Encyclopedia Extender Reforged ([smods](https://catalogue.smods.ru/?p=182530)) | Kahraman sayfasında seviye, teçhizat değeri, çeyiz fiyatı, esir/ordu durumu. Filtreler. Krallık bütçesi, esir düşman kahramanlar, klan borcu. MIT lisanslı, kayıtla eklenip çıkarılabilir. | Doğrulanmadı | Steam günlüğünde 1.3.12'ye kadar | Kısmen (gerçek kahraman bulucu var) | Evet: "kahraman bilgi kartı" popup'ı (çeyiz, servet, esir durumu) | kolay | Popup'ta hayır; ansiklopediye eklemek UIX ister |
| Party Screen Enhancements (ölü), Troop Sorting | Toplu yükseltme ve sıralama | — | 2022 | Yok | Belki (popup'tan "hepsini yükselt") | kolay | Popup ile hayır |
| Better Time | Hız düğmeleri | MCM, UIX | 1.1.3 | Var (hızlı ileri sarma) | — | — | — |
| Show Companion Requirements, Ask Where A Lord Is, Remove Fog of War | [Önceki araştırma](Bannerlord%20eksiklikleri%20ve%20mod%20f%C4%B1rsatlar%C4%B1/popular_gameplay_mods.md) | — | — | Var (konum bulucu, sis kaldırma) | — | — | — |

### B9. Hile ve editör modları (Character Reload, Bannerlord Tweaks, hile menüleri)

Ayrıntılı katalog: [cheat_editor_mods.md](Bannerlord%20eksiklikleri%20ve%20mod%20f%C4%B1rsatlar%C4%B1/cheat_editor_mods.md). Aşağıda yalnız **bizde henüz olmayan** özellikler var:

| Özellik | Hangi modda | Örtüşme | Değer | Zorluk | H/UIX |
|---|---|---|---|---|---|
| Evlendir / boşa | BLGM, Commander, Cheats | Yok | Evet (Özet #9) | kolay | Hayır |
| Hamileliği başlat, doğumu tetikle (sahnesiz) | BLGM, Commander, Cheats (olasılık ve süre çarpanı) | Yok | Evet | kolay | Hayır |
| Çocuğu büyüt | Character Reload ("Grow Up") | Kısmen (yaş editörü var) | Evet | kolay | Hayır |
| Yaşlanmayı dondur (kişi veya grup) | No Aging Extended, BannerWand | Yok | Evet | kolay | Hayır (günlük yeniden yazma; doğrulanmadı) |
| Kahraman, yoldaş ya da lord oluşturma | BLGM, Commander | Yok | Belki (kayıtta kalıcı varlık, risk) | orta | Hayır |
| Diriltme | Character Reload | Yok | Hayır (1.4.8'de çöküyor, önceki araştırma) | zor | — |
| Sancak düzenleme, krallık renk eşitleme | Character Reload, BLGM | Yok | Belki | orta | Hayır |
| Yerleşim kültürü değiştirme | BLGM `set_culture` | Yok | Evet | kolay | Hayır |
| Eşya niteliği (efsanevi vb.) ve teçhizat şablonu kaydet/yükle | BLGM | Kısmen (eşya üretici var) | Belki | orta | Hayır |
| Pazarlık ve ikna her zaman başarılı | Cheats, BannerWand | Yok | Evet (koz ve haberciyle güzel uyum) | orta | Muhtemelen H |
| Kime uygulanacağı seçimi (oyuncu / klan / krallık / küçük fraksiyon) | BannerWand | Kısmen (ben / ordu / müttefik var) | Evet | kolay | Hayır |
| Konsol komut katmanı | BLGM | Yok | Belki (test için) | kolay | Hayır |
| Bannerlord Tweaks tarzı denge ayarları (demircilik dayanıklılığı, turnuva ödülü, kuşatma vb.) | Bannerlord Tweaks (2020, ölü); Kaoses (2023, ölü) | Kısmen (Kurallar bölümü) | Evet: yaşayan bir halefi yok | kolay–orta | Çoğu model zinciriyle hayır |

### B10. War Sails ve deniz eklemeleri

| Mod | Ne ekler | Bağımlılık | Son bilinen sürüm | Örtüşme | Değer | Zorluk | H/UIX |
|---|---|---|---|---|---|---|---|
| [War Sails Tweaks](https://www.nexusmods.com/mountandblade2bannerlord/mods/12697) | Deniz beceri XP çarpanı; oyuncu ve yapay zekâ için ayrı deniz yolculuk hızı (varsayılan 1,25); gemi alım/satım/onarım/yükseltme fiyat çarpanları; deniz morali çökme koruması | Doğrulanmadı | "Game v1.4.X" | Kısmen (parti hızı, XP çarpanı var) | Evet (Özet #14) | kolay | Hız için hayır |
| [Jomsborg Pirates](https://www.nexusmods.com/mountandblade2bannerlord/mods/10304) | Kıyılarda tüccar avlayan korsan fraksiyonu | BL, H, MCM, UIX | v1.1.3.3 | Yok | Belki: "korsan kiralama / kaper mektubu" entrikası | orta | Hayır |
| [Station Troops With Fleet](https://www.nexusmods.com/mountandblade2bannerlord/mods/9768) | Karaya çıkarken askerleri filoda bırakma | Doğrulanmadı | Doğrulanmadı | Yok | Hayır | — | — |
| [Simple Naval Improvements](https://www.nexusmods.com/mountandblade2bannerlord/mods/9657) | Küresel yelken hızı artışı (yapay zekâ denize daha sık açılsın diye) | Doğrulanmadı | v2.0 hazırlanıyor | Kısmen | — | — | — |
| [Naval Physics Crash Patch](https://www.nexusmods.com/mountandblade2bannerlord/mods/11455) | Büyük deniz savaşlarında çökme düzeltmesi | Doğrulanmadı | War Sails v1.4.5 | Yok | Hayır | — | — |
| [LG Naval Ship Variants](https://www.nexusmods.com/mountandblade2bannerlord/mods/13642), [WarsailsBannerSails](https://www.nexusmods.com/mountandblade2bannerlord/mods/10696), The Carrack (MOTM adayı) | Gövde varyantları, yelkende sancak, yeni gemi (The Carrack'ın sayfası doğrulanmadı) | — | — | Yok | Hayır (varlık işi) | — | — |
| Helmsman, Ship Ferries, Army Fleets, Fleet Menu, Retinues konvoy editörü | [Önceki araştırma](Bannerlord%20eksiklikleri%20ve%20mod%20f%C4%B1rsatlar%C4%B1/popular_gameplay_mods.md) | — | — | Yok | Belki: "feribot ışınlaması" bizde zaten var (denizde ışınlanma) | — | — |
| BannerWand (sonsuz gemi canı), UFO Bundle (gemi, figür ve yükseltme verme) | [Önceki araştırma](Bannerlord%20eksiklikleri%20ve%20mod%20f%C4%B1rsatlar%C4%B1/cheat_editor_mods.md) | — | — | Yok | Evet: deniz hileleri (Özet #14) | orta | Gemi API'si doğrulanmadı |

**Boşluk:** Özel bir kaper (privateer) modu bulunamadı ([arama özeti](https://www.nexusmods.com/mountandblade2bannerlord/mods/12697)). "Limanda filo sabotajı" ve "kaper mektubu" bizim entrika motoruyla özgün olur.

### B11. Çerçeve modlar ve bağımlılık

| Kütüphane | Son sürüm (Ekim 2026) | Not |
|---|---|---|
| [Harmony (BUTR)](https://github.com/BUTR/Bannerlord.Harmony/releases) | 2.4.2.248 (21 Tem) | Sürüm notunda oyun sürümü yok. Önceki araştırmaya göre son bir yılda resmî olmayan "Harmony Updated v1.4.6" yeniden derlemesi de popüler oldu. |
| [ButterLib](https://github.com/BUTR/Bannerlord.ButterLib/releases) | 2.12.0 (23 Ağu) | "v1.0.x–v1.5.x"; 1.4.8 ve 1.5.1'e güncellendi |
| [UIExtenderEx](https://github.com/BUTR/Bannerlord.UIExtenderEx/releases) | 3.0.3 (7 Eki) | v3 prefab derliyor. 3.0.2'de, Windows Smart App Control prefab DLL'ini engellediğinde çökmeyi önleyen düzeltme var. Yeni ana sürüm demek, yeni kırılma riski demek. |
| [MCM](https://github.com/Aragas/Bannerlord.MBOptionScreen/releases) | 5.13.1 (17 Eki) | **5.13.0'dan itibaren modlar ayarlarını kendi `SubModule.xml` dosyalarında tanımlayabiliyor; MCM kuruluysa bunları çiziyor.** Biçim, MCM DLL'ine derleme zamanı referansı gerekip gerekmediği ve değerin nasıl geri okunacağı **doğrulanmadı.** Doğrulanırsa bizim için "MCM varsa ayar ekranı, yoksa popup" seçeneği sunar. |

**Bu rapordaki örneklemde bağımlılık dağılımı.** Bağımlılığı bilinen 38 mod sayıldı; Nexus'taki "Mods requiring this file" sayıları doğrulanmadı.

| Bağımlılık seti | Mod sayısı | Örnekler |
|---|---|---|
| Tam BUTR yığını (H + BL + UIX + MCM) | 12 | Diplomacy, BK Redux, Diplomacy & Intrigue, Dynasty and Intrigue, Crown and Kin, AI Influence, Courier, Dramalord, Jomsborg Pirates, Cheats Reload, Better Smithing Continued, My Little Warband (asıl sürüm) |
| H + BL + UIX (MCM önerilir) | 1 | UFO's Cheat Mods Bundle |
| H + UIX + MCM | 4 | Bellum Civile, Player Settlements, Surrender Tweaks, RoT – Dynasty and Succession |
| H + MCM | 5 | Marry Anyone, Banner Flavor Events, Captivity Events (Updated 1.4.7), BannerWand, Xorberax's Legacy |
| H + UIX | 2 | Party AI Controls, Retinues (v2) |
| Yalnız H | 5 | Family Control, Serve as Soldier, Enlisted, Kingdom Politics Expanded, Character Reload Fix |
| Yalnız MCM (MCM kendisi H/BL/UIX çeker) | 3 | Telepathy, DCC, Houses And Heirs |
| MCM + UIX | 1 | Better Time |
| Listelenmemiş / yok | 5 | Fourberie, Improved Garrisons, Calradia at War, Distinguished Service, AutoTrader |

- Bu örneklemde 38 modun 33'ü (yaklaşık %87) dolaylı ya da doğrudan Harmony istiyor.
- Kaynaklar: yukarıdaki tablolar ve [önceki araştırma](Bannerlord%20eksiklikleri%20ve%20mod%20f%C4%B1rsatlar%C4%B1/popular_gameplay_mods.md).
- Bağımlılıksız büyük modlar (Fourberie, Improved Garrisons) en uzun yaşayanlar arasında. Bu, bizim "Harmony yok" kararımızı destekliyor.

---

## C. Yetişkin (18+) modlar: yalnızca ad, bağlantı ve kategori düzeyinde

Kapsam notu: Burada açık içerik yeniden üretilmez. Bu modların çoğu LoversLab'de ya da Nexus'ta yetişkin içerik filtresi arkasında. Sayfalar doğrudan açılamadığı için bilgiler arama özetlerinden.

**Bilinen adlar**

| Mod | Bağlantı | Kategori | Not |
|---|---|---|---|
| Captivity Events (CE) | [Nexus 1226](https://www.nexusmods.com/mountandblade2bannerlord/mods/1226); [CE (Updated 1.4.7) 12254](https://www.nexusmods.com/mountandblade2bannerlord/mods/12254); LoversLab topluluk yapımı ([ayna açıklaması](https://aqxaromods.com/othergame/mount-amp-blade-2-bannerlord/18832-captivity-events-v1301.html)) | Esaret olayları, kölelik ve zorla çalıştırma, seks işçiliği. Kendi olay ve resimlerini yapma araçları. | Yalnız yetişkinler için ([GamePressure](https://www.gamepressure.com/editorials/the-best-mods-to-mount-and-blade-ii-bannerlord/improved-garrisons/zc282)). 1.4.7 fork'u: H 2.4.2, MCM 5.11.4+, "Default Captivity Events" paketi. |
| Hot Butter | [Ayna sayfası](https://aqxaromods.com/othergame/mount-amp-blade-2-bannerlord/18831-hot-butter-v207.html); [vgtimes](https://vgtimes.ru/games/mount-blade-2-bannerlord/files/52989-vozmozhnost-zanjatsja-seksom.html) | Animasyonlu cinsel sahne çerçevesi; eşler ve taverna kızlarıyla sahneler; CE sahneleri | v2.0.7 "1.3.X sorunları düzeltildi, War Sails ile çalışır" (ayna, doğrulanmadı) |
| Random Event Expansion | [Nexus 9907](https://www.nexusmods.com/mountandblade2bannerlord/mods/9907) | CE eklentisi; 700+ olay, 300+ çizim | Yetişkin içerik olasılığı yüksek (doğrulanmadı) |
| Concubines + Bastard Children → Illegitimate Children for AI Influence | [Concubines 5455](https://www.nexusmods.com/mountandblade2bannerlord/mods/5455); [10910](https://www.nexusmods.com/mountandblade2bannerlord/mods/10910); [GitHub fork](https://github.com/Shadow32055/Bannerlord.Concubines) | Cariyelik yoluyla birden çok "eş", gayrimeşru çocuklar | GitHub fork'unun Ara 2025 sürümü 1.3.9 hedefli (arama özeti) |
| Marriage and Fertility | [smods aynası](https://catalogue.smods.ru/?p=266677) | Çok eşlilik (oyuncu ve NPC), evlilik yaşı ayarı | Steam'de; 1.4.8 durumu doğrulanmadı |
| Dramalord | [Nexus 6682](https://www.nexusmods.com/mountandblade2bannerlord/mods/6682) | İlişkiler, gizli sevgililer, "friends-with-benefits" gebelik seçeneği | Açık sahne içerdiği **doğrulanmadı**. Ana içeriği metin ve olay. |
| Bannerlord Expanded – Spouses Expanded | [Nexus 8034 üzerinden anılıyor](https://nexusmods.com/mountandblade2bannerlord/mods/8034) | Çok eş | Doğrulanmadı |
| Krallık politikası olarak çok eşlilik | [forum yorumu](https://forums.nexusmods.com/profile/1563234-c4w/) (Nexus 783) | Çok eşlilik | Doğrulanmadı |
| Çıplaklık / beden dokuları | [2020 Steam tartışması](https://steamcommunity.com/app/261550/discussions/0/2144217924377078710) | İç çamaşırı kaldırma | Güncel bir mod **doğrulanmadı** |
| "Bannerlord with Adult Flavour" koleksiyonu | [Nexus koleksiyonu](https://www.nexusmods.com/games/mountandblade2bannerlord/collections/0oirte) | "Vanilla öncelikli, hafif yetişkin tonlu" 11 mod | Mod listesi doğrulanmadı |

**Kategori bazında karar**

| Kategori | Örnek | Fade-to-black uyarlama mümkün mü? | Karar ve nasıl |
|---|---|---|---|
| Romantizm ve baştan çıkarma | Dramalord (flört, buluşma), Romance (şiir, hediye, turnuva adağı) | **Evet** | "Baştan çıkarma" entrikası: aşamalar şiir, hediye ya da gizli buluşma; sahne yerine tek satır metin ("Gece geç saatlere kadar sürdü…"). Sonuç: ilişki artışı, koz ya da sır; yakalanırsa skandal. **Yalnız iki taraf da 18+ yetişkinse.** |
| Sevgililer, harem, çok eşlilik | Concubines, Marriage and Fertility, Marry Anyone (çok eşlilik seçeneği) | **Kısmen** | *Gizli sevgili* yalnız sır ve ilişki durumu olarak tutulur: evet. Gerçek çok eşlilik ve harem yapılmaz: motorun tek eşli `Spouse` modeline aykırı, Harmony ister, kayıt riski yüksek. |
| Gayrimeşru çocuklar | Family Control, Concubines, RoT – Dynasty and Succession | **Evet** | Sır türü "gayrimeşru çocuk": ifşa, şantaj ve meşrulaştırma kararı (Özet #4, #8). Doğum anlatısı yok; yalnız bildirim. |
| Gebelik kontrolü | Family Control, Dynasty Core, BLGM | **Evet** | Editör ve hile: gebeliği başlat, klan için kapat, doğurganlık çarpanı. Tamamen sayısal, açık içerik yok. 18+ kontrolü kodda zorunlu. |
| Genelev ve taverna işletmeleri | CE (seks işçiliği), Hot Butter (genelev) | **Cinsel hizmet olarak hayır** | Yapmayacağız. Cinsel olmayan uyarlama kabul: "han/taverna işletmesi" geliri ve gölge ağına muhbir kaynağı (Life in Calradia tarzı). |
| Çıplaklık ve animasyon paketleri | Hot Butter, beden dokuları | **Hayır** | **Yapmayacağız**: açık cinsel animasyon, sahne ve çıplaklık varlığı yok. |
| Rızasız içerik ve cinsel kölelik | CE (esaret içinde cinsel içerik) | **Hayır** | **Yapmayacağız.** Cinsel olmayan esir mekanikleri ayrı ele alınır: sorgu (sahnesiz), rehine, fidye. Kölelik ekonomisi (BK tarzı) kapsam dışı. |
| Reşit olmayanları içeren her şey | — | **Kesinlikle hayır** | Romantizm, evlilik, gebelik, baştan çıkarma ve sevgili sırlarının hepsinde iki tarafın da 18 yaş ve üstü olması kodda zorunlu olmalı. Not: Bellum Civile'deki çocuk rehineler yalnız siyasi. Rehine fikrini alırsak çocuk rehineler için romantik/cinsel her etkileşim kapalı kalmalı. |

---

## D. Harmony / UIExtenderEx / MCM gerektiren fikirler

Taç ve Hançer şu an bağımlılıksız. Aşağıdakiler ya bağımlılık ister ya da bağımlılıksız bir yedek yola sahip.

| Fikir | Neden gerekiyor | Gereken | Bağımlılıksız alternatif |
|---|---|---|---|
| Ansiklopedi sayfalarına düğme (Character Reload, DCC, Courier tarzı "haberci gönder") | Yerleşik ViewModel ve prefab'a ekleme | UIX (ya da VM'e Harmony yaması) | Insert menüsünden kahraman seçici (zaten var) |
| Klan veya krallık ekranına "Hanedan / Konsey" sekmesi (Dynasty and Intrigue, Bellum Civile Privy Council) | Yerleşik Gauntlet ekranını genişletmek | UIX | Kendi popup'ımız ya da kendi Gauntlet katmanımız ([technical_feasibility.md §9](Bannerlord%20eksiklikleri%20ve%20mod%20f%C4%B1rsatlar%C4%B1/technical_feasibility.md)) |
| Parti ekranında toplu yükselt ve sırala | Parti ekranı VM'i | UIX veya H | Popup'tan "hepsini yükselt" eylemi |
| Gerçek çok eşlilik | `Hero.Spouse` tek eş; evlilik ve miras kodu | H (yoğun) | Yapılmayacak (bkz. C) |
| Esirin kaçışını engelleme (rehine, Prevent escape) | Esaret kampanya davranışı içinde | H (muhtemelen) | Rehineyi esir yerine "misafir" bayrağıyla kendi durumumuzda tutmak |
| Krallık ardıllık ve seçim kuralını değiştirme (primogenitür) | Seçim ve karar mantığı | H | Yalnız klan varisi: ölüm olayında lideri bizim seçtiğimiz varise geçirmek (olay sırası doğrulanmadı) |
| Pazarlık ve ikna her zaman başarılı | Pazarlık ve ikna modelleri içinde | H veya model değiştirme | Model zinciri denenebilir (doğrulanmadı) |
| Vanilla gebelik ve doğurganlığı kalıcı değiştirme (Family Control, Dynasty Core) | `PregnancyModel` | Model zinciri (resmî), bazı ayrıntılar için H | Editörden tek seferlik gebelik başlatma (Action API) |
| Ayar ekranı (MCM) | Kalıcı ayarlar | MCM (→ H/BL/UIX) | Mevcut `SettingsStore` + popup. MCM 5.13'ün SubModule.xml ayarları doğrulanırsa "MCM varsa ekran, yoksa popup". |
| Diyalog satırı ekleme ("Seninle özel bir konuyu konuşmalıyız…") | — | Gerekmez (`CampaignGameStarter.AddPlayerLine`) | — |
| Şehir menüsüne "Saray günü" veya "Gölgelerde iş çevir" | — | Gerekmez (`AddGameMenuOption`) | — |
| Konsey makamlarının vergi, asker ve iyileşme etkileri | Oyun modelleri | Gerekmez (`AddModel`, zincirleme; `PartySpeedModel` deneyimi var). Diğer modların modelleriyle çakışabilir. | — |

---

## Kaynaklar

**Bu oturumda doğrudan okunan (GitHub)**
- Bellum Civile README: https://github.com/OhMyMike57/BellumCivile
- Banner Kings Redux README ve wiki: https://github.com/GIO443/bannerlord-banner-kings-redux/blob/main/README.md , https://github.com/GIO443/bannerlord-banner-kings-redux/wiki
- Diplomacy & Intrigue README: https://github.com/namphan19/Bannerlord-Diplomacy-Intrigue
- Dramalord deposu ve SubModule.xml: https://github.com/steffenkopany/Dramalord , https://raw.githubusercontent.com/steffenkopany/Dramalord/master/_Module/SubModule.xml
- Concubines fork: https://github.com/Shadow32055/Bannerlord.Concubines
- MCM sürümleri ve 5.13.0: https://github.com/Aragas/Bannerlord.MBOptionScreen/releases , https://github.com/Aragas/Bannerlord.MBOptionScreen/releases/tag/v5.13.0 , https://github.com/Aragas/Bannerlord.MBOptionScreen
- UIExtenderEx sürümleri: https://github.com/BUTR/Bannerlord.UIExtenderEx/releases
- ButterLib sürümleri: https://github.com/BUTR/Bannerlord.ButterLib/releases
- Harmony sürümleri: https://github.com/BUTR/Bannerlord.Harmony/releases

**Arama özetiyle görülen Nexus sayfaları**
- Entrika ve siyaset:
  - https://www.nexusmods.com/mountandblade2bannerlord/mods/2969 (Fourberie)
  - https://www.nexusmods.com/mountandblade2bannerlord/mods/2693 (Intrigues)
  - https://www.nexusmods.com/mountandblade2bannerlord/mods/13595 (Diplomacy & Intrigue)
  - https://www.nexusmods.com/mountandblade2bannerlord/mods/10764 (Dynasty and Intrigue)
  - https://www.nexusmods.com/mountandblade2bannerlord/mods/12035 (Magnum Opus)
  - https://www.nexusmods.com/mountandblade2bannerlord/mods/13940 (BannerCourt)
  - https://www.nexusmods.com/mountandblade2bannerlord/mods/5522 (Hold Court)
  - https://www.nexusmods.com/mountandblade2bannerlord/mods/13882 (Regard and Standing)
  - https://www.nexusmods.com/mountandblade2bannerlord/mods/13562 (Bridges to Remember)
  - https://www.nexusmods.com/mountandblade2bannerlord/mods/12828 (Crown and Kin)
  - https://www.nexusmods.com/mountandblade2bannerlord/mods/10609 (Bellum Civile)
  - https://www.nexusmods.com/mountandblade2bannerlord/mods/10922?tab=files (BK Redux)
  - https://www.nexusmods.com/mountandblade2bannerlord/mods/7886 , https://www.nexusmods.com/mountandblade2bannerlord/mods/13393 , https://www.nexusmods.com/mountandblade2bannerlord/mods/11496 (BK Redux yamaları)
  - https://www.nexusmods.com/mountandblade2bannerlord/mods/13617 (Diplomacy Overhaul)
- Aile:
  - https://www.nexusmods.com/mountandblade2bannerlord/mods/6682 (Dramalord)
  - https://www.nexusmods.com/mountandblade2bannerlord/mods/13050 (Houses And Heirs)
  - https://www.nexusmods.com/mountandblade2bannerlord/mods/13193 (Dynasty Core)
  - https://www.nexusmods.com/mountandblade2bannerlord/mods/13938 (Noble Household)
  - https://www.nexusmods.com/mountandblade2bannerlord/mods/13309 (RoT – Dynasty and Succession)
  - https://www.nexusmods.com/mountandblade2bannerlord/mods/9032 (Romance)
  - https://www.nexusmods.com/mountandblade2bannerlord/mods/11949 (Marry Anyone Continued)
  - https://www.nexusmods.com/mountandblade2bannerlord/mods/12496 (Female Lord Matrilineal Marriage)
  - https://www.nexusmods.com/mountandblade2bannerlord/mods/9605 (Brut.FamilyTree)
  - https://www.nexusmods.com/mountandblade2bannerlord/mods/5455 (Concubines)
  - https://www.nexusmods.com/mountandblade2bannerlord/mods/10910 (Illegitimate Children for AI Influence)
- Ekonomi:
  - https://www.nexusmods.com/mountandblade2bannerlord/mods/10796 (Living Economy)
  - https://www.nexusmods.com/mountandblade2bannerlord/mods/9671 (HAP)
  - https://www.nexusmods.com/mountandblade2bannerlord/mods/9558 (Economy Overhaul)
  - https://www.nexusmods.com/mountandblade2bannerlord/mods/8698 (Improved Economy For AI Lords)
- Yoldaşlar:
  - https://www.nexusmods.com/mountandblade2bannerlord/mods/13846 (Delegated Missions)
  - https://www.nexusmods.com/mountandblade2bannerlord/mods/12912 (Companion Orders)
  - https://www.nexusmods.com/mountandblade2bannerlord/mods/3506 (Send Companions Recruiting)
  - https://www.nexusmods.com/mountandblade2bannerlord/mods/12106 (Realistic Courier)
  - https://www.nexusmods.com/mountandblade2bannerlord/mods/12933 (Companion Dynasties)
  - https://www.nexusmods.com/mountandblade2bannerlord/mods/3150 (Make Companions Vassals)
  - https://www.nexusmods.com/mountandblade2bannerlord/mods/8358 (They Married)
  - https://www.nexusmods.com/mountandblade2bannerlord/mods/3074 (Deserving Companions)
- Olaylar:
  - https://www.nexusmods.com/mountandblade2bannerlord/mods/9137 (Banner Flavor Events)
  - https://www.nexusmods.com/mountandblade2bannerlord/mods/1410 (Random Events)
  - https://www.nexusmods.com/mountandblade2bannerlord/mods/9907 (Random Event Expansion)
  - https://www.nexusmods.com/mountandblade2bannerlord/mods/12730 (World Events Announcer)
  - https://www.nexusmods.com/mountandblade2bannerlord/mods/1226 , https://www.nexusmods.com/mountandblade2bannerlord/mods/12254 (Captivity Events)
- Deniz:
  - https://www.nexusmods.com/mountandblade2bannerlord/mods/12697 (War Sails Tweaks)
  - https://www.nexusmods.com/mountandblade2bannerlord/mods/10304 (Jomsborg Pirates)
  - https://www.nexusmods.com/mountandblade2bannerlord/mods/9768 (Station Troops With Fleet)
  - https://www.nexusmods.com/mountandblade2bannerlord/mods/9657 (Simple Naval Improvements)
  - https://www.nexusmods.com/mountandblade2bannerlord/mods/11455 (Naval Physics Crash Patch)
  - https://www.nexusmods.com/mountandblade2bannerlord/mods/13642 , https://www.nexusmods.com/mountandblade2bannerlord/mods/10696 (gemi varyantları, yelken sancağı)
- Liste sayfaları:
  - https://www.nexusmods.com/mountandblade2bannerlord/mods/top
  - https://www.nexusmods.com/mountandblade2bannerlord/mods/motm
  - https://www.nexusmods.com/mountandblade2bannerlord/mods/newrecently
  - https://www.nexusmods.com/games/mountandblade2bannerlord/collections/0oirte

**Diğer**
- GamePressure:
  - https://www.gamepressure.com/download.asp?ID=77322 (Intrigues)
  - https://www.gamepressure.com/download/mount-blade-ii-bannerlord-intrigues-v108-mod/z912e0a (Intrigues)
  - https://www.gamepressure.com/editorials/the-best-mods-to-mount-and-blade-ii-bannerlord/improved-garrisons/zc282 (CE, Hot Butter)
- Steam:
  - https://steamcommunity.com/sharedfiles/filedetails/changelog/2875710877 (Fourberie)
  - https://steamcommunity.com/sharedfiles/filedetails/changelog/3233297311?p=2 (Dramalord)
  - https://steamcommunity.com/workshop/filedetails/discussion/2880158010/7048779603517444007 (BK köle/malikâne perk'leri)
  - https://steamcommunity.com/app/261550/discussions/0/2144217924377078710 (çıplaklık, 2020)
- smods aynaları: https://catalogue.smods.ru/archives/271407 , https://catalogue.smods.ru/archives/402946 , https://catalogue.smods.ru/?p=182530 , https://catalogue.smods.ru/archives/411715 , https://catalogue.smods.ru/?p=266677
- Toplayıcı: https://discovermods.com/bannerlord/diplomacy/ , https://discovermods.com/bannerlord/captivity-events/
- Yetişkin mod aynaları ve sayfaları: https://aqxaromods.com/othergame/mount-amp-blade-2-bannerlord/18832-captivity-events-v1301.html , https://aqxaromods.com/othergame/mount-amp-blade-2-bannerlord/18831-hot-butter-v207.html , https://vgtimes.ru/games/mount-blade-2-bannerlord/files/52989-vozmozhnost-zanjatsja-seksom.html
- Tekil sayfalar:
  - https://www.gry-online.pl/download/mount-blade-ii-bannerlord-complex-characters-v105-mod/zf152cd (Complex Characters)
  - https://www.moddb.com/mods/lord-plunder (Lord Plunder)
  - https://www.gamewatcher.com/mount-and-blade-2-bannerlord-settlement-finances-mod (Settlement Finances)
  - https://www.nexusmods.com/pine/news/14658 (Life in Calradia röportajı)
  - https://forums.nexusmods.com/profile/41002870-dovah164/ (yoldaş onay sistemi önerisi)
  - https://forums.nexusmods.com/profile/1563234-c4w/ (çok eşlilik politikası yorumu)

**Önceki araştırmalar**
- [popular_gameplay_mods.md](Bannerlord%20eksiklikleri%20ve%20mod%20f%C4%B1rsatlar%C4%B1/popular_gameplay_mods.md)
- [cheat_editor_mods.md](Bannerlord%20eksiklikleri%20ve%20mod%20f%C4%B1rsatlar%C4%B1/cheat_editor_mods.md)
- [technical_feasibility.md](Bannerlord%20eksiklikleri%20ve%20mod%20f%C4%B1rsatlar%C4%B1/technical_feasibility.md)
