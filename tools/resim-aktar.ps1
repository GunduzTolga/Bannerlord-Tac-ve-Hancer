# Olay karti resimlerini oyuna ve depoya aktarir.
#
# Kullanim: urettigin resimleri bir klasore koy (varsayilan: Masaustu\TVH Resimler), dosya adi listedeki ad olsun
# (docs/gorseller/olay-kartlari.md; or. tur_casusluk.png, sonuc_kur_2.png). Sonra:
#     powershell -ExecutionPolicy Bypass -File tools\resim-aktar.ps1
#
# Ne yapar:
#  - Adi duzeltir (buyuk harf, Turkce harf, bosluk, tire) ve listeyle karsilastirir; yanlis adda en yakin adi onerir.
#  - Resmi ortadan 16:9 kirpar, 1600x900'e kucultur, JPG (kalite 92) kaydeder.
#  - Oyunun Modules\CheatMenu\Images klasorune ve depodaki Module\Images klasorune koyar.
#    Ayni addaki eski .png/.jpeg silinir: oyun once .png'ye bakar, eskisi kalirsa yenisi gorunmez.
#  - Sonunda 49 kartin kacinin bittigini ve eksik oncelikli (*) kartlari yazar.
param(
    [string]$Kaynak = (Join-Path ([Environment]::GetFolderPath('Desktop')) 'TVH Resimler'),
    [string]$OyunKlasoru = 'C:\Program Files (x86)\Steam\steamapps\common\Mount & Blade II Bannerlord',
    [string]$DepoKlasoru = (Join-Path $PSScriptRoot '..\Module\Images'),
    [int]$Genislik = 1600,
    [int]$Yukseklik = 900,
    [switch]$DepoyaKopyalama
)
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing

# ---- Gecerli adlar: listedeki tablo satirlari "| 1 | `tur_casusluk.png` | ... | * |" ----
$listFile = Join-Path $PSScriptRoot '..\docs\gorseller\olay-kartlari.md'
$cards = @()
foreach ($line in Get-Content -LiteralPath $listFile -Encoding UTF8) {
    if ($line -match '^\|\s*\d+\s*\|\s*`([a-z0-9_]+)\.png`\s*\|.*\|\s*(\S*)\s*\|\s*$') {
        $cards += [pscustomobject]@{ Name = $Matches[1]; Star = ($Matches[2] -ne '') }
    }
}
if ($cards.Count -eq 0) { throw "Listede kart bulunamadi: $listFile" }
$baseNames = $cards | ForEach-Object { $_.Name }
$valid = New-Object System.Collections.Generic.HashSet[string]
foreach ($b in $baseNames) { [void]$valid.Add($b); foreach ($n in 2..5) { [void]$valid.Add("${b}_$n") } }

# Turkce harfler ve ayiricilar: "Sonuç Kur-2" -> "sonuc_kur_2"
$turkish = @{ [char]0x00E7 = 'c'; [char]0x011F = 'g'; [char]0x0131 = 'i'; [char]0x0130 = 'i'; [char]0x00F6 = 'o';
              [char]0x015F = 's'; [char]0x00FC = 'u'; [char]0x00C7 = 'c'; [char]0x011E = 'g'; [char]0x00D6 = 'o';
              [char]0x015E = 's'; [char]0x00DC = 'u' }
function Normalize([string]$name) {
    $sb = New-Object System.Text.StringBuilder
    foreach ($ch in $name.ToCharArray()) { if ($turkish.ContainsKey($ch)) { [void]$sb.Append($turkish[$ch]) } else { [void]$sb.Append($ch) } }
    $s = $sb.ToString().ToLowerInvariant() -replace '[\s\-\.]+', '_' -replace '[^a-z0-9_]', '' -replace '_+', '_'
    return $s.Trim('_')
}

# Levenshtein uzakligi (iki satirla): yanlis yazilan ada en yakin listedeki adi bulmak icin.
function Distance([string]$a, [string]$b) {
    $prev = 0..$b.Length
    for ($i = 1; $i -le $a.Length; $i++) {
        $cur = @($i) + (1..$b.Length | ForEach-Object { 0 })
        for ($j = 1; $j -le $b.Length; $j++) {
            $cost = if ($a[$i - 1] -eq $b[$j - 1]) { 0 } else { 1 }
            $up = $prev[$j] + 1
            $left = $cur[$j - 1] + 1
            $diag = $prev[$j - 1] + $cost
            $cur[$j] = [Math]::Min([Math]::Min($up, $left), $diag)
        }
        $prev = $cur
    }
    return $prev[$b.Length]
}

function Save-Card([string]$source, [string]$name, [string[]]$folders) {
    $image = [System.Drawing.Image]::FromFile($source)
    try {
        # Ortadan 16:9 kirp
        $target = $Genislik / $Yukseklik
        $w = $image.Width; $h = $image.Height
        if ($w / $h -gt $target) { $cw = [int][Math]::Round($h * $target); $ch = $h } else { $cw = $w; $ch = [int][Math]::Round($w / $target) }
        $crop = New-Object System.Drawing.Rectangle ([int](($w - $cw) / 2)), ([int](($h - $ch) / 2)), $cw, $ch

        $bitmap = New-Object System.Drawing.Bitmap $Genislik, $Yukseklik
        $g = [System.Drawing.Graphics]::FromImage($bitmap)
        $g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
        $g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::HighQuality
        $g.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
        $g.CompositingQuality = [System.Drawing.Drawing2D.CompositingQuality]::HighQuality
        $g.DrawImage($image, (New-Object System.Drawing.Rectangle 0, 0, $Genislik, $Yukseklik), $crop, [System.Drawing.GraphicsUnit]::Pixel)
        $g.Dispose()

        $codec = [System.Drawing.Imaging.ImageCodecInfo]::GetImageEncoders() | Where-Object { $_.MimeType -eq 'image/jpeg' }
        $params = New-Object System.Drawing.Imaging.EncoderParameters 1
        $params.Param[0] = New-Object System.Drawing.Imaging.EncoderParameter ([System.Drawing.Imaging.Encoder]::Quality), 92L
        foreach ($folder in $folders) {
            New-Item -ItemType Directory -Force -Path $folder | Out-Null
            foreach ($ext in '.png', '.jpeg') { $old = Join-Path $folder ($name + $ext); if (Test-Path -LiteralPath $old) { Remove-Item -LiteralPath $old } }
            $bitmap.Save((Join-Path $folder ($name + '.jpg')), $codec, $params)
        }
        $bitmap.Dispose()
        return "{0}x{1} -> {2}x{3}" -f $w, $h, $Genislik, $Yukseklik
    }
    finally { $image.Dispose() }
}

# ---- Hedefler ----
$gameImages = Join-Path $OyunKlasoru 'Modules\CheatMenu\Images'
$folders = @()
if (Test-Path -LiteralPath (Join-Path $OyunKlasoru 'Modules\CheatMenu')) { $folders += $gameImages } else { Write-Host "Oyun klasorunde mod yok ($OyunKlasoru); yalnizca depoya kopyalanacak." -ForegroundColor Yellow }
if (-not $DepoyaKopyalama) { $folders += [IO.Path]::GetFullPath($DepoKlasoru) }
if ($folders.Count -eq 0) { throw 'Kopyalanacak hedef yok.' }
if (-not (Test-Path -LiteralPath $Kaynak)) {
    New-Item -ItemType Directory -Path $Kaynak | Out-Null
    Write-Host "Kaynak klasor olusturuldu: $Kaynak  -> resimleri buraya koyup tekrar calistir." -ForegroundColor Yellow
}

# ---- Aktar ----
$done = 0; $skipped = 0
foreach ($file in Get-ChildItem -LiteralPath $Kaynak -File | Sort-Object Name) {
    $ext = $file.Extension.ToLowerInvariant()
    if ($ext -notin '.png', '.jpg', '.jpeg', '.bmp') {
        if ($ext -eq '.webp') { Write-Host "  ATLANDI  $($file.Name): WEBP okunamiyor, PNG ya da JPG olarak kaydet" -ForegroundColor Yellow; $skipped++ }
        continue
    }
    $name = Normalize ([IO.Path]::GetFileNameWithoutExtension($file.Name))
    if (-not $valid.Contains($name)) {
        $best = $valid | Sort-Object { Distance $name $_ } | Select-Object -First 2
        Write-Host "  ATLANDI  $($file.Name): listede yok. Bunu mu demek istedin: $($best -join ' / ')" -ForegroundColor Yellow
        $skipped++
        continue
    }
    try {
        $info = Save-Card $file.FullName $name $folders
        Write-Host "  TAMAM    $($file.Name) -> $name.jpg ($info)" -ForegroundColor Green
        $done++
    }
    catch {
        Write-Host "  HATA     $($file.Name): $($_.Exception.Message)" -ForegroundColor Red
        $skipped++
    }
}

# ---- Durum: oyundaki (yoksa depodaki) klasore gore ----
$check = $folders[0]
$have = @{}
if (Test-Path -LiteralPath $check) {
    foreach ($f in Get-ChildItem -LiteralPath $check -File) { $have[[IO.Path]::GetFileNameWithoutExtension($f.Name)] = $true }
}
$finished = @($cards | Where-Object { $have.ContainsKey($_.Name) })
$variants = @($have.Keys | Where-Object { $_ -match '_[2-5]$' -and $valid.Contains($_) })
$missingStar = @($cards | Where-Object { $_.Star -and -not $have.ContainsKey($_.Name) } | ForEach-Object { $_.Name })

Write-Host ''
Write-Host "Aktarilan: $done, atlanan: $skipped"
Write-Host ("Kartlar: {0}/{1} hazir, ek surum: {2}  ({3})" -f $finished.Count, $cards.Count, $variants.Count, $check)
if ($missingStar.Count -gt 0) { Write-Host ("Eksik oncelikli (*) kartlar ({0}): {1}" -f $missingStar.Count, ($missingStar -join ', ')) }
else { Write-Host 'Oncelikli kartlarin hepsi hazir.' -ForegroundColor Green }
