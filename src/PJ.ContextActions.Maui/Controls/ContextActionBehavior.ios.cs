using UIKit;

namespace PJ.ContextActions.Maui;
partial class ContextActionBehavior : PlatformBehavior<View, UIView>
{
	public Func<UIContextMenuInteractionDelegate>? InteractionDelegateFactory { get; set; }
	UIContextMenuInteraction? uiInteraction;
	UIView? _platformView;
	View? _bindable;

	static partial void OnIsEnabledPropertyChanged(ContextActionBehavior behavior, bool oldValue, bool newValue)
	{
		if (behavior._platformView is null || behavior._bindable is null)
			return;

		if (newValue && behavior.MenuItems.Count > 0)
		{
			if (behavior.uiInteraction is null)
			{
				var menuToCreate = CreateMenuItems(behavior.MenuItems, behavior._bindable, behavior._bindable);
				var @delegate = behavior.InteractionDelegateFactory?.Invoke() ?? new InteractionDelegate([.. menuToCreate]);
				behavior.uiInteraction = new UIContextMenuInteraction(@delegate);
			}
			behavior._platformView.AddInteraction(behavior.uiInteraction);
		}
		else if (behavior.uiInteraction is not null)
		{
			behavior._platformView.RemoveInteraction(behavior.uiInteraction);
		}
	}

	protected override void OnAttachedTo(View bindable, UIView platformView)
	{
		_bindable = bindable;
		_platformView = platformView;

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
		_platformView = null;
		_bindable = null;
	}
}
