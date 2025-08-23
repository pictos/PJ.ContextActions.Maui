using Microsoft.Maui.Controls.Handlers.Items;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using WMenuFlyout = Microsoft.UI.Xaml.Controls.MenuFlyout;
using WMenuFlyoutItem = Microsoft.UI.Xaml.Controls.MenuFlyoutItem;

namespace PJ.ContextActions.Maui;

sealed class PJCollectionViewHandler : CollectionViewHandler
{
	MenuItem[]? menuItems;
	Command<CommandBag>? mauiCommand;

	protected override ListViewBase CreatePlatformView()
	{
		var listView = base.CreatePlatformView();

		listView.ContainerContentChanging += OnContainerContentChanging;

		return listView;
	}

	protected override void DisconnectHandler(ListViewBase platformView)
	{
		platformView.ContainerContentChanging -= OnContainerContentChanging;
		base.DisconnectHandler(platformView);
	}

	void OnContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
	{
		if (args.InRecycleQueue || args.ItemContainer is null)
			return;

		if (menuItems is null)
		{
			Assert(VirtualView is not null);
			var contextActions = ContextActions.GetContextActions((CollectionView)VirtualView);

			menuItems = [.. contextActions];
		}

		var menuFlyout = new WMenuFlyout();
		var model = args.Item.ToCollectionViewModel();

		mauiCommand ??= new Command<CommandBag>(static bag =>
		{
			bag.item.FireClicked(bag.cvItem);
			var command = bag.item.Command;

			if (command?.CanExecute(bag.cvItem) is true)
			{
				command.Execute(bag.cvItem);
			}
		});

		foreach (var item in menuItems)
		{
			item.BindingContext = VirtualView.BindingContext;

			var menuFlyoutItem = new WMenuFlyoutItem
			{
				Text = item.Text,
				Command = mauiCommand,
				CommandParameter = new CommandBag(model, item)
			};

			if (!string.IsNullOrEmpty(item.Icon))
			{
				menuFlyoutItem.Icon = item.Icon.CreateIconElementFromIconPath();
			}

			menuFlyout.Items.Add(menuFlyoutItem);
		}

		args.ItemContainer.ContextFlyout = menuFlyout;
	}
}
