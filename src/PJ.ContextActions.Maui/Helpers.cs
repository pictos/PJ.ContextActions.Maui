#if WINDOWS
using Microsoft.UI.Xaml.Controls;
#endif
using static System.Reflection.BindingFlags;

namespace PJ.ContextActions.Maui;
static class Helpers
{
#if WINDOWS
	static Type? itemTemplateContextType;

	public static object ToCollectionViewModel(this object item)
	{
		itemTemplateContextType ??= item.GetType();

		var propertyInfo = itemTemplateContextType.GetProperty("Item", Instance | Public);

		Assert(propertyInfo is not null);

		var value = propertyInfo.GetValue(item, null);

		Assert(value is not null);

		return value;
	}

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
#endif
}
