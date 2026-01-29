using System.Diagnostics;
using PJ.ContextActions.Maui;

namespace PJ.ContextActions.Sample;

public partial class MainPage : ContentPage
{
	public Command<MenuItemResult> ClickCommand { get; }

	public MainPage()
	{
		InitializeComponent();
		var list = new List<string>();

		for (var i = 0; i < 100; i++)
			list.Add($"Item {i}");

		cv.ItemsSource = list;

		ClickCommand = new Command<MenuItemResult>((i) =>
		{
			Debug.Assert(i is not null);

			//Debug.Assert(i is string);

			System.Diagnostics.Debug.WriteLine($"item {i.Text} clicado: {i.Item}");
		});

		BindingContext = this;
	}

	void MenuItem_Clicked(object sender, EventArgs e)
	{
		var i = (MenuItemResult)sender;
		System.Diagnostics.Debug.WriteLine($"item {i.Text} clicado: {i.Item}");
	}
}
