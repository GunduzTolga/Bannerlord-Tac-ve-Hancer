using System;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;

namespace CheatMenu.Bundle
{
    /// <summary>
    /// İçine paketlenen Diplomacy moduyla uyum. Diplomacy'nin türlerine adıyla ulaşılır: mod yoksa hiçbir şey yapmaz.
    /// </summary>
    internal static class DiplomacyCompat
    {
        private const string ManagerType = "Diplomacy.DiplomaticAction.DiplomaticAgreementManager";

        private static bool _resolved;
        private static MethodInfo _hasPact;

        /// <summary>Diplomacy yüklü ve saldırmazlık anlaşmalarına ulaşılabiliyor mu.</summary>
        internal static bool IsAvailable
        {
            get
            {
                Resolve();
                return _hasPact != null;
            }
        }

        /// <summary>
        /// İki krallık arasındaki saldırmazlık anlaşmasını bitirir; anlaşma vardıysa true.
        /// Diplomacy anlaşmayı savaş ilanında kendiliğinden bitirmez (yalnızca kendi arayüzünden savaş açmayı engeller):
        /// hile menüsü ya da entrika savaş açtıktan sonra anlaşma kâğıt üstünde sürer, yapay zekânın kararlarını etkilerdi.
        /// </summary>
        internal static bool EndNonAggressionPact(Kingdom a, Kingdom b)
        {
            if (a == null || b == null || a == b || !IsAvailable) return false;
            try
            {
                object[] args = { a, b, null };
                if (!(bool)_hasPact.Invoke(null, args) || args[2] == null) return false;
                MethodInfo expire = args[2].GetType().GetMethod("Expire", BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null);
                if (expire == null) return false;
                expire.Invoke(args[2], null);
                return true;
            }
            catch (Exception ex)
            {
                Debug.Print("[CheatMenu] Diplomacy pact could not be ended: " + ex);
                return false;
            }
        }

        private static void Resolve()
        {
            if (_resolved) return;
            _resolved = true;
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type manager = assembly.GetType(ManagerType, false);
                if (manager == null) continue;
                MethodInfo has = manager.GetMethod("HasNonAggressionPact", BindingFlags.Public | BindingFlags.Static);
                ParameterInfo[] p = has?.GetParameters();
                if (p != null && p.Length == 3 && p[0].ParameterType == typeof(Kingdom) && p[1].ParameterType == typeof(Kingdom)
                    && p[2].ParameterType.IsByRef && has.ReturnType == typeof(bool))
                    _hasPact = has;
                return;
            }
        }
    }
}
