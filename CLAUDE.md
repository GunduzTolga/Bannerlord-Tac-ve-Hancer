# Taç ve Hançer — proje rehberi (Claude için)

Bannerlord 1.4.8 tek oyunculu modu (Id `CheatMenu`, görünen ad "Taç ve Hançer"): hileler + CK3 tarzı entrika + Divan +
içine paketlenmiş başka modlar. Yalnızca kullanıcının kişisel kullanımı için; paketlenen modların sahipleri izin verdi.

Kullanıcıyla Türkçe, samimi ("knka") konuş. Kullanıcı yorulmak istemiyor: bir şeyi kendin yapabiliyorsan (derleme, kopyalama,
denetim, git) **ona yaptırma, kendin yap**. Yalnızca oyunda bakılması gerekeni iste.

## Oturuma başlarken
- Devir notu: `docs/DEVIR.md` (son durum, açık sorunlar, sıradaki işler). Önce onu oku.
- Test listesi ve kararlar: `YAPILACAKLAR.md`. Paketlenen modlar: `third_party/BENIOKU.md`, `third_party/paketler.txt`.
- Tasarımlar: `docs/superpowers/specs/`.

## Komutlar (Windows, repo kökünde)
- Hepsi bir arada (test + derleme + oyuna kopyalama + paketleme + arayüz ve DLL denetimi): `powershell -ExecutionPolicy Bypass -File .\build.ps1`
  Oyun klasörü farklıysa: `.\build.ps1 -p:GameFolder="D:\...\Mount & Blade II Bannerlord"`. Derlerken oyun kapalı olmalı.
- Yalnızca testler: `dotnet run --project tests/CheatMenu.Tests/CheatMenu.Tests.csproj -c Release -f net8.0`
- Oyun kayıtları: `C:\ProgramData\Mount and Blade II Bannerlord\logs\rgl_log_*.txt`;
  bizim dosyalar: `Belgeler\Mount and Blade II Bannerlord\Configs\TacVeHancer\` (çökme kaydı `cokme_*.txt`, `uyum_raporu.txt`).
- Oyunun kendi arayüz dosyaları yerelde var: `<oyun>\Modules\Native|SandBox|SandBoxCore\GUI\Prefabs` ve `GUI\Brushes`. Arayüzü
  "birebir oyun gibi" yaparken önce bunlara bak (bulutta yoktu, tahmin etmek zorunda kalınmıştı).

## Mimari
- `src/Intrigue/Core/*Rules.cs`: saf kurallar (oyunsuz, `tests/` test eder). `src/Intrigue/*.cs`: oyun katmanı.
- Kayıt: `IntrigueState` metin kayıtları. Oyun modelleri `AddModel<T>` ile zincirlenir, hep `BaseModel`'e devreder.
- Arayüz: Gauntlet prefab'ları `Module/GUI/Prefabs/TacVeHancer`, fırçalar `Module/GUI/Brushes/TacVeHancer.xml`.
  Entrika paneli Klan ekranının bir sekmesi (`src/UI/ClanIntrigueTab.cs`, UIExtenderEx mixin + prefab eki). Dört bölüm (Entrika, Kişiler,
  Saray, Savaş ve barış) ve alt sekmeler: `docs/superpowers/specs/2026-10-21-arayuz-duzeni.md`.
  `TvhUi.GameLooks`: açılışta oyunun fırçaları bizimkilere kopyalanır. `tools/GuiCheck` her prefab'ı denetler (`--game` ile oyuna karşı).
- Paketleme: `tools/bundle.ps1` `third_party/<mod>` klasörlerini dağıtılan modüle katar; `src/Bundle/*` uyum kodları.
  `tools/RefCheck`: paketli DLL'ler oyunda yüklenebilir mi. `tools/RsPatch`: Regard and Standing'den Calradia Remembers bağlantısını söker.
  `src/Bundle/CompatReport.cs`: kampanya açılınca modların çakışma raporu.

## Kurallar
- Her değişiklikten sonra `build.ps1` (ya da en azından testler + GuiCheck) temiz geçmeli.
- Arayüz metinleri yalnızca Latin-1 + ŞşİıĞğ.
- Yeni özellik yeni pencere açmaz: Entrika ekranının bir alt sekmesine girer (kullanıcı isteği, 21 Ekim). Ayrı pencere yalnızca olay
  kartları ve Savaş Raporu (Diplomacy'nin harita girişi).
- Yetişkin içerik yalnızca metin ve perde kapanır; 18+ zorunlu. Reddedilenler: cariyelik; esirleri, eşleri ya da adlı karakterleri
  zorla fuhşa sokmak; eşini pazarlamak. Genelev: adlı, gönüllü, ücretli yetişkin çalışanlar. Yoldaşlar da çalışan ya da yönetici
  olabilir (21 Ekim, kullanıcı kararı) ama yalnızca kendi kararıyla: karakterine göre kabul ya da ret eder, istediği hafta bırakır,
  payı kendi kesesine gider, zorlama yolu yok. Eş, sevgili, esir, aile ve lordlar çalışan olarak asla; 19 yaş altı asla. Eşin
  yalnızca yönetici olabilir, o da kendi kararıyla (kullanıcı eşini çalışan olarak sordu; "eşini pazarlamak" çizgisi gereği yapılmadı).
  Varsayılan kapalı.
- Commit mesajları İngilizce, açıklayıcı; model adı yazma. Kullanıcı istemedikçe PR açma.
