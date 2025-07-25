using CoreGraphics;
using Foundation;
using Microsoft.Maui.Controls.Handlers.Items;
using UIKit;

namespace PJ.ContextActions.Sample.Platforms.iOS;
public class MyCVHandler : CollectionViewHandler
{
	protected override ItemsViewController<ReorderableItemsView> CreateController(ReorderableItemsView itemsView, ItemsViewLayout layout)
	{
		return new MyReorderableItemsViewController(itemsView, layout);
	}
}

class MyReorderableItemsViewController : ReorderableItemsViewController<ReorderableItemsView>
{
	public MyReorderableItemsViewController(ReorderableItemsView reorderableItemsView, ItemsViewLayout layout) : base(reorderableItemsView, layout)
	{
	}


	protected override UICollectionViewDelegateFlowLayout CreateDelegator()
	{
		return new MyDelegate(ItemsViewLayout, this);
	}
}

class MyDelegate : ReorderableItemsViewDelegator<ReorderableItemsView,
	ReorderableItemsViewController<ReorderableItemsView>>
{
	public MyDelegate(ItemsViewLayout itemsViewLayout, ReorderableItemsViewController<ReorderableItemsView> itemsViewController) : base(itemsViewLayout, itemsViewController)
	{
	}

	public override UIContextMenuConfiguration? GetContextMenuConfiguration(UICollectionView collectionView, NSIndexPath indexPath,
		CGPoint point)
	{
		int row = indexPath.Row;
		var edit = UIAction.Create("editar", UIImage.ActionsImage, "1", (action) => { Log(row); });
		var send = UIAction.Create("enviar", UIImage.ActionsImage, "2", (action) => { Log(row); });

		var createMenu = UIMenu.Create([edit, send]);

		return UIContextMenuConfiguration.Create(null, null, (x) => createMenu);
	}

	static void Log(int id)
	{

		Console.WriteLine("###############");
		Console.WriteLine("###############");
		Console.WriteLine($"Pressed {id}");
		Console.WriteLine("###############");
		Console.WriteLine("###############");
	}
}