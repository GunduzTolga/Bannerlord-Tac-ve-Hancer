# Casus başı görevi: Ajan yetiştir

21 Ekim 2026. Kullanıcı isteği: "yoldaş atasak olmuyor mu? o da casus yetiştirir falan". Casus başı (yoldaş ya da aile, krallık
gerekmez) dördüncü görev olarak kendi ajanlarını yetiştirir.

## Neden

Gölge ağı bir şehre bağlı: yalnızca o şehirde tam, o krallıkta yarım bonus verir ve bir çete liderine bağımlıdır. Ajanlar casus
başının yanında, her yerde işe yarar; ama azdır, pahalıdır ve yakalanınca kaybolur. Krallığı olmayan oyuncu da casusluğunu
büyütebilir.

## Kurallar (`AgentRules`, saf, testli)

| Kural | Değer |
|---|---|
| Kadro üst sınırı | 1 + Roguery/50, en fazla 5 (Roguery 0: 1, 100: 3, 200+: 5) |
| Yetiştirme | Görev "Ajan yetiştir" iken 2 haftada 1 ajan; eğitim haftada 200 altın |
| Bakım | Her ajan haftada 50 altın, görev ne olursa olsun. Ödenemezse bir ajan dağılır |
| Etki | Ajan başına bütün entrikalara +2 başarı, −1 yakalanma (en fazla +10 / −5). Kişi hedefli entrikalarda gölge ağı bonusuyla toplanır, formüldeki ortak üst sınır geçerli (+15 başarı, −6 yakalanma). Sabotaj ve kışkırtmada ayrıca eklenir |
| Kayıp | Bir entrikan ortaya çıkarsa bir ajan yakalanır |
| Casus başı yoksa | Ajanlar bekler ama bonus vermez; eğitim casus başı meşgulken (esir, vali, parti lideri) durur |

Kadro dolunca görev kendiliğinden biter ("kadro dolu" kartı). Casus başının Roguery'si artınca yeniden seçilebilir.

## Arayüz

- Casus başı → Görev kartında yeni seçenek: "Ajan yetiştir: N/M ajan (haftada X)", dolu ise soluk ve nedeni yazar.
- Görev satırında ajan sayısı ("· ajan N").
- Yeni ajan hazır olunca kısa bildirim, ajan yakalanınca ya da dağılınca kartın sonuç satırında.

## Kayıt

`meta` kaydına `agents` (sayı) ve `agenttrain` (biriken eğitim haftası). Eski kayıtlarda 0.

## Oyunda dene

- [ ] Yoldaşı casus başı ata, Görev → Ajan yetiştir; iki hafta sonra "Yeni ajan hazır" bildirimi, altın düştü.
- [ ] Görev satırında "· ajan 1"; bir entrika başlatırken başarı yüzdesi ajan başına +2 arttı.
- [ ] Bir entrikan ortaya çıkınca sonuçta "ajanlarından biri yakalandı" yazıyor, sayı azaldı.
- [ ] Kaydet-yükle: ajan sayısı duruyor.
