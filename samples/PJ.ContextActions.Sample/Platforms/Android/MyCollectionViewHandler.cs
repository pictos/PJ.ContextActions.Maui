using System.Security.Principal;
using Android.Content;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using Microsoft.Maui.Controls.Handlers.Items;

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

		holder.ItemView.Tag = position;
		var contextMenuListener = new ItemContextMenuListener(position);
		holder.ItemView.SetOnCreateContextMenuListener(contextMenuListener);

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
	readonly int position;

	public ItemContextMenuListener(int position)
	{
		this.position = position;
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
			mItem.SetOnMenuItemClickListener(new MenuItemClickListener(position,item));
		}

		System.Diagnostics.Debug.WriteLine($"Floating context menu created for position: {position} (listener-based, no Activity!)");
	}
}

public class MenuItemClickListener : Java.Lang.Object, IMenuItemOnMenuItemClickListener
{
	readonly int position;
	readonly MenuItem menuItem;

	public MenuItemClickListener(int position, MenuItem item)
	{
		this.menuItem = item;
	}

	public bool OnMenuItemClick(IMenuItem? item)
	{
		if (item is null) return false;

		menuItem.FireClicked();

		if (menuItem.Command?.CanExecute(null) is true)
		{
			menuItem.Command?.Execute(null);
		}

		return true; // Consume the click event
	}
}