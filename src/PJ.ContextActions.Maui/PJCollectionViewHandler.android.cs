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


sealed class PJViewAdapter : ReorderableItemsViewAdapter<ReorderableItemsView, IGroupableItemsViewSource>
{
	IGroupableItemsViewSource itemsSource = default!;
	ReorderableItemsView collectionView;
	internal static MenuItem[]? MenuItems;

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

		if (MenuItems is null)
		{
			MenuItems = new MenuItem[contextActions.Count];

			foreach (var (index, item) in contextActions.Index())
			{
				item.BindingContext = cv.BindingContext;
				MenuItems[index] = item;
			}
		}

		position -= itemsSource.HasHeader ? 1 : 0;

		var size = itemsSource.Count - (itemsSource.HasHeader ? 1 : 0) - (itemsSource.HasHeader ? 1 : 0);

		if (position >= 0 && position < size)
		{
			var element = itemsSource.GetItem(position + 1);
			var contextMenuListener = new ItemContextMenuListener(element);
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

sealed class ItemContextMenuListener : Java.Lang.Object, global::Android.Views.View.IOnCreateContextMenuListener
{
	readonly object element;

	public ItemContextMenuListener(object element)
	{
		this.element = element;
	}

	public void OnCreateContextMenu(IContextMenu? menu, Android.Views.View? v, IContextMenuContextMenuInfo? menuInfo)
	{
		if (menu is null || v is null)
		{
			return;
		}

		if (PJViewAdapter.MenuItems is null)
		{
			return;
		}

		var menuItems = PJViewAdapter.MenuItems;

		foreach (var (index, item) in menuItems.Index())
		{
			var mItem = menu.Add(0, index + 1, index, item.Text);
			Assert(mItem is not null);
			mItem.SetOnMenuItemClickListener(new MenuItemClickListener(new(element, item)));
		}
	}
}

sealed class MenuItemClickListener : Java.Lang.Object, IMenuItemOnMenuItemClickListener
{
	readonly CommandBag bag;

	public MenuItemClickListener(CommandBag bag)
	{
		this.bag = bag;
	}

	public bool OnMenuItemClick(IMenuItem item)
	{
		if (item is null)
		{
			return false;
		}

		var menuItem = bag.item;
		var element = bag.cvItem;

		menuItem.FireClicked(element);

		if (menuItem.Command?.CanExecute(element) is true)
		{
			menuItem.Command.Execute(element);
		}

		return true;
	}
}