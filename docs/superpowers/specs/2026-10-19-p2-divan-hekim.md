# P2.13 — Divan: saray hekimi

**Tarih:** 2026-10-19 · **Durum:** Kodlandı (oyunda test bekliyor) · **Yol haritası:** bölüm 4, sıra 13
**Kullanıcı isteği:** "Tamam knka, hekimle devam et" (divanın ikinci koltuğu; öneri listesinden).

## Neden

CK3'teki saray hekimi. Bannerlord'da doğumda ölüm ve yaşlılıktan ölüm var, ama oyuncunun bunlara karşı yapabileceği bir şey yok. Oyunun
cerrahı (parti rolü) yalnızca savaştaki askerlere bakar. Hekim, oyunun kendi ölüm modellerine bağlanarak klanını korur. Yeni bir
sağlık sistemi icat edilmiyor.

## Koltuk

- **Kim:** klanından bir yetişkin (sen değil); başka bir divan koltuğunda oturan olamaz (casus başı, hazinedar, hekim: kişi başına bir
  koltuk). Esir, vali ya da parti lideriyken sarayda değildir, etkileri durur.
- **Nerede:** Entrika ekranı > Divan > "SARAY HEKİMİ" koltuğu. Liste menüsü: Insert > Entrika > Divan > Saray hekimi.
- **Beceri:** Medicine. **30'un altı şarlatandır**: tedavisi riski artırır. Kart uyarır.

| Etki | Formül (Medicine = M) | Örnek: M 0 / 120 / 270 |
|---|---|---|
| Yaşlılıktan ölüm riski | −(M − 30)/5 %, −%10 ile %50 arası | +%6 / −%18 / −%48 |
| Doğumda ölüm riski | −(M − 30)/3 %, −%10 ile %80 arası | +%10 / −%30 / −%80 |
| Başarılı suikastten kurtarma | (M − 30)/6 %, en fazla %40 | %0 / %15 / %40 |
| Eşinin gizli gebeliğini fark etme (haftalık) | 5 + M/6 %, en fazla %50 | %5 / %25 / %50 |
| Ömür tahmini hata payı | 45 − M/6 %, %5–60 | %45 / %25 / %5 |

## Nasıl bağlanıyor

- **Yaşlılık:** oyunun `HeroDeathProbabilityCalculationModel` modeli zincirlenir; yalnızca senin klanındakiler için olasılık azalır
  (şarlatanda artar). Oyunun günlük olasılığı yıllığa çevrilip raporda gösterilir: 1 − (1 − günlük)^84.
- **Doğum:** oyunun gebelik modelindeki "doğumda ölüm" değeri tek bir genel sayı. Oyun önce doğum olayını yayınlar, sonra annenin
  ölüm zarını atar: mod olaydan anneyi öğrenir ve aynı anda okunan değeri o anneye göre (klanındaysa) hesaplar.
- **Suikast:** yapay zekânın sana kurduğu suikast başarılı olursa, ölümden önce hekimin zarı atılır. Tutarsa kurban ağır yaralı
  (canı beşte bir) kurtulur ve "Suikastten kurtuldu" kartı gelir. Hekim kendini kurtaramaz.
- **Gizli gebelik:** eşinin gizli ilişkisinden gebeyse (P2.6b), hekim haftalık zarla fark eder ve ilişki ortaya çıkar: oyunun
  bilinen yüzleş / görmezden gel / boşa kartı gelir. Eşin hekimse kendini ele vermez.
- **Uyarı:** senin yıllık ölüm riskin hekimin gözüyle %10'u geçerse yılda bir "Hekimin uyarıyor" kartı: varisini düşün
  (Editörler > Aile editörü).
- **Beceri:** hekim çalıştığı her hafta Medicine'e 40, kurtardığı her canda 500 XP alır.

## Sağlık raporu (Divan sekmesi)

Klanındaki yetişkinler (ve gebe olanlar): sen, eşin, sonra yaşa göre. Yaş, can, yaralı, esir, gebe (doğuma kalan gün: kendi
gebeliklerimizde kesin, oyunun gebeliğinde gebe kaldığı günden tahmin) ve hekimin gözüyle yıllık ölüm riski. Risk %10'u geçince kırmızı.

## Kayıt

Hekim, uyarının tekrar tarihi ve klanındaki annelerin gebe kaldığı gün ("conceived") kampanya kaydına yazılır.

## Bilinmeyenler (oyunda denenecek)

- Oyun doğumda gerçekten önce olayı yayınlayıp sonra ölüm zarını mı atıyor? Değilse doğum koruması işlemez (zarar da vermez).
- Oyunun yaşlılık olasılığı günlük mü? Rapordaki yıllık riskler makul mü (60 yaşında birkaç yüzde, 80'de daha çok)?
- Ölü doğum oyunda anneden bağımsız bir sayı; hekim bunu etkilemiyor.

## Resimler

`sonuc_hekim` (uyarı) ve `sonuc_panzehir` (suikastten kurtarma), üçer sürüm.

## Test

- Koruma yüzdeleri, şarlatan sınırı, olasılık uygulama, yıllık risk, kayıt: `tests/CheatMenu.Tests` (PhysicianTests).
- Arayüz: GuiCheck (TvhCouncilSeat, TvhCouncilPanel).
- Oyunda: YAPILACAKLAR > P2.13.
