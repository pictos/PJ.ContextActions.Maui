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


### 3. Usage Example (CollectionView)

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

### 4. Using ContextActions in Any View

You can use ContextActions with any view by attaching the `ContextActionBehavior` to the view's Behaviors collection. For example, to add context actions to an `Image`:

> **Warning:** The `MenuItem`'s `BindingContext` will be the same as the control's `BindingContext`. However, the `ContextActionBehavior` itself does not have a `BindingContext`.

#### XAML

```xml
<ContentPage
    xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
    xmlns:pj="clr-namespace:PJ.ContextActions.Maui;assembly=PJ.ContextActions.Maui">

    <Image Source="dotnet_bot.png">
        <Image.Behaviors>
            <pj:ContextActionBehavior>
                <pj:ContextActionBehavior.MenuItems>
                    <pj:MenuItem
                        Clicked="MenuItem_Clicked"
                        Icon="dotnet_bot.png"
                        Text="Primeiro" />
                    <pj:MenuItem Command="{Binding ClickCommand}" Text="Segundo" />
                </pj:ContextActionBehavior.MenuItems>
            </pj:ContextActionBehavior>
        </Image.Behaviors>
    </Image>
</ContentPage>
```

#### Code-behind (C#)

```csharp
public partial class MainPage : ContentPage
{
    public Command<object> ClickCommand { get; }

    public MainPage()
    {
        InitializeComponent();

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

### 5. Custom Implementations for Platform-Specific Behaviors

ContextActionBehavior allows you to provide your own custom implementation for platform-specific delegates and listeners.

#### iOS Custom Delegate

You can provide a custom `UIContextMenuInteractionDelegate` implementation by setting the `InteractionDelegateFactory` property:

```csharp
var image = new Image();
var behavior = new ContextActionBehavior
{
    MenuItems = { /* your menu items */ },
#if IOS
    InteractionDelegateFactory = () => new MyCustomInteractionDelegate()
#endif
};
image.Behaviors.Add(behavior);
```

Your custom implementation might look like:

```csharp
// You can also inherit from the Delegate used in this library and expand for that.
public class MyCustomInteractionDelegate : UIContextMenuInteractionDelegate
{
    // Custom implementation for handling context menu on iOS
    public override UIContextMenuConfiguration? GetConfigurationForMenu(UIContextMenuInteraction interaction, CGPoint location)
    {
        // Your custom logic here
        return UIContextMenuConfiguration.Create(null, null, menu => {
            // Create and return your custom UIMenu
        });
    }
}
```

#### Android Custom Listener

Similarly, for Android you can provide a custom `IOnCreateContextMenuListener` implementation:

```csharp
var image = new Image();
var behavior = new ContextActionBehavior
{
    MenuItems = { /* your menu items */ },
#if ANDROID
    ContextMenuListenerFactory = () => new MyCustomContextMenuListener()
#endif
};
image.Behaviors.Add(behavior);
```

Your custom implementation might look like:

```csharp
// You can also inherit from the Listener used in this library and expand for that.
public class MyCustomContextMenuListener : Java.Lang.Object, Android.Views.View.IOnCreateContextMenuListener
{
    public void OnCreateContextMenu(IContextMenu? menu, Android.Views.View? v, IContextMenuContextMenuInfo? menuInfo)
    {
        // Your custom logic for creating context menu items
        if (menu is null && v is null)
        {
            return;
        }

        // Add custom menu items
        // Handle clicks, etc.
    }
}
```

## Support

This project is open source and maintained by one person. If you need urgent fixes or custom features, you can support the development through [github sponsor](https://github.com/sponsors/pictos/sponsorships?sponsor=pictos&tier_id=485056&preview=false).
