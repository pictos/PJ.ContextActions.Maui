using Microsoft.Maui.Platform;

namespace PJ.ContextActions.Maui;

sealed partial class MauiColorToWSolidColorBrush : Microsoft.UI.Xaml.Data.IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, string language)
	{
		if (value is not Color color)
		{
			throw new InvalidOperationException($"The value passed isn't of type {typeof(Color)}, it's {value?.GetType().ToString() ?? "Null"} instead.");
		}

		return color.ToPlatform();
	}

	public object ConvertBack(object value, Type targetType, object parameter, string language)
	{
		throw new NotImplementedException();
	}
}