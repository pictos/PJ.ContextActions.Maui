using Android.Text;
using Android.Text.Style;
using Android.Views;
using Microsoft.Maui.Platform;
using AView = Android.Views.View;
using View = Microsoft.Maui.Controls.View;

namespace PJ.ContextActions.Maui;
partial class ContextActionBehavior : PlatformBehavior<View, AView>
{
	AView? _platformView;
	View? _bindable;

	public Func<AView.IOnCreateContextMenuListener>? ContextMenuListenerFactory { get; set; }

	static partial void OnIsEnabledPropertyChanged(ContextActionBehavior behavior, bool oldValue, bool newValue)
	{
		if (behavior._platformView is null || behavior._bindable is null)
			return;

		if (newValue && behavior.MenuItems.Count > 0)
		{
			var listener = behavior.ContextMenuListenerFactory?.Invoke()
				?? new AViewContextMenuListener([.. behavior.MenuItems], behavior._bindable);
			behavior._platformView.SetOnCreateContextMenuListener(listener);
		}
		else
		{
			behavior._platformView.SetOnCreateContextMenuListener(null);
		}
	}

	protected override void OnAttachedTo(View bindable, AView platformView)
	{
		_bindable = bindable;
		_platformView = platformView;

		if (!IsEnabled || MenuItems.Count is 0)
		{
			return;
		}

		var contextMenuListener = ContextMenuListenerFactory?.Invoke() ?? new AViewContextMenuListener([.. MenuItems], bindable);
		platformView.SetOnCreateContextMenuListener(contextMenuListener);
	}

	protected override void OnDetachedFrom(View bindable, AView platformView)
	{
		platformView.SetOnCreateContextMenuListener(null);
		_bindable = null;
		_platformView = null;
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
			mItem.SetEnabled(item.IsEnabled);
			ApplyTextColor(mItem, item);
			mItem.SetOnMenuItemClickListener(new MenuItemClickListener(new(view, item)));
		}
	}

	static void ApplyTextColor(IMenuItem mItem, MenuItem item)
	{
		if (item.TextColor is not { } textColor)
			return;

		var spannable = new SpannableString(item.Text);
		spannable.SetSpan(
			new ForegroundColorSpan(textColor.ToPlatform()),
			0, item.Text.Length,
			SpanTypes.ExclusiveExclusive);
		mItem.SetTitle(spannable);
	}
}