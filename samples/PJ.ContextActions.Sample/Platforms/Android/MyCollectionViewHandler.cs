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
	public MyViewAdapter(ReorderableItemsView reorderableItemsView, Func<Microsoft.Maui.Controls.View, Context, ItemContentView>? createView = null) : base(reorderableItemsView, createView)
	{
	}

	protected override IGroupableItemsViewSource CreateItemsSource()
	{
		return itemsSource = base.CreateItemsSource();
	}

	public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
	{
		base.OnBindViewHolder(holder, position);

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
		if (menu == null || v is null) return;

		menu.SetHeaderTitle($"Item {position}");

		var editItem = menu.Add(0, 1, 0, "Edit Item")!;
		var deleteItem = menu.Add(0, 2, 1, "Delete Item")!;
		var shareItem = menu.Add(0, 3, 2, "Share Item")!;
		var addItem = menu.Add(0, 4, 3, "Add Item")!;


		editItem.SetOnMenuItemClickListener(new MenuItemClickListener(position, "Edit"));
		deleteItem.SetOnMenuItemClickListener(new MenuItemClickListener(position, "Delete"));
		shareItem.SetOnMenuItemClickListener(new MenuItemClickListener(position, "Share"));
		addItem.SetOnMenuItemClickListener(new MenuItemClickListener(position, "Add"));

		System.Diagnostics.Debug.WriteLine($"Floating context menu created for position: {position} (listener-based, no Activity!)");
	}
}

public class MenuItemClickListener : Java.Lang.Object, IMenuItemOnMenuItemClickListener
{
	readonly int position;
	readonly string action;

	public MenuItemClickListener(int position, string action)
	{
		this.position = position;
		this.action = action;
	}

	public bool OnMenuItemClick(IMenuItem? item)
	{
		if (item == null) return false;

		System.Diagnostics.Debug.WriteLine($"{action} selected for item at position {position}");

		// Handle the action based on the type
		switch (action)
		{
			case "Edit":
				// Handle edit action
				System.Diagnostics.Debug.WriteLine($"Handling edit for position {position}");
				break;
			case "Delete":
				// Handle delete action
				System.Diagnostics.Debug.WriteLine($"Handling delete for position {position}");
				break;
			case "Share":
				// Handle share action
				System.Diagnostics.Debug.WriteLine($"Handling share for position {position}");
				break;
			case "Add":
				// Handle add action
				System.Diagnostics.Debug.WriteLine($"Handling add for position {position}");
				break;
		}

		return true; // Consume the click event
	}
}