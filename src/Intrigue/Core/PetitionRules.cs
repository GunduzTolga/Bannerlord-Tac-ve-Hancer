using System;
using System.Collections.Generic;
using System.Linq;

namespace CheatMenu.Intrigue.Core
{
    /// <summary>Dilekçe sahibinin ya da etkilenen eşrafın türü (oyunun eşraf türleri).</summary>
    public enum NotableKind
    {
        None = 0,
        Artisan = 1,
        Merchant = 2,
        GangLeader = 3,
        Preacher = 4,
        Headman = 5,
    }

    /// <summary>Kararın beslediği kişilik özelliği (oyunun kendi özellikleri).</summary>
    public enum TraitKind
    {
        None = 0,
        Mercy = 1,
        Generosity = 2,
        Honor = 3,
        Calculating = 4,
        Valor = 5,
    }

    /// <summary>Bir kararın sonuçları: hepsi düz sayı.</summary>
    public sealed class PetitionEffect
    {
        public int Loyalty;
        public int Security;
        public int Prosperity;
        public int Food;
        public int Gold;
        public int Influence;
        /// <summary>Dilekçe sahibiyle ilişki.</summary>
        public int PetitionerRelation;
        /// <summary>Başka bir eşrafla ilişki (OtherKind).</summary>
        public NotableKind OtherKind;
        public int OtherRelation;
        public TraitKind Trait;
        public int TraitXp;
        /// <summary>Yakalanan casusu sorgula: yabancı bir lord hakkında rapor.</summary>
        public bool Intel;
    }

    public sealed class PetitionOption
    {
        public string Label;
        public PetitionEffect Effect;
    }

    public sealed class Petition
    {
        public int Id;
        public string Title;
        /// <summary>{yer} yerleşimin adıyla değişir.</summary>
        public string Text;
        public NotableKind Petitioner;
        /// <summary>Pazar, lonca, çete gibi şehir işleri: kalede çıkmaz.</summary>
        public bool TownOnly;
        public PetitionOption[] Options;
    }

    /// <summary>
    /// Saray günü (P2.9, tasarım: docs/superpowers/specs/2026-10-19-p2-saray-gunu.md): kendi şehrinde ya da kalende divan kurarsın, üç
    /// dilekçe gelir. Kararlar oyunun kendi değerlerini (sadakat, güvenlik, refah, erzak) ve kişilik özelliklerini etkiler. Saf: veri ve
    /// formüller; oyunsuz test edilir.
    /// </summary>
    public static class PetitionRules
    {
        public const int CooldownDays = 21;
        public const int PetitionsPerSession = 3;

        private static PetitionOption O(string label, PetitionEffect effect) => new PetitionOption { Label = label, Effect = effect };

        public static readonly Petition[] All =
        {
            new Petition
            {
                Id = 1, Title = "Toprak anlaşmazlığı", Petitioner = NotableKind.Headman,
                Text = "İki aile aynı tarlanın kendilerine ait olduğunu söylüyor. Biri köklü ve varlıklı, eski bir tapusu var; öbürü yoksul ama tarlayı "
                    + "yıllardır o işliyor. {yer} halkı kararını bekliyor.",
                Options = new[]
                {
                    O("Tapu esastır: varlıklı aileye ver", new PetitionEffect { Loyalty = -1, PetitionerRelation = 5, Trait = TraitKind.Honor, TraitXp = 30 }),
                    O("Toprağı işleyen yoksul aileye ver", new PetitionEffect { Loyalty = 3, PetitionerRelation = -5, Trait = TraitKind.Mercy, TraitXp = 30 }),
                    O("Tarlayı böl, ikisine de vergi indirimi", new PetitionEffect { Gold = -200, Loyalty = 2, Trait = TraitKind.Calculating, TraitXp = 30 }),
                },
            },
            new Petition
            {
                Id = 2, Title = "Ekmek hırsızı", Petitioner = NotableKind.Merchant, TownOnly = true,
                Text = "Pazarda ekmek çalarken yakalanan aç bir adam önüne getirildi. Tüccarlar ibret olsun diye ağır ceza istiyor.",
                Options = new[]
                {
                    O("Kırbaçlatıp şehirden sür", new PetitionEffect { Security = 3, Loyalty = -2, PetitionerRelation = 5, Trait = TraitKind.Mercy, TraitXp = -30 }),
                    O("Bir ay angaryada çalıştır", new PetitionEffect { Security = 1, Prosperity = 10, Trait = TraitKind.Honor, TraitXp = 20 }),
                    O("Affet ve karnını doyur", new PetitionEffect { Gold = -50, Loyalty = 2, Security = -2, PetitionerRelation = -5, Trait = TraitKind.Mercy, TraitXp = 40 }),
                },
            },
            new Petition
            {
                Id = 3, Title = "Tüccarların vergi talebi", Petitioner = NotableKind.Merchant, TownOnly = true,
                Text = "Tüccarlar, kervan yolları güvensiz olduğu için pazar vergisinin indirilmesini istiyor. Yoksa kervanlarını başka şehirlere "
                    + "götüreceklerini ima ediyorlar.",
                Options = new[]
                {
                    O("Vergiyi indir", new PetitionEffect { Gold = -600, Prosperity = 60, PetitionerRelation = 10, Trait = TraitKind.Generosity, TraitXp = 20 }),
                    O("Vergiyi koru, yollara muhafız koy", new PetitionEffect { Gold = -300, Security = 4, PetitionerRelation = 5, Trait = TraitKind.Calculating, TraitXp = 20 }),
                    O("Reddet", new PetitionEffect { Prosperity = -20, PetitionerRelation = -10 }),
                },
            },
            new Petition
            {
                Id = 4, Title = "Askerin dulu", Petitioner = NotableKind.None,
                Text = "Senin için savaşta ölen bir askerin dulu, iki küçük çocuğuyla önüne çıktı ve onlara bakacak bir şey istedi.",
                Options = new[]
                {
                    O("Ona bir maaş bağla", new PetitionEffect { Gold = -500, Loyalty = 3, Trait = TraitKind.Generosity, TraitXp = 40 }),
                    O("Saray mutfağında iş ver", new PetitionEffect { Gold = -100, Loyalty = 1, Trait = TraitKind.Calculating, TraitXp = 20 }),
                    O("Hazine boş, reddet", new PetitionEffect { Loyalty = -2, Trait = TraitKind.Generosity, TraitXp = -30 }),
                },
            },
            new Petition
            {
                Id = 5, Title = "Çete şikâyeti", Petitioner = NotableKind.Merchant, TownOnly = true,
                Text = "Esnaf, çetenin dükkânlardan haraç topladığından ve muhafızların göz yumduğundan şikâyetçi.",
                Options = new[]
                {
                    O("Muhafızlarla çeteyi dağıt", new PetitionEffect { Security = 6, Loyalty = 1, OtherKind = NotableKind.GangLeader, OtherRelation = -15, Trait = TraitKind.Valor, TraitXp = 20 }),
                    O("Haraçtan pay al", new PetitionEffect { Gold = 400, Security = -3, OtherKind = NotableKind.GangLeader, OtherRelation = 10, Trait = TraitKind.Honor, TraitXp = -40 }),
                    O("Görmezden gel", new PetitionEffect { Loyalty = -2, PetitionerRelation = -10 }),
                },
            },
            new Petition
            {
                Id = 6, Title = "Salgın", Petitioner = NotableKind.Preacher, TownOnly = true,
                Text = "Aşağı mahallede ateşli bir hastalık yayılıyor. Halk korkuyor; bazıları şehirden kaçmaya başladı.",
                Options = new[]
                {
                    O("Mahalleyi kapat (karantina)", new PetitionEffect { Prosperity = -40, Loyalty = -1, Security = 1, Trait = TraitKind.Calculating, TraitXp = 20 }),
                    O("Hekimlere ve ilaca para ver", new PetitionEffect { Gold = -700, Loyalty = 3, PetitionerRelation = 5, Trait = TraitKind.Generosity, TraitXp = 30 }),
                    O("Dua etsinler", new PetitionEffect { Prosperity = -20, Loyalty = -2 }),
                },
            },
            new Petition
            {
                Id = 7, Title = "Kıtlık", Petitioner = NotableKind.Headman,
                Text = "Hasat kötü geçti. {yer} halkı ambarların açılmasını istiyor; tüccarlar ise tahılı pahalıya satmayı öneriyor.",
                Options = new[]
                {
                    O("Ambarları aç", new PetitionEffect { Food = -40, Loyalty = 4, PetitionerRelation = 5, Trait = TraitKind.Generosity, TraitXp = 30 }),
                    O("Tahılı pahalıya sat", new PetitionEffect { Food = -20, Gold = 500, Loyalty = -4, Trait = TraitKind.Mercy, TraitXp = -20 }),
                    O("Komşulardan tahıl satın al", new PetitionEffect { Gold = -600, Food = 40, Loyalty = 2 }),
                },
            },
            new Petition
            {
                Id = 8, Title = "Zengin oğlanın suçu", Petitioner = NotableKind.Merchant, TownOnly = true,
                Text = "Zengin bir tüccarın oğlu sarhoşken bir zanaatkârın çırağını yaraladı. Baba oğlunun affını, zanaatkâr adalet istiyor.",
                Options = new[]
                {
                    O("Kanun herkese eşit: hapset", new PetitionEffect { PetitionerRelation = -15, OtherKind = NotableKind.Artisan, OtherRelation = 10, Trait = TraitKind.Honor, TraitXp = 40 }),
                    O("Kan bedeli ödesin", new PetitionEffect { PetitionerRelation = -5, OtherKind = NotableKind.Artisan, OtherRelation = 5, Trait = TraitKind.Calculating, TraitXp = 20 }),
                    O("Babasının hatırına affet", new PetitionEffect { Loyalty = -2, PetitionerRelation = 10, OtherKind = NotableKind.Artisan, OtherRelation = -15, Trait = TraitKind.Honor, TraitXp = -30 }),
                },
            },
            new Petition
            {
                Id = 9, Title = "Tapınak bağışı", Petitioner = NotableKind.Preacher,
                Text = "Rahipler, kışın çöken tapınak çatısı için bağış istiyor. Halk senin ne vereceğini merak ediyor.",
                Options = new[]
                {
                    O("Cömertçe bağışla", new PetitionEffect { Gold = -800, Loyalty = 4, PetitionerRelation = 10, Trait = TraitKind.Generosity, TraitXp = 30 }),
                    O("Az ver", new PetitionEffect { Gold = -200, Loyalty = 1 }),
                    O("Reddet", new PetitionEffect { Loyalty = -1, PetitionerRelation = -10 }),
                },
            },
            new Petition
            {
                Id = 10, Title = "Yakalanan casus", Petitioner = NotableKind.None,
                Text = "Muhafızlar {yer} içinde yabancı bir casus yakaladı. Kimin için çalıştığını söylemiyor ama elinden mühürlü notlar çıktı.",
                Options = new[]
                {
                    O("Sorgula: efendisini öğren", new PetitionEffect { Security = 1, Intel = true, Trait = TraitKind.Calculating, TraitXp = 20 }),
                    O("İbret olsun diye as", new PetitionEffect { Security = 4, Trait = TraitKind.Mercy, TraitXp = -30 }),
                    O("Serbest bırak, efendisine mesaj olsun", new PetitionEffect { Security = -2, Trait = TraitKind.Mercy, TraitXp = 20 }),
                },
            },
            new Petition
            {
                Id = 11, Title = "Lonca tekeli", Petitioner = NotableKind.Artisan, TownOnly = true,
                Text = "Zanaatkârlar loncası, şehirde yalnızca kendi üyelerinin üretim yapmasını istiyor ve karşılığında hazineye pay öneriyor.",
                Options = new[]
                {
                    O("İzin ver", new PetitionEffect { Gold = 300, Prosperity = -40, PetitionerRelation = 15, Trait = TraitKind.Calculating, TraitXp = 20 }),
                    O("Reddet: rekabet kalsın", new PetitionEffect { Prosperity = 20, PetitionerRelation = -10 }),
                },
            },
            new Petition
            {
                Id = 12, Title = "Köylülerin koruma talebi", Petitioner = NotableKind.Headman,
                Text = "Bağlı köylerin muhtarları haydutlardan şikâyetçi: sürüler çalınıyor, yollar güvensiz.",
                Options = new[]
                {
                    O("Devriye için para ver", new PetitionEffect { Gold = -500, Security = 5, PetitionerRelation = 10, Trait = TraitKind.Valor, TraitXp = 20 }),
                    O("Kendileri silahlansın", new PetitionEffect { Security = -1, Loyalty = -1, PetitionerRelation = -5 }),
                },
            },
        };

        public static Petition Find(int id) => All.FirstOrDefault(p => p.Id == id);

        /// <summary>Bu yerleşimde çıkabilecek dilekçeler (kalede şehir işleri yok).</summary>
        public static List<Petition> Available(bool isTown) => All.Where(p => isTown || !p.TownOnly).ToList();

        /// <summary>Divanda gelecek üç dilekçe; zar çağıranın (tohumlu).</summary>
        public static List<Petition> Draw(bool isTown, Func<int, int> pick)
        {
            List<Petition> pool = Available(isTown);
            var drawn = new List<Petition>();
            while (drawn.Count < PetitionsPerSession && pool.Count > 0)
            {
                Petition next = pool[pick(pool.Count)];
                pool.Remove(next);
                drawn.Add(next);
            }
            return drawn;
        }

        /// <summary>Seçenek düğmesinde yazan sonuçlar: "sadakat +3, 500 dinar".</summary>
        public static string EffectText(PetitionEffect e)
        {
            var parts = new List<string>();
            void Add(string name, int value)
            {
                if (value != 0) parts.Add(name + " " + (value > 0 ? "+" : "") + value);
            }
            if (e.Gold < 0) parts.Add(-e.Gold + " dinar");
            if (e.Gold > 0) parts.Add("+" + e.Gold + " dinar");
            Add("sadakat", e.Loyalty);
            Add("güvenlik", e.Security);
            Add("refah", e.Prosperity);
            Add("erzak", e.Food);
            Add("etki", e.Influence);
            if (e.Trait != TraitKind.None && e.TraitXp != 0) parts.Add(TraitName(e.Trait) + (e.TraitXp > 0 ? " +" : " -"));
            if (e.Intel) parts.Add("rapor");
            return string.Join(", ", parts);
        }

        public static string TraitName(TraitKind trait)
        {
            switch (trait)
            {
                case TraitKind.Mercy: return "Merhamet";
                case TraitKind.Generosity: return "Cömertlik";
                case TraitKind.Honor: return "Onur";
                case TraitKind.Calculating: return "Hesapçılık";
                case TraitKind.Valor: return "Cesaret";
                default: return "";
            }
        }
    }
}
