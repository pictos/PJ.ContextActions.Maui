using CoreGraphics;
using UIKit;

namespace PJ.ContextActions.Maui;
partial class ContextActionBehavior
{
	public class InteractionDelegate : UIContextMenuInteractionDelegate
	{
		readonly ICollection<MenuItem> menuItems;
		readonly BindableObject bindable;
		readonly object element;

		public InteractionDelegate(ICollection<MenuItem> menuItems, BindableObject bindable, object element)
		{
			this.menuItems = menuItems;
			this.bindable = bindable;
			this.element = element;
		}

		public override UIContextMenuConfiguration? GetConfigurationForMenu(UIContextMenuInteraction interaction, CGPoint location)
		{
			return UIContextMenuConfiguration.Create(null, null, _ => UIMenu.Create([.. CreateMenuItems(menuItems, bindable, element)]));
		}
	}
}