using System.Diagnostics;
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
