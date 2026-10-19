# Oyun API taraması: hazır duran ama oyuncuya açılmamış sistemler

**Tarih:** 2026-10-19 · **Kaynak:** Bannerlord 1.4.8 referans DLL'leri (Core + SandBox)
**Kullanıcı isteği:** "Bir sürü API var, oyuna eklenebilecek ama eklememişler sanki."

## Nasıl tarandı, sınırı ne

Referans DLL'lerde yalnızca imzalar var: sınıf, metot ve parametre adları. Metotların içi yok. Bu yüzden aşağıdakiler "bu kapı var"
demek; nasıl çalıştığı ve hangi sahnenin gerektiği ancak oyunda denenerek kesinleşir. Her biri uygulanmadan önce küçük bir deneme
gerektirir.

## Bulgular

### Oynanabilir görevler (`SandBox.SandBoxMissions`)

Oyunun içinde bazı görevler var; oyun bunları yalnızca kendi görevlerinde ya da öğreticide açıyor. Mod bunları kendi olaylarından
açabilir.

| API | Oyunda ne için | Bizde ne olabilir |
|---|---|---|
| `OpenArenaDuelMission(scene, location, duelCharacter, civilian, horses, onDuelEnd(kazanan), health, levels)` | Arenada düello (görevlerde) | **Düello:** bir lorda meydan oku. Eşinin sevgilisi, kan davalın, sana hakaret eden. Kazanan döner: onur, ilişki, sır kapanır ya da açılır. |
| `OpenTownCenterShadowATargetMission(...)` | Bir hedefi şehirde gizlice takip etme | **Casusluğu kendin yap:** casusluk entrikasında "kendin takip et" seçeneği. Başarırsan aşama ya da rapor hemen gelir. |
| `OpenDisguiseMission(scene, willSetUpContact, from, levels)` | Kılık değiştirip düşman şehre girme | **Kılık değiştir:** düşman şehirde gölge ağı kurmak ya da çete lideriyle buluşmak için gizlice gir. |
| `OpenPrisonBreakMission(scene, location, prisoner)` | Zindandan yoldaş kurtarma | **Zindan baskını:** esir düşmüş ailenden birini kurtar ya da kaçırma entrikasını kendin bitir. |
| `OpenAlleyFightMission(..., playerTroops, rivalTroops)` | Ara sokak çatışması | **Çete savaşı:** gölge ağının olduğu şehirde rakip çeteyle sokak kavgası; kazanırsan ağ güçlenir. |
| `OpenMeetingMission(scene, character)`, `OpenConversationMission(..., specialScene, ...)` | Sahnede buluşma ve konuşma | **Ulak görüşmesi sahnede:** haritadaki konuşma penceresi yerine bir salonda görüşme. |

`StealthMissionController`, `StealthZone`, nöbetçiler ve "yakalanma sayacı" da var (1.3 ile gelen gizlilik). Saklanarak suikast ya da
kaçırma görevi bunun üzerine kurulabilir. En zor ve en riskli olanı bu, çünkü sahne hazırlığı gerekiyor.

### Oyunun kendi 3B sahne bildirimleri (`SceneInformationPopupTypes`)

`MBInformationManager.ShowSceneNotification(...)` ile gösteriliyor. Oyun bunları yalnızca kendi olaylarında açıyor.

| Sahne | Bizde |
|---|---|
| `NewBornSceneNotificationItem(baba, anne, zaman)` | Kurdan ya da sevgiliden doğan çocukta oyunun doğum sahnesi |
| `MarriageSceneNotificationItem(damat, gelin, zaman, bağlam)` | Aile editöründe evlendirince düğün sahnesi |
| `HeroExecutionSceneNotificationData.CreateForInformingPlayer(...)` | Suikast ya da idam haberlerinde |
| `HeirComingOfAge...`, `DeathOldAge...`, `BecomeKing...` | Varis seçimi (P2.8) ve taht entrikalarında |

### Aile ve hanedan (`TaleWorlds.CampaignSystem.Actions`)

| API | Bizde |
|---|---|
| `AdoptHeroAction.Apply(hero)` | **Evlat edinme / meşrulaştırma (P2.8):** sevgiliden doğan çocuğu ya da yetimi klana al |
| `ApplyHeirSelectionAction.ApplyByDeath / ByRetirement(heir)` | **Varis seçimi (P2.8):** oyun bunu yalnızca sen ölünce soruyor |
| `DisableHeroAction.Apply(hero)` | **Manastıra / inzivaya çekilme:** CK3'teki gibi aldatan eşi ya da tahta göz diken akrabayı oyundan çekmek |
| `ChangeRomanticStateAction` + `RomanceCampaignBehavior` | Oyunun kendi kur aşamaları; P2.6'yı oyunun kur ekranıyla bağlamak mümkün |
| `MarriageAction`, `MakePregnantAction` | Aile editörü (P2.7) |

### Diplomasi ve dünya

| API | Not |
|---|---|
| `StartAllianceDecision`, `TradeAgreementDecision`, `ProposeCallToWarAgreementDecision` | Oyunda var ve oyuncuya açık. Entrika bunlara bağlanabilir: ittifakı bozmak için söylenti yaymak. |
| `PoliticalStagnationAndBorderIncidentCampaignBehavior` | Oyunun "sınır olayları". P2.5 krizlerini bunlarla birleştirmek düşünülebilir. |
| `BribeGuardsAction`, `PayForCrimeAction` | Suç puanını rüşvetle silmek; P2.5 ile uyumlu. |
| `MapWeatherModel`, `ApplyWeatherEffectsModel` | Hava durumu savaşa etki ediyor. **Alevli ok** (War Sails) için: yağmurda sönmesi, gece savaşında ışık. |
| `ChangeShipOwnerAction`, `DestroyShipAction`, `RepairShipAction` | Gemi sistemi ana oyunda; War Sails paketi bunlarla da yapılabilir (gemi kaçırmak, kundaklamak). |

### Düşük öncelik

- Masa oyunları (`BoardGameCampaignBehavior`, Tablut, Seega...): esirle ya da lordla oyun oynayıp ilişki kazanmak. Eğlenceli ama
  entrikaya katkısı az.
- `EmissarySystemCampaignBehavior`, `RetirementCampaignBehavior`: oyunun kendi içinde kullanılıyor; eklenecek bir şey görmedim.

## Öneri sırası (mantık filtresinden)

1. **Düello:** en az risk, en çok etki. Aldatma, kan davası ve hakaret için gerçek bir çözüm.
2. **Oyunun 3B sahneleri:** doğum ve düğün. Ucuz, görsel olarak büyük fark.
3. **Evlat edinme ve varis:** P2.8'in temeli.
4. **Casusluğu kendin yap:** takip görevi ve kılık değiştirme.
5. **Zindan baskını** ve **çete savaşı.**
6. **Manastıra çekilme.**
7. **Gizlilikle suikast:** en zoru.

Her biri ayrı tasarım notu ve oyunda deneme ister. Görev açan API'lerde hangi sahne adının verileceği (şehir merkezi, arena,
zindan) oyunun kendi yerleşim verisinden okunacak.
