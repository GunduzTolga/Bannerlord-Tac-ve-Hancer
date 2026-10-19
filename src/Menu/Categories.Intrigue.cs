using System;
using System.Collections.Generic;
using CheatMenu.Intrigue;
using TaleWorlds.CampaignSystem;

namespace CheatMenu.Menu
{
    /// <summary>Entrika. E0: yalnızca kayıt testi; asıl sistem E1'de.</summary>
    public static partial class Categories
    {
        private static void ShowIntrigue()
        {
            IntrigueBehavior intrigue = IntrigueBehavior.Current
                ?? throw new InvalidOperationException("Entrika sistemi bu kampanyada yüklenmedi");
            string marker = intrigue.State.SaveTestMarker;

            ShowCategory("Entrika", new List<MenuEntry>
            {
                MenuEntry.Simple("Kayıt testi: işaret bırak", () =>
                {
                    intrigue.State.SaveTestMarker = "gün " + (int)CampaignTime.Now.ToDays;
                    MenuUI.Success("İşaret bırakıldı: " + intrigue.State.SaveTestMarker + ". Şimdi kaydet ve kaydı yeniden yükle.");
                }),
            }, ShowIntrigue,
                "Entrika sistemi yapım aşamasında. Kayıt testi: işaret bırak, kaydet, kaydı yeniden yükle, "
                + "işaret burada duruyor mu bak. Şu anki işaret: " + (marker ?? "yok"));
        }
    }
}
