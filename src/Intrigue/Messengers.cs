using System;
using System.Linq;
using CheatMenu.Intrigue.Core;
using CheatMenu.Menu;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;

namespace CheatMenu.Intrigue
{
    /// <summary>
    /// Haberciler (P2.2, tasarım: docs/superpowers/specs/2026-10-19-p2-haberci-saray.md). Uzaktaki lorda koz talebi mektupla gider;
    /// mesafeye göre birkaç gün sonra varır ve o gün sonuçlanır. Yüz yüzeyken (aynı yerleşim ya da çok yakın) koz hemen kullanılır.
    /// Lordu kendi tarafına çekmek oyunun kendi ikna konuşmasıyla yapılır (bilgi ona bonus verir: IntriguePersuasionModel).
    /// </summary>
    public static class Messengers
    {
        // ---- Mesafe ----

        /// <summary>Senin partinle hedef arasındaki harita mesafesi; konumu belli değilse null.</summary>
        public static float? DistanceTo(Hero target)
        {
            MobileParty main = MobileParty.MainParty;
            if (main == null) return null;
            Settlement where = SchemeFactors.TargetSettlement(target);
            if (where != null) return main.Position.Distance(where.Position);
            if (target.PartyBelongedTo != null) return main.Position.Distance(target.PartyBelongedTo.Position);
            return null;
        }

        public static bool IsFaceToFace(Hero target)
        {
            Settlement mine = MobileParty.MainParty?.CurrentSettlement;
            if (mine != null && SchemeFactors.TargetSettlement(target) == mine) return true;
            float? distance = DistanceTo(target);
            return distance.HasValue && distance.Value <= LetterRules.FaceToFaceDistance;
        }

        public static int TravelDays(Hero target) => LetterRules.TravelDays(DistanceTo(target) ?? 0f);

        public static Letter PendingTo(IntrigueState state, Hero target) => state.Letters.FirstOrDefault(l => l.TargetId == target.StringId);

        /// <summary>Haberci gönderilemiyorsa neden (türden bağımsız).</summary>
        private static string CannotSend(IntrigueState state, Hero target)
        {
            if (Hero.MainHero.IsPrisoner) return "Esirken haberci gönderemezsin";
            if (!DistanceTo(target).HasValue) return "Yeri belli değil: haberci onu bulamaz";
            if (PendingTo(state, target) != null) return "Ona giden bir haberci zaten yolda";
            int cost = LetterRules.MessengerCost(TravelDays(target));
            return SchemeFactors.CanAfford(cost) ? null : "Yeterli altının yok (" + SchemeFactors.Money(cost) + ")";
        }

        // ---- Koz talebi ----

        /// <summary>Koz kullanımı: yüz yüzeyse hemen; değilse haberciyle (bırakma her zaman hemen: iyi niyet).</summary>
        public static void UseHook(IntrigueState state, Hero target, HookActions.Use use)
        {
            if (use == HookActions.Use.Release || IsFaceToFace(target))
            {
                HookActions.Apply(state, target, use);
                return;
            }
            string blocked = HookActions.Options(target).First(o => o.Use == use).Blocked ?? CannotSend(state, target);
            if (state.ActiveHook(target.StringId, SchemeFactors.Today) == null) blocked = "Bu kişi üzerinde geçerli bir kozun yok";
            if (blocked != null) throw new InvalidOperationException(blocked);
            Send(state, target, LetterKind.HookDemand, (int)use);
        }

        /// <summary>Koz düğmesinin etiketine eklenen not: "(haberciyle 3 gün, 125 dinar)" ya da "".</summary>
        public static string HookRouteText(Hero target, HookActions.Use use)
        {
            if (use == HookActions.Use.Release || IsFaceToFace(target)) return "";
            int days = TravelDays(target);
            return " (haberciyle " + days + " gün, " + SchemeFactors.Money(LetterRules.MessengerCost(days)) + ")";
        }

        // ---- Gönderme ve varış ----

        private static void Send(IntrigueState state, Hero target, LetterKind kind, int use)
        {
            int days = TravelDays(target);
            SchemeFactors.Pay(LetterRules.MessengerCost(days));
            int today = SchemeFactors.Today;
            state.Letters.Add(new Letter
            {
                Id = state.NextLetterId++, Kind = kind, TargetId = target.StringId, Use = use, SentDay = today, ArriveDay = today + days,
            });
            const string what = "koz talebi";
            state.AddLog(today, "Haberci yola çıktı (" + what + "): " + target.Name + ", " + days + " gün");
            MenuUI.Success("Haberci yola çıktı: " + target.Name + " (" + what + "), yaklaşık " + days + " gün");
        }

        /// <summary>Günlük: varan mektuplar sonuçlanır.</summary>
        public static void DailyTick()
        {
            IntrigueState state = IntrigueBehavior.Current?.State;
            if (state == null || state.Letters.Count == 0) return;
            int today = SchemeFactors.Today;
            foreach (Letter letter in state.Letters.Where(l => l.ArriveDay <= today).ToList())
            {
                state.Letters.Remove(letter);
                try
                {
                    Resolve(state, letter);
                }
                catch (Exception ex)
                {
                    TaleWorlds.Library.Debug.Print("[CheatMenu] letter " + letter.Id + " failed: " + ex);
                }
            }
        }

        private static void Resolve(IntrigueState state, Letter letter)
        {
            Hero target = Hero.Find(letter.TargetId);
            if (target == null || !target.IsAlive)
            {
                SchemeOutcomes.Notify(state, "Haberci geri döndü", "Mektubun sahibine ulaşamadı: " + (target?.Name.ToString() ?? "alıcı") + " artık hayatta değil.",
                    null, CardImages.Messenger);
                return;
            }
            ResolveHookDemand(state, letter, target);
        }

        private static void ResolveHookDemand(IntrigueState state, Letter letter, Hero target)
        {
            var use = (HookActions.Use)letter.Use;
            HookActions.Option option = HookActions.Options(target).FirstOrDefault(o => o.Use == use);
            string blocked = state.ActiveHook(target.StringId, SchemeFactors.Today) == null ? "kozun bu arada geçersiz oldu" : option?.Blocked;
            if (blocked != null)
            {
                SchemeOutcomes.Notify(state, "Mektup sonuçsuz: " + target.Name,
                    "Habercin " + target.Name + " ile buluştu ama talebin yerine getirilemedi: " + blocked.ToLower(Tr.Culture) + ". Kozun harcanmadı.",
                    target, CardImages.Messenger);
                return;
            }
            HookActions.Apply(state, target, use);
        }
    }
}
