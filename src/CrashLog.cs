using System;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Threading;

namespace CheatMenu
{
    /// <summary>
    /// Oyun çökerse tam yığın izini (hangi kodun neyi çağırdığını) dosyaya yazar: oyunun günlüğü ve Windows yalnızca en üstteki
    /// metodu kaydediyor, bu da suçluyu bulmaya yetmiyor. Dosya: Belgeler\Mount and Blade II Bannerlord\Configs\TacVeHancer\
    /// cokme_YYYYMMDD.txt. Yalnızca dosya işlemi yapar; oyunun hiçbir API'sine dokunmaz, kendisi asla istisna fırlatmaz.
    /// </summary>
    public static class CrashLog
    {
        // Oyun bazı istisnaları kendi yakalayıp yeniden fırlatıyor, yığın o zaman kaybolur. Fırlatıldığı anda yığını not alırız;
        // yalnızca NullReferenceException ve oturum başına en fazla bu kadar (normal oyunda nadir, dosya şişmesin).
        private const int MaxFirstChanceNotes = 20;
        private const long MaxFileBytes = 2 * 1024 * 1024;

        private static readonly object FileLock = new object();
        private static int _firstChanceNotes;
        private static bool _installed;
        [ThreadStatic] private static bool _writing;

        public static string Folder => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "Mount and Blade II Bannerlord", "Configs", "TacVeHancer");

        /// <summary>Modun ilk satırında: ondan sonraki her çökme kayda geçer.</summary>
        public static void Install()
        {
            if (_installed) return;
            _installed = true;
            AppDomain.CurrentDomain.UnhandledException += OnUnhandled;
            AppDomain.CurrentDomain.FirstChanceException += OnFirstChance;
        }

        private static void OnUnhandled(object sender, UnhandledExceptionEventArgs e) =>
            Write("ÇÖKME: yakalanmayan istisna (oyun kapanıyor)", e.ExceptionObject?.ToString() ?? "(istisna nesnesi yok)");

        private static void OnFirstChance(object sender, FirstChanceExceptionEventArgs e)
        {
            if (_writing || !(e.Exception is NullReferenceException)) return;
            if (Interlocked.Increment(ref _firstChanceNotes) > MaxFirstChanceNotes) return;
            // Environment.StackTrace: istisnanın fırlatıldığı anki yığının tamamı (henüz hiçbir catch onu kesmedi).
            Write("NullReferenceException fırlatıldı (yakalanmış da olabilir)", e.Exception.Message + Environment.NewLine + Environment.StackTrace);
        }

        private static void Write(string title, string body)
        {
            if (_writing) return;
            _writing = true;
            try
            {
                lock (FileLock)
                {
                    Directory.CreateDirectory(Folder);
                    string path = Path.Combine(Folder, "cokme_" + DateTime.Now.ToString("yyyyMMdd") + ".txt");
                    if (File.Exists(path) && new FileInfo(path).Length > MaxFileBytes) File.Delete(path);
                    File.AppendAllText(path, "==== " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "  " + title + Environment.NewLine
                        + body + Environment.NewLine + Environment.NewLine);
                }
            }
            catch
            {
                // Günlük yazılamıyorsa yapacak bir şey yok; çökme kaydı oyunu ayrıca bozmamalı.
            }
            finally
            {
                _writing = false;
            }
        }
    }
}
