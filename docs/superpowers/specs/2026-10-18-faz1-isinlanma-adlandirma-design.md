# Faz 1 — Işınlanma ve Adlandırma (Tasarım)

**Tarih:** 2026-10-18 · **Oyun:** v1.4.8 + War Sails v1.2.8 · **Dayanak:** `reports/Bannerlord eksiklikleri ve mod fırsatları.md`, `research_notes/.../technical_feasibility.md` §1–2

## Kullanıcı kararları
- Fareyle ışınlanma: **Ctrl+Alt + Sol tık** (Ctrl+tık vanilla hile modunun, Alt+tık Parley'nin).
- Yerleşime ışınlanınca: **her zaman kapının önüne** (içeri girmez).
- Adlandırma kapsamı: **her şey** (her kahraman, klan, krallık, yerleşim, parti).

## Kullanıcı akışı
Insert menüsünün kök listesine iki kategori eklenir:

**Harita**
- *Yerleşime ışınlan…* → arama kutulu liste (şehir/kale/köy; `Pravend (Şehir, Vlandia)`) → kapının önüne.
  Denizdeysen karaya geçilir.
- *Bir lordun yanına ışınlan…* → arama kutulu liste (lordlar + yoldaşlar, çocuklar hariç) →
  partisi varsa yanına (1–2 birim uzağa, üstüne değil), yerleşimdeyse kapısına; esir/konumsuzsa hata mesajı.
  Hedef denizdeyse ve gemin yoksa reddedilir.
- Açıklama satırında ipucu: "Ctrl+Alt + Sol tık: haritada tıkladığın yere ışınlan".
- Işınlanınca menü kapanır, kamera partiye kayar.

**Ctrl+Alt + Sol tık** (yalnızca kampanya haritası ekranında, açık menü/popup yokken): imlecin altındaki geçerli
noktaya ışınlanma. Deniz noktası + gemi yok → reddedilir.

**Adlandır**
- Kısa yollar: *Kendimi*, *Partimi*, *Bulunduğum yerleşimi*.
- Listeler: *Bir lord veya yoldaş…*, *Bir eşraf…*, *Bir klan…*, *Bir krallık…*, *Bir yerleşim…*, *Bir parti…*
  (lord/kervan/oyuncu partileri; haydut, köylü, garnizon hariç).
- Seçince metin kutusu mevcut adla dolu açılır → onay → yeşil mesaj → Adlandır menüsü yeniden açılır.
- Yerleşim ve partide boş ad = orijinal ada dön. Parti adlandırırken "oyun bazı olaylarda adı sıfırlayabilir" notu.

## Işınlanma kuralları
Engeller (kırmızı mesajla reddedilir): sahne/savaş içindeyken (`Mission.Current`), savaş (`MapEvent`), kuşatma
(`SiegeEvent`/`BesiegedSettlement`), sal durumu, başkasının ordusu, esirlik.

Adımlar oyunun kendi hile-modu Ctrl+tık kodunu (`MapScreen.HandleLeftMouseButtonClick`) izler:
1. Yerleşimdeysen / karşılaşma açıksa: `PlayerEncounter.LeaveSettlement()` + `PlayerEncounter.Finish(true)`.
2. `MainParty.Position = hedef`, `SetMoveModeHold()`.
3. Hedefin kara/deniz durumu partininkinden farklıysa `ChangeIsCurrentlyAtSeaCheat()`.
4. Ordunun lideriysen `AttachedParties` aynı noktaya.
5. `MobileParty.All` ve `Settlement.All` için `UpdateVisibilityAndInspected(MainParty.Position)`.
6. `MapScreen.Instance.TeleportCameraToMainParty()`.

## Adlandırma kuralları
Oyunun kendi ekranlarının metin şablonları kullanılır (cinsiyet/şablon değişkenleri bozulmasın):
- Kahraman: `str_generic_character_firstname` (CHARACTER_FIRSTNAME) + `str_generic_character_name`
  (CHARACTER_NAME), ikisine CHARACTER_GENDER → `Hero.SetName(full, first)`; ordu lideriyse `Army.UpdateName()`.
- Klan: `str_generic_clan_name` (CLAN_NAME) → `Clan.ChangeClanName(t, t)`.
- Krallık: `str_generic_kingdom_name` (KINGDOM_NAME) + `str_generic_kingdom_short_name` (KINGDOM_SHORT_NAME)
  → `Kingdom.ChangeKingdomName(full, short)`.
- Yerleşim / parti: `Party.SetCustomName(text)`; sıfırlama `TextObject.GetEmpty()` (oyunun kendi yöntemi).
  Yerleşimin asıl `_name` alanı save'e yazılmadığı için ona dokunulmaz.

`NameRules` (saf, test edilir): kırp; boş → yalnızca izin verilen türlerde geçerli; en fazla 50 karakter;
`{` ve `}` yasak (TextObject bunları değişken sanar).

## Bileşenler
- `src/NameRules.cs` — ad doğrulama (saf).
- `src/Menu/Picker.cs` — arama kutulu genel seçici (`isSeachAvailable: true`), seçimi bir sonraki kareye erteler.
- `src/Menu/MenuUI.cs` — `AskName` eklenir; `ShowList`'e isteğe bağlı açıklama metni.
- `src/Menu/MenuEntry.cs` — `Text` satır türü (ad soran eylem).
- `src/Cheats/Teleport.cs` — engel kontrolü + yerleşime / kahramana / konuma ışınlama.
- `src/Cheats/CursorTeleport.cs` — Ctrl+Alt+tık ve imleç konumu.
- `src/Cheats/Renamer.cs` — 5 tür adlandırma.
- `src/Menu/Categories.cs` — Harita ve Adlandır kategorileri.
- `src/SubModule.cs` — `CursorTeleport.Tick()`.
- `src/CheatMenu.csproj` — `SandBox.View`, `SandBox`, `TaleWorlds.ScreenSystem` referansları.

## Kayıt ve başarımlar
Yalnızca oyunun kendi kaydettiği alanlar (pozisyon, deniz durumu, ad alanları). Özel kayıt tipi yok; mod
kaldırılınca kayıt açılır, isimler kalır. Konsol hileleri ve hile modu kullanılmaz; başarım kontrolüne dokunulmaz.

## Doğrulama
`NameRules` birim testleri; derleme; DLL'in oyun assembly'leriyle tip yükleme kontrolü; oyun içi kontrol listesi
(şehre/lorda/imlece ışınlanma, denizde gemisiz ret, ordu birlikte gelir, 5 tür adlandırma, kaydet-yükle sonrası
adların kalması, yerleşim adının harita etiketinde güncellenmesi).

## Kapsam dışı
İçeri girerek ışınlanma, eşraf dışı tüm NPC'ler (askerler vb.), yerleşim "içeri gir" seçeneği, isim çevirisi.
