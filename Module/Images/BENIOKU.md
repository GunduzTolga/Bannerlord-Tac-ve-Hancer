# Olay kartı resimleri

Bu klasöre konan resimler entrika olay kartlarında gösterilir. Oyunun sprite paketine çevirmek gerekmez:
dosyayı buraya koy, oyunu yeniden başlatmadan bile bir sonraki kartta görünür.

- **En kolay yol:** ürettiğin resimleri `Masaüstü\TVH Resimler` klasörüne koy (adları listedeki gibi) ve depoda
  `powershell -ExecutionPolicy Bypass -File tools\resim-aktar.ps1` çalıştır. Betik adı düzeltir, 16:9'a kırpıp 1600×900 JPG yapar,
  hem oyuna hem buraya koyar ve kaç kartın bittiğini yazar.
- Biçim: PNG ya da JPG, 16:9 (önerilen 1600×900). Aynı adla hem .png hem .jpg varsa oyun .png'yi gösterir.
- Ad: listedeki dosya adı (küçük harf, Türkçe karakter yok), örn. `olay_rusvetci_hizmetkar.png`.
- Liste ve prompt'lar: depodaki `docs/gorseller/olay-kartlari.md`.
- Aynı karta birden çok resim koyabilirsin: `ad_2.png` … `ad_5.png`. Oyun her seferinde var olanlardan rastgele birini gösterir.
- Resim yoksa kartta ilgili kişinin portresi görünür; hiçbir şey bozulmaz.
