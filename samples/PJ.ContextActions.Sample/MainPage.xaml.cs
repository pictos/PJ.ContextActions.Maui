using PJ.ContextActions.Sample.Pages;

namespace PJ.ContextActions.Sample;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

	async void OnSimpleSampleClicked(object sender, EventArgs e)
		=> await Shell.Current.GoToAsync(nameof(SimpleSamplePage));

	async void OnTextColorIsEnabledClicked(object sender, EventArgs e)
		=> await Shell.Current.GoToAsync(nameof(TextColorIsEnabledPage));
}

