using PJ.ContextActions.Sample.Pages;

namespace PJ.ContextActions.Sample;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		Routing.RegisterRoute(nameof(SimpleSamplePage), typeof(SimpleSamplePage));
		Routing.RegisterRoute(nameof(TextColorIsEnabledPage), typeof(TextColorIsEnabledPage));
	}
}
