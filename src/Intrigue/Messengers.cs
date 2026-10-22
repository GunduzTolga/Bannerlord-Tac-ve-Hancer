using System;
using System.Linq;
using CheatMenu.Intrigue.Core;
using CheatMenu.Menu;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;

namespace CheatMenu.Intrigue
{
    /// <summary>
    /// Haberciler (P2.2, tasarım: docs/superpowers/specs/2026-10-19-p2-haberci-saray.md). Uzaktaki lorda iki tür mektup gider:
    /// koz talebi ve görüşme daveti (isteğe bağlı hediyeyle). Mesafeye göre birkaç gün sonra varır. Görüşmeyi kabul eden lordla
    /// haritadan konuşma penceresi açılır: lordu bulmaya gitmeden oyunun kendi ikna konuşması (krallığa davet vb.) yapılır.
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

        // ---- Görüşme daveti (ulak) ----

        /// <summary>Ulağı kabul etmiş, görüşmeye hazır lord; yoksa null.</summary>
        public static Audience AudienceWith(IntrigueState state, Hero hero) =>
            state.Audiences.FirstOrDefault(a => a.HeroId == hero.StringId && a.Until > SchemeFactors.Today);

        /// <summary>null: bu lorda ulak gönderilebilir; değilse neden.</summary>
        public static string AudienceBlocked(IntrigueState state, Hero target)
        {
            if (target == null || !target.IsAlive) return "Hayatta değil";
            if (!target.IsLord || target.Clan == Clan.PlayerClan) return "Yalnızca başka klanların lordlarına";
            if (target.IsPrisoner) return "Esir: ulağı kabul edemez";
            if (AudienceWith(state, target) != null) return "Zaten seninle görüşmeye hazır: Görüş";
            return CannotSend(state, target);
        }

        /// <summary>Hediye bu lorda şimdi ilişki getirir mi (30 günde bir).</summary>
        public static bool GiftCounts(IntrigueState state, Hero target) =>
            !(state.GiftUntil.TryGetValue(target.StringId, out int until) && SchemeFactors.Today < until);

        public static void SendAudience(IntrigueState state, Hero target, int gift)
        {
            string blocked = AudienceBlocked(state, target);
            if (blocked == null && gift > 0 && !SchemeFactors.CanAfford(LetterRules.MessengerCost(TravelDays(target)) + gift))
                blocked = "Yeterli altının yok";
            if (blocked != null) throw new InvalidOperationException(blocked);
            SchemeFactors.Pay(gift);
            Send(state, target, LetterKind.Audience, gift);
        }

        /// <summary>Görüşmeye hazır lordla haritadan konuşma penceresini açar.</summary>
        public static void OpenAudience(IntrigueState state, Hero hero)
        {
            Audience audience = AudienceWith(state, hero) ?? throw new InvalidOperationException(hero.Name + " şu an görüşmeye hazır değil");
            if (!hero.IsAlive) throw new InvalidOperationException("Hayatta değil");
            if (hero.IsPrisoner) throw new InvalidOperationException(hero.Name + " şu an esir");
            if (Campaign.Current.ConversationManager?.IsConversationInProgress == true) throw new InvalidOperationException("Başka bir konuşma sürüyor");
            UI.IntrigueScreen.Close();
            // Konuşma haritanın üstünde açılır: Klan ekranındaysak önce o kapanır (düello ve takip gibi).
            UI.ClanIntrigueTab.WhenOnMap(() =>
            {
                if (!hero.IsAlive || hero.IsPrisoner || Campaign.Current.ConversationManager?.IsConversationInProgress == true)
                {
                    MenuUI.Error(hero.Name + " ile şu an görüşülemiyor");
                    return;
                }
                state.Audiences.Remove(audience);
                CampaignMapConversation.OpenConversation(
                    new ConversationCharacterData(CharacterObject.PlayerCharacter, PartyBase.MainParty),
                    new ConversationCharacterData(hero.CharacterObject, hero.PartyBelongedTo?.Party));
            });
        }

        private static void ResolveAudience(IntrigueState state, Letter letter, Hero target)
        {
            int gift = letter.Use;
            int today = SchemeFactors.Today;
            // Esirken ya da savaşın ortasındayken ulak bekler; çok uzarsa vazgeçer.
            if (target.IsPrisoner || SchemeFactors.InBattle(target))
            {
                if (today - letter.SentDay < LetterRules.MaxLetterDelayDays)
                {
                    letter.ArriveDay = today + 1;
                    state.Letters.Add(letter);
                    return;
                }
                Refund(gift);
                SchemeOutcomes.Notify(state, "Ulak geri döndü: " + target.Name,
                    "Ulağın " + target.Name + " ile görüşemedi" + (target.IsPrisoner ? " (esir)" : " (savaşta)") + "."
                    + (gift > 0 ? " Hediyen geri geldi." : ""), target, CardImages.Messenger);
                return;
            }
            if (Hero.MainHero.GetRelation(target) <= LetterRules.AudienceRefuseRelation)
            {
                Refund(gift);
                SchemeOutcomes.Notify(state, "Ulak geri çevrildi: " + target.Name,
                    target.Name + " seninle görüşmeyi reddetti; aranız buna fazla kötü." + (gift > 0 ? " Hediyeni de geri yolladı." : ""),
                    target, CardImages.Messenger);
                state.AddLog(today, "Ulak geri çevrildi: " + target.Name);
                return;
            }
            int relation = 0;
            if (gift > 0)
            {
                target.ChangeHeroGold(gift);
                if (GiftCounts(state, target))
                {
                    relation = LetterRules.GiftRelation(gift) + Diplomat.GiftBonus();
                    SchemeOutcomes.Relate(target, relation);
                    state.GiftUntil[target.StringId] = today + LetterRules.GiftCooldownDays;
                }
            }
            state.Audiences.RemoveAll(a => a.HeroId == target.StringId);
            if (Bundle.DiplomacyCompat.MessengerAvailable)
            {
                // Görüşme Diplomacy'nin habercisiyle yapılır; bizim ulak yalnızca hediyeyi götürdü.
                SchemeOutcomes.Notify(state, "Hediyen ulaştı: " + target.Name,
                    target.Name + " hediyeni kabul etti" + (relation > 0 ? ": ilişki +" + relation + "." : ".")
                    + " Görüşmek istersen Diplomacy habercisi gönder (ulak kartı ya da ansiklopedideki düğme).", target, CardImages.Messenger);
                state.AddLog(today, "Hediye ulaştı: " + target.Name + (gift > 0 ? " (" + SchemeFactors.Money(gift) + ")" : ""));
                return;
            }
            state.Audiences.Add(new Audience { HeroId = target.StringId, Until = today + LetterRules.AudienceWaitDays, Relation = relation });
            state.AddLog(today, "Ulak kabul edildi: " + target.Name + (gift > 0 ? " (hediye " + SchemeFactors.Money(gift) + ")" : ""));
        }

        private static void Refund(int gift)
        {
            if (gift > 0) Hero.MainHero.ChangeHeroGold(gift);
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
            string what = kind == LetterKind.Audience ? "görüşme daveti" : "koz talebi";
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
                if (letter.Kind == LetterKind.Audience) Refund(letter.Use);
                SchemeOutcomes.Notify(state, "Haberci geri döndü", "Mektubun sahibine ulaşamadı: " + (target?.Name.ToString() ?? "alıcı") + " artık hayatta değil."
                    + (letter.Kind == LetterKind.Audience && letter.Use > 0 ? " Hediyen geri geldi." : ""), null, CardImages.Messenger);
                return;
            }
            if (letter.Kind == LetterKind.Audience) ResolveAudience(state, letter, target);
            else ResolveHookDemand(state, letter, target);
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
