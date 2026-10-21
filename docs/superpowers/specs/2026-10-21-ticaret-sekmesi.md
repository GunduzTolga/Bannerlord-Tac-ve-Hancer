# Saray > Ticaret: AutoTrader'ın ayarları

21 Ekim 2026. Kullanıcı: "AutoTrader yapalım" — paketli AutoTrader'ın kalan son ayrı penceresi (Alt+A ile açılan ayar ekranı) yeni
kural gereği (yeni özellik yeni pencere açmaz) Entrika ekranına taşındı. Yeri: **Saray** bölümü, "Saray · Krallık · Divan · Ticaret · Kronik".
Ticaretin kendisi yine AutoTrader'ın: kasaba ve köy menüsündeki "Otomatik ticaret yap" seçeneği ve kervanlarla konuşurken
"Mallarına bakmak istiyorum (Otomatik ticaret)".

## Ne var

- **Başlık şeridi:** ad, nasıl çalıştığı, sağda dört hazır ayar düğmesi (Varsayılan, Ganimet satıcısı, Uzak ticaret, Yakın ticaret;
  AutoTrader'ın kendi düğmeleriyle aynı değerler, onay ister).
- **Sol sütun** (`TvhStepRow`, -/+ ile basamaklı):
  - FİYAT: fiyat yöntemi (yeşil/kırmızı ya da ortalama fiyat); ortalamaysa mesafeye göre ağırlık, ağırlıksızsa arama yarıçapı
    (80-1000, 1000 sınırsız); alış ve satış eşiği (ortalamanın %0-200'ü).
  - YÜK VE KESE: kullanılacak envanter (%0-150), tek bir mal en çok (envanterin en fazla o kadarı), War Sails varsa filo kapasitesi,
    kesede bırakılacak asker maaşı (0-50 gün).
  - ELDE TUTULACAKLAR: tahıl en az/en çok (500 ve üstü "hepsi"), öteki yiyecekler en az/en çok. En çok, en azdan aşağı inmez.
- **Sağ sütun:** AL / SAT: atlar, silahlar, zırhlar, ticaret malları, yiyecek ve içecek, hayvanlar; her biri tek satırda
  (dokunma / yalnızca al / yalnızca sat / al ve sat); satılacak silah ve zırh kademesi (0-6). ÖZEL: bütün sığırları sat, kömür ve külçe
  sat, eritilebilecek silahları tut, yiyeceği ve keresteyi "en az" değerine tamamla, kısayol (Alt+A ya da Alt+A+T).

Basamaklar AutoTrader'ın kaydırıcı aralıklarından (`Core/TradeSettingsRules`, testli); ayar dosyasında listede olmayan bir değer varsa o da
basamaklara katılır, kaybolmaz.

## Kayıt ve yönlendirme

- Ayarlar AutoTrader'ın (`AutoTrader.AutoTraderConfig` statik özellikleri, `Configs\AutoTraderConfig.xml`); her değişiklikte onun `Save`'i
  çağrılır (`Bundle/AutoTraderCompat`). Ayrı bir "Tamam" yok: ekran bir sonraki karede yeniden kurulur.
- `AutoTraderSubModule.OpenSettingsMenu` (Alt+A) Harmony ön yamasıyla `IntrigueScreen.OpenTab(TabTrade)`'e yönlenir (`BundleShim`);
  tuşlar basılı kaldıkça her karede çağrıldığı için 1,5 saniyelik bekleme var. Kampanya durumu yoksa AutoTrader'ın penceresi açılır.

## Türkçe

AutoTrader'ın hiç Türkçesi yoktu: 56 metni (menü seçeneği, kervan satırı, uyarılar, kendi penceresinin yazıları)
`third_party/AutoTrader/ModuleData/Languages/TR/std_AutoTrader-tr.xml`'e çevrildi; açılıştaki "Ayarlar için ALT + A" mesajı sekmenin yerini söyler.
