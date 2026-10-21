using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core.ViewModelCollection.ImageIdentifiers;
using TaleWorlds.Library;

namespace CheatMenu.UI.ViewModels
{
    /// <summary>Resimli olay kartı: solda resim (yoksa kişinin portresi), sağda başlık, metin ve seçenekler.</summary>
    public sealed class EventCardVM : TvhViewModel
    {
        public EventCardVM(string caption, string title, string text, string image, Hero hero, IEnumerable<EventCards.CardOption> options,
            Action<EventCards.CardOption> choose)
        {
            Caption = caption ?? "";
            Title = title ?? "";
            Text = text ?? "";
            // Kartın kendi resmi yoksa aynı konudan bir yedeği (CardImages.Fallbacks), o da yoksa portre.
            string chosen = TvhImages.PickVariant(image);
            foreach (string candidate in Intrigue.Core.CardImages.WithFallbacks(image))
            {
                chosen = TvhImages.PickVariant(candidate);
                if (TvhImages.Get(chosen) != null) break;
            }
            HasImage = TvhImages.Get(chosen) != null;
            ImageName = HasImage ? chosen : "";
            HasPortrait = !HasImage && hero != null;
            Portrait = PortraitOf(HasPortrait ? hero : null);
            HeroName = !HasImage && hero != null ? hero.Name.ToString() : "";
            Options = new MBBindingList<CardOptionVM>();
            foreach (EventCards.CardOption option in options) Options.Add(new CardOptionVM(option, choose));
            TextBottomMargin = Options.Count * OptionHeight + 12f;
        }

        /// <summary>TvhCardOption.xml'deki seçenek yüksekliği + aralık.</summary>
        private const float OptionHeight = 74f;

        [DataSourceProperty] public string Caption { get; }
        [DataSourceProperty] public string Title { get; }
        [DataSourceProperty] public string Text { get; }
        [DataSourceProperty] public bool HasImage { get; }
        [DataSourceProperty] public string ImageName { get; }
        [DataSourceProperty] public ImageIdentifierVM Portrait { get; }
        [DataSourceProperty] public bool HasPortrait { get; }
        [DataSourceProperty] public bool HasNoVisual => !HasImage && !HasPortrait;
        [DataSourceProperty] public string HeroName { get; }
        [DataSourceProperty] public string PlaceholderText => "TAÇ VE HANÇER";
        [DataSourceProperty] public MBBindingList<CardOptionVM> Options { get; }
        /// <summary>Metin alanı seçeneklerin üstünde biter.</summary>
        [DataSourceProperty] public float TextBottomMargin { get; }

        public override void OnFinalize()
        {
            base.OnFinalize();
            Portrait?.OnFinalize();
        }
    }

    public sealed class CardOptionVM : TvhViewModel
    {
        private readonly EventCards.CardOption _option;
        private readonly Action<EventCards.CardOption> _choose;

        public CardOptionVM(EventCards.CardOption option, Action<EventCards.CardOption> choose)
        {
            _option = option;
            _choose = choose;
        }

        [DataSourceProperty] public string Label => _option.Label;
        [DataSourceProperty] public string Hint => _option.DisabledHint ?? "";
        [DataSourceProperty] public bool HasHint => !string.IsNullOrEmpty(_option.DisabledHint);
        [DataSourceProperty] public bool IsEnabled => _option.DisabledHint == null;

        public void ExecuteChoose()
        {
            if (IsEnabled) _choose(_option);
        }
    }
}
