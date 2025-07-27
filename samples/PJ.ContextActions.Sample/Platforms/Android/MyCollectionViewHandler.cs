using System.Security.Principal;
using Android.Content;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using Microsoft.Maui.Controls.Handlers.Items;
using Xamarin.Google.ErrorProne.Annotations;

namespace PJ.ContextActions.Sample.Platforms.Android;

class MyCVHandler : CollectionViewHandler
{
	protected override RecyclerView CreatePlatformView()
	{
		return new MyRecyclerView(Context, GetItemsLayout, CreateAdapter);
	}

	protected override ReorderableItemsViewAdapter<ReorderableItemsView, IGroupableItemsViewSource> CreateAdapter()
	{
		return new MyViewAdapter(VirtualView);
	}
}

public class MyRecyclerView : MauiRecyclerView<ReorderableItemsView, GroupableItemsViewAdapter<ReorderableItemsView, IGroupableItemsViewSource>, IGroupableItemsViewSource>
{
	public MyRecyclerView(Context context, Func<IItemsLayout> getItemsLayout, Func<GroupableItemsViewAdapter<ReorderableItemsView, IGroupableItemsViewSource>> getAdapter) : base(context, getItemsLayout, getAdapter)
	{

	}
}

public class MyViewAdapter : ReorderableItemsViewAdapter<ReorderableItemsView, IGroupableItemsViewSource>
{
	IGroupableItemsViewSource itemsSource = default!;
	ReorderableItemsView collectionView;

	internal static MenuItem[]? MenuItems;

	public MyViewAdapter(ReorderableItemsView reorderableItemsView, Func<Microsoft.Maui.Controls.View, Context, ItemContentView>? createView = null) : base(reorderableItemsView, createView)
	{
		collectionView = reorderableItemsView;
	}

	protected override IGroupableItemsViewSource CreateItemsSource()
	{
		return itemsSource = base.CreateItemsSource();
	}

	public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
	{
		base.OnBindViewHolder(holder, position);

		if (collectionView is not MyCV myCV)
		{
			return;
		}

		var contextActions = myCV.ContextActions;
		if (MenuItems is null && contextActions.Count > 0)
		{
			MenuItems = new MenuItem[contextActions.Count];

			foreach (var (index, item) in contextActions.Index())
			{
				item.BindingContext = myCV.BindingContext;
				MenuItems[index] = item;
			}
		}


		// TODO: IS this safe? I mean, there can be only one header?
		position -= itemsSource.HasHeader ? 1 : 0;
		var size = itemsSource.Count - (itemsSource.HasFooter ? 1 : 0) - (itemsSource.HasHeader ? 1 : 0);

		if (position >= 0 && position < size)
		{
			var element = itemsSource.GetItem(position + 1);
			holder.ItemView.Tag = position;
			var contextMenuListener = new ItemContextMenuListener(element);
			holder.ItemView.SetOnCreateContextMenuListener(contextMenuListener);
		}

		System.Diagnostics.Debug.WriteLine($"Set context menu listener for position {position}");
	}

	public override void OnViewRecycled(Java.Lang.Object holder)
	{
		base.OnViewRecycled(holder);

		if (holder is RecyclerView.ViewHolder viewHolder)
		{
			viewHolder.ItemView.SetOnCreateContextMenuListener(null);
		}

		System.Diagnostics.Debug.WriteLine("ViewHolder recycled - context menu cleaned up");
	}
}

public class ItemContextMenuListener : Java.Lang.Object, global::Android.Views.View.IOnCreateContextMenuListener
{
	readonly object element;

	public ItemContextMenuListener(object element)
	{
		this.element = element;
	}

	public void OnCreateContextMenu(IContextMenu? menu, global::Android.Views.View? v, IContextMenuContextMenuInfo? menuInfo)
	{
		if (menu is null || v is null) return;

		if (MyViewAdapter.MenuItems is null)
		{
			return;
		}

		var menuItems = MyViewAdapter.MenuItems;

		foreach (var (index, item) in menuItems.Index())
		{
			var mItem = menu.Add(0, index + 1, index, item.Text)!;
			mItem.SetOnMenuItemClickListener(new MenuItemClickListener(new(element, item)));
		}

		System.Diagnostics.Debug.WriteLine($"Floating context menu created for position: {element} (listener-based, no Activity!)");
	}
}

public class MenuItemClickListener : Java.Lang.Object, IMenuItemOnMenuItemClickListener
{
	readonly CommandBag bag;

	public MenuItemClickListener(CommandBag bag)
	{
		this.bag = bag;
	}

	public bool OnMenuItemClick(IMenuItem? item)
	{
		if (item is null) return false;

		var menuItem = bag.item;
		var element = bag.cvItem;

		menuItem.FireClicked(element);

		if (menuItem.Command?.CanExecute(element) is true)
		{
			menuItem.Command?.Execute(element);
		}

		return true; // Consume the click event
	}
}

public record CommandBag(object cvItem, MenuItem item);