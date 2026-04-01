using System.Diagnostics;
using PJ.ContextActions.Maui;

namespace PJ.ContextActions.Sample.Pages;

public partial class SimpleSamplePage : ContentPage
{
	public Command<MenuItemResult> ClickCommand { get; }

	public SimpleSamplePage()
	{
		InitializeComponent();

		var items = new List<string>();
		for (var i = 0; i < 30; i++)
			items.Add($"Item {i}");

		cv.ItemsSource = items;

		ClickCommand = new Command<MenuItemResult>(result =>
		{
			Debug.Assert(result is not null);
			Debug.WriteLine($"[Command] item '{result.Text}' clicked: {result.Item}");
		});

		BindingContext = this;
	}

	void MenuItem_Clicked(object sender, EventArgs e)
	{
		var result = (MenuItemResult)sender;
		Debug.WriteLine($"[Clicked] item '{result.Text}' clicked: {result.Item}");
	}
}
