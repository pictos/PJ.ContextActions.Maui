namespace PJ.ContextActions.Maui;

public partial class ContextActionBehavior
{
	public ICollection<MenuItem> MenuItems { get; set; } = [];

	public static readonly BindableProperty IsEnabledProperty =
		BindableProperty.Create(nameof(IsEnabled), typeof(bool), typeof(ContextActionBehavior), true,
			propertyChanged: OnIsEnabledChanged);

	public bool IsEnabled
	{
		get => (bool)GetValue(IsEnabledProperty);
		set => SetValue(IsEnabledProperty, value);
	}

	static partial void OnIsEnabledPropertyChanged(ContextActionBehavior behavior, bool oldValue, bool newValue);

	static void OnIsEnabledChanged(BindableObject bindable, object oldValue, object newValue)
	{
		var behavior = (ContextActionBehavior)bindable;
		OnIsEnabledPropertyChanged(behavior, (bool)oldValue, (bool)newValue);
	}
}