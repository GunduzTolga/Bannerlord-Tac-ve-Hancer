using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;

namespace CheatMenu.Bundle
{
    /// <summary>
    /// İçine paketlenen Regard and Standing ile uyum (tasarım: docs/superpowers/specs/2026-10-20-regardstanding-iliskiler.md).
    /// Lordlar arası gizli ilişkilerin tek sahibi o mod: ilişkileri ve gerçek babaları o kurar ve saklar; casus başı onları bulur.
    /// Modun herkese açık bir API'si yok, iç türlerine adıyla ulaşılır (1.5.0'a göre). Bir alan ya da metot bulunamazsa
    /// <see cref="Available"/> false olur ve Taç ve Hançer kendi gizli ilişki sırlarına döner.
    /// </summary>
    internal static class RegardStandingCompat
    {
        /// <summary>Regard and Standing'deki bir ilişkinin o anki kopyası.</summary>
        internal sealed class Affair
        {
            public string A;
            public string B;
            public int Day;
            /// <summary>Bitiş günü; sürüyorsa negatif.</summary>
            public int End;
            public bool Exposed;
            internal object Raw;

            public bool Ongoing => End < 0;
            public bool Involves(string id) => A == id || B == id;
            public string Other(string id) => A == id ? B : A;
        }

        /// <summary>Bir ilişkiden doğan (ya da doğacak) çocukların gerçek ebeveynleri.</summary>
        internal sealed class Parentage
        {
            public string Mother;
            public string Father;
            /// <summary>Annenin o sıradaki kocası (resmî baba); yoksa null.</summary>
            public string Husband;
            public bool Born;
            public List<string> Children = new List<string>();
        }

        private static bool _resolved;
        private static FieldInfo _all, _secrets;
        private static FieldInfo _a, _b, _day, _end, _exposed;
        private static FieldInfo _mother, _father, _husband, _born, _children;
        private static MethodInfo _expose, _breakOff;

        internal static bool Available
        {
            get
            {
                Resolve();
                return _all != null;
            }
        }

        /// <summary>Kahramanın bütün ilişki kayıtları (bitmişler ve ifşa olmuşlar dahil), eskiden yeniye.</summary>
        internal static List<Affair> AffairsOf(Hero hero)
        {
            if (hero == null || !Available) return new List<Affair>();
            string id = hero.StringId;
            return Read().Where(a => a.Involves(id)).OrderBy(a => a.Day).ToList();
        }

        /// <summary>İki kişi arasındaki son ilişki kaydı; yoksa null.</summary>
        internal static Affair Between(Hero a, Hero b)
        {
            if (a == null || b == null || !Available) return null;
            return Read().Where(x => x.Involves(a.StringId) && x.Involves(b.StringId)).OrderBy(x => x.Day).LastOrDefault();
        }

        /// <summary>Kahramanın sürmekte olan ilişkisinin öbür tarafı; yoksa null.</summary>
        internal static Hero CurrentLover(Hero hero)
        {
            Affair affair = AffairsOf(hero).LastOrDefault(a => a.Ongoing);
            return affair == null ? null : Hero.Find(affair.Other(hero.StringId));
        }

        /// <summary>Kahramanın anne ya da gerçek baba olduğu kayıtlar.</summary>
        internal static List<Parentage> ParentageOf(Hero hero)
        {
            var result = new List<Parentage>();
            if (hero == null || !Available) return result;
            try
            {
                foreach (object raw in (IEnumerable)_secrets.GetValue(null))
                {
                    var p = new Parentage
                    {
                        Mother = (string)_mother.GetValue(raw),
                        Father = (string)_father.GetValue(raw),
                        Husband = (string)_husband.GetValue(raw),
                        Born = (bool)_born.GetValue(raw),
                    };
                    if (_children.GetValue(raw) is IEnumerable<string> children) p.Children.AddRange(children);
                    if (p.Mother == hero.StringId || p.Father == hero.StringId) result.Add(p);
                }
            }
            catch (Exception ex)
            {
                Fail("parentage could not be read", ex);
            }
            return result;
        }

        /// <summary>
        /// İki kişinin ilişkisini o modun skandal kurallarıyla ifşa eder; ifşa eden <paramref name="by"/> (ilişkidekiler ona kin tutar).
        /// O modda bu çiftin bir ilişki kaydı varsa true (zaten ifşa olmuşsa yeniden işlenmez): o zaman bizim ilişki cezalarımız
        /// uygulanmaz, skandal iki kez sayılmasın. Kayıt yoksa false ve kendi kurallarımız uygulanır.
        /// </summary>
        internal static bool Expose(Hero a, Hero b, Hero by)
        {
            Affair affair = Between(a, b);
            if (affair == null) return false;
            if (affair.Exposed) return true;
            try
            {
                _expose.Invoke(null, new[] { affair.Raw, by });
                return true;
            }
            catch (Exception ex)
            {
                Fail("affair could not be exposed", ex);
                return false;
            }
        }

        /// <summary>Sürmekte olan ilişkiyi o modda bitirir (ör. eşin yüzleşmeden sonra); bittiyse true.</summary>
        internal static bool BreakOff(Hero a, Hero b)
        {
            Affair affair = Between(a, b);
            if (affair == null || !affair.Ongoing) return false;
            try
            {
                _breakOff.Invoke(null, new[] { affair.Raw });
                return true;
            }
            catch (Exception ex)
            {
                Fail("affair could not be broken off", ex);
                return false;
            }
        }

        private static List<Affair> Read()
        {
            var result = new List<Affair>();
            try
            {
                foreach (object raw in (IEnumerable)_all.GetValue(null))
                {
                    result.Add(new Affair
                    {
                        A = (string)_a.GetValue(raw),
                        B = (string)_b.GetValue(raw),
                        Day = (int)_day.GetValue(raw),
                        End = (int)_end.GetValue(raw),
                        Exposed = (bool)_exposed.GetValue(raw),
                        Raw = raw,
                    });
                }
            }
            catch (Exception ex)
            {
                Fail("affairs could not be read", ex);
            }
            return result;
        }

        private static void Resolve()
        {
            if (_resolved) return;
            _resolved = true;
            Type affairs = null;
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.GetName().Name != "RegardStanding") continue;
                affairs = assembly.GetType("RegardStanding.Affairs", false);
                if (affairs != null) break;
            }
            if (affairs == null) return;

            const BindingFlags Static = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
            const BindingFlags Instance = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            Type affair = affairs.GetNestedType("Affair", BindingFlags.Public | BindingFlags.NonPublic);
            Type secret = affairs.GetNestedType("Secret", BindingFlags.Public | BindingFlags.NonPublic);
            if (affair == null || secret == null) return;

            FieldInfo all = affairs.GetField("All", Static);
            FieldInfo secrets = affairs.GetField("Secrets", Static);
            FieldInfo a = Field(affair, "A", typeof(string), Instance), b = Field(affair, "B", typeof(string), Instance);
            FieldInfo day = Field(affair, "Day", typeof(int), Instance), end = Field(affair, "End", typeof(int), Instance);
            FieldInfo exposed = Field(affair, "Exposed", typeof(bool), Instance);
            FieldInfo mother = Field(secret, "Mother", typeof(string), Instance), father = Field(secret, "Father", typeof(string), Instance);
            FieldInfo husband = Field(secret, "Husband", typeof(string), Instance), born = Field(secret, "Born", typeof(bool), Instance);
            FieldInfo children = Field(secret, "Children", typeof(List<string>), Instance);
            MethodInfo expose = affairs.GetMethod("Expose", Static, null, new[] { affair, typeof(Hero) }, null);
            MethodInfo breakOff = affairs.GetMethod("BreakOff", Static, null, new[] { affair }, null);

            bool ok = all != null && typeof(IEnumerable).IsAssignableFrom(all.FieldType)
                && secrets != null && typeof(IEnumerable).IsAssignableFrom(secrets.FieldType)
                && new object[] { a, b, day, end, exposed, mother, father, husband, born, children, expose, breakOff }.All(x => x != null);
            if (!ok)
            {
                Debug.Print("[CheatMenu] Regard and Standing found, but its affairs are not where 1.5.0 had them; using our own romance secrets");
                return;
            }
            _secrets = secrets;
            _a = a; _b = b; _day = day; _end = end; _exposed = exposed;
            _mother = mother; _father = father; _husband = husband; _born = born; _children = children;
            _expose = expose; _breakOff = breakOff;
            _all = all;
        }

        private static FieldInfo Field(Type type, string name, Type fieldType, BindingFlags flags)
        {
            FieldInfo field = type.GetField(name, flags);
            return field != null && field.FieldType == fieldType ? field : null;
        }

        private static void Fail(string what, Exception ex) => Debug.Print("[CheatMenu] Regard and Standing: " + what + ": " + ex);
    }
}
