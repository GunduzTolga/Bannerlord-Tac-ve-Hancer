# Packs the mods listed in third_party\paketler.txt into the deployed Tac ve Hancer module, so the launcher shows one mod.
# Their files never go to GitHub: third_party\<mod>\ is git-ignored, only the list is committed.
# src\CheatMenu.csproj runs this after copying our own files. It can also run alone:
#   powershell -ExecutionPolicy Bypass -File tools\bundle.ps1 -ModuleOut "<game>\Modules\CheatMenu"
#
# Rules
# - Folders merge into the module root as they are (bin, GUI, ModuleData, AssetPackages, ...). Only
#   bin\Win64_Shipping_Client is taken: our own DLL exists only for that platform.
# - Files in a mod's root folder (settings, readme) go to Bundled\<mod>\ (see src\Bundle\BundleShim.cs).
# - ModuleData\Languages\...\language_data.xml files at the same path are merged into one.
# - Any other file that exists twice with different contents stops the build.
# - SubModule.xml: their submodules go before ours (after CheatMenu.Bundle), their Xmls and dependencies are added.
# - Files a previous run copied that are no longer bundled are deleted (list in Bundled\dosyalar.txt).
# This script stays ASCII: Windows PowerShell 5.1 reads BOM-less UTF-8 scripts as ANSI.
param(
    [Parameter(Mandatory = $true)][string]$ModuleOut,
    [string]$Root
)
$ErrorActionPreference = 'Stop'
# Windows PowerShell 5.1 leaves $PSScriptRoot empty in parameter defaults.
if (-not $Root) { $Root = Split-Path -Parent (Split-Path -Parent $MyInvocation.MyCommand.Definition) }

$thirdParty = Join-Path $Root 'third_party'
$listFile = Join-Path $thirdParty 'paketler.txt'
$ourModule = Join-Path $Root 'Module'
$manifestRel = 'Bundled\dosyalar.txt'
$mainSubModule = 'CheatMenu'
$utf8 = New-Object System.Text.UTF8Encoding($false)

function Get-Rel([string]$base, [string]$path) { $path.Substring($base.TrimEnd('\').Length + 1) }

function Save-Xml([xml]$doc, [string]$path) {
    $settings = New-Object System.Xml.XmlWriterSettings
    $settings.Indent = $true
    $settings.IndentChars = "`t"
    $settings.Encoding = $utf8
    $dir = Split-Path -Parent $path
    if (-not (Test-Path $dir)) { New-Item -ItemType Directory -Force $dir | Out-Null }
    $writer = [System.Xml.XmlWriter]::Create($path, $settings)
    try { $doc.Save($writer) } finally { $writer.Close() }
}

function Load-Xml([string]$path) {
    $doc = New-Object System.Xml.XmlDocument
    $doc.Load($path)
    # XML nodes are enumerable: without the comma PowerShell would return their children.
    return ,$doc
}

# --- Packages in load order
$packages = @()
if (Test-Path $listFile) {
    foreach ($line in [System.IO.File]::ReadAllLines($listFile)) {
        $name = $line.Trim()
        if (-not $name -or $name.StartsWith('#')) { continue }
        $dir = Join-Path $thirdParty $name
        if (-not (Test-Path (Join-Path $dir 'SubModule.xml'))) {
            Write-Host "bundle: $name not found in third_party, skipped"
            continue
        }
        $packages += [pscustomobject]@{ Name = $name; Dir = $dir; Xml = (Load-Xml (Join-Path $dir 'SubModule.xml')) }
    }
}

# --- Our own files: a bundled mod must never overwrite them.
$ours = @{}
foreach ($f in Get-ChildItem $ourModule -Recurse -File) { $ours[(Get-Rel $ourModule $f.FullName).ToLowerInvariant()] = $true }
$ours['bin\win64_shipping_client\cheatmenu.dll'] = $true
$ours['bin\win64_shipping_client\cheatmenu.pdb'] = $true

$written = @{}      # lower-case relative path -> relative path as written
$owner = @{}        # lower-case relative path -> package that wrote it
$languageData = @{} # lower-case relative path -> list of source files
$langPattern = '^ModuleData\\Languages\\(.+\\)?language_data\.xml$'
$copied = 0

foreach ($p in $packages) {
    foreach ($f in Get-ChildItem $p.Dir -Recurse -File) {
        $rel = Get-Rel $p.Dir $f.FullName
        if ($rel -eq 'SubModule.xml') { continue }
        if ($rel -like 'bin\*' -and $rel -notlike 'bin\Win64_Shipping_Client\*') { continue }
        if (-not $rel.Contains('\')) { $rel = "Bundled\$($p.Name)\$rel" }
        $key = $rel.ToLowerInvariant()

        if ($rel -match $langPattern) {
            if (-not $languageData.ContainsKey($key)) {
                $languageData[$key] = New-Object System.Collections.ArrayList
                if ($ours.ContainsKey($key)) { [void]$languageData[$key].Add((Join-Path $ourModule $rel)) }
            }
            [void]$languageData[$key].Add($f.FullName)
            $written[$key] = $rel
            continue
        }
        if ($ours.ContainsKey($key)) { throw "bundle: $($p.Name) has $rel, which is a Tac ve Hancer file" }
        if ($owner.ContainsKey($key)) {
            $first = Join-Path $thirdParty "$($owner[$key])\$rel"
            if ((Get-FileHash $first).Hash -ne (Get-FileHash $f.FullName).Hash) {
                throw "bundle: $rel exists in both $($owner[$key]) and $($p.Name) with different contents"
            }
            continue
        }

        $dest = Join-Path $ModuleOut $rel
        $existing = Get-Item $dest -ErrorAction SilentlyContinue
        if (-not $existing -or $existing.Length -ne $f.Length -or $existing.LastWriteTimeUtc -ne $f.LastWriteTimeUtc) {
            $destDir = Split-Path -Parent $dest
            if (-not (Test-Path $destDir)) { New-Item -ItemType Directory -Force $destDir | Out-Null }
            Copy-Item $f.FullName $dest -Force
            $copied++
        }
        $written[$key] = $rel
        $owner[$key] = $p.Name
    }
}

# --- language_data.xml: one file per path, with every mod's LanguageFile lines.
foreach ($key in $languageData.Keys) {
    $sources = $languageData[$key]
    $doc = Load-Xml $sources[0]
    $langRoot = $doc.DocumentElement
    $paths = @{}
    foreach ($n in $langRoot.SelectNodes('LanguageFile')) { $paths[$n.GetAttribute('xml_path').ToLowerInvariant()] = $true }
    for ($i = 1; $i -lt $sources.Count; $i++) {
        $other = (Load-Xml $sources[$i]).DocumentElement
        if ($other.GetAttribute('id') -ne $langRoot.GetAttribute('id')) {
            throw "bundle: $($written[$key]) has different language ids: '$($langRoot.GetAttribute('id'))' and '$($other.GetAttribute('id'))'"
        }
        foreach ($a in $other.Attributes) { if (-not $langRoot.HasAttribute($a.Name)) { [void]$langRoot.SetAttributeNode($doc.ImportNode($a, $true)) } }
        foreach ($n in $other.SelectNodes('LanguageFile')) {
            $path = $n.GetAttribute('xml_path').ToLowerInvariant()
            if ($paths.ContainsKey($path)) { continue }
            $paths[$path] = $true
            [void]$langRoot.AppendChild($doc.ImportNode($n, $true))
        }
    }
    Save-Xml $doc (Join-Path $ModuleOut $written[$key])
}

# --- SubModule.xml
$module = Load-Xml (Join-Path $ourModule 'SubModule.xml')
$m = $module.DocumentElement
$bundledIds = @{}
foreach ($p in $packages) { $bundledIds[$p.Xml.DocumentElement.SelectSingleNode('Id').GetAttribute('value')] = $true }

function Get-Section([string]$name) {
    $node = $m.SelectSingleNode($name)
    if (-not $node) { $node = $m.AppendChild($module.CreateElement($name)) }
    return ,$node
}

function Add-Dependencies([System.Xml.XmlElement]$from, [string]$section, [string]$item, [string]$idAttribute) {
    $target = Get-Section $section
    $have = @{}
    foreach ($n in $target.SelectNodes($item)) { $have[$n.GetAttribute($idAttribute)] = $true }
    foreach ($n in $from.SelectNodes("$section/$item")) {
        $id = $n.GetAttribute($idAttribute)
        if ($bundledIds.ContainsKey($id) -or $have.ContainsKey($id)) { continue }
        $have[$id] = $true
        [void]$target.AppendChild($module.ImportNode($n, $true))
    }
}

$subModules = Get-Section 'SubModules'
$ourMain = $null
foreach ($n in $subModules.SelectNodes('SubModule')) { if ($n.SelectSingleNode('Name').GetAttribute('value') -eq $mainSubModule) { $ourMain = $n } }
if (-not $ourMain) { throw "bundle: SubModule '$mainSubModule' not found in Module\SubModule.xml" }
$names = @{}
foreach ($n in $subModules.SelectNodes('SubModule')) { $names[$n.SelectSingleNode('Name').GetAttribute('value')] = 'Tac ve Hancer' }
$xmls = Get-Section 'Xmls'

foreach ($p in $packages) {
    $x = $p.Xml.DocumentElement
    Add-Dependencies $x 'DependedModules' 'DependedModule' 'Id'
    Add-Dependencies $x 'DependedModuleMetadatas' 'DependedModuleMetadata' 'id'
    [void]$subModules.InsertBefore($module.CreateComment(" Paketlenen mod: $($p.Name) "), $ourMain)
    foreach ($n in $x.SelectNodes('SubModules/SubModule')) {
        # Two BUTR "Bannerlord Module Loader" entries in one module would load only the first one's DLLs.
        $name = $n.SelectSingleNode('Name').GetAttribute('value')
        if ($names.ContainsKey($name)) { throw "bundle: submodule name '$name' is in both $($names[$name]) and $($p.Name)" }
        $names[$name] = $p.Name
        [void]$subModules.InsertBefore($module.ImportNode($n, $true), $ourMain)
    }
    foreach ($n in $x.SelectNodes('Xmls/XmlNode')) { [void]$xmls.AppendChild($module.ImportNode($n, $true)) }
}
Save-Xml $module (Join-Path $ModuleOut 'SubModule.xml')

# --- Remove what an earlier run copied and is no longer bundled.
$manifest = Join-Path $ModuleOut $manifestRel
$removed = 0
if (Test-Path $manifest) {
    foreach ($rel in [System.IO.File]::ReadAllLines($manifest)) {
        $key = $rel.ToLowerInvariant()
        if (-not $rel -or $written.ContainsKey($key) -or $ours.ContainsKey($key) -or ($rel.Split([char]92) -contains '..')) { continue }
        $path = Join-Path $ModuleOut $rel
        if (Test-Path $path -PathType Leaf) { Remove-Item $path -Force; $removed++ }
        $dir = Split-Path -Parent $path
        while ($dir.Length -gt $ModuleOut.TrimEnd('\').Length -and (Test-Path $dir) -and -not (Get-ChildItem $dir -Force)) {
            Remove-Item $dir -Force
            $dir = Split-Path -Parent $dir
        }
    }
}
if ($written.Count -gt 0) {
    $dir = Split-Path -Parent $manifest
    if (-not (Test-Path $dir)) { New-Item -ItemType Directory -Force $dir | Out-Null }
    [System.IO.File]::WriteAllLines($manifest, [string[]]($written.Values | Sort-Object), $utf8)
} elseif (Test-Path $manifest) {
    Remove-Item $manifest -Force
    $dir = Split-Path -Parent $manifest
    if (-not (Get-ChildItem $dir -Force)) { Remove-Item $dir -Force }
}

$list = ($packages | ForEach-Object { $_.Name }) -join ', '
if (-not $list) { $list = 'none' }
Write-Host "bundle: $list ($($written.Count) files, $copied copied, $removed removed)"
