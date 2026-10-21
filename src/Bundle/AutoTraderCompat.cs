using System;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.Library;

namespace CheatMenu.Bundle
{
    /// <summary>
    /// İçine paketlenen AutoTrader'ın ayarları (AutoTrader.AutoTraderConfig'in statik özellikleri; dosyası Configs\AutoTraderConfig.xml).
    /// Saray > Ticaret sekmesi bunları okuyup yazar, her değişiklikte AutoTrader'ın kendi Save'i çağrılır. Mod yoksa hiçbir şey yapmaz.
    /// </summary>
    internal static class AutoTraderCompat
    {
        private static Type _config;
        private static bool _resolved;

        private static Type Config
        {
            get
            {
                if (!_resolved)
                {
                    _resolved = true;
                    _config = AccessTools.TypeByName("AutoTrader.AutoTraderConfig");
                }
                return _config;
            }
        }

        internal static bool IsAvailable => Config != null;

        private static PropertyInfo Property(string name) =>
            Config?.GetProperty(name, BindingFlags.Public | BindingFlags.Static) ?? throw new InvalidOperationException("AutoTrader ayarı bulunamadı: " + name);

        /// <summary>Sayı ya da aç/kapa ayarını sayı olarak okur (açık 1, kapalı 0); okunamazsa 0.</summary>
        internal static int Get(string name)
        {
            try
            {
                object value = Property(name).GetValue(null);
                return value is bool b ? (b ? 1 : 0) : Convert.ToInt32(value);
            }
            catch (Exception ex)
            {
                Debug.Print("[CheatMenu] AutoTrader setting " + name + " could not be read: " + ex.Message);
                return 0;
            }
        }

        /// <summary>Ayarı yazar (aç/kapa ayarında sıfırdan farklı her değer açık); dosyaya Save ile.</summary>
        internal static void Set(string name, int value)
        {
            PropertyInfo property = Property(name);
            property.SetValue(null, property.PropertyType == typeof(bool) ? (object)(value != 0) : value);
        }

        /// <summary>AutoTrader'ın kendi kaydı (Configs\AutoTraderConfig.xml).</summary>
        internal static void Save()
        {
            MethodInfo save = Config?.GetMethod("Save", BindingFlags.Public | BindingFlags.Static, null, Type.EmptyTypes, null)
                ?? throw new InvalidOperationException("AutoTrader'ın kayıt işlemi bulunamadı");
            save.Invoke(null, null);
        }

        /// <summary>War Sails (deniz DLC'si) var mı: varsa "filo kapasitesini kullan" ayarı anlamlı.</summary>
        internal static bool FleetAvailable
        {
            get
            {
                try
                {
                    return AccessTools.Method("AutoTrader.Warsails.WarsailsDetector:IsWarsailsDLCAvailable")?.Invoke(null, null) is bool available && available;
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}
