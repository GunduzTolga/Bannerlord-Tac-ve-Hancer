# Regard and Standing ile gizli ilişkiler

20 Ekim 2026. Kullanıcı kararı: "Gizli ilişkileri RegardStanding'e bırak; bizim casus başımız onun gerçek ilişkilerini bulsun, bizim
uydurma ilişki sırlarımız kapansın."

## Neden

Regard and Standing (RS, 1.5.0) içine paketlendi (`third_party\RegardStanding`). Lordlar arasında **gerçek** gizli ilişkiler kuruyor:
- Kim kiminle, ne zamandan beri, bitti mi, ortaya çıktı mı.
- Bu ilişkilerden çocuklar doğuyor; gerçek babayı ve resmî kocayı kaydediyor.
- Kaçmalar ve skandallar da onda.

Bizim gizli ilişki sırlarımız (P2.4) ise casusluk başarılı olunca zarla **uyduruluyordu**. Eşinin gizli hayatı (P2 6b) da eşine kendi
zarıyla sevgili buluyordu. İkisi birlikte açık olsaydı dünyada iki ayrı "kim kiminle" gerçeği olurdu. Artık tek sahip RS.

## Ne değişti (RS yüklüyken)

| Yer | Eskiden | Şimdi |
|---|---|---|
| Gizli ilişki sırrı (casusluk, esir sorgusu, genelev, takip: hepsi `RomanceSecrets.Consider/Fill`) | Hedef evliyse zarla "var" sayılır, uygun bir lord seçilirdi | Hedefin RS'deki henüz ifşa olmamış ilişkisi bulunur: önce süren, yoksa en son biten. Yoksa bu sır yok. |
| Gayrimeşru çocuk sırrı | Bilinen bir sevgiliden, resmî babası başkası olan çocuklar arasından seçilirdi | RS'nin gerçek baba kayıtlarından: doğmuş, hayatta, resmî babası gerçek babası olmayan ve sen olmayan, senin klanında olmayan çocuk. |
| İfşa (`ExposeEffects`) | Bizim ilişki cezalarımız | Çift RS'de kayıtlıysa skandalı RS işler: kin, dışlanma, dedikodu; ilişki biter. İfşa eden sen olarak bildirilir, ilişkidekiler sana kin tutar. Bizim cezalarımız bu durumda uygulanmaz, skandal iki kez sayılmasın. |
| Sırrın metni | Hep "ilişkisi var" | RS'de ilişki bittiyse "vardı". İkisi de evli değilse bunu söyler. |
| Eşinin gizli hayatı (`SpouseLife.Weekly`) | Eşin kendi zarımızla sevgili edinir, buluşur, gizli gebelik bizim sistemimizle | Eşinin RS'deki süren ilişkisi kayda alınır. Senden önce biterse ya da sevgili değişirse kayıt da değişir. Çocuklar RS'nin gerçek baba kayıtlarından eklenir. Araştırma, karşı casusluk, hekimin fark etmesi, yüzleşme ve boşanma aynen çalışır. |
| Yüzleşmede eşin ilişkiyi bitirirse | Bizim kaydımız silinirdi | RS'de de bitirilir (`BreakOff`). |

**Değişmeyenler:**
- Senin kendi kur ve sevgili ilişkilerin, "eşini bilgi için gönder", oyuncunun kendi ilişkisinin ortaya çıkması. RS oyuncuyu kapsamıyor (`Affairs.Free`: MainHero hariç).
- Eşine söylemek (özel bilgi, skandal yok) aynen kalır.
- Eski kayıtlardaki uydurma sırların RS'de karşılığı yok. Onlara eski kurallar uygulanır.

RS yoksa ya da iç yapısı beklenenden farklıysa (`RegardStandingCompat.Available` false) her şey eskisi gibi çalışır.

## Teknik

- `src\Bundle\RegardStandingCompat.cs`: RS'nin herkese açık bir API'si yok (yalnızca onur borcu ve şövalyeler). İç türlerine adıyla ulaşılır:
  - `Affairs.All` (`Affair`: A, B, Day, End < 0 ise sürüyor, Exposed)
  - `Affairs.Secrets` (`Secret`: Mother, Father, Husband, Born, Children)
  - `Affairs.Expose(Affair, Hero)` ve `Affairs.BreakOff(Affair)`

  Her alanın türü denetlenir; biri eksikse katman kapanır.
- Yaş: RS ilişki, hoşlanma ve evlilikte iki tarafı 18–55, gebelikte anneyi 18–42 yaşla sınırlıyor, çocukları her yerde dışarıda tutuyor
  (`Affairs.Free`, `Attachments.Eligible`, `Courtship.NotFree`, `Affairs.CanConceive`). Bizim tarafta da hedef ve öbür taraf için yetişkin denetimi sürüyor.
- RS'nin açılış yamaları (diplomasi modelinde etkin ilişki ve harita bildirimleri) çökme tetiği taşımıyor. Lord sayfası ve ikna
  yamalarını kampanya içinde saat başı uyguluyor. Diplomacy'nin aynı metottaki yaması yalnızca boş kişi denetimi, çakışmıyor.
- Gizli suikastlarımız katili boş bildirdiği için RS onları sana yükleyemez. Yalnızca oyunun bildiği öldürmeler kin doğurur.

## Oyunda dene

- [ ] Launcher'da tek mod; Mod Seçenekleri'nde "Regard and Standing" sayfası var.
- [ ] Bir süre oyna (RS ilişkileri yılda sınırlı sayıda başlar), sonra evli bir lorda casusluk yap. Gizli ilişki sırrı çıkarsa, adı geçen sevgili RS'nin
      haberleriyle tutarlı mı? (Sarayda "A Romance at Court" ya da "A Scandal at Court" bildirimleri.)
- [ ] O sırrı ifşa et: sonuç kartında "skandal sarayda yayıldı"; RS'nin skandal bildirimi geliyor; aynı çift için ikinci bir ceza listesi yok.
- [ ] Evli bir lordun ilişkisi yoksa casusluk "gizli ilişki" bulmuyor (eskiden zarla çıkabiliyordu).
- [ ] Eşin RS'de bir ilişki başlatırsa: Eşini araştır → bulunuyor; kartta sevgilinin adı doğru; yüzleşip bitirirse RS'de de bitiyor.
