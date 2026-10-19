# Taç ve Hançer — Yol Haritası (profesyonel sürüm)

**Tarih:** 2026-10-19 · **Durum:** Onaylandı; **P0, P1, P2.1 (esir sorgulama), P2.2 (haberci + saray istihbaratı), P2.3 (casus başı görevleri), P2.4 (gizli ilişki
ve gayrimeşru çocuk sırları) P2.5 (ağ ifşasından diplomatik kriz), gece evi işletmesi, esiri hizmete davet, P2.6 (kur, sevgili, gebelik), eşinin gizli hayatı / boşanma P2.7 (aile editörü) ve düello kodlandı** (oyunda test bekliyor; P1 uygulama notu bölüm 3'ün sonunda; tasarımlar `2026-10-19-p2-esir-sorgulama.md`,
`2026-10-19-p2-haberci-saray.md`, `2026-10-19-p2-casus-basi-gorevleri.md`, `2026-10-19-p2-gizli-iliski.md`,
`2026-10-19-p2-diplomatik-kriz.md`, `2026-10-19-p2-gece-evi.md`, `2026-10-19-p2-sevgili.md`, `2026-10-19-p2-esin-gizli-hayati.md`, `2026-10-19-p2-aile-editoru.md`, `2026-10-19-duello.md`; resimler `docs/gorseller/olay-kartlari.md`). Kullanıcı yönlendirmesi: "basite kaçma, profesyonel olsun. Harmony, MCM gibi ne gerekiyorsa kurarız.
Resim ve video eklenebilir. Ama özellikler mantıklı olsun, saçma özellik olmasın."
**Girdiler:**
- `research_notes/nexus_mod_fikirleri.md` (Nexus araştırması)
- `docs/superpowers/specs/2026-10-19-entrika-design.md` (entrika tasarımı)

---

## 1. Mantık filtresi: bir özellik ancak şu dört soruya "evet" ise girer

1. **Oyunda karşılığı var mı?** Sonucu Bannerlord'un kendi sistemlerinden birini değiştirmeli: ilişki, altın, etki, krallık, esir, ölüm, kişilik, yerleşim.
   "Hak iddiası" gibi oyunda karşılığı olmayan bir şey, ona gerçek bir etki tasarlanana kadar girmez.
2. **Temaya uyuyor mu?** Taç (hanedan, saray, siyaset) ve Hançer (entrika). Ayrı bir oyun olmamalı.
3. **Dengeyi koruyor mu?** Bedeli, riski ve sınırı olmalı. Bedava güç yalnızca Hileler menüsünde.
4. **Mevcut sistemi besliyor mu?** Sır, koz, rapor, aşama ve olay motorunu kullanmalı. Yanına kopuk yeni bir mekanik eklememeli.

**Rakip notu:** Fourberie (Nexus 2969) cinayet, kaçırma, kan davası ve sabotajda bize çok benziyor.
Bizi ayıranlar: sır ve koz ekonomisi, seçimli ve resimli olay kartları, karşı casusluk kararları, aile/saray bağlantısı.
İki mod birlikte kurulunca test edilmeli; aynı şeyi ikinci kez yapan özellik eklemeyelim.

---

## 2. Faz P0 — Profesyonel altyapı (bu akşamki testten sonra)

| İş | Neden |
|---|---|
| Proje dosyası SDK tarzına geçer. Oyunun referans DLL'leri ve kütüphaneler sürümü sabitlenmiş NuGet paketlerinden gelir (Bannerlord.ReferenceAssemblies 1.4.8, Lib.Harmony, Bannerlord.UIExtenderEx, Bannerlord.ButterLib, Bannerlord.MCM). | Derleme oyun klasörüne bağlı kalmaz, her makinede aynı sonucu verir. Profesyonel modlar böyle kurulur. |
| GitHub'a otomatik derleme ve test (her gönderimde). | Telefondan bile "derleniyor mu, testler geçiyor mu" görülür. Eve gelince bozuk bir sürümle uğraşılmaz. |
| `SubModule.xml` bağımlılıkları ve yükleme sırası: Harmony → ButterLib → UIExtenderEx → MCM → Taç ve Hançer. | Launcher eksik bağımlılığı kendisi söyler. |
| Ayarlar **MCM**'e taşınır: Kurallar ve entrika ayarları kaydırıcılı, açıklamalı tek bir ekranda (Esc → Mod Options). Eski `CheatMenu.txt` bir kez içe aktarılır. | Oyuncunun beklediği standart ayar ekranı. |
| Harmony yalnızca oyun davranışını gerçekten değiştirmemiz gerektiğinde kullanılır. Her yamanın kendi testi ve "yama tutmadı" uyarısı olur. | Oyun güncellemesi bir yamayı bozarsa sessizce değil, açıkça görünür. |

**Bağımlılık politikası:**
- Entrika mantığı (formüller, kayıt, kampanya davranışı) Harmony'ye dayanmaz.
- UIExtenderEx yalnızca arayüz katmanında kullanılır.
- Böylece bir oyun yaması çıkınca en kötü ihtimalle bir buton kaybolur; kayıtlar ve entrikalar çalışmaya devam eder.

---

## 3. Faz P1 — Entrika ekranı ve olay kartları (Gauntlet arayüzü)

Popup listeleri yerine CK3'teki gibi bir **Entrika ekranı**. Kendi Gauntlet katmanımızla yapılır; açılış yolları:
- bir kısayol tuşu
- Insert menüsü
- UIExtenderEx ile klan ekranına ve harita çubuğuna eklenen "Entrika" butonu

| Bölüm | İçerik |
|---|---|
| Casus başı kartı | Portre, Roguery, Hesapçı, eşzamanlı entrika sınırı, ağ sınırı, durum (çalışıyor / esir / meşgul) |
| Süren entrikalar | Her biri bir kart: hedefin portresi, aşama çubuğu, kalan gün, başarı ve yakalanma yüzdesi, yöntem; butonlar: harekete geç (rüşvet seçimi), iptal |
| Yeni entrika | Tür sekmeleri ve filtreli hedef listesi (krallık, klan, tanıştıklarım, raporlularım): portre + tahmini başarı + maliyet; sağda özet ve "Başlat" |
| Sırlar ve kozlar | Kişi başına sırlar (ifşa, şantaj), kozlar (kalan gün, kullanım butonları) |
| Gölge ağları | Şehir, seviye, bakım, bonus; uzaktan dağıtma |
| Sana karşı entrikalar | Sezilenler ve karşı hamle butonları |
| Kronik | Tüm entrika geçmişi (20 değil, sınırsız ve sayfalı) ve dosyaya aktarma |

**Olay kartları:** düz popup yerine resimli kart. Solda resim, sağda başlık, metin ve seçenekler.
Her seçeneğin yanında etkisi küçük simgelerle (başarı, yakalanma, altın) gösterilir. Resim listesi ve prompt'lar: `docs/gorseller/olay-kartlari.md`.

**Ansiklopedi:** bir lordun sayfasında (UIExtenderEx) bildiğin sırlar, üzerindeki koz ve son rapor tarihi. Sayfadan doğrudan "entrika başlat".

**Video:** şimdilik yok. Mantıklı bir yeri olmadığı sürece eklemiyoruz. Olay kartlarının resimleri hem daha uygun hem daha az yer kaplıyor.

### P1 uygulama notu (kodlandı)

| Karar | Neden |
|---|---|
| Ekran, olay kartı ve harita düğmesi **kendi Gauntlet katmanlarımız** (harita ekranına eklenir); oyunun prefab'larına yama yok | Oyun güncellemesi kendi arayüzünü değiştirse de bizimkiler bozulmaz. Harita düğmesi oyunun harita çubuğuna değil, üst kenara konur (yeri MCM'den) |
| Fırçalar yalnızca her sürümde bulunan düz beyaz kare sprite'ını kullanır; renkler bizden. Yazı tipi adı yazılmaz, oyunun varsayılanı atanır | Sürüm değişince eksik sprite/yazı tipi yüzünden ekran boş kalmasın; Türkçe karakterler oyunun eşlemesiyle gelir |
| Resimler çalışma anında `Module/Images` klasöründen yüklenir (PNG/JPG; sprite paketi yok) | Kullanıcı resmi klasöre atar, iş biter. Resim yoksa kartta kişinin portresi görünür |
| Olay kartı açılamazsa (ayar kapalı, özel parça kayıtlı değil, hata) aynı seçenekler oyunun seçim penceresiyle sorulur. Ekran açılamazsa liste menüsü açılır | Arayüz sorunu oyunu durdurmasın; karar her zaman sorulabilsin |
| Ekran açıkken oyun durur ve klavye haritaya gitmez; Esc / Kapat / kısayol kapatır | Popup'lardaki gibi güvenli kullanım |
| Kronik 20'den **300** olaya çıktı, dosyaya aktarılır (Belgeler\...\Configs\TacVeHancer) | "Sınırsız"a yakın ama kayıt dosyası şişmez |
| CI'da **arayüz denetimi** (`tools/GuiCheck`): parça/özellik adları oyunun referans DLL'lerine, `@bağlantı`/komutlar ViewModel'lere karşı | Oyunu açamadan yakalanabilecek arayüz hataları gönderimden önce yakalanır |
| **Ansiklopedi ve oyunun harita çubuğu** bağlantısı ertelendi | UIExtenderEx ile eklemek için oyunun `EncyclopediaHeroPage.xml` ve `MapBar.xml` dosyalarının yapısı gerekiyor (kullanıcıdan istenecek). O zamana kadar aynı bilgiler ekranın **Kişiler** sekmesinde: rapor, sırlar, koz, entrika başlat |

---

## 4. Faz P2 — Araştırmadan seçilenler (mantık filtresinden geçenler)

| Sıra | Özellik | Neden mantıklı | Oyundaki etkisi |
|---|---|---|---|
| 1 ✅ | **Esir sorgulama** (kodlandı) | Elindeki esir lordlar boşa duruyor; sır altyapısını doğrudan besler | Sorgu: rapor, sır ve efendisi hakkında bilgi. Baskı Merhamet ve ilişki bedeli; anlaşma esiri bırakır. Sahne yok, metin var. |
| 2 ✅ | **Haberci / mektup** (kodlandı; saray istihbaratı; ulakla uzaktan görüşme + hediye; lord kazanmak oyunun ikna sistemiyle, bilgi bonus verir) | Koz kullanmak için lordu haritada kovalamak gereksiz zahmet | Uzaktan koz harcama ve tehdit; gecikmeli cevap (birkaç gün) |
| 3 ✅ | **Casus başı görev modu** (kodlandı) | Karşı casusluğa oyuncu kararı katar | Üç mod: karşı casusluk (sezme ↑), muhalefeti izle (krallığındaki hoşnutsuzları raporlar), söylenti yay (seçili lordun hükümdarla ilişkisi yavaşça ↓) |
| 4 ✅ | **Gizli ilişki ve gayrimeşru çocuk sırları** (kodlandı; sahnesiz) | Şantajın en doğal yakıtı | Sevgili gerçek bir lord, sır iki tarafta; gayrimeşru çocuk; ifşa, şantaj, aldatılan eşe söyle. İki tarafın da 18+ olması kodda zorunlu ve testli. Meşrulaştırma 8. sıraya (aile ağacına dokunmamak için). |
| 5 ✅ | **Ağ ifşasından diplomatik kriz** (kodlandı) | Gölge ağına gerçek bir risk katmanı | Yabancı krallıkta ağ çökerse oyunun suç puanı, hükümdar hesap sorar (tazminat / inkâr / ret), gerilim birikir; 60'ı aşarsa o krallık oyunun kendi savaş kararını oylar |
| 5b ✅ | **Gece evi işletmesi** (kodlandı; kullanıcı isteği; varsayılan kapalı) | Gölge ağına bağlı bir bilgi kaynağı: müşteriler konuşur. Atölyelerin kopyası değil | Gölge ağı olan şehirde açılır; isimsiz, gönüllü ve ücretli çalışanlar (pay düşükse ayrılırlar); haftalık gelir, lordlar hakkında rapor ve sır; kendi şehrinde dedikodu, başka şehirde baskın |
| 6 ✅ | **Kur, sevgili, eşini bilgi için göndermek, gebelik** (kodlandı; sahnesiz) | CK3'ün temel entrikalarından; ilişki, sır ve koz üretir. Kullanıcı isteği: "sevgililik olsun, ondan çocuk yapabilelim"; "eşini bilgi almak için gönder, gerisi ikisinin kararı" | Üç aşamalı kur (hediye, şiir, gizli buluşma), oyunun çekicilik modeliyle. Sen kur yaparsan sevgilin olur: gizli buluşmalar, "tedbirli ol" seçeneği, gebelik. Eşin (kendi kabulüyle) bilgi için kur yapar: rapor, sır, ikisinin kararıyla yakınlık ve gebelik. Doğum oyunun kendi işlemiyle; resmî baba farklıysa sır olur. Yakalanırsa skandal. Yalnızca yetişkinler. |
| 6b ✅ | **Eşinin gizli hayatı ve boşanma** (kodlandı; kullanıcı isteği; ayar varsayılan açık) | CK3'te eş de aldatabilir; araştırmak ve boşamak oyunda yok | Eşin, araları bozuksa ya da uzun süre ayrı kalırsanız sevgili edinebilir, gebelik gizli kalır; casus başı araştırır ya da karşı casusluk bulur; yüzleş / görmezden gel / boşa; boşanan eş ailesinin klanına döner |
| 7 ✅ | **Aile editörü ekleri** (kodlandı) | Rakip editörlerde var, bizde eksik | Evlendir (oyunun uygunluk kontrolüyle) / boşa, gebelik başlat (oyunun gebeliği), "çocuk istemiyorlar", çocuğu büyüt, yaşlanmayı dondur (Editörler > Aile editörü) |
| 8 | **Varis seçimi ve meşrulaştırma** | Vanilla'da varis kontrolsüz; hanedan teması | Klan varisini seç; gayrimeşru çocuğu meşrulaştır (etki ve Onur bedeli). P2.4'ten devralındı. |
| 9 | **Saray günü** | Barış zamanı içeriği ve olay kartlarının doğal yeri | Şehir menüsünden dilekçeler ve yargılamalar; kararlar kişilik, sadakat, güvenlik ve altını etkiler |
| 10 | **Rehine anlaşmaları** | Aile, diplomasi ve kaçırmanın kesişimi | Barış ya da ittifak teminatı. Esir değil "misafir" olarak tutulur (Harmony gerekmez). |

**Oyun API taramasından adaylar** (`2026-10-19-oyun-api-taramasi.md`): düello (✅ kodlandı), oyunun 3B sahneleri (doğum, düğün), evlat edinme ve
varis (P2.8'e), casusluğu kendin yap (takip ve kılık değiştirme görevleri), zindan baskını, çete savaşı, manastıra çekilme, gizlilikle
suikast. Alevli ok (War Sails) kullanıcının DLC dosyaları gelince.

**Şimdilik almadıklarımız ve nedeni:**

| Fikir | Neden şimdi değil |
|---|---|
| Hak iddiası uydurma | Bannerlord'da "hak iddiası" diye bir sistem yok. Ona gerçek bir etki tasarlanırsa (ör. fief oylamasında ağırlık) yeniden düşünülür. |
| Yapay zekâlar arası entrika ve haber akışı | Dünyayı dengesizleştirebilir. Önce oyuncuya karşı entrika oyunda denensin; sonra nadir ve ayarlı bir sürümü düşünülür. |
| Borç defteri | Ayrı bir ekonomi sistemi; entrikaya katkısı dolaylı. |
| Ajan görevlendirme (yoldaşı N gün göreve yollamak) | Mevcut entrika sistemiyle aynı işi yapıyor. |
| War Sails kuralları | Mantıklı ama entrika dışı. Hileler ve Kurallar'a küçük bir paket olarak ayrıca eklenebilir. |

---

## 5. Yetişkin (18+) içerik politikası

| Yapılır (sahnesiz, yalnızca metin) | Yapılmaz |
|---|---|
| Baştan çıkarma entrikası, gizli sevgili ve gayrimeşru çocuk sırları, gebelik kontrolü (editör), meşrulaştırma; eşin kendi kabulüyle bilgi için kur yapması; gece evi işletmesi (isimsiz, gönüllü ve ücretli yetişkin çalışanlar, varsayılan kapalı) | Açık cinsel sahne, animasyon ya da çıplaklık; çok eşlilik ve harem; cariyelik; esirleri, eşleri, aileyi ya da herhangi bir kahramanı başkalarına "hizmet" ettirmek; çalışanları zorlamak ya da tutmak; rızasız her şey; reşit olmayanları içeren her şey |

- Romantizm, gebelik ve baştan çıkarma ile ilgili her işlemde iki tarafın da yetişkin olması kodda kontrol edilir; testlerde de denetlenir.
- Han/taverna işletmesi istenirse cinsel olmayan bir gelir ve muhbir kaynağı olarak tasarlanabilir.

---

## 6. Sıra

1. **Bu akşam:** E0–E2 testi (YAPILACAKLAR.md). Sonuçlara göre düzeltmeler ve denge ayarı.
2. **P0:** altyapı ve MCM ayarları.
3. **P1:** Entrika ekranı ve olay kartları (resimler kullanıcıdan).
4. **P2:** yukarıdaki sırayla, her biri ayrı tasarım notu ve testle.
5. **E3** (entrika tasarımındaki): sabotaj ve kışkırtma P2 sırasına göre yeniden değerlendirilir.
   Sabotaj Fourberie'de de var, kendi farkımızı bulmadan eklenmez.
