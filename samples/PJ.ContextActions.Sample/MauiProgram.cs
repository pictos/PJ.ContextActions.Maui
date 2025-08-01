﻿using Microsoft.Extensions.Logging;


#if ANDROID
using PJ.ContextActions.Sample.Platforms.Android;
#elif IOS
using PJ.ContextActions.Sample.Platforms.iOS;
#elif WINDOWS
using PJ.ContextActions.Sample.Platforms.Windows;
#endif

namespace PJ.ContextActions.Sample;
public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			})
			.ConfigureMauiHandlers(h =>
			{
#if IOS || ANDROID || WINDOWS
				h.AddHandler(typeof(MyCV), typeof(MyCVHandler));

				h.AddHandler(typeof(CollectionView), typeof(MyCVHandler));
#endif
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
