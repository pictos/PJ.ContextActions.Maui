using CoreGraphics;
using UIKit;

namespace PJ.ContextActions.Maui;
partial class ContextActionBehavior
{
	public class InteractionDelegate : UIContextMenuInteractionDelegate
	{
		readonly UIMenuElement[] menuItems;

		public InteractionDelegate(UIMenuElement[] menuItems)
		{
			this.menuItems = menuItems;
		}

		public override UIContextMenuConfiguration? GetConfigurationForMenu(UIContextMenuInteraction interaction, CGPoint location)
		{
			return UIContextMenuConfiguration.Create(null, null, _ => UIMenu.Create(menuItems));
		}
	}
}