namespace PJ.ContextActions.Maui;
public static class AppBuilderExtensions
{
	public static MauiAppBuilder UseContextActions(this MauiAppBuilder builder)
	{
		builder.ConfigureMauiHandlers(h =>
		{
			h.AddHandler(typeof(CollectionView), typeof(PJCollectionViewHandler));
		});

		return builder;
	}
}
