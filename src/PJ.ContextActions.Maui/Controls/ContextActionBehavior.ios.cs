using UIKit;

namespace PJ.ContextActions.Maui;
partial class ContextActionBehavior : PlatformBehavior<View, UIView>
{
	public Func<UIContextMenuInteractionDelegate>? InteractionDelegateFactory { get; set; }
	UIContextMenuInteraction uiInteraction = default!;
	protected override void OnAttachedTo(View bindable, UIView platformView)
	{
		if (MenuItems.Count is 0)
		{
			return;
		}

		var menuToCreate = CreateMenuItems(MenuItems, bindable, bindable);

		var @delegate = InteractionDelegateFactory?.Invoke() ?? new InteractionDelegate([.. menuToCreate]);

		uiInteraction = new UIContextMenuInteraction(@delegate);

		platformView.AddInteraction(uiInteraction);
	}

	protected override void OnDetachedFrom(View bindable, UIView platformView)
	{
		platformView.RemoveInteraction(uiInteraction);
		uiInteraction.Dispose();
	}
}
