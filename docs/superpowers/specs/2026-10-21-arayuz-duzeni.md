# Arayüz düzeni: her şey tek ekranda

21 Ekim 2026. Kullanıcı: "Bir sürü özellik ekliyoruz, bunları mantıklı şekilde yönetilebilir tasarla; her yerde farklı farklı ekranlar
olmasın. Tasarım da önemli."

## İlke

Bütün özellikler **Entrika ekranında** (Klan ekranının Entrika sekmesi; bağlanamazsa ayrı Entrika ekranı). Ekran dört bölüme ayrılır, her
bölümün alt sekmeleri vardır; her özellik tek bir alt sekmede durur. **Yeni bir özellik yeni pencere açmaz**, bir alt sekmeye girer.

| Bölüm | Alt sekmeler | Ne için |
|---|---|---|
| **Entrika** | Süren · Yeni entrika · Ağlar · Sana karşı | casus başının işleri (soldaki casus başı sütunu yalnızca burada) |
| **Kişiler** | Kişiler · Lordlar · Esirler | insanlar: dosyalar, lordlarla iletişim (ulak, çağır, ziyafet), esirler |
| **Saray** | Saray · Krallık · Divan · Kronik | krallığın sarayı; klanlara bağış, tımar (şehir ve kale) verme ve iade; divan koltukları ve raporları; olayların kaydı |
| **Savaş ve barış** | Krallıklar · Savaşlar · İsyanlar | bütün krallıklar ve bağlar (ittifak, ticaret, saldırmazlık, savaş ilanı), her savaşın durumu ve Barış Masası, isyan grupları |

## Ayrı pencere kalanlar (yalnızca bunlar)

- **Olay kartları:** seçim ve bildirim; hangi ekranda olursan ol üstte açılır.
- **Savaş Raporu:** Diplomacy'nin haritadaki savaş yorgunluğu göstergesinden açılan rapor; Savaşlar sekmesindeki "Savaş Raporu"
  düğmesinden de açılır. İçindeki "Barış Masası..." raporu kapatıp Savaşlar sekmesine götürür.

## Yönlendirmeler

- Haritadaki Entrika düğmesi ve kısayol tuşu: en son açık sekme.
- Insert > Entrika (liste menüsü): girişler ilgili sekmeyi açar (Lordlar, Krallıklar, İsyanlar, Barış Masası).
- Diplomacy'nin İsyanlar düğmesi (Krallık ekranı, ansiklopedi): Krallık ekranı ya da ansiklopedi kapanır, İsyanlar sekmesi o krallıkla açılır.
- Elçi kartı "Barış Masası'na otur", Divan'daki "Barış Masası" düğmesi, Savaş Raporu: Savaşlar sekmesi o savaşla açılır.
- Saray'daki "İsyanlar..." düğmesi: seçili krallığın isyan grupları.
- Diplomacy'nin "Altın Bağışla", "Tımar ver" (Krallık > Klanlar, ansiklopedi) ve "Return Fief" düğmeleri: Saray > Krallık, o klan
  seçili (`2026-10-21-krallik-sekmesi.md`).
- Kod: `IntrigueScreen.OpenAt(focus)`, `OpenTab`, `OpenWar`, `OpenRebellions`, `OpenRealm`, `OpenKingdoms`; ViewModel'de
  `IntrigueScreenVM.FocusWar`, `FocusRebellions`, `FocusRealm`, `FocusKingdoms`.

## Görünüş kuralları

- Üst satır: bölümler (oyunun başlık sekmesi, `TvhGroupTab`); ikinci satır: alt sekmeler (küçük düğme, `TvhSubTab`); çizgi; içerik.
- Her panelin başlık şeridi aynı: solda bilgi (süzgeçler, krallık, özet), sağda ana düğme ve altında koşulu (ör. Ziyafet ver, Grup kur).
- Listeler: oyunun kaydırma çubuğu (`Standard.VerticalScrollbar`), kartlar `Tvh.Card` ya da seçilebilir `Tvh.ListItem`.
- Bilinmeyen içerik ya da engel: düğme gri, nedeni yanında (kırmızı ya da soluk); boş liste yerine ne yapılacağını anlatan metin.

## Kaldırılanlar

`LordsScreen`, `RebellionsScreen`, `PeaceTableScreen` ve prefab'ları (`TvhLords`, `TvhRebellions`, `TvhPeaceTable`): yerlerine
`TvhLordsPanel`, `TvhRebellionsPanel`, `TvhWarsPanel` + `TvhPeaceTablePanel`. Saray'daki "Lordlar..." düğmesi (Lordlar artık yan sekme).
Diplomacy'nin bağış, fief verme ve iade pencereleri yönlendirildi (krallıkta değilsen ya da bir şey ters giderse yine onlar açılır).
