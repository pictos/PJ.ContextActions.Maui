using Microsoft.Maui.Controls.Handlers.Items;
using Microsoft.Maui.Controls.Platform;
using Microsoft.UI.Xaml.Controls;
using WMenuFlyout = Microsoft.UI.Xaml.Controls.MenuFlyout;
using WMenuFlyoutItem = Microsoft.UI.Xaml.Controls.MenuFlyoutItem;

namespace PJ.ContextActions.Maui;

public class PJCollectionViewHandler : CollectionViewHandler
{
	protected MenuItem[]? menuItems;
	Command<CommandBag>? mauiCommand;

	protected override ListViewBase CreatePlatformView()
	{
		var listView = base.CreatePlatformView();

		listView.ContainerContentChanging += OnContainerContentChangingDelegate;

		return listView;
	}

	protected override void DisconnectHandler(ListViewBase platformView)
	{
		platformView.ContainerContentChanging -= OnContainerContentChangingDelegate;
		base.DisconnectHandler(platformView);
	}

	/// <summary>
	/// Handles the ContainerContentChanging event for the ListView, setting up context menus for each item.
	/// </summary>
	/// <param name="sender">The ListViewBase that triggered the event.</param>
	/// <param name="args">Event arguments containing information about the changed container content.</param>
	/// <remarks>
	/// This method configures a context menu (MenuFlyout) for each item in the collection view.
	/// It initializes menu items from the CollectionView's context actions, binds commands,
	/// and sets up icons if specified. The context menu appears when the user right-clicks on an item.
	/// </remarks>
	protected virtual void OnContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
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
		var model = ((ItemTemplateContext)args.Item).Item;

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

	void OnContainerContentChangingDelegate(ListViewBase sender, ContainerContentChangingEventArgs args) =>
		OnContainerContentChanging(sender, args);
}
