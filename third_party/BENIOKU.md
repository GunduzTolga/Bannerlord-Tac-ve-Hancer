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
| Diplomacy | `Bannerlord.Diplomacy` | BUTR yükleyicisi oyunun sürümüne uygun DLL'i kendisi seçer. |
| DLX Hair Layers | `HairAddons` | Dosyalarını DLL'in yanından bulur. |
| Women in Calradia | `WomeninCalradia` | Ayar dosyası `Bundled\WomeninCalradia\config.xml`. |
| AutoTrader | `AutoTrader` | Ayarları ve günlüğü Belgeler\...\Configs'te; oyun 1.4.6 için derlenmiş, 1.4.8'de bütün çağrıları karşılık buluyor. |
| Regard and Standing | `RegardStanding` | Ayarları Belgeler\RegardStanding\regard_standing.txt (ve Mod Seçenekleri). Lordlar arası gizli ilişkilerin sahibi o; casus başımız onun gerçek ilişkilerini bulur (`src\Bundle\RegardStandingCompat.cs`, tasarım: `2026-10-20-regardstanding-iliskiler.md`). |

| Extra Complexion | `ScarsExtra` | Yüze yara, makyaj, ten ayrıntısı, dövme ve savaş boyası katmanları (yalnızca yetişkin erkek/kadın tenlerine). Dokularını DLL'in yanından (`Assets`, `Assets\Custom`) bulur. `ModuleData\skins.xslt`'si Women in Calradia'nınkiyle tek stil dosyasında birleşir (farklı yerlere dokunuyorlar: yüz dokuları / dövme katmanları). |
| Companion Autonomy | `CompanionAutonomy` | Parti yöneten yoldaşlara bütçe: harçlık, teçhizat, ordu düzenine göre asker, terfi, at; ayarlar Klan > Birlikler'de. Klan ekranındaki eklentisi parti satırında, bizim Entrika sekmemizle çakışmıyor. |

Pakete girmeyenler:
- **Hot Butter:** genelevin yetişkin içerik taşımayan mekanikleri onun fikirlerinden kendi kodumuzla yazıldı.
- **Dramalord** ve **Family Control:** yetişkin içerikli modlar. İlişki, gebelik, yüzleşme ve boşanma gibi yetişkin içerik taşımayan
  kısımları bizde zaten var (gizli ilişki, sevgili, eşin gizli hayatı, aile editörü).
