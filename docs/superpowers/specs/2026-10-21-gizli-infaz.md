# Esiri gizlice öldürtmek (gizli infaz)

21 Ekim 2026. Kullanıcı isteği: "Bazı lordları tutsak alıyorsun, idam edebiliyorsun ama çok fazla kaybı var. Bunu gizlice yapma
seçeneği olsun; casusluk becerilerine orantılı olsun."

## Neden

Oyunun açık idamı birçok lordla ilişkiyi birden düşürür. Ortaçağda bunun yolu belliydi: esir "hastalandı", "kaçarken öldü" ya da
"düştü". Bedeli para ve risk: gardiyanlar konuşabilir.

## Kurallar (`PrisonDeathRules`, saf, testli)

| Yöntem | Bedel | Şüphe tabanı | Oyunda ölüm |
|---|---|---|---|
| Zindan hastalığı | 1.500 | %20 | doğal ölüm (`ApplyByOldAge`) |
| Kaza | 1.000 | %28 | yaralardan (`ApplyByWounds`) |
| Kaçarken öldürüldü | 500 | %36 | yaralardan |

- **Şüphe** = taban − Roguery/8 (en fazla −25) + klan kademesi ×2 + hükümdarsa 15 + yeni yakalandıysa (7 günden az) 10 + partindeyse 5
  + zorluk (kolay −5, zor +5); %5–90. Roguery: seninki ya da casus başınınki, hangisi yüksekse.
- **İfşa**: şüphe doğarsa %(45 − Roguery/8), %10–45. Kartta ikisi de yazar.
- **Temiz**: kimse sormaz; yalnızca Merhamet düşer.
- **Şüphe**: ölenin klanındaki yetişkinlerle −25, krallığının hükümdarıyla −10; Merhamet −.
- **İfşa**: suikastin izi kadar ağır (`ApplyHeavyExposure`): klanıyla −50, krallığının lordlarıyla −20, krallığındaki onurlu lordlarla
  −10, suç puanı, Onur ve Merhamet −−, yakınları kan davası.
- "Entrikalar asla ortaya çıkmaz" hilesi açıksa hiç şüphe doğmaz. Doğum ve Ölüm ayarı o ölüm türünü kapatıyorsa seçenek gri.

## Nerede

- Esir kartı (Insert > Entrika > Esirleri sorgula ya da Klan > Entrika > Esirler > Esirle konuş) > **Kaderini belirle...** > Hizmetine
  davet et / **Gizlice öldürt...**
- Klan > Entrika > Esirler: her esirin kartında **Gizlice öldürt...** düğmesi (esir uzak bir zindandaysa da: gardiyanlar ayarlar).

## Resimler

Sonuç kartı suikast resimlerini kullanır: hastalık `sonuc_suikast_zehir`, kaza `sonuc_suikast_kaza`, firar `sonuc_suikast_hancer`.
