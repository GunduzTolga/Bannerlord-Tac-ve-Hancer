# Taç ve Hançer — Mount & Blade II: Bannerlord modu

Tek oyunculu kampanya için, oyunun kendi popup pencerelerini kullanan, bağımlılıksız (Harmony/MCM yok) "her şeyi içeren" mod:
hileler, editörler, kurallar ve yapım aşamasındaki CK3 tarzı **entrika sistemi**
(tasarım: `docs/superpowers/specs/2026-10-19-entrika-design.md`).

Eski adı CheatMenu. İç kimlik (`CheatMenu`) değişmedi: oyun klasörü, ayar dosyası ve kayıtlar aynen çalışır.

- **Oyun sürümü:** Bannerlord v1.4.8 (War Sails v1.2.8 ile uyumlu)
- **Kapsam:** yalnızca tek oyunculu Campaign / Sandbox. Multiplayer ve Custom Battle'da hiçbir şey yapmaz.

## Kullanım

- Kampanyada **Insert** → ana menü: Entrika, Hileler, Editörler, Klan & krallık, Bilgi, Kurallar.
- Haritada **Ctrl+Alt + Sol tık** → tıklanan yere ışınlanma.

| Kategori | Neler var |
|---|---|
| **Entrika** | Yapım aşamasında (E0: kayıt testi). Casus başı, casusluk, sırlar, suikast E1'de gelecek |
| **Hileler →** | |
| Para & Ekonomi | Altın, etki, tahıl |
| Karakter | Odak/attribute puanı, yetenekler, can |
| Ordu / Parti | Asker ekleme (oyundaki tüm askerler, kademeye göre, resimli), terfi XP'si, yaralı iyileştirme, moral, parti hızı x2 |
| Savaş içi | Ölümsüzlük, tek vuruş, sınırsız cephane ([Ben] / [Ordu], isteğe bağlı müttefikler dahil) |
| Harita | Yerleşime / bir lordun yanına ışınlanma, ileri sarma hızı 4x/8x/16x |
| **Editörler →** | |
| Adlandır | Kahraman, eşraf, klan, krallık, yerleşim, parti |
| Kahraman editörü | Yaş, yetenek, nitelik, odak, perk, kişilik, görünüm (berber ekranı) |
| İlişkiler | Herhangi bir lord/eşrafla ilişki |
| Yerleşim editörü | Sahiplik, refah, sadakat, güvenlik, yiyecek, milis, hane, tüm projeleri bitir |
| Eşya ekle | İkonlu, aramalı eşya listesi |
| Klan & krallık | Katıl/ayrıl, krallık kur, hükümdar ol, kademe, savaş/barış, klan taşı, lider/hükümdar değiştir, krallık yok et |
| Bilgi | Lord bulucu (gerçek konum + kamera), harita sisini kaldır |
| Kurallar | Yetenek XP ve asker XP çarpanı, parti boyutu bonusu, öğrenme sınırını kaldır, yoldaş sınırını kaldır, parti sayısı sınırını artır / kaldır |

Kalıcı ayarlar (ileri sarma hızı, kurallar): `Belgeler\Mount and Blade II Bannerlord\Configs\CheatMenu.txt`.

## Başarımlar

Bannerlord, resmi olmayan bir modül yüklü olan kampanyada başarımları kapatır ve bunu kayda kalıcı olarak yazar.
Bu mod o kontrolü **atlatmaz**. Başarımlar senin için önemliyse hileyi ayrı bir kampanyada kullan.
Başarımları hâlâ açık olan bir kayıt yüklenirse mod bir uyarı gösterir.

## Kayıt güvenliği

Mod kayda kendi veri türünü yazmaz; tüm değişiklikler oyunun kendi kaydettiği alanlara yapılır.
Mod kaldırılsa bile kayıtlar açılır. Oyunun konsol hilelerini çağırmaz (bunlar kayda "hile kullanıldı" işareti koyar).

## Derleme

Gerekenler: Visual Studio 2026 (MSBuild) ve .NET Framework 4.8.1 hedefleme paketi. Ayrıca .NET SDK gerekmez.

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\build.ps1
```

Betik önce testleri (`tests/CheatMenu.Tests`) derleyip çalıştırır, sonra modu derler ve
`<oyun klasörü>\Modules\CheatMenu` içine kopyalar. Oyun klasörü farklıysa `-p:GameFolder=...` ile
`src/CheatMenu.csproj` derlenebilir. Derlerken oyun kapalı olmalı (DLL kilitlenir).

Ardından Launcher → Mods → **Taç ve Hançer**'i işaretle.

## Proje yapısı

- `src/` — mod kodu (`Cheats/` eylemler ve oyun modelleri, `Intrigue/` entrika sistemi, `Menu/` popup menüleri)
- `tests/` — oyunsuz çalışan birim testleri (sayı/ad doğrulama, ayar dosyası, entrika kayıt biçimi)
- `Module/SubModule.xml` — mod manifesti
- `docs/superpowers/` — her fazın tasarım (specs) ve uygulama (plans) belgeleri
- `reports/`, `research_notes/` — Bannerlord eksiklikleri ve mod fırsatları araştırması
