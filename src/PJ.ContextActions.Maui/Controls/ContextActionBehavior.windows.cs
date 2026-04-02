using Microsoft.Maui.Platform;
using Microsoft.UI.Xaml;
using WBinding = Microsoft.UI.Xaml.Data.Binding;
using WMenuFlyout = Microsoft.UI.Xaml.Controls.MenuFlyout;
using WMenyFlyoutItem = Microsoft.UI.Xaml.Controls.MenuFlyoutItem;
using WSolidColorBrush = Microsoft.UI.Xaml.Media.SolidColorBrush;

namespace PJ.ContextActions.Maui;

partial class ContextActionBehavior : PlatformBehavior<View, FrameworkElement>
{
	static readonly MauiColorToWSolidColorBrush colorConverter = new();
	protected override void OnAttachedTo(View bindable, FrameworkElement platformView)
	{
		if (MenuItems.Count is 0)
		{
			return;
		}

		platformView.ContextFlyout = CreateMenu(bindable);
	}

	protected override void OnDetachedFrom(View bindable, FrameworkElement platformView)
	{
		platformView.ContextFlyout = null;
	}

	WMenuFlyout CreateMenu(View view)
	{
		var contextMenu = new WMenuFlyout();
		var items = contextMenu.Items;

		var mauiCommand = new Command<CommandBag>(static bag =>
		{
			bag.HandleCommandBag();
		});

		foreach (var item in MenuItems)
		{
			item.BindingContext = view.BindingContext;
			var flyoutItem = new WMenyFlyoutItem
			{
				Text = item.Text,
				Command = mauiCommand,
				CommandParameter = new CommandBag(view, item),
				Icon = item.Icon?.CreateIconElementFromIconPath(),
			};

			flyoutItem.SetBinding(WMenyFlyoutItem.IsEnabledProperty, new WBinding { Path = new PropertyPath(nameof(item.IsEnabled)), Source = item });

			if (item.TextColor is { } textColor)
			{
				flyoutItem.Foreground = new WSolidColorBrush(textColor.ToWindowsColor());
			}

			flyoutItem.SetBinding(WMenyFlyoutItem.ForegroundProperty, new WBinding { Path = new PropertyPath(nameof(item.TextColor)), Source = item, Converter = colorConverter });

			items.Add(flyoutItem);
		}

		return contextMenu;
	}
}
