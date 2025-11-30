#if WINDOWS
using Microsoft.UI.Xaml.Controls;
#elif IOS
using UIKit;
#endif
using static System.Reflection.BindingFlags;

namespace PJ.ContextActions.Maui;
static class Helpers
{
	public static T? GetValueOrDefault<T>(this WeakReference<T> weak)
		where T : class
		=> weak.TryGetTarget(out var value) ? value : default;
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
					item.FireClicked(element);
					var command = item.Command;
					if (command?.CanExecute(element) is true)
					{
						command.Execute(element);
					}
				});

			yield return action;
		}

		static UIImage? CreateImage(string? icon)
		{
			return string.IsNullOrEmpty(icon) ? null : new UIImage(icon);
		}
	}
#endif
}
