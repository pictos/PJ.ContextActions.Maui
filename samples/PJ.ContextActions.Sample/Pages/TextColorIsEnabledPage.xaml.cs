using System.Diagnostics;
using PJ.ContextActions.Maui;

namespace PJ.ContextActions.Sample.Pages;

public partial class TextColorIsEnabledPage : ContentPage
{
	public TextColorIsEnabledPage()
	{
		InitializeComponent();
	}

	void OnMenuItemClicked(object sender, EventArgs e)
	{
		var result = (MenuItemResult)sender;
		StatusLabel.Text = $"'{result.Text}' tapped";
		Debug.WriteLine($"[Clicked] item '{result.Text}' clicked: {result.Item}");
	}

	void OnBehaviorEnabledToggled(object sender, ToggledEventArgs e)
	{
		ToggledBehavior.IsEnabled = e.Value;
		ToggledLabel.Text = e.Value ? "Long-press me" : "Context menu is disabled";
	}
}
