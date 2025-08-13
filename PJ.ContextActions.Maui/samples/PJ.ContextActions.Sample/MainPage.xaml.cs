using System.Diagnostics;

namespace PJ.ContextActions.Sample;

public partial class MainPage : ContentPage
{
	public Command<object> ClickCommand { get; }

	public MainPage()
	{
		InitializeComponent();
		var list = new List<string>();

		for (var i = 0; i < 100; i++)
			list.Add($"Item {i}");

		cv.ItemsSource = list;

		ClickCommand = new Command<object>((i) =>
		{
			Debug.Assert(i is not null);

			Debug.Assert(i is string);

			System.Diagnostics.Debug.WriteLine($"Segundo item clicado: {i}");
		});

		BindingContext = this;
	}

	void MenuItem_Clicked(object sender, EventArgs e)
	{
		System.Diagnostics.Debug.WriteLine($"Primeiro item clicado: {sender}");
	}
}
