using CoreGraphics;
using Foundation;
using Microsoft.Maui.Controls.Handlers.Items;
using UIKit;

namespace PJ.ContextActions.Sample.Platforms.iOS;
public class MyCVHandler : CollectionViewHandler
{
	protected override ItemsViewController<ReorderableItemsView> CreateController(ReorderableItemsView itemsView, ItemsViewLayout layout)
	{
		var x = VirtualView;
		var z = itemsView;
		return new MyReorderableItemsViewController(itemsView, layout);
	}
}

class MyReorderableItemsViewController : ReorderableItemsViewController<ReorderableItemsView>
{
	public MyReorderableItemsViewController(ReorderableItemsView reorderableItemsView, ItemsViewLayout layout) : base(reorderableItemsView, layout)
	{
		ReorderableItemsView = reorderableItemsView;
	}

	public ReorderableItemsView ReorderableItemsView { get; }

	protected override UICollectionViewDelegateFlowLayout CreateDelegator()
	{

		return new MyDelegate(ItemsViewLayout, this);
	}
}

class MyDelegate : ReorderableItemsViewDelegator<ReorderableItemsView,
	ReorderableItemsViewController<ReorderableItemsView>>
{
	readonly ReorderableItemsViewController<ReorderableItemsView> itemsViewController;

	public MyDelegate(ItemsViewLayout itemsViewLayout, ReorderableItemsViewController<ReorderableItemsView> itemsViewController) : base(itemsViewLayout, itemsViewController)
	{
		this.itemsViewController = itemsViewController;
	}

	public override UIContextMenuConfiguration? GetContextMenuConfiguration(UICollectionView collectionView, NSIndexPath indexPath,
		CGPoint point)
	{
		int row = indexPath.Row;

		if (itemsViewController is not MyReorderableItemsViewController vc)
		{
			return null;
		}

		var element = vc.ItemsSource[indexPath];

		var myCv = (MyCV)vc.ItemsView;

		var items = myCv.ContextActions;

		if (items.Count is 0)
			return null;

		var createMenu = UIMenu.Create([.. CreateMenuItems(items, myCv, element)]);

		return UIContextMenuConfiguration.Create(null, null, (x) => createMenu);


		static IEnumerable<UIMenuElement> CreateMenuItems(IEnumerable<MenuItem> items, BindableObject cv, object element)
		{
			foreach (var (index, item) in items.Index())
			{
				item.BindingContext = cv.BindingContext;
				var action = UIAction.Create(item.Text ?? throw new NullReferenceException("Text can't be null!"),
					null,
					index.ToString(),
					_ => { item.FireClicked(element); item.Command?.Execute(element); });
				yield return action;
			}
		}
	}
}