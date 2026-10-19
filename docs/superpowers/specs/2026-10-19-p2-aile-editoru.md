# P2.7 — Aile editörü

**Tarih:** 2026-10-19 · **Durum:** Kodlandı (oyunda test bekliyor) · **Yol haritası:** bölüm 4, sıra 7
**Nerede:** Insert > Editörler > **Aile editörü**. Önce sen, sonra klanın (yaşa göre), sonra diğer soylular ve yoldaşlar listelenir.

## Neden

Rakip editörlerde var, bizde eksikti. Oyunun kendi kurallarının dışına çıkmaz: evlilik ve gebelik oyunun kendi kontrollerinden
geçer. Bu yüzden editör yaş, akrabalık ya da Doğum ve Ölüm ayarı gibi kuralları delmez.

## İşlemler

| İşlem | Nasıl |
|---|---|
| **Evlendir** | Aday listesi oyunun evlilik uygunluk kontrolünden geçenlerdir: yaş, akrabalık, cins, medeni durum. Evlilik oyunun kendi işlemiyle yapılır (`MarriageAction`): klan değişimi ve bildirim oyunun kendisi. |
| **Boşa** | Sen ya da eşinse entrikadaki boşanma, sonuçlarıyla birlikte ("Eşinin gizli hayatı"). Başkalarında eş bağı kalkar; evlilikle klana girmiş ve lider olmayan taraf doğduğu klana döner. Hamileyken ve dönecek kişi bir partiye liderlik ediyorken kapalı. |
| **Gebelik başlat** | Oyunun kendi gebeliği (`MakePregnantAction`): baba eşidir, süre oyunun gebelik süresi. Yalnızca evli, 18–45 yaş arası bir kadın için, oyunun Doğum ve Ölüm ayarı açıksa. |
| **Çocuk istemiyorlar** | Çiftten biri işaretliyse oyunun gebelik ihtimali 0 olur (oyunun gebelik modeli zincirlenir). Kur ve sevgili gebelikleri (P2.6) de olmaz. |
| **Çocuğu büyüt** | Bir yaş ya da doğrudan yetişkinlik yaşına (oyunun ayarı, 18). Ara geçişleri ve yetişkinliği oyunun yaşlanma sistemi ertesi gün işler. |
| **Yaşlanmayı dondur** | Her gün doğum günü bir gün ileri alınır; yaşı sabit kalır. Kapatınca normal yaşlanır. |

Gebeliği sonlandırma seçeneği yok. Yol haritasındaki "gebeliği kapat" maddesi, "çocuk istemiyorlar" ile önlemek olarak yapıldı.

## Kayıt

"Çocuk istemiyorlar" ("nochild") ve yaşlanması dondurulanlar ("frozen") kampanya kaydına yazılır.

## Test

- Kayıt: `tests/CheatMenu.Tests` (durum testi).
- Oyunda: YAPILACAKLAR > P2.7.
