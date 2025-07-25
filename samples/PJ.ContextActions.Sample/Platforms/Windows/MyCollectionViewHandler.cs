using System.Collections;
using System.Diagnostics;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Handlers.Items;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using WMenuFlyout = Microsoft.UI.Xaml.Controls.MenuFlyout;
using WMenuFlyoutItem = Microsoft.UI.Xaml.Controls.MenuFlyoutItem;


namespace PJ.ContextActions.Sample.Platforms.Windows;

public class MyCVHandler : CollectionViewHandler
{
	MenuItem[]? menuItems;
	CollectionViewSource collectionviewsource = default!;

	object[] itemsSource = [];

	protected override ListViewBase CreatePlatformView()
	{
		var listViewBase = base.CreatePlatformView();
		listViewBase.ContainerContentChanging += OnContainerContentChanging;
		return listViewBase;
	}

	protected override CollectionViewSource CreateCollectionViewSource()
	{
		return collectionviewsource = base.CreateCollectionViewSource();
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

		itemsSource = [.. (VirtualView.ItemsSource as IEnumerable)];


		// Show your MenuFlyout here, and you have access to dataItem
		var menuFlyout = new WMenuFlyout();
		var index = args.ItemIndex;
		foreach (var item in menuItems!)
		{
			item.BindingContext = VirtualView.BindingContext;
			menuFlyout.Items.Add(new WMenuFlyoutItem { Text = item.Text, Command = item.Command, CommandParameter = itemsSource[index] });
		}


		Debug.WriteLine($"#### Index {index} ######");

		// Remove previous handler to avoid multiple subscriptions
		args.ItemContainer.ContextFlyout = menuFlyout;
		//args.ItemContainer.PointerPressed -= ItemContainer_PointerPressed;
		//args.ItemContainer.PointerPressed += ItemContainer_PointerPressed;
	}

	// TODO grab the item inside the ItemsSource in a better way
	void ItemContainer_PointerPressed(object sender, PointerRoutedEventArgs e)
	{
		if (!e.GetCurrentPoint(null).Properties.IsRightButtonPressed)
		{
			return;
		}

		// sender is the ListViewItem or GridViewItem
		if (sender is not ListViewItem itemContainer)
		{
			return;
		}

		// Get the bound data item

		var dataItem = itemContainer.Content;

		var itemsSource = collectionviewsource.Source as IReadOnlyCollection<object> ?? [];
		var index = -1;

		foreach (var element in itemsSource)
		{
			index++;
			if (element == dataItem)
			{
				break;
			}
		}


		var source = VirtualView.ItemsSource;
		index++;
		object myObj = default!;
		foreach (var obj in source)
		{
			index--;
			if (index is 0)
			{
				myObj = obj;
				break;
			}
		}



		// Show your MenuFlyout here, and you have access to dataItem
		var menuFlyout = new WMenuFlyout();

		foreach (var item in menuItems!)
		{
			item.BindingContext = VirtualView.BindingContext;
			menuFlyout.Items.Add(new WMenuFlyoutItem { Text = item.Text, Command = item.Command, CommandParameter = myObj });
		}

		menuFlyout.ShowAt(itemContainer, e.GetCurrentPoint(itemContainer).Position);

		e.Handled = true;
	}
}
