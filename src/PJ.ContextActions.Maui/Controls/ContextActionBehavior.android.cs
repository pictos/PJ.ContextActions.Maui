using Android.Views;
using AView = Android.Views.View;
using View = Microsoft.Maui.Controls.View;

namespace PJ.ContextActions.Maui;
partial class ContextActionBehavior : PlatformBehavior<View, AView>
{
	public Func<AView.IOnCreateContextMenuListener>? ContextMenuListenerFactory { get; set; }

	protected override void OnAttachedTo(View bindable, AView platformView)
	{
		if (MenuItems.Count is 0)
		{
			return;
		}

		var contextMenuListener = ContextMenuListenerFactory?.Invoke() ?? new AViewContextMenuListener([.. MenuItems], bindable);
		platformView.SetOnCreateContextMenuListener(contextMenuListener);
	}

	protected override void OnDetachedFrom(View bindable, AView platformView)
	{
		platformView.SetOnCreateContextMenuListener(null);
	}
}

sealed class AViewContextMenuListener : Java.Lang.Object, AView.IOnCreateContextMenuListener
{
	readonly MenuItem[] menuItems;
	readonly View view;

	public AViewContextMenuListener(MenuItem[] menuItems, View view)
	{
		this.menuItems = menuItems;
		this.view = view;
	}

	public void OnCreateContextMenu(Android.Views.IContextMenu? menu, AView? v, Android.Views.IContextMenuContextMenuInfo? menuInfo)
	{
		if (menu is null || v is null)
		{
			return;
		}

		foreach (var (index, item) in menuItems.Index())
		{
			item.BindingContext = view.BindingContext;
			var mItem = menu.Add(0, index + 1, index, item.Text);
			Assert(mItem is not null);
			mItem.SetOnMenuItemClickListener(new MenuItemClickListener(new(view, item)));
		}
	}
}