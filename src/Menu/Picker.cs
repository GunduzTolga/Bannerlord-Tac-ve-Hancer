using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core;
using TaleWorlds.Core.ImageIdentifiers;

namespace CheatMenu.Menu
{
    /// <summary>Arama kutulu, tek seçimli genel liste. Faz 2 editörleri de bunu kullanır.</summary>
    public static class Picker
    {
        // Kahraman listesinde portre: her satır oyunun portre çizicisine bir resim ister; çok uzun listede (tüm lordlar) ağırlaşır.
        private const int MaxPortraits = 80;

        /// <param name="image">null ve liste kahramanlardan oluşuyorsa (en fazla MaxPortraits): oyunun portresi.</param>
        public static void Pick<T>(string title, IEnumerable<T> items, Func<T, string> label, Action<T> onPicked, Action onBack,
            Func<T, ImageIdentifier> image = null, bool sort = true)
            where T : class
        {
            List<T> all = items.ToList();
            if (image == null && typeof(Hero).IsAssignableFrom(typeof(T)) && all.Count <= MaxPortraits) image = item => Portrait(item as Hero);
            IEnumerable<InquiryElement> query = all.Select(item => new InquiryElement(item, label(item), image?.Invoke(item)));
            if (sort) query = query.OrderBy(element => element.Title, StringComparer.CurrentCultureIgnoreCase);
            List<InquiryElement> elements = query.ToList();

            if (elements.Count == 0)
            {
                MenuUI.Error(title + ": liste boş");
                MenuUI.Defer(onBack);
                return;
            }

            MBInformationManager.ShowMultiSelectionInquiry(
                new MultiSelectionInquiryData(
                    title, elements.Count + " kayıt - aramak için yazmaya başla", elements,
                    true, 1, 1, "Seç", "Geri",
                    selected =>
                    {
                        // Bir sonraki karede işlensin: onPicked yeni popup (ad kutusu) açabilir.
                        if (selected != null && selected.Count > 0 && selected[0].Identifier is T item)
                            MenuUI.Defer(() => onPicked(item));
                    },
                    _ => MenuUI.Defer(onBack),
                    "", true),
                true, false);
        }

        /// <summary>Oyunun portresi; çizilemezse resimsiz satır.</summary>
        private static ImageIdentifier Portrait(Hero hero)
        {
            try
            {
                return hero?.CharacterObject == null ? null
                    : new CharacterImageIdentifier(CampaignUIHelper.GetCharacterCode(hero.CharacterObject, false));
            }
            catch (Exception ex)
            {
                TaleWorlds.Library.Debug.Print("[CheatMenu] picker portrait failed: " + ex);
                return null;
            }
        }
    }
}
