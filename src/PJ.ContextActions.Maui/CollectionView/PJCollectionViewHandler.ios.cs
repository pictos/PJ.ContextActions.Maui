using CoreGraphics;
using Foundation;
using Microsoft.Maui.Controls.Handlers.Items;
using UIKit;

namespace PJ.ContextActions.Maui;

sealed class PJCollectionViewHandler : CollectionViewHandler
{
	protected override ItemsViewController<ReorderableItemsView> CreateController(ReorderableItemsView itemsView, ItemsViewLayout layout)
	{
		return new PJReorderableItemsViewController(itemsView, layout);
	}
}

sealed class PJReorderableItemsViewController : ReorderableItemsViewController<ReorderableItemsView>
{
	public PJReorderableItemsViewController(ReorderableItemsView reorderableItemsView, ItemsViewLayout layout) : base(reorderableItemsView, layout)
	{

	}

	protected override UICollectionViewDelegateFlowLayout CreateDelegator()
	{
		return new PJDelegator(ItemsViewLayout, this);
	}
}

/// <summary>
/// Custom delegator for handling context menu interactions in CollectionView on iOS.
/// Extends the ReorderableItemsViewDelegator to provide context menu functionality.
/// </summary>
/// <remarks>
/// This class intercepts tap and hold gestures on collection items and displays
/// a context menu with actions defined in the CollectionView's ContextActions property.
/// </remarks>
public class PJDelegator : ReorderableItemsViewDelegator<ReorderableItemsView, ReorderableItemsViewController<ReorderableItemsView>>
{
	protected ReorderableItemsViewController<ReorderableItemsView> itemsViewController;
	List<MenuItem>? items;

	public PJDelegator(ItemsViewLayout itemsViewLayout, ReorderableItemsViewController<ReorderableItemsView> itemsViewController) : base(itemsViewLayout, itemsViewController)
	{
		this.itemsViewController = itemsViewController;
	}

	public override UIContextMenuConfiguration? GetContextMenuConfiguration(UICollectionView collectionView, NSIndexPath indexPath, CGPoint point)
	{
		var row = indexPath.Row;

		if (itemsViewController is not PJReorderableItemsViewController vc)
		{
			return null;
		}

		var element = vc.ItemsSource[indexPath];
		var cv = (CollectionView)vc.ItemsView;

		items ??= ContextActions.GetContextActions(cv);

		if (items.Count is 0)
		{
			return null;
		}

		var createMenu = UIMenu.Create([.. CreateMenuItems(items, cv, element)]);

		return UIContextMenuConfiguration.Create(null, null, _ => createMenu);
	}
}
