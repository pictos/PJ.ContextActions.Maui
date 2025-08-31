namespace PJ.ContextActions.Maui;

public partial class ContextActionBehavior
{
	public ICollection<MenuItem> MenuItems { get; set; } = [];

}


#if IOS
partial class ContextActionBehavior : PlatformBehavior<View, UIKit.UIView>
{

}
#elif ANDROID
partial class ContextActionBehavior : PlatformBehavior<View, Android.Views.View>
{

}
#endif