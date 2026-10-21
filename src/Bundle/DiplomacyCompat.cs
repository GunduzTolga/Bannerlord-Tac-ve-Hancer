using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
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

        /// <summary>
        /// İki krallık arasında, bedelsiz ve soru sormadan, verilen gün kadar saldırmazlık anlaşması kurar (rehine anlaşması süresince).
        /// Zaten bir anlaşma varsa ya da Diplomacy yoksa false.
        /// </summary>
        internal static bool FormNonAggressionPact(Kingdom a, Kingdom b, float days)
        {
            if (a == null || b == null || a == b || !IsAvailable) return false;
            try
            {
                object[] check = { a, b, null };
                if ((bool)_hasPact.Invoke(null, check)) return false;
                Type action = AccessTools.TypeByName("Diplomacy.DiplomaticAction.NonAggressionPact.FormNonAggressionPactAction");
                MethodInfo apply = action?.GetMethod("Apply", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy,
                    null, new[] { typeof(Kingdom), typeof(Kingdom), typeof(bool), typeof(bool), typeof(float?), typeof(bool) }, null);
                if (apply == null) return false;
                // forcePlayerCharacterCosts: false, bypassCosts: true, süre, queryPlayer: false
                apply.Invoke(null, new object[] { a, b, false, true, (float?)days, false });
                return true;
            }
            catch (Exception ex)
            {
                Debug.Print("[CheatMenu] Diplomacy pact could not be formed: " + ex);
                return false;
            }
        }

        /// <summary>Diplomacy'nin bir krallıktaki isyancı grubu (iç savaş): kim, ne istiyor, ne kadar güçlü.</summary>
        internal sealed class RebelFactionInfo
        {
            public string Name;
            public Clan Sponsor;
            public List<Clan> Clans = new List<Clan>();
            /// <summary>"ayrılık" ya da "tahttan indirme".</summary>
            public string Demand;
            public float StrengthRatio;
            public float RequiredRatio;
            public bool AtWar;
        }

        /// <summary>Krallıktaki isyancı gruplar; Diplomacy yoksa ya da okunamazsa boş liste.</summary>
        internal static List<RebelFactionInfo> RebelFactions(Kingdom kingdom)
        {
            var list = new List<RebelFactionInfo>();
            if (kingdom == null) return list;
            try
            {
                Type manager = AccessTools.TypeByName("Diplomacy.CivilWar.RebelFactionManager");
                MethodInfo get = manager?.GetMethod("GetRebelFaction", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(Kingdom) }, null);
                if (!(get?.Invoke(null, new object[] { kingdom }) is IEnumerable factions)) return list;
                foreach (object faction in factions)
                {
                    Traverse t = Traverse.Create(faction);
                    var info = new RebelFactionInfo
                    {
                        Name = t.Property("Name").GetValue()?.ToString() ?? "",
                        Sponsor = t.Property("SponsorClan").GetValue() as Clan,
                        Demand = t.Property("RebelDemandType").GetValue()?.ToString() == "Abdication" ? "tahttan indirme" : "ayrılık",
                        StrengthRatio = t.Property("StrengthRatio").GetValue() is float ratio ? ratio : 0f,
                        RequiredRatio = t.Property("RequiredStrengthRatio").GetValue() is float required ? required : 0f,
                        AtWar = t.Property("AtWar").GetValue() is bool atWar && atWar,
                    };
                    if (t.Property("Clans").GetValue() is IEnumerable clans) info.Clans.AddRange(clans.OfType<Clan>());
                    list.Add(info);
                }
            }
            catch (Exception ex)
            {
                Debug.Print("[CheatMenu] Diplomacy rebel factions could not be read: " + ex);
            }
            return list;
        }

        // ---- İsyanlar (Entrika ekranı > Savaş ve barış > İsyanlar): Diplomacy'nin grupları ve kendi düğme mantığı ----

        /// <summary>Krallıktaki isyancı grupların kendileri (Diplomacy.CivilWar.Factions.RebelFaction); yoksa boş.</summary>
        internal static List<object> RawRebelFactions(Kingdom kingdom)
        {
            var list = new List<object>();
            if (kingdom == null) return list;
            try
            {
                MethodInfo get = AccessTools.TypeByName("Diplomacy.CivilWar.RebelFactionManager")
                    ?.GetMethod("GetRebelFaction", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(Kingdom) }, null);
                if (get?.Invoke(null, new object[] { kingdom }) is IEnumerable factions) list.AddRange(factions.Cast<object>());
            }
            catch (Exception ex)
            {
                Debug.Print("[CheatMenu] Diplomacy rebel factions could not be read: " + ex);
            }
            return list;
        }

        /// <summary>
        /// Diplomacy'nin bir grup için kendi ViewModel'i (Diplomacy.ViewModel.RebelFactionItemVM): katıl, ayrıl, isyanı başlat ve destek topla
        /// onun kurallarıyla çalışsın. onComplete: pencere kapanmalıysa (ansiklopediye gidiş); refresh: grup değişti.
        /// </summary>
        internal static object CreateFactionItem(object faction, Action onComplete, Action refresh)
        {
            Type type = AccessTools.TypeByName("Diplomacy.ViewModel.RebelFactionItemVM");
            return type == null || faction == null ? null : Activator.CreateInstance(type, faction, onComplete, refresh);
        }

        /// <summary>Krallığın grup penceresi ViewModel'i (Diplomacy.ViewModel.RebelFactionsVM): grup kurma kuralı ve seçimi onun.</summary>
        internal static object CreateFactionsWindow(Kingdom kingdom, Action onComplete)
        {
            Type type = AccessTools.TypeByName("Diplomacy.ViewModel.RebelFactionsVM");
            return type == null || kingdom == null ? null : Activator.CreateInstance(type, kingdom, onComplete);
        }

        /// <summary>Bir nesnenin özelliği (Diplomacy'nin iç türleri için); okunamazsa varsayılan.</summary>
        internal static T Read<T>(object target, string property)
        {
            if (target == null) return default;
            try
            {
                return Traverse.Create(target).Property(property).GetValue() is T value ? value : default;
            }
            catch
            {
                return default;
            }
        }

        /// <summary>Diplomacy ViewModel'inin bir komutunu çalıştırır (OnJoin, OnLeave...).</summary>
        internal static void Invoke(object target, string method)
        {
            MethodInfo info = target?.GetType().GetMethod(method, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null);
            if (info == null) throw new InvalidOperationException("Diplomacy komutu bulunamadı: " + method);
            info.Invoke(target, null);
        }

        /// <summary>null: oyuncunun klanı bu gruba katılabilir; değilse Diplomacy'nin verdiği neden.</summary>
        internal static string JoinBlocked(object faction)
        {
            try
            {
                MethodInfo can = AccessTools.TypeByName("Diplomacy.CivilWar.Actions.JoinFactionAction")?.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                    .FirstOrDefault(m => m.Name == "CanApply" && m.GetParameters().Length == 3);
                if (can == null) return "Diplomacy kuralı bulunamadı";
                object[] args = { Clan.PlayerClan, faction, null };
                return (bool)can.Invoke(null, args) ? null : args[2]?.ToString() ?? "Katılamazsın";
            }
            catch (Exception ex)
            {
                Debug.Print("[CheatMenu] Diplomacy join check failed: " + ex);
                return "Diplomacy kuralına ulaşılamadı";
            }
        }

        /// <summary>
        /// Diplomacy'nin Savaş Raporu ViewModel'i (Diplomacy.ViewModel.DetailWarVM): bizim penceremizin üst kısmı (sancaklar, savaş
        /// yorgunluğu, ilişki, tarih) olduğu gibi onunla çizilir (UI.WarReportScreen). onComplete: onun kapatma komutu.
        /// </summary>
        internal static ViewModel CreateWarDetail(Kingdom enemy, Action onComplete)
        {
            Type type = AccessTools.TypeByName("Diplomacy.ViewModel.DetailWarVM");
            return type == null || enemy == null ? null : Activator.CreateInstance(type, enemy, onComplete) as ViewModel;
        }

        /// <summary>Savaş yorgunluğu dökümünün bir satırı: olay türü, iki tarafın sayısı ve yüzdesi, olayların ipucu (BasicTooltipViewModel).</summary>
        internal sealed class WarBreakdown
        {
            public string Text, OursPercent, TheirsPercent;
            public int Ours, Theirs;
            public object OursHint, TheirsHint;
        }

        /// <summary>DetailWarVM.Breakdowns: kayıplar, yağmalanan köy ve kervanlar, kaybedilen yerler, esir ve ölen lordlar, savaşın süresi.</summary>
        internal static List<WarBreakdown> WarBreakdowns(object detailWarVm)
        {
            var list = new List<WarBreakdown>();
            if (!(Read<object>(detailWarVm, "Breakdowns") is IEnumerable rows)) return list;
            foreach (object row in rows)
                list.Add(new WarBreakdown
                {
                    Text = Read<string>(row, "Text") ?? "",
                    Ours = Read<int>(row, "ValueFaction1"),
                    Theirs = Read<int>(row, "ValueFaction2"),
                    OursPercent = Read<string>(row, "WarExhaustionValueFaction1") ?? "",
                    TheirsPercent = Read<string>(row, "WarExhaustionValueFaction2") ?? "",
                    OursHint = Read<object>(row, "Faction1ValueHint"),
                    TheirsHint = Read<object>(row, "Faction2ValueHint"),
                });
            return list;
        }

        /// <summary>Diplomacy'nin savaş yorgunluğu (0-100): a'nın b ile savaşındaki yorgunluğu; okunamazsa -1.</summary>
        internal static float WarExhaustion(Kingdom a, Kingdom b)
        {
            try
            {
                object manager = AccessTools.Property("Diplomacy.WarExhaustion.WarExhaustionManager:Instance")?.GetValue(null);
                MethodInfo get = manager?.GetType().GetMethod("GetWarExhaustion", new[] { typeof(Kingdom), typeof(Kingdom) });
                return get != null && a != null && b != null ? (float)get.Invoke(manager, new object[] { a, b }) : -1f;
            }
            catch (Exception ex)
            {
                Debug.Print("[CheatMenu] Diplomacy war exhaustion failed: " + ex);
                return -1f;
            }
        }

        /// <summary>
        /// Barış Masası'nın uyduğu Diplomacy barış koşulları: savaşın en kısa süresi, görev bağı, oyuncunun kuşatması, isyancı krallık.
        /// (Bedel ve yetki koşulları sayılmaz: bedel masadaki şartlar, yetkiyi masa kendisi denetler.)
        /// </summary>
        private static readonly string[] TreatyConditions =
        {
            "Diplomacy.DiplomaticAction.WarPeace.Conditions.HasEnoughTimeElapsedForPeaceCondition",
            "Diplomacy.DiplomaticAction.WarPeace.Conditions.SatisfiesQuestConditionsForPeaceCondition",
            "Diplomacy.DiplomaticAction.WarPeace.Conditions.NoPlayerSiegeCondition",
            "Diplomacy.DiplomaticAction.GenericConditions.NotRebelKingdomCondition",
        };

        /// <summary>null: Diplomacy'nin bu koşulları barışa izin veriyor (ya da Diplomacy yok); değilse ilk engelin metni.</summary>
        internal static string TreatyBlocked(Kingdom kingdom, Kingdom enemy)
        {
            foreach (string typeName in TreatyConditions)
            {
                try
                {
                    Type type = AccessTools.TypeByName(typeName);
                    MethodInfo apply = type?.GetMethod("ApplyCondition", BindingFlags.Public | BindingFlags.Instance);
                    if (apply == null || apply.GetParameters().Length != 5) continue;
                    object condition = Activator.CreateInstance(type, true);
                    object[] args = { kingdom, enemy, null, false, false };
                    if (!(bool)apply.Invoke(condition, args)) return args[2]?.ToString() ?? "Diplomacy barışa izin vermiyor";
                }
                catch (Exception ex)
                {
                    Debug.Print("[CheatMenu] Diplomacy peace condition " + typeName + " failed: " + ex);
                }
            }
            return null;
        }

        // ---- Krallık sekmesi: Diplomacy'nin altın bağışı, fief verme ve fief iadesi (kuralları ve ilişki hesapları onun) ----

        private static MethodInfo Static(string type, string method, int parameters) =>
            AccessTools.TypeByName(type)?.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                .FirstOrDefault(m => m.Name == method && m.GetParameters().Length == parameters);

        /// <summary>
        /// Klana altın bağışlar: Diplomacy'nin bağış penceresinin kendi işlemi (altın klanın komutanlarına paylaşılır, liderle ilişki,
        /// Cömert/Hesapçı izi). Bağışlandıysa true.
        /// </summary>
        internal static bool DonateGold(Clan clan, int amount)
        {
            try
            {
                Type type = AccessTools.TypeByName("Diplomacy.ViewModel.DonateGoldVM");
                if (type == null || clan == null || amount <= 0) return false;
                object vm = Activator.CreateInstance(type, clan, (Action)(() => { }));
                Traverse.Create(vm).Property("IntValue").SetValue(amount);
                Invoke(vm, "ExecutePropose");
                (vm as ViewModel)?.OnFinalize();
                return true;
            }
            catch (Exception ex)
            {
                Debug.Print("[CheatMenu] Diplomacy gold donation failed: " + ex);
                return false;
            }
        }

        /// <summary>null: bu klana fief verilebilir; değilse Diplomacy'nin nedeni (kendi klanın, hükümdar değilsin, paralı, fiefin yok).</summary>
        internal static string GrantFiefBlocked(Clan clan)
        {
            try
            {
                MethodInfo can = Static("Diplomacy.Actions.GrantFiefAction", "CanGrantFief", 2);
                if (can == null) return "Diplomacy'nin toprak verme kuralı bulunamadı";
                object[] args = { clan, null };
                return (bool)can.Invoke(null, args) ? null : args[1] as string ?? "Verilemez";
            }
            catch (Exception ex)
            {
                Debug.Print("[CheatMenu] Diplomacy grant fief check failed: " + ex);
                return "Diplomacy'nin kuralına ulaşılamadı";
            }
        }

        /// <summary>Fief verilirse alanın liderleyle ilişki artışı (Diplomacy'nin önizlemesi); okunamazsa -1.</summary>
        internal static int GrantFiefPreview(Settlement settlement, Hero hero)
        {
            try
            {
                return Static("Diplomacy.Actions.GrantFiefAction", "PreviewPositiveRelationChange", 2)?.Invoke(null, new object[] { settlement, hero }) is int value ? value : -1;
            }
            catch
            {
                return -1;
            }
        }

        /// <summary>Fiefi klana verir (Diplomacy: ilişki, daha az fiefi olan klanların kıskançlığı, iz).</summary>
        internal static void GrantFief(Settlement settlement, Clan clan)
        {
            MethodInfo apply = Static("Diplomacy.Actions.GrantFiefAction", "Apply", 2)
                ?? throw new InvalidOperationException("Diplomacy'nin toprak verme işlemi bulunamadı");
            apply.Invoke(null, new object[] { settlement, clan });
        }

        /// <summary>Bu krallığa iade edilebilecek, ondan alınmış yerlerin (Diplomacy'nin kuralıyla); Diplomacy yoksa boş.</summary>
        internal static List<Settlement> ReturnableFiefs(Kingdom kingdom)
        {
            try
            {
                return (Static("Diplomacy.Actions.ReturnFiefAction", "GetReturnableFiefs", 1)?.Invoke(null, new object[] { kingdom }) as IEnumerable)?
                    .OfType<Settlement>().ToList() ?? new List<Settlement>();
            }
            catch (Exception ex)
            {
                Debug.Print("[CheatMenu] Diplomacy returnable fiefs failed: " + ex);
                return new List<Settlement>();
            }
        }

        /// <summary>İade edilirse eski sahibiyle ilişki artışı (Diplomacy'nin önizlemesi); okunamazsa -1.</summary>
        internal static int ReturnFiefPreview(Settlement settlement, Kingdom kingdom)
        {
            try
            {
                return Static("Diplomacy.Actions.ReturnFiefAction", "PreviewRelationChange", 2)?.Invoke(null, new object[] { settlement, kingdom }) is int value ? value : -1;
            }
            catch
            {
                return -1;
            }
        }

        /// <summary>Fiefi alındığı krallığa iade eder (Diplomacy: eski sahibine, ilişki, o krallığın klanlarının onayı, yayılmacılık azalır).</summary>
        internal static void ReturnFief(Settlement settlement, Kingdom kingdom)
        {
            MethodInfo apply = Static("Diplomacy.Actions.ReturnFiefAction", "Apply", 2)
                ?? throw new InvalidOperationException("Diplomacy'nin iade işlemi bulunamadı");
            apply.Invoke(null, new object[] { settlement, kingdom });
        }

        // ---- Haberci: yüz yüze görüşme Diplomacy'nin habercisiyle (onun bedeli, yolu ve salondaki konuşması) ----

        private static object MessengerCost(Hero hero) =>
            AccessTools.Method("Diplomacy.Costs.DiplomacyCostCalculator:DetermineCostForSendingMessenger", new[] { typeof(Hero) })?.Invoke(null, new object[] { hero });

        /// <summary>Diplomacy'nin habercisi kullanılabiliyor mu (mod yüklü ve kampanyada olay nesnesi var).</summary>
        internal static bool MessengerAvailable
        {
            get
            {
                try
                {
                    return AccessTools.Property("Diplomacy.Events.DiplomacyEvents:Instance")?.GetValue(null) != null
                        && AccessTools.TypeByName("Diplomacy.Messengers.MessengerManager") != null;
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>Habercinin bedeli (altın); okunamazsa -1.</summary>
        internal static int MessengerGold(Hero hero)
        {
            try
            {
                object cost = MessengerCost(hero);
                return cost != null ? (int)Math.Round(Traverse.Create(cost).Property("Value").GetValue<float>()) : -1;
            }
            catch
            {
                return -1;
            }
        }

        /// <summary>Diplomacy'nin habercisi varış saati (yaklaşık); okunamazsa -1.</summary>
        internal static int MessengerHours(Hero hero)
        {
            try
            {
                return AccessTools.Method("Diplomacy.Messengers.MessengerManager:GetHourToArrive", new[] { typeof(Hero) })?.Invoke(null, new object[] { hero }) is int hours
                    ? hours : -1;
            }
            catch
            {
                return -1;
            }
        }

        /// <summary>null: bu kişiye Diplomacy habercisi gönderilebilir (bedel dahil); değilse Diplomacy'nin verdiği neden.</summary>
        internal static string MessengerBlocked(Hero hero)
        {
            try
            {
                object cost = MessengerCost(hero);
                MethodInfo can = AccessTools.TypeByName("Diplomacy.Messengers.MessengerManager")?.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                    .FirstOrDefault(m => m.Name == "CanSendMessengerWithCost" && m.GetParameters().Length == 3);
                if (cost == null || can == null) return "Diplomacy habercisi bulunamadı";
                object[] args = { hero, cost, null };
                return (bool)can.Invoke(null, args) ? null : args[2]?.ToString() ?? "Gönderilemez";
            }
            catch (Exception ex)
            {
                Debug.Print("[CheatMenu] Diplomacy messenger check failed: " + ex);
                return "Diplomacy habercisine ulaşılamadı";
            }
        }

        /// <summary>Diplomacy'nin habercisini gönderir (ansiklopedideki düğmeyle aynı); gönderildiyse true.</summary>
        internal static bool SendMessenger(Hero hero)
        {
            if (MessengerBlocked(hero) != null) return false;
            try
            {
                object events = AccessTools.Property("Diplomacy.Events.DiplomacyEvents:Instance")?.GetValue(null);
                MethodInfo sent = events?.GetType().GetMethod("OnMessengerSent", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(Hero) }, null);
                if (sent == null) return false;
                sent.Invoke(events, new object[] { hero });
                return true;
            }
            catch (Exception ex)
            {
                Debug.Print("[CheatMenu] Diplomacy messenger could not be sent: " + ex);
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
