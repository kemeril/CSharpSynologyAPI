using SynologyRestDAL.Vs;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
//using Windows.UI.Xaml.Markup;

namespace KDSVideo.UIHelpers
{
    public abstract class NavigationItemBase { }

    public sealed class NavigationCategory : NavigationItemBase
    {
        public string Name { get; set; }
        public string Tooltip { get; set; }
        public Symbol Glyph { get; set; }
        public Library Library { get; set; }
    }

    public sealed class NavigationMenuSeparator : NavigationItemBase { }

    public sealed class NavigationMenuHeader : NavigationItemBase
    {
        public string Name { get; set; }
    }

    //[ContentProperty(Name = "ItemTemplate")]
    public sealed class NavigationViewItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate HeaderTemplate { get; set; }
        public DataTemplate SeparatorTemplate { get; set; }
        public DataTemplate ItemTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            return item is NavigationMenuSeparator
                ? SeparatorTemplate
                : item is NavigationMenuHeader
                    ? HeaderTemplate
                    : item is NavigationCategory
                        ? ItemTemplate
                        : null;
        }
    }
}
