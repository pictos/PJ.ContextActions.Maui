using System.Collections;
using System.Diagnostics;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Handlers.Items;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using WMenuFlyout = Microsoft.UI.Xaml.Controls.MenuFlyout;
using WMenuFlyoutItem = Microsoft.UI.Xaml.Controls.MenuFlyoutItem;
using static System.Reflection.BindingFlags;


namespace PJ.ContextActions.Sample.Platforms.Windows;

record CommandBag(object cvItem, MenuItem item);
public class MyCVHandler : CollectionViewHandler
{
	MenuItem[]? menuItems;
	Command<CommandBag>? mauiCommand;

	protected override ListViewBase CreatePlatformView()
	{
		var listViewBase = base.CreatePlatformView();
		listViewBase.ContainerContentChanging += OnContainerContentChanging;
		return listViewBase;
	}

	// TODO grab the item inside the ItemsSource in a better way
	void OnContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
	{
		if (args.InRecycleQueue || args.ItemContainer == null)
			return;

		if (menuItems is null)
		{
			Debug.Assert(VirtualView is not null);

			var myCv = (MyCV)VirtualView;

			menuItems = [.. myCv.ContextActions];
		}

		// Show your MenuFlyout here, and you have access to dataItem
		var menuFlyout = new WMenuFlyout();

		var model = args.Item.CollectionViewModel();

		mauiCommand ??= new Command<CommandBag>(static bag =>
		{
			var command = bag.item.Command;

			if (command?.CanExecute(bag.cvItem) is true)
			{
				command.Execute(bag.cvItem);
			}
			bag.item.FireClicked(bag.cvItem);
		});

		foreach (var item in menuItems!)
		{
			item.BindingContext = VirtualView.BindingContext;

			menuFlyout.Items.Add(new WMenuFlyoutItem
			{
				Text = item.Text,
				Command = mauiCommand,
				CommandParameter = new CommandBag(model, item)
			});
		}


		Debug.WriteLine($"#### Index {args.ItemIndex} ######");

		// Remove previous handler to avoid multiple subscriptions
		args.ItemContainer.ContextFlyout = menuFlyout;
		//args.ItemContainer.PointerPressed -= ItemContainer_PointerPressed;
		//args.ItemContainer.PointerPressed += ItemContainer_PointerPressed;
	}

	// TODO grab the item inside the ItemsSource in a better way
	//public void ItemContainer_PointerPressed(object sender, PointerRoutedEventArgs e)
	//{
	//	if (!e.GetCurrentPoint(null).Properties.IsRightButtonPressed)
	//	{
	//		return;
	//	}

	//	// sender is the ListViewItem or GridViewItem
	//	if (sender is not ListViewItem itemContainer)
	//	{
	//		return;
	//	}

	//	// Get the bound data item

	//	var dataItem = itemContainer.Content;

	//	var itemsSource = collectionviewsource.Source as IReadOnlyCollection<object> ?? [];
	//	var index = -1;

	//	foreach (var element in itemsSource)
	//	{
	//		index++;
	//		if (element == dataItem)
	//		{
	//			break;
	//		}
	//	}


	//	var source = VirtualView.ItemsSource;
	//	index++;
	//	object myObj = default!;
	//	foreach (var obj in source)
	//	{
	//		index--;
	//		if (index is 0)
	//		{
	//			myObj = obj;
	//			break;
	//		}
	//	}



	//	// Show your MenuFlyout here, and you have access to dataItem
	//	var menuFlyout = new WMenuFlyout();

	//	foreach (var item in menuItems!)
	//	{
	//		item.BindingContext = VirtualView.BindingContext;
	//		menuFlyout.Items.Add(new WMenuFlyoutItem { Text = item.Text, Command = item.Command, CommandParameter = myObj });
	//	}

	//	menuFlyout.ShowAt(itemContainer, e.GetCurrentPoint(itemContainer).Position);

	//	e.Handled = true;
	//}
}


static class ReflectionEx
{
	// Microsoft.Maui.Controls.Platform.ItemTemplateContext
	static Type? itemTemplateContextType;

	public static object CollectionViewModel(this object item)
	{
		itemTemplateContextType ??= item.GetType();

		var props = itemTemplateContextType.GetProperties();

		var propertyInfo = itemTemplateContextType.GetProperty("Item", Instance | Public);

		Debug.Assert(propertyInfo is not null);

		var value = propertyInfo.GetValue(item, null);

		Debug.Assert(value is not null);

		return value;
	}
}