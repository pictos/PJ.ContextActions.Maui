using System.Diagnostics;
using System.Globalization;
using PJ.ContextActions.Maui;

namespace PJ.ContextActions.Sample.Pages;

public partial class TextColorIsEnabledPage : ContentPage
{
	public TextColorIsEnabledPage()
	{
		InitializeComponent();
		BindingContext = new TextColorIsEnabledViewModel();
	}

	void OnMenuItemClicked(object sender, EventArgs e)
	{
		var result = (MenuItemResult)sender;
		StatusLabel.Text = $"'{result.Text}' tapped";
		Debug.WriteLine($"[Clicked] item '{result.Text}' clicked: {result.Item}");
	}
}

sealed class MenuItemTextColorConverter : IValueConverter
{
	static readonly Color disabledColor = Colors.Gray;
	static readonly Color enabledColor = Colors.Green;

	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (value is null)
		{
			return disabledColor;
		}

		return ((bool)value) ? enabledColor : disabledColor;
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}