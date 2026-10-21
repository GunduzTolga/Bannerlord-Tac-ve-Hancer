# Divan: diplomat

21 Ekim 2026. Kullanıcı seçimi ("Divan'a diplomat"). Divan'ın beşinci koltuğu; Charm'a dayanır.

## Ne yapar (`Diplomat`, kurallar `DiplomatRules`, testli)

Sarayda olmalı (esir, rehine, vali, parti lideri ya da genelevde değil). Katkıları, diplomat sarayda değilken sıfır:

| Nerede | Katkı |
|---|---|
| Barış Masası | pazarlık gücüne Charm/20 (en fazla +15); masada "diplomatın (+N)" yazar |
| Elçi | kaybeden krallığın elçisinin haftalık gelme olasılığına Charm/30 (en fazla +%10; elçi hiç gelemiyorsa eklenmez) |
| Lordlar | yanına çağrılan ve ziyafete davet edilen lordun gelme olasılığına Charm/25 (en fazla +%12; savaştaki gelmez) |
| Ulak ve hediye | hediyenin ilişkisine Charm 100'de +1, 200'de +2 |

## Haftalık

- **İlişkileri geliştir görevi:** seçtiğin lordla (hükümdarlar ve klan liderleri önce) her hafta 1 + Charm/75 (en fazla 4) ilişki;
  ilişki 60'a varınca görev biter, bildirim gelir. Diplomat 50 Charm deneyimi kazanır.
- **Savaş uyarısı:** barışta olduğun her krallık için oyunun savaş ilanı puanı (`DiplomacyModel.GetScoreOfDeclaringWar`, o krallığın
  hükümdar klanının gözüyle): 0'ın üstü "savaşa hazırlanıyor", 25.000'e kadar altı "savaş konuşuluyor", daha altı "barış yanlısı".
  Diplomat %40 - Charm/6 (en az %5) olasılıkla bir basamak yanılır (haftalık sabit). "Hazırlanıyor" görünen krallık için %30 + Charm/4
  olasılıkla uyarı kartı (aynı krallık için 14 günde bir), oyunun verdiği nedenle.

## Nerede

- Entrika ekranı > Divan: beşinci kart (kartlar 248 piksele indi; "Görevden al" düğmesi "Azlet" oldu). Raporda **DİPLOMASİ**: görev
  satırı ("İlişki görevi..." / "Görevi bitir"), her krallığın tehdidi ve hükümdarıyla ilişkin ("İlişki görevi" düğmesi), savaştıkların
  için barış satırı (pazarlık gücün, şartsız barış olasılığı, **Barış Masası** düğmesi).
- Insert > Entrika > Divan > **Diplomat**: ata/değiştir, görev ver, görevi bitir, azlet; raporu.
- Kayıt: `diplomat`, `diptarget`, `diplomatwarn` kayıtları. Varis değişince koltuk ve görev boşalır.

## PC'de doğrulanacak

`GetScoreOfDeclaringWar`'ın ölçeği: "konuşuluyor" eşiği (`DiplomatRules.WarTalkMargin` = 25.000) tahmin; oyunda raporda bütün krallıklar
hep aynı basamakta görünürse eşik ayarlanır.
