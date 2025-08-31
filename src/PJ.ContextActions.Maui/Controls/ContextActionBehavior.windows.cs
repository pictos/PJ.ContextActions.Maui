using Microsoft.UI.Xaml;
using WMenuFlyout = Microsoft.UI.Xaml.Controls.MenuFlyout;
using WMenyFlyoutItem = Microsoft.UI.Xaml.Controls.MenuFlyoutItem;

namespace PJ.ContextActions.Maui;
partial class ContextActionBehavior : PlatformBehavior<View, FrameworkElement>
{
	protected override void OnAttachedTo(View bindable, FrameworkElement platformView)
	{
		if (MenuItems.Count is 0)
		{
			return;
		}

		platformView.ContextFlyout = CreateMenu(bindable);
	}

	WMenuFlyout CreateMenu(View view)
	{
		var contextMenu = new WMenuFlyout();
		var items = contextMenu.Items;

		var mauiCommand = new Command<CommandBag>(static bag =>
		{
			bag.item.FireClicked(bag.cvItem);
			var command = bag.item.Command;

			if (command?.CanExecute(bag.cvItem) is true)
			{
				command.Execute(bag.cvItem);
			}
		});

		foreach (var item in MenuItems)
		{
			item.BindingContext = view.BindingContext;
			items.Add(new WMenyFlyoutItem
			{
				Text = item.Text,
				Command = mauiCommand,
				CommandParameter = new CommandBag(view, item),
				Icon = item.Icon?.CreateIconElementFromIconPath()
			});
		}

		return contextMenu;
	}
}
