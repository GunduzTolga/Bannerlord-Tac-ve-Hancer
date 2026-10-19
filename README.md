# Taç ve Hançer — Mount & Blade II: Bannerlord modu

Tek oyunculu kampanya için "her şeyi içeren" mod:
hileler, editörler, kurallar ve CK3 tarzı **entrika sistemi**
(tasarım: `docs/superpowers/specs/2026-10-19-entrika-design.md`).

Eski adı CheatMenu. İç kimlik (`CheatMenu`) değişmedi: oyun klasörü, ayar dosyası ve kayıtlar aynen çalışır.

- **Oyun sürümü:** Bannerlord v1.4.8 (War Sails v1.2.8 ile uyumlu)
- **Kapsam:** yalnızca tek oyunculu Campaign / Sandbox. Multiplayer ve Custom Battle'da hiçbir şey yapmaz.

## Kurulum

Önce şu dört modu kur (Nexus Mods ya da Steam Workshop; ikisi de ücretsiz, popüler modların çoğu bunları kullanır):

| Mod | Nexus | Gereken en düşük sürüm |
|---|---|---|
| Harmony | Bannerlord.Harmony | 2.2.2 |
| ButterLib | Bannerlord.ButterLib | 2.11.1 (1.4.8 için 2.12.0 önerilir) |
| UIExtenderEx | Bannerlord.UIExtenderEx | 2.13.3 (3.0.3 önerilir) |
| Mod Configuration Menu v5 (MCM) | Bannerlord.MBOptionScreen | 5.13.1 |

Launcher → Mods sırası: **Harmony → ButterLib → UIExtenderEx → MCM → (oyunun kendi modülleri) → Taç ve Hançer**.
Launcher eksik ya da yanlış sırada olanı uyarı olarak gösterir. Bir bağımlılık başlatılamazsa mod yine açılır ama ana menüde kırmızı bir uyarı çıkar.

Ayarlar: **Esc → Mod Seçenekleri → Taç ve Hançer** (MCM). Aynı ayarlar Insert → Kurallar'da da var; ikisi birbirini günceller.

## Kullanım

- Kampanyada **Insert** → ana menü: Entrika, Hileler, Editörler, Klan & krallık, Bilgi, Kurallar.
- Haritada **Ctrl+Alt + Sol tık** → tıklanan yere ışınlanma.

| Kategori | Neler var |
|---|---|
| **Entrika** | Casus başı; casusluk (rapor + gerçek kişilikten sırlar); şantaj ve kozlar (altın, etki, esir, klan koparma; lord diyaloğundan da); kaçırma; suikast (aşamalar, 10 olay, rüşvet, kan davası); şehirlerde gölge ağı; yapay zekânın sana kurduğu entrikalar ve karşı casusluk |
| **Hileler →** | |
| Para & Ekonomi | Altın, etki, tahıl |
| Karakter | Odak/attribute puanı, yetenekler, can |
| Ordu / Parti | Asker ekleme (oyundaki tüm askerler, kademeye göre, resimli), terfi XP'si, yaralı iyileştirme, moral, parti hızı x2 |
| Savaş içi | Ölümsüzlük, tek vuruş, sınırsız cephane ([Ben] / [Ordu], isteğe bağlı müttefikler dahil) |
| Harita | Yerleşime / bir lordun yanına ışınlanma, ileri sarma hızı 4x/8x/16x |
| Entrika hileleri | Aşamaları tamamla, asla fark edilme, her zaman başarılı |
| **Editörler →** | |
| Adlandır | Kahraman, eşraf, klan, krallık, yerleşim, parti |
| Kahraman editörü | Yaş, yetenek, nitelik, odak, perk, kişilik, görünüm (berber ekranı) |
| İlişkiler | Herhangi bir lord/eşrafla ilişki |
| Yerleşim editörü | Sahiplik, refah, sadakat, güvenlik, yiyecek, milis, hane, tüm projeleri bitir |
| Eşya ekle | İkonlu, aramalı eşya listesi |
| Klan & krallık | Katıl/ayrıl, krallık kur, hükümdar ol, kademe, savaş/barış, klan taşı, lider/hükümdar değiştir, krallık yok et |
| Bilgi | Lord bulucu (gerçek konum + kamera), harita sisini kaldır |
| Kurallar | Yetenek XP ve asker XP çarpanı, parti boyutu bonusu, öğrenme sınırını kaldır, yoldaş sınırını kaldır, parti sayısı sınırını artır / kaldır, entrika zorluğu |

## Entrika — kısa kullanım

1. **Entrika → Casus başı ata:** klanından Roguery'si yüksek bir yoldaş ya da aile üyesi. Entrikaların gücü ondan gelir.
2. **Yeni entrika → Casusluk:** hedefin konumu, gücü, kişiliği ve belki bir sırrı.
   Rapor 60 gün geçerli; o hedefe suikastta +10 verir. Tanışmadığın birini de suikast listesine ekler.
3. **Yeni entrika → Suikast:** en az 2 aşamadan sonra **Süren entrikalar → Harekete geç**.
   Bekledikçe şansın artar ama her aşamada yakalanma zarı atılır. Harekete geçerken rüşvetle +5/+10/+15.
4. **Şantaj:** casuslukla bulduğun bir sırrı koza çevir; kozu Entrika > Kozlar'dan ya da lordla konuşurken "özel bir konu" açarak kullan.
5. **Kaçırma:** suikast gibi işler, başarılıysa hedef partinde esir olur.
6. **Gölge ağı:** çete lideri olan bir şehirde kasaba menüsü → "Gölgelerde iş çevir". O şehirdeki ve krallıktaki entrikalarına bonus verir.
7. **Sana karşı entrikalar:** senden nefret eden lordlar ve kan davalılar ailene entrika kurabilir; casus başın sezerse karşı hamle seçersin
   (Kurallar'dan kapatılabilir; varsayılan "Az").
8. Olaylar, kararlar ve sonuçlar haritada popup olarak gelir; savaştayken olanlar haritaya dönünce gösterilir.
9. Yakalanırsan ilişki, suç puanı ve Onur kaybedersin. Bir hükümdarı öldürürsen savaş çıkabilir; kendi krallığından birini öldürürsen hükümdarın hesap sorar.

Formüller, örnek hesaplar ve sonraki fazlar: `docs/superpowers/specs/2026-10-19-entrika-design.md`.

Kalıcı ayarlar (ileri sarma hızı, kurallar): `Belgeler\Mount and Blade II Bannerlord\Configs\CheatMenu.txt`.

## Başarımlar

Bannerlord, resmi olmayan bir modül yüklü olan kampanyada başarımları kapatır ve bunu kayda kalıcı olarak yazar.
Bu mod o kontrolü **atlatmaz**. Başarımlar senin için önemliyse hileyi ayrı bir kampanyada kullan.
Başarımları hâlâ açık olan bir kayıt yüklenirse mod bir uyarı gösterir.

## Kayıt güvenliği

Mod kayda kendi veri türünü yazmaz; tüm değişiklikler oyunun kendi kaydettiği alanlara yapılır.
Mod kaldırılsa bile kayıtlar açılır. Oyunun konsol hilelerini çağırmaz (bunlar kayda "hile kullanıldı" işareti koyar).

## Derleme

Gerekenler: **.NET SDK 8 ya da üstü** (ücretsiz: `winget install Microsoft.DotNet.SDK.8` ya da Visual Studio Installer →
".NET masaüstü geliştirme"). İlk derlemede internet gerekir: oyunun referans DLL'leri ve kütüphaneler, sürümü sabitlenmiş
NuGet paketleri olarak bir kez iner. Derleme için oyunun kurulu olması gerekmez; kuruluysa mod oraya kopyalanır.

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\build.ps1
```

Betik önce testleri (`tests/CheatMenu.Tests`) derleyip çalıştırır, sonra modu derler ve
`<oyun klasörü>\Modules\CheatMenu` içine kopyalar. Oyun klasörü farklıysa:
`.\build.ps1 -p:GameFolder="D:\Oyunlar\Mount & Blade II Bannerlord"`. Derlerken oyun kapalı olmalı (DLL kilitlenir).

GitHub her gönderimde modu oyunsuz derler ve testleri çalıştırır (Actions sekmesi, "Derleme ve testler").

Ardından Launcher → Mods → **Taç ve Hançer**'i işaretle.

## Proje yapısı

- `src/` — mod kodu (`Cheats/` eylemler ve oyun modelleri, `Intrigue/` entrika sistemi, `Menu/` popup menüleri,
  `Mcm/` MCM ayar ekranı, `Patches/` Harmony yamaları)
- `tests/` — oyunsuz çalışan birim testleri (sayı/ad doğrulama, ayar dosyası, entrika kayıt biçimi)
- `Module/SubModule.xml` — mod manifesti
- `docs/superpowers/` — her fazın tasarım (specs) ve uygulama (plans) belgeleri; yol haritası: `specs/2026-10-19-yol-haritasi.md`
- `docs/gorseller/` — olay kartı görsellerinin listesi ve prompt'ları
- `.github/workflows/` — otomatik derleme ve test
- `reports/`, `research_notes/` — Bannerlord eksiklikleri ve mod fırsatları araştırması
