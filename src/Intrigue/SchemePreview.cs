using System.Collections.Generic;
using CheatMenu.Intrigue.Core;
using TaleWorlds.CampaignSystem;

namespace CheatMenu.Intrigue
{
    /// <summary>
    /// Başlatmadan önce bir entrikanın özeti: başarı aralığı, maliyet, süre, riskler. Hem Insert menüsündeki onay penceresi
    /// hem Entrika ekranının "Yeni entrika" paneli bunu gösterir; ikisi aynı sayıları söylesin diye tek yerde.
    /// </summary>
    public static class SchemePreview
    {
        /// <summary>En az ve en fazla aşamadaki başarı yüzdesi (casuslukta ikisi aynı).</summary>
        public static void SuccessRange(IntrigueState state, Hero spymaster, Hero target, SchemeKind kind, out int low, out int high)
        {
            SchemeInputs inputs = SchemeFactors.Inputs(state, spymaster, target, kind, null);
            if (kind == SchemeKind.Spy)
            {
                low = high = SchemeMath.SpySuccess(inputs);
                return;
            }
            low = SchemeMath.Success(kind, inputs);
            inputs.CompletedPhases = SchemeRules.MaxPhases(kind);
            high = SchemeMath.Success(kind, inputs);
        }

        public static string ChanceText(IntrigueState state, Hero spymaster, Hero target, SchemeKind kind)
        {
            SuccessRange(state, spymaster, target, kind, out int low, out int high);
            return low == high ? "%" + low : "%" + low + "-" + high;
        }

        /// <summary>Özet satırları (hedef ve maliyet dahil).</summary>
        public static List<string> Lines(IntrigueState state, SchemeKind kind, Hero target)
        {
            Hero spymaster = SchemeFactors.Spymaster(state);
            SchemeInputs inputs = SchemeFactors.Inputs(state, spymaster, target, kind, null);
            int cost = SchemeFactors.StartCost(kind, target);
            int days = SchemeMath.PhaseDays(kind, SchemeFactors.Roguery(spymaster));
            int today = SchemeFactors.Today;

            var lines = new List<string>
            {
                "Hedef: " + SchemeFactors.TargetLabel(target),
                "Maliyet: " + SchemeFactors.Money(cost) + " (sende " + SchemeFactors.Money(Hero.MainHero.Gold) + ")",
            };
            int min = SchemeRules.MinPhases(kind), max = SchemeRules.MaxPhases(kind);
            if (SchemeRules.IsStrike(kind))
            {
                int low = SchemeMath.Success(kind, inputs);
                inputs.CompletedPhases = max;
                lines.Add("Aşamalar: en az " + min + ", en fazla " + max + "; her aşama yaklaşık " + days + " gün");
                lines.Add("Başarı: " + min + " aşamada %" + low + ", " + max + " aşamada %" + SchemeMath.Success(kind, inputs)
                    + " (harekete geçerken rüşvetle +5/+10/+15)");
                lines.Add("Her aşamada yakalanma: %" + SchemeMath.Discovery(kind, inputs));
                if (inputs.HasHook) lines.Add("Üzerinde kozun var: +10 başarı.");
                if (inputs.NetworkSuccess > 0) lines.Add("Gölge ağı desteği: başarı +" + inputs.NetworkSuccess + ", yakalanma -" + inputs.NetworkDiscovery + ".");
                lines.Add(state.HasValidReport(target.StringId, today)
                    ? "Güncel istihbarat raporun var: +10 başarı."
                    : "Güncel raporun yok: önce casusluk yaparsan +10 başarı.");
                if (inputs.TargetAlerted) lines.Add("Hedef tetikte: -15 başarı.");
                if (inputs.TargetIsPlayersCaptive) lines.Add("Hedef senin elinde: başarı +20, ama ölürse ilk senden şüphelenilir (iz bırakma ×2).");
                lines.Add("");
                lines.Add(kind == SchemeKind.Kidnap
                    ? "Başarılı olursa hedef partinde esir olur. Ortaya çıkarsa: ilişki cezası, suç puanı ve Onur kaybı (suikasttan hafif); "
                        + "hükümdarsa savaş çıkabilir."
                    : "Ortaya çıkarsa: ağır ilişki cezası, suç puanı, Onur kaybı ve kurbanın yakınlarıyla kan davası. "
                        + "Hedef hükümdarsa savaş çıkabilir; kendi krallığındansa hükümdarın hesap sorar.");
            }
            else if (kind == SchemeKind.Blackmail)
            {
                Secret secret = SchemeFactors.BestSecret(state, target);
                int low = SchemeMath.BlackmailSuccess(inputs);
                inputs.CompletedPhases = max;
                lines.Add("Elindeki sır: " + (secret != null ? SecretRules.Name(secret.Kind) : "-"));
                lines.Add("Aşamalar: 1-2; her aşama yaklaşık " + days + " gün (2. aşama başarı +15)");
                lines.Add("Başarı: 1 aşamada %" + low + ", 2 aşamada %" + SchemeMath.BlackmailSuccess(inputs)
                    + "; her aşamada yakalanma %" + SchemeMath.BlackmailDiscovery(inputs));
                lines.Add("");
                lines.Add("Başarılı olursa " + SchemeMath.HookDays + " gün geçerli bir koz: altın, etki, esirlerini bıraktırma, "
                    + "krallık değiştirtme. Kullanınca ilişkiniz düşer. Tutmazsa sır yanar ve ilişki -20.");
            }
            else
            {
                lines.Add("Aşamalar: 1-2; her aşama yaklaşık " + days + " gün (2. aşama yalnızca sır bulma şansını artırır)");
                lines.Add("Başarı: %" + SchemeMath.SpySuccess(inputs) + ", her aşamada yakalanma: %" + SchemeMath.SpyDiscovery(inputs));
                if (!target.HasMet) lines.Add("Tanışmadınız: başarı -10.");
                lines.Add("");
                lines.Add("Sonuç: " + SchemeMath.ReportValidDays + " gün geçerli rapor (konum, güç, altın, ilişkiler, kişilik)"
                    + (target.IsLord ? " ve belki bir sır." : ". Sırlar yalnızca lordlarda aranır."));
            }
            return lines;
        }

        /// <summary>Tür seçiminde gösterilen kısa açıklama.</summary>
        public static string KindDescription(SchemeKind kind)
        {
            switch (kind)
            {
                case SchemeKind.Sabotage:
                case SchemeKind.Incite:
                    return SettlementSchemes.KindDescription(kind);
                case SchemeKind.Spy:
                    return "Rapor ve sır: 1-2 aşama, düşük risk. Rapor, o hedefe suikast ve kaçırmada +10 başarı verir "
                        + "ve tanışmadığın birini de hedef listesine ekler.";
                case SchemeKind.Blackmail:
                    return "Bildiğin bir sırrı koza çevir: 1-2 aşama, orta risk. Önce casuslukla bir sır bulmalısın.";
                case SchemeKind.Kidnap:
                    return "Hedefi esir al: 2-4 aşama, çok yüksek risk. Fidye, takas ya da elinde tutmak sana kalmış.";
                default:
                    return "Hedefi öldür: 2-5 aşama, çok yüksek risk. Ortaya çıkarsa kan davası ve hükümdarın hesabı.";
            }
        }

        /// <summary>Hiç hedef yoksa neden.</summary>
        public static string NoTargetsText(SchemeKind kind) =>
            SchemeRules.TargetsSettlement(kind) ? SettlementSchemes.NoTargetsText(kind)
            : kind == SchemeKind.Spy ? "Casusluk yapılacak kimse bulunamadı."
            : kind == SchemeKind.Blackmail ? "Şantaj için ifşa edilmemiş bir sırrını bildiğin bir lord gerekir (önce casusluk)."
            : SchemeRules.Name(kind) + " için tanıştığın ya da hakkında güncel raporun olan, yeri belli bir hedef gerekir.";
    }
}
