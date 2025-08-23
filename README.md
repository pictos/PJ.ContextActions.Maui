# PJ.ContextActions.Maui

<img width="400" height="800" alt="image" src="https://github.com/user-attachments/assets/2fd7879a-fffa-4c6f-b722-0375238157bc" />


## Usage

> **Note:** MacCatalyst is NOT supported by this library. Supported platforms are Android, iOS, and Windows (via .NET MAUI).
> **Note:** Android does NOT support icons, this is a platform limitation.

### 1. Installation
Add the PJ.ContextActions.Maui [NuGet package](https://www.nuget.org/packages/PJSouzaSoftware.ContextActions.Maui) to your .NET MAUI project.



### 2. Initialization
In your `MauiProgram.cs`, add `.UseContextActions()` to the builder:

```csharp
using PJ.ContextActions.Maui;

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
        .UseContextActions();
    // ...
    return builder.Build();
}
```

### 3. Usage Example

#### XAML
Add the pj namespace and use `<pj:ContextActions.ContextActions>` inside your CollectionView:

```xml
<ContentPage
    xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
    xmlns:pj="clr-namespace:PJ.ContextActions.Maui;assembly=PJ.ContextActions.Maui">

    <CollectionView x:Name="cv">
        <pj:ContextActions.ContextActions>
            <pj:MenuItem Clicked="MenuItem_Clicked" Text="Primeiro" />
            <pj:MenuItem Command="{Binding ClickCommand}" Text="Segundo" />
        </pj:ContextActions.ContextActions>
        <CollectionView.ItemTemplate>
            <DataTemplate>
                <Label Text="{Binding .}" />
            </DataTemplate>
        </CollectionView.ItemTemplate>
    </CollectionView>
</ContentPage>
```

#### Code-behind (C#)
Set up the ItemsSource, Command, and event handler in your page's code-behind:

```csharp
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
            System.Diagnostics.Debug.WriteLine($"Segundo item clicado: {i}");
        });

        BindingContext = this;
    }

    void MenuItem_Clicked(object sender, EventArgs e)
    {
        System.Diagnostics.Debug.WriteLine($"Primeiro item clicado: {sender}");
    }
}
```

## Support

This project is open source and maintained by one person. If you need urgent fixes or custom features, you can support the development through [github sponsor](https://github.com/sponsors/pictos/sponsorships?sponsor=pictos&tier_id=485056&preview=false).
