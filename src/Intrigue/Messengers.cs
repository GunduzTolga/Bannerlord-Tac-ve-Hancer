using System;
using System.Linq;
using CheatMenu.Intrigue.Core;
using CheatMenu.Menu;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;

namespace CheatMenu.Intrigue
{
    /// <summary>
    /// Haberciler (P2.2, tasarım: docs/superpowers/specs/2026-10-19-p2-haberci-saray.md). Uzaktaki lorda koz talebi ve
    /// taraf değiştirme daveti mektupla gider; mesafeye göre birkaç gün sonra varır ve o gün sonuçlanır.
    /// Yüz yüzeyken (aynı yerleşim ya da çok yakın) koz hemen kullanılır.
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
        private static string CannotSend(IntrigueState state, Hero target, int extraCost)
        {
            if (Hero.MainHero.IsPrisoner) return "Esirken haberci gönderemezsin";
            if (!DistanceTo(target).HasValue) return "Yeri belli değil: haberci onu bulamaz";
            if (PendingTo(state, target) != null) return "Ona giden bir haberci zaten yolda";
            int cost = LetterRules.MessengerCost(TravelDays(target)) + extraCost;
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
            string blocked = HookActions.Options(target).First(o => o.Use == use).Blocked ?? CannotSend(state, target, 0);
            if (state.ActiveHook(target.StringId, SchemeFactors.Today) == null) blocked = "Bu kişi üzerinde geçerli bir kozun yok";
            if (blocked != null) throw new InvalidOperationException(blocked);
            Send(state, target, LetterKind.HookDemand, (int)use, 0);
        }

        /// <summary>Koz düğmesinin etiketine eklenen not: "(haberciyle 3 gün, 125 dinar)" ya da "".</summary>
        public static string HookRouteText(Hero target, HookActions.Use use)
        {
            if (use == HookActions.Use.Release || IsFaceToFace(target)) return "";
            int days = TravelDays(target);
            return " (haberciyle " + days + " gün, " + SchemeFactors.Money(LetterRules.MessengerCost(days)) + ")";
        }

        // ---- Taraf değiştirme daveti ----

        /// <summary>null: davet gönderilebilir; değilse neden.</summary>
        public static string DefectionBlocked(IntrigueState state, Hero target)
        {
            if (target == null || !target.IsAlive) return "Hayatta değil";
            string rule = HookActions.DefectionBlocked(target);
            if (rule != null) return rule;
            if (Court.KnowledgeOf(state, target) == Court.Knowledge.Unknown)
                return "Önce onu tanı: casusluk, esir sorgusu ya da o krallıkta bir gölge ağı";
            int today = SchemeFactors.Today;
            if (state.DefectionUntil.TryGetValue(target.StringId, out int until) && today < until)
                return "Son davetini reddetti; " + (until - today) + " gün sonra yeniden dene";
            return CannotSend(state, target, LetterRules.DefectionGift(SchemeFactors.TargetTier(target)));
        }

        public static int DefectionChance(IntrigueState state, Hero target)
        {
            float ours = Clan.PlayerClan.Kingdom?.CurrentTotalStrength ?? 0f;
            float theirs = target.Clan?.Kingdom?.CurrentTotalStrength ?? 0f;
            float ratio = theirs > 0f ? ours / theirs : 2f;
            return LetterRules.DefectionChance(Court.RelationWithRuler(target), Hero.MainHero.GetRelation(target),
                target.GetTraitLevel(DefaultTraits.Honor), ratio, state.HasValidReport(target.StringId, SchemeFactors.Today),
                SettingsStore.Current.IntrigueDifficulty);
        }

        /// <summary>Olasılık metni: raporla tam, yalnızca söylentiyle yaklaşık (onluk aralık).</summary>
        public static string DefectionChanceText(IntrigueState state, Hero target)
        {
            int chance = DefectionChance(state, target);
            if (Court.KnowledgeOf(state, target) == Court.Knowledge.Exact) return "%" + chance;
            int low = chance / 10 * 10;
            return "yaklaşık %" + low + "-" + Math.Min(100, low + 10);
        }

        public static void SendDefection(IntrigueState state, Hero target)
        {
            string blocked = DefectionBlocked(state, target);
            if (blocked != null) throw new InvalidOperationException(blocked);
            Send(state, target, LetterKind.Defection, 0, LetterRules.DefectionGift(SchemeFactors.TargetTier(target)));
        }

        // ---- Gönderme ve varış ----

        private static void Send(IntrigueState state, Hero target, LetterKind kind, int use, int gift)
        {
            int days = TravelDays(target);
            SchemeFactors.Pay(LetterRules.MessengerCost(days) + gift);
            int today = SchemeFactors.Today;
            state.Letters.Add(new Letter
            {
                Id = state.NextLetterId++, Kind = kind, TargetId = target.StringId, Use = use, SentDay = today, ArriveDay = today + days,
            });
            string what = kind == LetterKind.Defection ? "davet mektubu" : "koz talebi";
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
            if (letter.Kind == LetterKind.HookDemand) ResolveHookDemand(state, letter, target);
            else ResolveDefection(state, target);
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

        private static void ResolveDefection(IntrigueState state, Hero target)
        {
            int today = SchemeFactors.Today;
            string rule = HookActions.DefectionBlocked(target);
            if (rule != null)
            {
                SchemeOutcomes.Notify(state, "Davet boşa gitti: " + target.Name,
                    "Mektubun vardığında teklif artık geçerli değildi: " + rule.ToLower(Tr.Culture) + ".", target, CardImages.Messenger);
                return;
            }
            Kingdom from = target.Clan.Kingdom;
            Hero ruler = from.Leader;
            if (CheatState.IntrigueAlwaysSucceed || state.Roll100() <= DefectionChance(state, target))
            {
                ChangeClanInfluenceAction.Apply(Clan.PlayerClan, -SchemeMath.DefectionInfluenceCost);
                ChangeKingdomAction.ApplyByJoinToKingdomByDefection(target.Clan, from, Clan.PlayerClan.Kingdom, default, true);
                SchemeOutcomes.Relate(target, LetterRules.DefectionRelationBonus);
                SchemeOutcomes.Notify(state, "Davet kabul edildi: " + target.Clan.Name,
                    target.Name + ", " + from.Name + " krallığını bıraktı; " + target.Clan.Name + " klanı artık " + Clan.PlayerClan.Kingdom.Name
                    + " saflarında.\n\nSonuç: " + SchemeMath.DefectionInfluenceCost + " etki harcandı, " + target.Name + " ile ilişki +"
                    + LetterRules.DefectionRelationBonus + ".", target, CardImages.Messenger);
                state.AddLog(today, "Davet kabul edildi: " + target.Clan.Name + " " + from.Name + " krallığından katıldı");
                return;
            }

            state.DefectionUntil[target.StringId] = today + LetterRules.DefectionCooldownDays;
            SchemeOutcomes.Relate(target, -LetterRules.RefusalRelationPenalty);
            string text = target.Name + " teklifini reddetti; hediyen geri gelmez. İlişki -" + LetterRules.RefusalRelationPenalty + ".";
            if (ruler != null && ruler != target && state.Roll100() <= LetterRules.ForwardChance(target.GetTraitLevel(DefaultTraits.Honor)))
            {
                SchemeOutcomes.Relate(ruler, -LetterRules.ForwardedRulerPenalty);
                text += "\n\nDahası mektubu hükümdarı " + Tr.Dative(ruler.Name.ToString()) + " götürdü: ihanete teşvikten haberi var. "
                    + ruler.Name + " ile ilişki -" + LetterRules.ForwardedRulerPenalty + ".";
            }
            SchemeOutcomes.Notify(state, "Davet reddedildi: " + target.Name, text, target, CardImages.Messenger);
            state.AddLog(today, "Davet reddedildi: " + target.Name);
        }
    }
}
