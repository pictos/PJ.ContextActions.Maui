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
	public ReorderableItemsView ReorderableItemsView { get; }

	public PJReorderableItemsViewController(ReorderableItemsView reorderableItemsView, ItemsViewLayout layout) : base(reorderableItemsView, layout)
	{
		ReorderableItemsView = reorderableItemsView;
	}

	protected override UICollectionViewDelegateFlowLayout CreateDelegator()
	{
		return new PJDelegator(ItemsViewLayout, this);
	}
}

sealed class PJDelegator : ReorderableItemsViewDelegator<ReorderableItemsView,
	ReorderableItemsViewController<ReorderableItemsView>>
{
	readonly ReorderableItemsViewController<ReorderableItemsView> itemsViewController;

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

		var items = ContextActions.GetContextActions(cv);

		if (items.Count is 0)
		{
			return null;
		}

		var createMenu = UIMenu.Create([.. CreateMenuItems(items, cv, element)]);

		return UIContextMenuConfiguration.Create(null, null, _ => createMenu);

		//TODO Implement UIImage
		static IEnumerable<UIMenuElement> CreateMenuItems(List<MenuItem> items, BindableObject cv, object element)
		{
			foreach (var (index, item) in items.Index())
			{
				item.BindingContext = cv.BindingContext;
				var action = UIAction.Create(
					item.Text,
					null,
					index.ToString(),
					_ =>
					{
						item.FireClicked(element);
						var command = item.Command;
						if (command?.CanExecute(element) is true)
						{
							command.Execute(element);
						}
					});

				yield return action;
			}
		}
	}
}
