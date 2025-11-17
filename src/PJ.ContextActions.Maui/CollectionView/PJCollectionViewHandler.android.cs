using Android.Content;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using Microsoft.Maui.Controls.Handlers.Items;

namespace PJ.ContextActions.Maui;

sealed class PJCollectionViewHandler : CollectionViewHandler
{
	protected override ReorderableItemsViewAdapter<ReorderableItemsView, IGroupableItemsViewSource> CreateAdapter()
	{
		return new PJViewAdapter(VirtualView);
	}
}


/// <summary>
/// Custom adapter for CollectionView that enables context actions (menu items) on long press.
/// </summary>
/// <remarks>
/// This adapter extends the standard ReorderableItemsViewAdapter to provide context menu
/// functionality for items in a CollectionView. It manages the creation and binding of
/// menu items to collection view items, allowing for contextual actions when a user
/// performs a long press on an item.
/// </remarks>
public class PJViewAdapter : ReorderableItemsViewAdapter<ReorderableItemsView, IGroupableItemsViewSource>
{
	IGroupableItemsViewSource itemsSource = default!;
	protected ReorderableItemsView collectionView;
	protected MenuItem[]? menuItems;

	public PJViewAdapter(ReorderableItemsView reorderableItemsView, Func<Microsoft.Maui.Controls.View, Context, ItemContentView>? createView = null) : base(reorderableItemsView, createView)
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

		if (collectionView is not CollectionView cv || itemsSource is null)
		{
			return;
		}

		var contextActions = ContextActions.GetContextActions(cv);

		if (contextActions.Count is 0)
		{
			return;
		}

		if (menuItems is null)
		{
			menuItems = new MenuItem[contextActions.Count];

			foreach (var (index, item) in contextActions.Index())
			{
				item.BindingContext = cv.BindingContext;
				menuItems[index] = item;
			}
		}

		position -= itemsSource.HasHeader ? 1 : 0;

		var size = itemsSource.Count - (itemsSource.HasHeader ? 1 : 0) - (itemsSource.HasHeader ? 1 : 0);

		if (position >= 0 && position < size)
		{
			var element = itemsSource.GetItem(position + 1);
			var contextMenuListener = new ItemContextMenuListener(element, menuItems);
			holder.ItemView.SetOnCreateContextMenuListener(contextMenuListener);
		}
	}

	public override void OnViewRecycled(Java.Lang.Object holder)
	{
		base.OnViewRecycled(holder);

		if (holder is RecyclerView.ViewHolder viewHolder)
		{
			viewHolder.ItemView.SetOnCreateContextMenuListener(null);
		}
	}
}

/// <summary>
/// Handles the creation of context menus for items in a CollectionView on Android.
/// </summary>
/// <remarks>
/// This class implements the Android-specific interface for creating context menus
/// when a user performs a long-press on an item in the CollectionView. It associates
/// the menu items with the data element that was pressed, allowing context-specific
/// actions to be performed.
/// </remarks>
sealed class ItemContextMenuListener : Java.Lang.Object, global::Android.Views.View.IOnCreateContextMenuListener
{
	readonly object element;
	readonly MenuItem[] items;

	public ItemContextMenuListener(object element, MenuItem[] items)
	{
		this.element = element;
		this.items = items;
	}

	public void OnCreateContextMenu(IContextMenu? menu, Android.Views.View? v, IContextMenuContextMenuInfo? menuInfo)
	{
		if (menu is null || v is null)
		{
			return;
		}

		foreach (var (index, item) in items.Index())
		{
			var mItem = menu.Add(0, index + 1, index, item.Text);
			Assert(mItem is not null);
			mItem.SetOnMenuItemClickListener(new MenuItemClickListener(new(element, item)));
		}
	}
}
