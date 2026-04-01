using Microsoft.Maui.Platform;
using Microsoft.UI.Xaml;
using WMenuFlyout = Microsoft.UI.Xaml.Controls.MenuFlyout;
using WMenyFlyoutItem = Microsoft.UI.Xaml.Controls.MenuFlyoutItem;
using WSolidColorBrush = Microsoft.UI.Xaml.Media.SolidColorBrush;

namespace PJ.ContextActions.Maui;

partial class ContextActionBehavior : PlatformBehavior<View, FrameworkElement>
{
	FrameworkElement? _platformView;
	View? _bindable;

	static partial void OnIsEnabledPropertyChanged(ContextActionBehavior behavior, bool oldValue, bool newValue)
	{
		if (behavior._platformView is null || behavior._bindable is null)
			return;

		if (newValue && behavior.MenuItems.Count > 0)
			behavior._platformView.ContextFlyout = behavior.CreateMenu(behavior._bindable);
		else
			behavior._platformView.ContextFlyout = null;
	}

	protected override void OnAttachedTo(View bindable, FrameworkElement platformView)
	{
		_bindable = bindable;
		_platformView = platformView;

		if (!IsEnabled || MenuItems.Count is 0)
		{
			return;
		}

		platformView.ContextFlyout = CreateMenu(bindable);
	}

	protected override void OnDetachedFrom(View bindable, FrameworkElement platformView)
	{
		platformView.ContextFlyout = null;
		_platformView = null;
		_bindable = null;
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
