using UIKit;

namespace PJ.ContextActions.Maui;
partial class ContextActionBehavior : PlatformBehavior<View, UIView>
{
	public Func<UIContextMenuInteractionDelegate>? InteractionDelegateFactory { get; set; }
	UIContextMenuInteraction? uiInteraction;

	static partial void OnIsEnabledPropertyChanged(ContextActionBehavior behavior, bool oldValue, bool newValue)
	{
		if (behavior.uiInteraction is null)
			return;

		// Re-attach or detach interaction based on IsEnabled
		// Note: UIContextMenuInteraction doesn't support toggling; caller should rebind
	}

	protected override void OnAttachedTo(View bindable, UIView platformView)
	{
		if (!IsEnabled || MenuItems.Count is 0)
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
		if (uiInteraction is not null)
		{
			platformView.RemoveInteraction(uiInteraction);
			uiInteraction.Dispose();
			uiInteraction = null;
		}
	}
}
