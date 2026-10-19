using System;
using System.IO;

namespace CheatMenu
{
    /// <summary>
    /// Ayarları Belgeler\Mount and Blade II Bannerlord\Configs\CheatMenu.txt dosyasında tutar.
    /// Kayıt (save) dosyalarına hiçbir şey yazmaz; dosya okunamazsa varsayılanlarla devam eder.
    /// Çalışma zamanındaki tek kaynak budur; MCM ekranı (Mcm.TacVeHancerSettings) buna ayna tutar.
    /// </summary>
    public static class SettingsStore
    {
        private static ModSettings _current;

        /// <summary>Her başarılı kayıttan sonra: MCM ekranının değerleri güncellensin.</summary>
        public static event Action Saved;

        public static ModSettings Current => _current ?? (_current = Load());

        private static string FilePath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "Mount and Blade II Bannerlord", "Configs", "CheatMenu.txt");

        /// <returns>Yazma başarılıysa true.</returns>
        public static bool Save()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
                File.WriteAllText(FilePath, Current.Format());
                Saved?.Invoke();
                return true;
            }
            catch (Exception ex)
            {
                TaleWorlds.Library.Debug.Print("[CheatMenu] settings save failed: " + ex);
                return false;
            }
        }

        private static ModSettings Load()
        {
            try
            {
                return File.Exists(FilePath) ? ModSettings.Parse(File.ReadAllText(FilePath)) : new ModSettings();
            }
            catch (Exception ex)
            {
                TaleWorlds.Library.Debug.Print("[CheatMenu] settings load failed: " + ex);
                return new ModSettings();
            }
        }
    }
}
