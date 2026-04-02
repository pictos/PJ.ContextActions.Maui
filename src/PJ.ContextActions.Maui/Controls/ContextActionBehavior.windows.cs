using Microsoft.Maui.Platform;
using Microsoft.UI.Xaml;
using WMenuFlyout = Microsoft.UI.Xaml.Controls.MenuFlyout;
using WMenyFlyoutItem = Microsoft.UI.Xaml.Controls.MenuFlyoutItem;
using WSolidColorBrush = Microsoft.UI.Xaml.Media.SolidColorBrush;

namespace PJ.ContextActions.Maui;

partial class ContextActionBehavior : PlatformBehavior<View, FrameworkElement>
{
	FrameworkElement? platformView;
	View? bindable;

	static partial void OnIsEnabledPropertyChanged(ContextActionBehavior behavior, bool oldValue, bool newValue)
	{
		if (behavior.platformView is null || behavior.bindable is null)
			return;

		if (newValue && behavior.MenuItems.Count > 0)
			behavior.platformView.ContextFlyout = behavior.CreateMenu(behavior.bindable);
		else
			behavior.platformView.ContextFlyout = null;
	}

	protected override void OnAttachedTo(View bindable, FrameworkElement platformView)
	{
		this.bindable = bindable;
		this.platformView = platformView;

		if (!IsEnabled || MenuItems.Count is 0)
		{
			return;
		}

		platformView.ContextFlyout = CreateMenu(bindable);
	}

	protected override void OnDetachedFrom(View bindable, FrameworkElement platformView)
	{
		platformView.ContextFlyout = null;
		this.platformView = null;
		this.bindable = null;
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
				IsEnabled = item.IsEnabled,
			};

			if (item.TextColor is { } textColor)
			{
				flyoutItem.Foreground = new WSolidColorBrush(textColor.ToWindowsColor());
			}

			items.Add(flyoutItem);
		}

		return contextMenu;
	}
}
