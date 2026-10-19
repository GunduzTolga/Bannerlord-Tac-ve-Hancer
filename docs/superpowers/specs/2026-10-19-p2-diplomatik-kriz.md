# P2.5 — Ağ ifşasından diplomatik kriz

**Tarih:** 2026-10-19 · **Durum:** Kodlandı (oyunda test bekliyor) · **Yol haritası:** bölüm 4, sıra 5

## Neden

Gölge ağı kurmanın bedeli şimdiye kadar yalnızca haftalık bakım ve şehrin sahibiyle −10 ilişkiydi. Başka bir krallığın şehrinde
casus ağı yakalanmak gerçek bir diplomatik olay olmalı. Ama yeni bir "suç" ya da "savaş" sistemi icat etmeden: oyunun kendi suç
puanı ve kendi krallık kararları kullanılır.

## Ne olur

Yapay zekâ her hafta ağını bulabilir (seviye başına %2, eskisi gibi). Ağ dağılır, şehrin sahibiyle −10 (eskisi gibi). Sonrası
şehrin kime ait olduğuna bağlı:

| Şehir | Sonuç |
|---|---|
| **Yabancı krallık, barıştasınız** | Diplomatik kriz (aşağıda). |
| Yabancı krallık, savaştasınız | Kimse şaşırmaz: yalnızca bildirim. |
| Kendi krallığın | İç skandal: hükümdarınla ilişki −5 × seviye (lordlarını gözetliyorsun). |
| Kendi şehrin ya da krallıksız şehir | Yalnızca bildirim. |

### Diplomatik kriz

1. **Suç:** o krallıkta oyunun kendi suç puanı +15 × seviye (15/30/45). Oyun kendisi işler: muhafızlar sertleşir, şehirde ceza
   ödeyerek silinir. Krallığın yoksa ve suç oyunun eşiğini aşarsa oyun sana savaş açabilir (oyunun kuralı).
2. **Gerilim:** o krallıkla casus gerilimi +10 × seviye (0–100; haftada −5 söner).
3. **Karar kartı** (hükümdarın elçisi):

| Cevap | Etkisi |
|---|---|
| **Tazminat öde** (2.500 / 4.000 / 5.500 dinar) | Eklenen suç silinir, ifşanın gerilimi geri alınır, hükümdarla −5. |
| **İnkâr et** (casus başının Roguery'si ve senin Charm'ın; büyük ağ zor) | Tutarsa: suçun yarısı silinir, gerilimin yarısı geri alınır, −5. Tutmazsa: hükümdarla −20, gerilim +15. |
| **Reddet** | Altın yok; hükümdarla −15, gerilim +20, suç kalır. |

İnkâr olasılığı: 30 + Roguery/5 + Charm/10 − 10 × seviye, zorluğa göre ±10, %5–80 arası.

### Savaş meclisi

- Gerilim **60**'ı aşarsa, o krallık seninle barıştaysa, aynı anda en fazla bir savaştaysa ve seninkinin en az yarısı kadar güçlüyse,
  her hafta %10–40 ihtimalle hükümdar **savaş meclisini toplar**: oyunun kendi "savaş ilan et" kararı o krallığa sunulur ve
  lordları oy verir. Savaş çıkıp çıkmayacağını oyunun kendi siyaseti belirler; mod zorla savaş açmaz.
- Meclis toplanınca gerilim 40'a iner (her hafta yeniden önerilmesin).
- Bir krallığın vasalıysan, hükümdarın senin yüzünden krallığı savaşın eşiğine getirdiğin için kızar: −20.
- Krallığın yoksa meclis toplanmaz (krallık kararı krallıklar arasıdır); asıl tehlike suç puanı.
- Tek bir reddedilmiş seviye 3 ifşası (30 + 20 = 50) savaşa yetmez. Birkaç skandal üst üste gelmeli.

## Görünüm

- Ağ çökünce tek kart: "Diplomatik kriz: Krallık". Ne olduğu, suç, gerilim ve üç seçenek bir arada.
- Cevaptan sonra sonuç kartı; savaş meclisinde ayrı kart.
- Saray sekmesinde krallığın satırında "casus gerilimi: gergin (35/100)".
- Şehir menüsü > Gölgelerde iş çevir: riski anlatan satır.
- Resimler: `sonuc_diplomatik_kriz`, `sonuc_savas_meclisi` (prompt'lar `docs/gorseller/olay-kartlari.md`).

## Kayıt

Gerilim ("tension") ve cevaplanmamış krizler ("crisis") kampanya kaydına yazılır; kriz kartı yüklemeden sonra da sorulur.
Krallık yok olursa, hükümdarı ölürse ya da savaş çıkarsa kriz kendiliğinden kapanır.

## Test

- Formüller ve kayıt: `tests/CheatMenu.Tests` (CrisisTests, durum testleri).
- Oyunda: YAPILACAKLAR > P2.5.
