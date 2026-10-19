using System;
using System.Collections.Generic;
using System.Linq;
using CheatMenu.Intrigue.Core;
using TaleWorlds.CampaignSystem;

namespace CheatMenu.Intrigue
{
    /// <summary>
    /// Gizli ilişki ve gayrimeşru çocuk sırları (P2.4, tasarım: docs/superpowers/specs/2026-10-19-p2-gizli-iliski.md). Sahnesiz, yalnızca
    /// metin. İki taraf da yetişkin olmak zorunda (SecretRules.CanBeLovers ve CouldBeParent). Bir sır iki tarafta da kayıtlıdır: ikisine de
    /// şantaj yapılabilir; ifşa ya da aldatılan eşe söylemek ikisini birden kullanır. Oyuncunun klanına ve aile ağacına dokunulmaz.
    /// </summary>
    public static class RomanceSecrets
    {
        public static bool Enabled => SettingsStore.Current.RomanceSecrets;

        public static int Age(Hero h) => (int)Math.Floor(h.Age);

        // ---- Bulma ----

        /// <summary>
        /// Gizli ilişki için sevgili adayları: yetişkin, inandırıcı yaşta, karşı cinsten (oyunun kendi evlilik sistemi gibi), akraba
        /// olmayan, oyuncunun klanından olmayan lordlar. Önce aynı krallıktan. Sıralı: tohumlu seçim kayıttan kayda aynı kalsın.
        /// </summary>
        public static List<Hero> LoverCandidates(Hero target)
        {
            if (target == null || target.IsChild || !SecretRules.IsAdult(Age(target))) return new List<Hero>();
            List<Hero> all = Hero.AllAliveHeroes
                .Where(h => h != target && h != target.Spouse && h != Hero.MainHero && h.IsLord && !h.IsChild && !h.IsDisabled
                    && h.IsFemale != target.IsFemale && h.Clan != null && h.Clan != Clan.PlayerClan
                    && SecretRules.CanBeLovers(Age(target), Age(h)) && !Kin(target, h))
                .OrderBy(h => h.StringId, StringComparer.Ordinal)
                .ToList();
            Kingdom kingdom = target.Clan?.Kingdom;
            List<Hero> near = kingdom != null ? all.Where(h => h.Clan.Kingdom == kingdom).ToList() : all.Where(h => h.Culture == target.Culture).ToList();
            return near.Count > 0 ? near : all;
        }

        /// <summary>Bu çiftin gayrimeşru çocuğu olabilecek çocuklar: annenin, resmî babası başkası olan; iki ebeveyn de gebelikte 18+.</summary>
        public static List<Hero> BastardCandidates(Hero a, Hero b)
        {
            if (a == null || b == null || a.IsFemale == b.IsFemale) return new List<Hero>();
            Hero mother = a.IsFemale ? a : b;
            Hero father = a.IsFemale ? b : a;
            return mother.Children
                .Where(c => c != null && c.IsAlive && c.Mother == mother && c.Father != null && c.Father != father && c.Father != Hero.MainHero
                    && c.Clan != Clan.PlayerClan
                    && SecretRules.CouldBeParent(Age(mother), Age(c)) && SecretRules.CouldBeParent(Age(father), Age(c)))
                .OrderBy(c => c.StringId, StringComparer.Ordinal)
                .ToList();
        }

        /// <summary>Hedefin bilinen (kayıtlı, hayattaki) sevgilisi; yoksa null.</summary>
        public static Hero KnownLover(IntrigueState state, Hero target) =>
            state.Secrets.Where(s => s.TargetId == target.StringId && s.Kind == SecretKind.Affair && s.OtherId != null)
                .Select(s => Hero.Find(s.OtherId))
                .FirstOrDefault(h => h != null && h.IsAlive);

        /// <summary>FindSecret'in romantik adayları. Zarlar çağıranın (tohumlu).</summary>
        public static void Consider(IntrigueState state, Hero target, HashSet<SecretKind> known, Func<int> roll100, List<SecretKind> candidates)
        {
            if (!Enabled || target.IsChild || !SecretRules.IsAdult(Age(target))) return;
            if (target.Spouse != null && !known.Contains(SecretKind.Affair) && roll100() <= SecretRules.AffairChance
                && LoverCandidates(target).Count > 0)
                candidates.Add(SecretKind.Affair);
            Hero lover = KnownLover(state, target);
            if (lover != null && !known.Contains(SecretKind.Bastard) && BastardCandidates(target, lover).Count > 0
                && roll100() <= SecretRules.BastardChance)
                candidates.Add(SecretKind.Bastard);
        }

        /// <summary>Seçilen romantik sırrın kişilerini doldurur (sevgili, çocuk).</summary>
        public static void Fill(IntrigueState state, Secret secret, Hero target, Func<int, int> pick)
        {
            if (secret.Kind == SecretKind.Affair)
            {
                List<Hero> lovers = LoverCandidates(target);
                secret.OtherId = lovers[pick(lovers.Count)].StringId;
            }
            else if (secret.Kind == SecretKind.Bastard)
            {
                Hero lover = KnownLover(state, target);
                List<Hero> children = BastardCandidates(target, lover);
                secret.OtherId = lover.StringId;
                secret.ChildId = children[pick(children.Count)].StringId;
            }
        }

        /// <summary>Sırrı kaydeder; romantikse öbür tarafa da (ikisine de şantaj yapılabilsin).</summary>
        public static void Record(IntrigueState state, Secret secret)
        {
            state.Secrets.Add(secret);
            if (!SecretRules.IsRomantic(secret.Kind) || secret.OtherId == null) return;
            if (state.Secrets.Any(s => s.TargetId == secret.OtherId && s.Kind == secret.Kind && s.OtherId == secret.TargetId && s.ChildId == secret.ChildId))
                return;
            state.Secrets.Add(new Secret { TargetId = secret.OtherId, Kind = secret.Kind, Day = secret.Day, OtherId = secret.TargetId, ChildId = secret.ChildId });
        }

        /// <summary>Aynı ilişkiye bağlı kayıtlar: iki taraf; gayrimeşru çocukta aynı çiftin gizli ilişkisi de.</summary>
        public static List<Secret> Linked(IntrigueState state, Secret secret)
        {
            if (!SecretRules.IsRomantic(secret.Kind) || secret.OtherId == null) return new List<Secret> { secret };
            bool SamePair(Secret s) =>
                (s.TargetId == secret.TargetId && s.OtherId == secret.OtherId) || (s.TargetId == secret.OtherId && s.OtherId == secret.TargetId);
            return state.Secrets
                .Where(s => s == secret || (SamePair(s) && (s.Kind == secret.Kind
                    ? s.ChildId == secret.ChildId
                    : s.Kind == SecretKind.Affair && secret.Kind == SecretKind.Bastard)))
                .ToList();
        }

        /// <summary>İki taraftan birine şantaj sürüyorsa sır harcanamaz.</summary>
        public static bool BlackmailRunning(IntrigueState state, Secret secret) =>
            state.FindScheme(secret.TargetId)?.Kind == SchemeKind.Blackmail
            || (secret.OtherId != null && state.FindScheme(secret.OtherId)?.Kind == SchemeKind.Blackmail);

        // ---- Metin ----

        /// <summary>Sırrın isimli açıklaması (kişi dosyası, kartlar, menü).</summary>
        public static string Describe(Secret secret)
        {
            if (!SecretRules.IsRomantic(secret.Kind) || secret.OtherId == null) return SecretRules.Description(secret.Kind);
            Hero target = Hero.Find(secret.TargetId);
            string other = Hero.Find(secret.OtherId)?.Name.ToString() ?? "biri";
            if (secret.Kind == SecretKind.Affair)
            {
                Hero spouse = target?.Spouse;
                return spouse != null
                    ? spouse.Name + " ile evli olduğu hâlde " + other + " ile gizli bir ilişkisi var."
                    : "Evli biriyle, " + other + " ile gizli bir ilişkisi var.";
            }
            Hero child = Hero.Find(secret.ChildId ?? "");
            if (target == null || child == null) return SecretRules.Description(secret.Kind);
            string official = child.Father?.Name.ToString() ?? "resmî babası";
            if (target.IsFemale)
                return (child.IsFemale ? "Kızı " : "Oğlu ") + Tr.Genitive(child.Name.ToString()) + " gerçek babası " + other
                    + "; resmî babası " + official + " bunu bilmiyor.";
            return Tr.Genitive(other) + " çocuğu " + child.Name + " aslında ondan; resmî babası " + official + " bunu bilmiyor.";
        }

        /// <summary>İfşa onayında sonuçların özeti.</summary>
        public static string ExposePreview(Secret secret)
        {
            if (!SecretRules.IsRomantic(secret.Kind) || secret.OtherId == null)
                return "İfşa edilirse hükümdarıyla (gizli ilişkide eşiyle) arası açılır.";
            return secret.Kind == SecretKind.Affair
                ? "İfşa edilirse saray skandalı: aldatılan eş(ler)le -" + SecretRules.ExposurePenalty(SecretKind.Affair) + ", kayınlarla -"
                  + SecretRules.InLawPenalty + "; aldatılan eş sevgiliyi de affetmez (-" + SecretRules.LoverResentment + ")."
                : "İfşa edilirse büyük skandal: resmî baba iki ebeveynle de -" + SecretRules.ExposurePenalty(SecretKind.Bastard)
                  + ", gerçek babanın eşiyle -" + SecretRules.BastardSpousePenalty + ", kayınla -" + SecretRules.InLawPenalty
                  + ". Gizli ilişki de ortaya çıkar. Çocuğa ve aile ağacına dokunulmaz.";
        }

        // ---- İfşa ----

        /// <summary>Herkese ifşanın ilişki etkileri; uygulananların listesi.</summary>
        public static List<string> ExposeEffects(Secret secret)
        {
            var parts = new List<string>();
            Hero a = Hero.Find(secret.TargetId);
            Hero b = Hero.Find(secret.OtherId);
            if (secret.Kind == SecretKind.Affair)
            {
                Betrayal(a, b, SecretRules.ExposurePenalty(SecretKind.Affair), parts);
                Betrayal(b, a, SecretRules.ExposurePenalty(SecretKind.Affair), parts);
                return parts;
            }
            if (a == null || b == null) return parts;
            Hero mother = a.IsFemale ? a : b;
            Hero father = a.IsFemale ? b : a;
            Hero official = Hero.Find(secret.ChildId ?? "")?.Father;
            int penalty = SecretRules.ExposurePenalty(SecretKind.Bastard);
            if (Alive(official))
            {
                Between(mother, official, -penalty, parts);
                Between(father, official, -penalty, parts);
                Hero inLaw = official.Clan?.Leader;
                if (Alive(inLaw) && inLaw != official && inLaw != mother && inLaw != father) Between(mother, inLaw, -SecretRules.InLawPenalty, parts);
            }
            if (Alive(father) && Alive(father.Spouse) && father.Spouse != mother) Between(father, father.Spouse, -SecretRules.BastardSpousePenalty, parts);
            return parts;
        }

        /// <summary>Aldatan evliyse: eşiyle -ceza, eşin klan lideriyle -15; aldatılan eş de sevgiliyle -15.</summary>
        private static void Betrayal(Hero cheater, Hero lover, int penalty, List<string> parts)
        {
            Hero spouse = cheater?.Spouse;
            if (!Alive(cheater) || !Alive(spouse) || spouse == lover) return;
            Between(cheater, spouse, -penalty, parts);
            if (Alive(lover)) Between(spouse, lover, -SecretRules.LoverResentment, parts);
            Hero inLaw = spouse.Clan?.Leader;
            if (Alive(inLaw) && inLaw != spouse && inLaw != cheater && inLaw != lover) Between(cheater, inLaw, -SecretRules.InLawPenalty, parts);
        }

        // ---- Aldatılan eşe söylemek ----

        /// <summary>Aldatılan eş: gizli ilişkide evli tarafın eşi; gayrimeşru çocukta resmî baba. Yoksa null.</summary>
        public static Hero Betrayed(Secret secret)
        {
            if (!SecretRules.IsRomantic(secret.Kind) || secret.OtherId == null) return null;
            Hero a = Hero.Find(secret.TargetId);
            Hero b = Hero.Find(secret.OtherId);
            Hero who = secret.Kind == SecretKind.Bastard
                ? Hero.Find(secret.ChildId ?? "")?.Father
                : a?.Spouse != null && a.Spouse != b ? a.Spouse : b?.Spouse != null && b.Spouse != a ? b.Spouse : null;
            return Alive(who) && who != Hero.MainHero ? who : null;
        }

        public static string WhisperBlocked(IntrigueState state, Secret secret)
        {
            if (!SecretRules.IsRomantic(secret.Kind) || secret.OtherId == null) return "Bu sırda söylenecek bir eş yok";
            if (secret.Exposed) return secret.Whispered ? "Zaten eşine söylendi" : "Zaten ifşa edildi";
            if (Betrayed(secret) == null) return "Aldatılan eş hayatta değil";
            if (BlackmailRunning(state, secret)) return "Şantaj sürerken olmaz";
            return null;
        }

        /// <summary>
        /// Skandal yerine özel bilgi: aldatılan eş aldatanla ve sevgiliyle küser, sana minnet duyar; kayınlar ve saray duymaz.
        /// Sır kullanılmış olur (bağlı kayıtlar da).
        /// </summary>
        public static void Whisper(IntrigueState state, Secret secret)
        {
            string blocked = WhisperBlocked(state, secret);
            if (blocked != null) throw new InvalidOperationException(blocked);
            Hero betrayed = Betrayed(secret);
            Hero a = Hero.Find(secret.TargetId);
            Hero b = Hero.Find(secret.OtherId);
            Hero cheater = secret.Kind == SecretKind.Bastard ? (a.IsFemale ? a : b) : a?.Spouse == betrayed ? a : b;
            Hero lover = cheater == a ? b : a;
            int penalty = SecretRules.ExposurePenalty(secret.Kind);
            var parts = new List<string>();
            if (Alive(cheater)) Between(cheater, betrayed, -penalty, parts);
            if (Alive(lover)) Between(lover, betrayed, -(secret.Kind == SecretKind.Bastard ? penalty : SecretRules.LoverResentment), parts);
            SchemeOutcomes.Relate(betrayed, SecretRules.WhisperGratitude);
            parts.Add(betrayed.Name + " sana minnettar: +" + SecretRules.WhisperGratitude);
            foreach (Secret s in Linked(state, secret))
            {
                s.Exposed = true;
                s.Whispered = true;
            }
            if (state.Roll100() <= SecretRules.SourceTracedChance && Alive(cheater))
            {
                SchemeOutcomes.Relate(cheater, -SecretRules.SourceTracedPenalty);
                parts.Add(cheater.Name + " söyleyenin sen olduğunu öğrendi: -" + SecretRules.SourceTracedPenalty);
            }
            SchemeOutcomes.Notify(state, "Eşine söylendi: " + betrayed.Name,
                "Adamın " + betrayed.Name + " ile baş başa konuştu ve bildiklerini anlattı. Saray duymadı; bu bir aile meselesi olarak kaldı.\n\n"
                + "Sonuç: " + string.Join(", ", parts) + ".",
                betrayed, CardImages.Whisper);
            state.AddLog(SchemeFactors.Today, "Eşine söylendi (" + SecretRules.Name(secret.Kind).ToLower(Tr.Culture) + "): " + betrayed.Name);
        }

        // ---- Yardımcılar ----

        private static bool Alive(Hero h) => h != null && h.IsAlive;

        private static void Between(Hero a, Hero b, int delta, List<string> parts)
        {
            if (!Alive(a) || !Alive(b) || a == b) return;
            if (a == Hero.MainHero) SchemeOutcomes.Relate(b, delta);
            else if (b == Hero.MainHero) SchemeOutcomes.Relate(a, delta);
            else SchemeOutcomes.RelateBetween(a, b, delta);
            parts.Add(a.Name + " ile " + b.Name + " arası " + delta);
        }

        /// <summary>Kan bağı ya da üvey/kayın-ebeveyn bağı (iki yönde).</summary>
        private static bool Kin(Hero a, Hero b) => KinOneWay(a, b) || KinOneWay(b, a);

        private static bool KinOneWay(Hero a, Hero b)
        {
            if (a.Father == b || a.Mother == b) return true;
            if (a.Father != null && a.Father == b.Father) return true;
            if (a.Mother != null && a.Mother == b.Mother) return true;
            if (a.Father?.Father == b || a.Father?.Mother == b || a.Mother?.Father == b || a.Mother?.Mother == b) return true;
            Hero spouse = a.Spouse;
            return spouse != null && (spouse.Father == b || spouse.Mother == b || b.Father == spouse || b.Mother == spouse);
        }
    }
}
