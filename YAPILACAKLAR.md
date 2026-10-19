# Yapılacaklar

Son güncelleme: 19 Ekim 2026 · Mod durumu: Faz 1, 2, 3A, 3B derlendi ve oyuna kuruldu.
Faz 1 ve 2 oyunda denendi ve çalışıyor; **3A ve 3B henüz denenmedi.**
**Kademeye göre asker ekleme** kodlandı; bilgisayarda `build.ps1` ile derlenip denenmeli.

> ⚠️ Telefondan kod yazılıp GitHub'a gönderilebilir, ama **derleyip oyuna kurmak bilgisayar ister**
> (Visual Studio + oyunun DLL'leri). Telefonda yapılanlar, bilgisayarda `build.ps1` çalışınca oyuna gelir.

## 1. Oyunda test et (bilgisayarda, yedekli bir kayıtta)

### Asker ekle (kademeye göre) — yeni
- [ ] Ordu / Parti → **Asker ekle (kademeye göre)** → kademeler (T1, T2 …) ve her birindeki asker türü sayısı görünüyor
- [ ] Bir kademeye gir → kendi kültürün dışındaki askerler, paralı askerler ve haydutlar da listede
- [ ] Bir asker seç → adet yaz → partiye ekleniyor, liste aynı kademeye geri dönüyor
- [ ] **Tüm kademeler** listesinde isimle arama çalışıyor
- [ ] Listede olmaması gereken tuhaf karakter (turnuva, görev, eğitim karakteri vb.) var mı? Varsa adını yaz, filtreyi daraltırım

### Faz 3A — Klan & krallık, Bilgi
- [ ] Bağımsızken bir krallığa **vasal olarak katıl**, sonra **krallığından ayrıl**
- [ ] **Paralı asker** olarak katıl → aynı krallığa vasal olarak geç
- [ ] **Kendi krallığını kur** (bağımsızken) → ansiklopedide "X Krallığı"
- [ ] Bir krallıktayken **hükümdar ol**
- [ ] **Klan kademesi** 5 yap → klan ekranında 5
- [ ] İki AI krallığı arasında **savaş ilan ettir**, sonra **barış yaptır**
- [ ] Bir AI klanını başka krallığa **taşı**; bir klanın **liderini** ve bir krallığın **hükümdarını** değiştir
- [ ] Bir AI krallığını **yok et**
- [ ] Bilgi → **Bir lordu bul** → kamera lorda kayıyor mu, konum doğru mu
- [ ] **Harita sisini kaldır** AÇIK → uzaktaki partiler görünüyor; kaydet-yükle → hâlâ açık; KAPALI → normal

### Faz 3B — Kurallar
- [ ] **Yetenek XP 5x** → bir savaş → yetenek çubukları belirgin hızlı doluyor
- [ ] **Asker XP 5x** → savaş sonrası terfiye hazır asker sayısı artıyor
- [ ] **Parti boyutu +100** → parti ekranında sınır +100
- [ ] **Öğrenme sınırını kaldır** → karakter ekranında öğrenme sınırı ~1000+ görünüyor ⚠️ *(görünmüyorsa söyle, en riskli kısım bu)*
- [ ] Oyunu kapat-aç → Kurallar ayarları duruyor
- [ ] Savaş içi → **(Ordu) + müttefikleri de kat** → ortak savaşta müttefik askerler de ölümsüz
- [ ] War Sails: gemideyken parti ekranı ve bir deniz savaşı normal

### Önceki fazlardan açık kalanlar
- [ ] Şehir adını değiştirince haritadaki yazı **hemen** değişiyor mu (yoksa kaydet-yükle sonrası mı)
- [ ] **Berber ekranı** haritadan açılınca düzgün mü
- [ ] Yaş değiştirdikten birkaç gün sonra yaş hâlâ doğru mu (Doğum ve Yaşlanma açıkken)

Bir şey bozuksa: ekrandaki kırmızı `[CheatMenu]` mesajını aynen yaz. Çökme olursa log:
`C:\ProgramData\Mount and Blade II Bannerlord\logs\` içindeki en yeni `rgl_log_*.txt`.

## 2. Sıradaki büyük iş — Faz 4

Rapor bunları v1.5 kararlı sürüme çıkana kadar bekletmeyi öneriyordu (v1.5 şu an beta). İstersen beklemeden başlarız.
- [ ] **Yoldaş / başka kahraman görünümü** (yüz editörü) — orta-zor
- [ ] **Kişiye özel yaş kilidi** (seçtiğin kişiler yaşlanmasın)
- [ ] **Zorunlu gizlilik görevlerini atlama** seçeneği
- [ ] **War Sails araçları** — gemi ve figürhed verme
- [ ] **Tek panelli editör merkezi** — popup yerine sekmeli tek ekran (en büyük iş)

## 3. Ertelenen küçük işler
- [ ] Savaş hilelerini **belirli klan/krallığa** uygulama (şu an: ben / ordum / müttefikler)
- [ ] **İttifak kurma** (önce oyunun ittifak API'si doğrulanmalı)
- [ ] **Tek perk silme** (şu an sadece ekleme var)
- [ ] **Toplu ilişki** değişikliği (ör. bir klanın tamamıyla)
- [ ] Askerlerin **günlük eğitim XP'si** çarpanı

## 4. Repo ile ilgili kararlar
- [ ] **Lisans** eklensin mi? (şu an yok = tüm hakları saklı; açık yapacaksan MIT önerilir)
- [ ] Repo **private** kalsın mı, herkese açık mı olsun?

## 5. v1.5 kararlı sürüm çıkınca
- [ ] Tüm fazları yeniden derle ve test et (oyun güncellemesi API'leri değiştirebilir)
- [ ] v1.5'in yerli olarak getirdiği özelliklerle çakışma var mı bak (başlangıç seçenekleri, klan partisi anahtarları)

## Nasıl devam edilir
Claude'a şunlardan birini yazman yeterli:
- "3A/3B testi: şunlar çalıştı, şunlar bozuk: …"
- "Faz 4'e geçelim" (ya da listeden tek bir madde)
- "Lisans ekle" / "Repoyu public yap"
