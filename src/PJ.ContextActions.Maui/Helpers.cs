#if WINDOWS
using Microsoft.UI.Xaml.Controls;
#elif IOS
using UIKit;
#endif
using System.Collections;
using static System.Reflection.BindingFlags;

namespace PJ.ContextActions.Maui;
static class Helpers
{

	public static bool Any(this IEnumerable enumerable)
	{
		var enumerator = enumerable.GetEnumerator();
		return enumerator.MoveNext();
	}
	
	public static T? GetValueOrDefault<T>(this WeakReference<T> weak)
		where T : class
		=> weak.TryGetTarget(out var value) ? value : default;

	public static void HandleCommandBag(this CommandBag bag)
	{
		var item = bag.item;
		var command = item.Command;
		object result = item.UseMenuResultModel ? new MenuItemResult(item.Text, bag.cvItem) : bag.cvItem;

		item.FireClicked(result);

		if (command?.CanExecute(result) is true)
		{
			command.Execute(result);
		}
	}

#if WINDOWS
	public static IconElement? CreateIconElementFromIconPath(this string iconPath)
	{
		try
		{
			// Create a BitmapIcon from the path
			var bitmapIcon = new BitmapIcon
			{
				ShowAsMonochrome = false
			};

			// First, try to load from app resources without prepending any path
			// This works for files set as MauiImage in the .csproj
			var uri = new System.Uri($"ms-appx:///{iconPath}");
			bitmapIcon.UriSource = uri;

			return bitmapIcon;
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"Failed to create icon from {iconPath}: {ex.Message}");
			return null;
		}
	}
#elif IOS
	public static IEnumerable<UIMenuElement> CreateMenuItems(ICollection<MenuItem> items, BindableObject cv, object element)
	{
		foreach (var (index, item) in items.Index())
		{
			item.BindingContext = cv.BindingContext;
			var action = UIAction.Create(
				item.Text,
				CreateImage(item.Icon),
				index.ToString(),
				_ =>
				{
					object result = item.UseMenuResultModel ? new MenuItemResult(item.Text, element) : element;
					item.FireClicked(result);
					var command = item.Command;
					if (command?.CanExecute(result) is true)
					{
						command.Execute(result);
					}
				});

			if (!item.IsEnabled)
				action.Attributes = UIMenuElementAttributes.Disabled;

			yield return action;
		}

		static UIImage? CreateImage(string? icon)
		{
			return string.IsNullOrEmpty(icon) ? null : new UIImage(icon);
		}
	}
#endif
}
