using System;
using System.Collections.Generic;
using System.Linq;
using CheatMenu.Intrigue.Core;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace CheatMenu.Intrigue
{
    /// <summary>
    /// Aşamalar arasında çıkan seçimli olaylar (tasarım 5.6). Bir olay aynı entrikada bir kez çıkar.
    /// Seçenek etkileri entrikanın birikmiş olay değerlerine (EventSuccess/EventDiscovery) ya da doğrudan oyuna yazılır.
    /// </summary>
    public static class IntrigueEvents
    {
        public sealed class Option
        {
            public string Label;
            /// <summary>null: seçilebilir. Değilse neden seçilemediği.</summary>
            public string DisabledHint;
            public Action Apply;
        }

        private sealed class Context
        {
            public IntrigueState State;
            public Scheme Scheme;
            public Hero Target;
            public Hero Spymaster;

            public string Name => Target.Name.ToString();
            public int Cost(double factor) => Math.Max(100, (int)Math.Round(Scheme.StartCost * factor));
        }

        private sealed class EventDef
        {
            public int Id;
            public string Title;
            public bool ForSpy;
            public Func<Context, bool> Condition = _ => true;
            public Func<Context, string> Text;
            public Func<Context, List<Option>> Options;
        }

        private static readonly List<EventDef> Events = new List<EventDef>
        {
            new EventDef
            {
                Id = 1, Title = "Rüşvetçi hizmetkâr", ForSpy = true,
                Text = c => Tr.Genitive(c.Name) + " hizmetkârlarından biri, efendisinin günlük düzenini anlatmak için para istiyor. "
                    + "Ağzı gevşek ama işe yarar biri.",
                Options = c => new List<Option>
                {
                    Paid(c, 0.3, "Öde", "+8 başarı", () => c.Scheme.EventSuccess += 8),
                    Free("Tehdit et: +5 başarı, yakalanma riski +4", () => { c.Scheme.EventSuccess += 5; c.Scheme.EventDiscovery += 4; }),
                    Free("Reddet", () => { }),
                },
            },
            new EventDef
            {
                Id = 2, Title = "Ajan yakalandı", ForSpy = true,
                Text = c => "Adamlarından biri " + Tr.Genitive(c.Name) + " muhafızlarına yakalandı. Henüz konuşmadı... henüz.",
                Options = c => new List<Option>
                {
                    Paid(c, 0.5, "Kurtar", "rüşvetle serbest bıraktır", () => { }),
                    Free("Sustur: konuşmadan ölsün (Merhamet -)", () => TraitLevelingHelper.OnIncidentResolved(DefaultTraits.Mercy, -20)),
                    Free("Kendi haline bırak: %35 ihtimalle konuşur", () =>
                    {
                        if (!CheatState.IntrigueNeverDiscovered && c.Scheme.Roll100() <= 35) SchemeOutcomes.Discovered(c.State, c.Scheme, c.Target);
                    }),
                },
            },
            new EventDef
            {
                Id = 3, Title = "Zehir mi, hançer mi?",
                Condition = c => c.Scheme.Kind == SchemeKind.Murder && c.Scheme.Method == SchemeMethod.None,
                Text = c => c.Spymaster.Name + " yöntemi soruyor: " + c.Name + " nasıl ölecek?",
                Options = c => new List<Option>
                {
                    Free("Zehir: daha sessiz (yakalanma -3), daha belirsiz (başarı -5)", () =>
                    {
                        c.Scheme.Method = SchemeMethod.Poison;
                        c.Scheme.EventDiscovery -= 3;
                        c.Scheme.EventSuccess -= 5;
                    }),
                    Free("Hançer: daha kesin (başarı +8), daha gürültülü (yakalanma +4)", () =>
                    {
                        c.Scheme.Method = SchemeMethod.Dagger;
                        c.Scheme.EventSuccess += 8;
                        c.Scheme.EventDiscovery += 4;
                    }),
                    new Option
                    {
                        Label = "Kaza süsü: başarı +3, iz bırakma şansı yarıya iner",
                        DisabledHint = SchemeFactors.IsTraveling(c.Target) ? null : "Yalnızca hedef yoldayken",
                        Apply = () =>
                        {
                            c.Scheme.Method = SchemeMethod.Accident;
                            c.Scheme.EventSuccess += 3;
                        },
                    },
                },
            },
            new EventDef
            {
                Id = 4, Title = "Hedef yer değiştirdi",
                Condition = c => c.Target.PartyBelongedTo != null && !c.Target.IsPrisoner,
                Text = c => c.Name + " beklenmedik şekilde yola çıktı. Adamların izini kaybetmek üzere.",
                Options = c => new List<Option>
                {
                    Paid(c, 0.2, "Takip et", "iz kaybolmasın", () => { }),
                    Free("Bekle: bu aşama iki kat uzun sürer", () => c.Scheme.PhaseDays *= 2),
                },
            },
            new EventDef
            {
                Id = 5, Title = "Rakip suikastçı",
                Condition = c => c.Scheme.Kind == SchemeKind.Murder,
                Text = c => "Başka birinin de " + Tr.Genitive(c.Name) + " ölümünü istediği ortaya çıktı. Onun adamları da aynı kapıların etrafında dolaşıyor.",
                Options = c => new List<Option>
                {
                    Free("İşbirliği yap: başarı +10, ama sırrı bilen biri daha var (yakalanma +5)", () =>
                    {
                        c.Scheme.EventSuccess += 10;
                        c.Scheme.EventDiscovery += 5;
                    }),
                    Free("İhbar et: " + c.Name + " sana minnet duyar (ilişki +10); entrika biter, para geri gelmez", () =>
                    {
                        SchemeOutcomes.Relate(c.Target, 10);
                        SchemeEngine.Close(c.State, c.Scheme, "Rakip suikastçıyı " + Tr.Dative(c.Name) + " ihbar ettin; kendi planından vazgeçtin.", c.Target);
                    }),
                    Free("Görmezden gel", () => { }),
                },
            },
            new EventDef
            {
                Id = 6, Title = "Muhafız değişimi",
                Text = c => Tr.Genitive(c.Name) + " muhafızları değişiyor. Yeni gelenler henüz düzeni öğrenmedi; bu pencere uzun sürmez.",
                Options = c => new List<Option>
                {
                    new Option
                    {
                        Label = "Hemen harekete geç: aşama sınırına bakılmaz, başarı +10",
                        DisabledHint = SchemeEngine.CannotExecuteReason(c.Scheme, true),
                        Apply = () => SchemeEngine.Execute(c.Scheme, 0, 10, true),
                    },
                    Free("Bekle", () => { }),
                },
            },
            new EventDef
            {
                Id = 7, Title = "Vicdan azabı",
                Condition = c => c.Spymaster.GetTraitLevel(DefaultTraits.Mercy) >= 1,
                Text = c => c.Spymaster.Name + " bu işten rahatsız: \"" + Tr.Accusative(c.Name) + " gerçekten " + (c.Scheme.Kind == SchemeKind.Kidnap ? "kaçırmak" : "öldürmek")
                    + " zorunda mıyız?\"",
                Options = c =>
                {
                    int chance = SchemeMath.PersuadeChance(Hero.MainHero.GetSkillValue(DefaultSkills.Charm));
                    return new List<Option>
                    {
                        Free("İkna et (Charm, %" + chance + "): olmazsa casus başıyla ilişki -5, başarı -5", () =>
                        {
                            if (c.Scheme.Roll100() <= chance) return;
                            SchemeOutcomes.Relate(c.Spymaster, -5);
                            c.Scheme.EventSuccess -= 5;
                        }),
                        Free("Emret: casus başıyla ilişki -15", () => SchemeOutcomes.Relate(c.Spymaster, -15)),
                        Free("Vazgeç: entrika biter, para geri gelmez", () =>
                            SchemeEngine.Close(c.State, c.Scheme, Tr.Genitive(c.Spymaster.Name.ToString()) + " vicdanını dinledin; " + Tr.Dative(c.Name) + " karşı plandan vazgeçtin.", c.Target)),
                    };
                },
            },
            new EventDef
            {
                Id = 8, Title = "Şüpheli tüccar", ForSpy = true,
                Text = c => Tr.Genitive(c.Name) + " çevresinde dolaşan bir tüccar, adamlarının peşine takılmış. Bir şeyler sezmiş olabilir.",
                Options = c => new List<Option>
                {
                    Paid(c, 0.3, "Sustur", "ağzını parayla kapat", () => { }),
                    Free("Yok say: yakalanma riski +5", () => c.Scheme.EventDiscovery += 5),
                },
            },
            new EventDef
            {
                Id = 9, Title = "Ziyafet daveti",
                Condition = c => c.Target.IsLord && !c.Target.IsPrisoner && IsTownOrCastle(c.Target.CurrentSettlement),
                Text = c => c.Name + ", " + Tr.Locative(c.Target.CurrentSettlement.Name.ToString()) + " bir ziyafet verecek. Kalabalık, şarap ve dikkatsiz muhafızlar...",
                Options = c => new List<Option>
                {
                    Free("Ziyafette vur: başarı +12, ama ortaya çıkarsa ilişki cezaları 1,5 kat", () =>
                    {
                        c.Scheme.EventSuccess += 12;
                        c.Scheme.FeastRisk = true;
                    }),
                    Free("Geç", () => { }),
                },
            },
            new EventDef
            {
                Id = 10, Title = "Çete liderinin teklifi",
                Condition = c => FriendlyGangLeader(c.Target) != null,
                Text = c => Tr.Locative(c.Target.CurrentSettlement.Name.ToString()) + "ki çete lideri " + FriendlyGangLeader(c.Target).Name
                    + " işin kokusunu almış ve adamlarını teklif ediyor.",
                Options = c => new List<Option>
                {
                    Free("Kabul et: başarı +10", () => c.Scheme.EventSuccess += 10),
                    Free("Reddet", () => { }),
                },
            },
        };

        /// <summary>Uygun ve bu entrikada henüz çıkmamış bir olay seçer ve kullanıldı diye işaretler; yoksa 0.</summary>
        public static int Pick(Scheme scheme, Hero target, Hero spymaster)
        {
            var context = new Context { Scheme = scheme, Target = target, Spymaster = spymaster };
            List<EventDef> eligible = Events
                .Where(e => (SchemeRules.IsStrike(scheme.Kind) || e.ForSpy) && !scheme.UsedEvents.Contains(e.Id) && e.Condition(context))
                .ToList();
            if (eligible.Count == 0) return 0;
            EventDef picked = eligible[scheme.Pick(eligible.Count)];
            scheme.UsedEvents.Add(picked.Id);
            return picked.Id;
        }

        /// <returns>Olay artık gösterilemiyorsa (hedef ya da casus başı yok) false.</returns>
        public static bool TryDescribe(IntrigueState state, Scheme scheme, out string title, out string text, out List<Option> options)
        {
            title = text = null;
            options = null;
            EventDef def = Events.FirstOrDefault(e => e.Id == scheme.PendingEvent);
            Hero target = Hero.Find(scheme.TargetId);
            Hero spymaster = SchemeFactors.Spymaster(state);
            if (def == null || target == null || !target.IsAlive || spymaster == null) return false;

            var context = new Context { State = state, Scheme = scheme, Target = target, Spymaster = spymaster };
            title = SchemeRules.Name(scheme.Kind) + " (" + target.Name + "): " + def.Title;
            text = def.Text(context);
            options = def.Options(context);
            return true;
        }

        private static Option Free(string label, Action apply) => new Option { Label = label, Apply = apply };

        private static Option Paid(Context c, double factor, string verb, string effect, Action apply)
        {
            int cost = c.Cost(factor);
            return new Option
            {
                Label = verb + " (" + SchemeFactors.Money(cost) + "): " + effect,
                DisabledHint = SchemeFactors.CanAfford(cost) ? null : "Yeterli altının yok",
                Apply = () =>
                {
                    SchemeFactors.Pay(cost);
                    apply();
                },
            };
        }

        private static bool IsTownOrCastle(Settlement settlement) => settlement != null && (settlement.IsTown || settlement.IsCastle);

        private static Hero FriendlyGangLeader(Hero target)
        {
            Settlement settlement = target.CurrentSettlement;
            if (settlement == null || !settlement.IsTown || target.IsPrisoner) return null;
            return settlement.Notables.FirstOrDefault(n => n.IsGangLeader && n.IsAlive && Hero.MainHero.GetRelation(n) >= 10);
        }
    }
}
