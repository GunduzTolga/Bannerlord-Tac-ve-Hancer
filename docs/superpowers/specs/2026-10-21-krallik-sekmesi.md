# Saray > Krallık: bağış, fief verme, fief iadesi

21 Ekim 2026. Kullanıcı seçimi: "Diplomacy pencerelerini taşı" — yeni kural gereği (yeni özellik yeni pencere açmaz) Diplomacy'nin kalan
ayrı pencereleri (altın bağışı, fief verme, fief iadesi) Entrika ekranına. Yeri: **Saray** bölümü, "Saray · Krallık · Divan · Kronik".

## Ne var

Liste ve ayrıntı, Savaşlar sekmesi gibi:

- **Başlık şeridi:** krallık ve hükümdar ("sensin"), sağda klan sayısı, kesen, yerlerin.
- **Solda klanlar** (`TvhClanItem`): sancak, ad ve kademe, lideri ve ilişkin, yerleri, gücü; "hükümdar klanı" ya da "paralı asker"
  etiketi. Sıra: hükümdar klanı, sonra kademe. Tıklanınca seçilir.
- **Sağda seçili klan:** liderin portresi ve bilgisi; altında iki sütun.
  - **Altın bağışla:** miktar basamaklı satır (`TvhStepRow`; basamaklar Barış Masası'ndaki "Altın ver" ile aynı:
    `PeaceTreatyRules.GiftSteps(kesen)`), liderle ilişki önizlemesi, "Bağışla". Herkes yapabilir (hükümdar olmak gerekmez).
  - **Fief ver** (yalnızca hükümdar): kendi şehir ve kalelerin, her birinde liderle ilişki önizlemesi; kuşatma altındaki gri.
    Onay sorulur (geri alınamaz; daha az yeri olan klanların kıskançlığı yazılır). Paralı klana verilmez.
  - **İade edilebilir yerler** (yalnızca hükümdar): barışta olduğun krallıklardan aldığın, Diplomacy'nin kuralına uyan yerler
    ("Pravend, Vlandia'ya · ilişki +N"). Onay sorulur; eski sahibiyle ilişki, o krallığın öteki klanları memnun, yayılmacılık azalır.
- Krallıkta değilsen açıklama.

## Kurallar ve hesaplar Diplomacy'nin

Biz yalnızca görünüş ve yönlendirme yapıyoruz; işlemler `Bundle/DiplomacyCompat` (reflection) üzerinden onun:

| İş | Diplomacy | Not |
|---|---|---|
| Bağış | `DonateGoldVM` kurulur, `IntValue` verilir, `ExecutePropose` | altın klanın komutanlarına paylaşılır, ilişki, Cömert/Hesapçı izi |
| Bağış önizlemesi | aynı formül: `RealmRules.DonationRelation` (testli) ve `DiplomacyModel.GetRelationIncreaseFactor` | |
| Fief verme | `GrantFiefAction.CanGrantFief`, `PreviewPositiveRelationChange`, `Apply` | kıskançlık: daha az yeri olanlar, ayarlı ceza (varsayılan -2) |
| İade | `ReturnFiefAction.GetReturnableFiefs`, `PreviewRelationChange`, `Apply` | ayarla kapatılabilir; savaştaki krallığa iade yok |

## Yönlendirmeler (BundleShim, Harmony ön yamaları)

- `DonateGoldInterface.ShowInterface(screenBase, clan)` → Krallık sekmesi, o klan seçili (Krallık > Klanlar'daki "Altın bağışla").
- `GrantFiefInterface.ShowFiefInterface(screenBase, hero, refreshAction)` → o kişinin klanı seçili (Krallık > Klanlar ve ansiklopedideki
  "Fief ver"). İki parametreli sürümü bunu çağırdığı için o da gelir.
- `ReturnFiefInterface.ShowReturnFiefInterface(screenBase, targetKingdom, refreshAction)` → Krallık sekmesi (iade bölümü).
- Krallıkta değilsen ya da kampanya durumu yoksa Diplomacy'nin penceresi açılır. Krallık ekranı ve ansiklopedi kapanır
  (`IntrigueScreen.OpenAt`).

## Kod

`UI/ViewModels/RealmVM.cs` (`RealmVM`, `ClanItemVM`), prefab'lar `TvhRealmPanel`, `TvhClanItem`; `IntrigueScreenVM.TabRealm`,
`FocusRealm(clan)`; `IntrigueScreen.OpenRealm(clan)`. `StepRowVM.SetSteps` (kese değişince basamaklar).
