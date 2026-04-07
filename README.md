# PJ.ContextActions.Maui

A .NET MAUI library that adds long-press context menus to any view and to `CollectionView` items — with support for icons, text color, and per-item enabled/disabled state.

<img width="400" height="800" alt="image" src="https://github.com/user-attachments/assets/2fd7879a-fffa-4c6f-b722-0375238157bc" />

> **Supported platforms:** Android, iOS, Windows.  
> **MacCatalyst is not supported.**  
> **Icons are not supported on Android** (platform limitation).  
> **TextColor is not supported on iOS** (platform limitation — `UIAction` has no text color API).

---

## Installation

Add the [NuGet package](https://www.nuget.org/packages/PJSouzaSoftware.ContextActions.Maui) to your .NET MAUI project:

```
dotnet add package PJSouzaSoftware.ContextActions.Maui
```

---

## Setup

Register the library in `MauiProgram.cs`:

```csharp
using PJ.ContextActions.Maui;

public static MauiApp CreateMauiApp()
{
    var builder = MauiApp.CreateBuilder();
    builder
        .UseMauiApp<App>()
        .UseContextActions();

    return builder.Build();
}
```

---

## Usage

### Context menu on any view (`ContextActionBehavior`)

Attach `ContextActionBehavior` to the `Behaviors` collection of any MAUI view. The menu appears on long-press (Android/iOS) or right-click (Windows).

```xml
<ContentPage
    xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
    xmlns:pj="clr-namespace:PJ.ContextActions.Maui;assembly=PJ.ContextActions.Maui">

    <Image Source="dotnet_bot.png">
        <Image.Behaviors>
            <pj:ContextActionBehavior>
                <pj:ContextActionBehavior.MenuItems>
                    <pj:MenuItem Text="Edit"   Clicked="OnEdit"   Icon="edit.png" />
                    <pj:MenuItem Text="Delete" Command="{Binding DeleteCommand}" />
                </pj:ContextActionBehavior.MenuItems>
            </pj:ContextActionBehavior>
        </Image.Behaviors>
    </Image>
</ContentPage>
```

> **Note:** `MenuItem.BindingContext` inherits from the host view's `BindingContext`. The `ContextActionBehavior` itself does not have a `BindingContext`.

---

### Context menu on `CollectionView` items

Use the `ContextActions` attached property on a `CollectionView`. Each item in the list gets the same set of menu items; commands receive the tapped data item as their parameter.

```xml
<CollectionView x:Name="cv">
    <pj:ContextActions.ContextActions>
        <pj:MenuItem Text="Edit"   Clicked="OnEdit"   Icon="edit.png" />
        <pj:MenuItem Text="Delete" Command="{Binding DeleteCommand}" />
    </pj:ContextActions.ContextActions>

    <CollectionView.ItemTemplate>
        <DataTemplate>
            <Label Text="{Binding .}" />
        </DataTemplate>
    </CollectionView.ItemTemplate>
</CollectionView>
```

---

## MenuItem properties

| Property | Type | Default | Description |
|---|---|---|---|
| `Text` | `string` | *(required)* | Label shown in the menu |
| `Icon` | `string?` | `null` | Image asset name. Not supported on Android. |
| `IsEnabled` | `bool` | `true` | Whether the item is interactive. Supports binding. |
| `TextColor` | `Color?` | `null` | Color of the menu item text. Supported on Android and Windows only. Supports binding. |
| `Command` | `ICommand?` | `null` | Command executed on tap. Receives the data item as parameter. |
| `UseMenuResultModel` | `bool` | `false` | When `true`, the command/event parameter is wrapped in a `MenuItemResult` that includes both the item text and the data item. |

---

## TextColor

Set a per-item text color directly in XAML:

```xml
<pj:MenuItem Text="✏️ Edit"      TextColor="CornflowerBlue" Clicked="OnEdit"   />
<pj:MenuItem Text="⭐ Favourite" TextColor="Gold"            Clicked="OnFavour" />
<pj:MenuItem Text="🗑️ Delete"   TextColor="Crimson"         Clicked="OnDelete" />
```

`TextColor` also supports binding, so it can be driven from a ViewModel:

```xml
<pj:MenuItem
    Text="Premium Feature 🔒"
    TextColor="{Binding PremiumColor}" />
```

---

## IsEnabled

Disable individual menu items to indicate unavailable actions:

```xml
<pj:MenuItem Text="Free Action"       IsEnabled="True"  Clicked="OnFree"    />
<pj:MenuItem Text="Premium Feature 🔒" IsEnabled="False" Clicked="OnPremium" />
```

`IsEnabled` supports binding, so it can be toggled at runtime from a ViewModel:

```xml
<pj:MenuItem
    Text="Premium Feature 🔒"
    IsEnabled="{Binding IsPremiumEnabled}" />
```

```csharp
public class MyViewModel : INotifyPropertyChanged
{
    bool _isPremiumEnabled;

    public bool IsPremiumEnabled
    {
        get => _isPremiumEnabled;
        set { _isPremiumEnabled = value; OnPropertyChanged(); }
    }

    // Toggle from a button, a purchase callback, etc.
    public ICommand ToggleCommand => new Command(() => IsPremiumEnabled = !IsPremiumEnabled);
}
```

---

## MenuItemResult

When `UseMenuResultModel="True"`, the `Clicked` event sender and `Command` parameter are a `MenuItemResult` instead of the raw data item:

```csharp
void OnEdit(object sender, EventArgs e)
{
    if (sender is MenuItemResult result)
    {
        Console.WriteLine(result.Text);   // menu item text, e.g. "Edit"
        Console.WriteLine(result.Item);   // the data item from the collection
    }
}
```

---

## Custom platform implementations

`ContextActionBehavior` exposes factory properties so you can swap in your own platform delegates.

### iOS

```csharp
var behavior = new ContextActionBehavior
{
    MenuItems = { /* items */ },
#if IOS
    InteractionDelegateFactory = () => new MyInteractionDelegate()
#endif
};
```

```csharp
public class MyInteractionDelegate : UIContextMenuInteractionDelegate
{
    public override UIContextMenuConfiguration? GetConfigurationForMenu(
        UIContextMenuInteraction interaction, CGPoint location)
    {
        return UIContextMenuConfiguration.Create(null, null, _ => /* your UIMenu */);
    }
}
```

### Android

```csharp
var behavior = new ContextActionBehavior
{
    MenuItems = { /* items */ },
#if ANDROID
    ContextMenuListenerFactory = () => new MyContextMenuListener()
#endif
};
```

```csharp
public class MyContextMenuListener : Java.Lang.Object, Android.Views.View.IOnCreateContextMenuListener
{
    public void OnCreateContextMenu(IContextMenu? menu, Android.Views.View? v, IContextMenuContextMenuInfo? menuInfo)
    {
        // build your menu
    }
}
```

---

## Support

This project is open source and maintained by one person. If you need urgent fixes or custom features, you can support the development through [GitHub Sponsors](https://github.com/sponsors/pictos/sponsorships?sponsor=pictos&tier_id=485056&preview=false).

