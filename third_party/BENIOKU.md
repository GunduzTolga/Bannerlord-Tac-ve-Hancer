# Paketlenen modlar

Taç ve Hançer başka modları kendi klasörüne paketler: launcher'da tek mod görünür, ayrıca kurulmaz.

- Her mod burada kendi klasöründe durur (`third_party\<mod>\`). Klasör, modun oyundaki `Modules\<mod>` klasörünün aynen kopyasıdır: `SubModule.xml` dahil.
- Hangi modların hangi sırayla yükleneceği `paketler.txt` dosyasında yazar.
- Derleme (`build.ps1`) önce Taç ve Hançer'i oyunun `Modules\CheatMenu` klasörüne kopyalar, sonra `tools\bundle.ps1` bu modları oraya ekler.
- **Repoda olanlar:** modların kodları (DLL), XML'leri, arayüz prefab'ları, dil ve ayar dosyaları. Telefondan çalışırken de görülsünler,
  kodları kendi modumuza taşınabilsin diye (mod sahiplerinin izniyle; mod yalnızca kişisel kullanım için).
- **Repoda olmayanlar:** büyük içerik (doku `.tpac/.dds`, resim, ses, video, 3B model, sahne, `.pdb`). `.gitignore` bunları dışarıda
  tutar; yalnızca PC'deki kopyada durur, `build.ps1` oradan alır. PC'deki kopya kaybolursa modu yeniden indirip klasörüne koymak yeter.
- Repo **gizli (private)** olmalı: başkalarının dosyaları açık bir repoda dağıtılmış olur.

## Mod eklemek ya da çıkarmak

1. Modun klasörünü buraya kopyala (çıkarıyorsan sil).
2. `paketler.txt`'ye adını yaz (çıkarıyorsan satırı sil). Taç ve Hançer her zaman en son yüklenir.
3. `build.ps1` çalıştır. Önceki derlemenin kopyaladığı ve artık paketlenmeyen dosyalar oyun klasöründen silinir.
4. Modun oyundaki ayrı klasörü varsa sil ve launcher listesinden kaldır. Yoksa iki kez yüklenir.

İki mod aynı dosyayı farklı içerikle getirirse derleme durur ve hangi dosya olduğunu yazar. Oyunun modül başına bir kez
okuduğu dosyalar çakışmaz, birleştirilir:
- dil listeleri (`ModuleData\Languages\...\language_data.xml`). Aynı klasörde farklı dil adı varsa (ör. SP: "Español" ve
  "Español (LA)") ikincisi alt klasöre (`SP\dil-1\`) yazılır; oyun alt klasörlerde de arar.
- stil dosyaları (`ModuleData\*.xslt`): şablonlar tek dosyada toplanır. Aynı yere iki farklı şablon gelirse derleme durur.
- `ModuleData\skins.xml` ve `ModuleData\project.mbproj`

Modun kendi klasörünü adıyla arayan kodu varsa (ör. `Modules\WomeninCalradia\config.xml`) paketlenince o dosyayı bulamaz.
Bunun yaması `src\Bundle\BundleShim.cs`'dedir; modun kök klasöründeki dosyalar `Modules\CheatMenu\Bundled\<mod>\` altına konur.

## Şu an paketlenenler

| Mod | Klasör | Not |
|---|---|---|
| Diplomacy | `Bannerlord.Diplomacy` | BUTR yükleyicisi oyunun sürümüne uygun DLL'i kendisi seçer. **Türkçesi bizde tamamlandı** (21 Ekim): `ModuleData\Languages\TR`'de eksik 118 metin ve 5 mod yükleme uyarısı eklendi ("Taç ve Hançer: eksik çeviriler" yorumunun altında); "tımar" her yerde "toprak" yapıldı (kullanıcı kararı); isyan kurma metnine nüfuz bedeli geri kondu. Mod güncellenince yeni metin kimliklerini kökteki İngilizce dosyalarla ve DLL'deki `{=...}` metinleriyle karşılaştır, eksikleri aynı yere ekle. |
| DLX Hair Layers | `HairAddons` | Dosyalarını DLL'in yanından bulur. **Türkçesi bizde** (21 Ekim): `ModuleData\Languages\TR\hra-tur.xml` (80 metin; kimlikler RU dosyasıyla aynı). |
| Women in Calradia | `WomeninCalradia` | Ayar dosyası `Bundled\WomeninCalradia\config.xml`. |
| AutoTrader | `AutoTrader` | Ayarları ve günlüğü Belgeler\...\Configs'te; oyun 1.4.6 için derlenmiş, 1.4.8'de bütün çağrıları karşılık buluyor. Ayar penceresi (Alt+A) bizim Entrika ekranının Saray > Ticaret sekmesine yönlenir (`src\Bundle\BundleShim.cs`, `AutoTraderCompat.cs`; tasarım `2026-10-21-ticaret-sekmesi.md`). **Türkçesi bizde** (21 Ekim): `ModuleData\Languages\TR\std_AutoTrader-tr.xml` (56 metin); mod güncellenince kökteki `module_strings.xml` ile karşılaştır. |
| Regard and Standing | `RegardStanding` | Ayarları Belgeler\RegardStanding\regard_standing.txt (ve Mod Seçenekleri). Lordlar arası gizli ilişkilerin sahibi o; casus başımız onun gerçek ilişkilerini bulur (`src\Bundle\RegardStandingCompat.cs`, tasarım: `2026-10-20-regardstanding-iliskiler.md`). Sürüm 1.5.0, **DLL yamalı** (`tools\RsPatch`): Calradia Remembers kurulu değilken oyun, o modun DLL'lerine verilen referansları çözemiyor ve her açılışta "dependency conflict" penceresi gösteriyordu; araç Calradia Remembers bağlantısını tamamen söker (o modun türlerini kullanan üyeler silinir ya da boşaltılır, DLL referansları kaldırılır). Mod o kısmı zaten yalnızca Calradia Remembers varken çalıştırıyordu. Mod güncellenince yeni DLL'e bir kez `dotnet run --project tools\RsPatch -- <RegardStanding.dll>` çalıştır. |

| Extra Complexion | `ScarsExtra` | Yüze yara, makyaj, ten ayrıntısı, dövme ve savaş boyası katmanları (yalnızca yetişkin erkek/kadın tenlerine). Dokularını DLL'in yanından (`Assets`, `Assets\Custom`) bulur. `ModuleData\skins.xslt`'si Women in Calradia'nınkiyle tek stil dosyasında birleşir (farklı yerlere dokunuyorlar: yüz dokuları / dövme katmanları). **Türkçesi bizde** (21 Ekim): `ModuleData\Languages\TR\scx-tur.xml` (27 metin). |
| Companion Autonomy | `CompanionAutonomy` | Parti yöneten yoldaşlara bütçe: harçlık, teçhizat, ordu düzenine göre asker, terfi, at; ayarlar Klan > Birlikler'de. Klan ekranındaki eklentisi parti satırında, bizim Entrika sekmemizle çakışmıyor. |

Pakete girmeyenler:
- **Hot Butter:** genelevin yetişkin içerik taşımayan mekanikleri onun fikirlerinden kendi kodumuzla yazıldı.
- **Dramalord** ve **Family Control:** yetişkin içerikli modlar. İlişki, gebelik, yüzleşme ve boşanma gibi yetişkin içerik taşımayan
  kısımları bizde zaten var (gizli ilişki, sevgili, eşin gizli hayatı, aile editörü).
