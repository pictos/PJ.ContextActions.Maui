using System.Windows.Input;

namespace PJ.ContextActions.Maui;

public class MenuItem : Element
{
	public string Text
	{
		get => field ?? throw new NullReferenceException("Property Text can't be null.");
		set;
	}

	public string? Icon { get; set; }

	public static readonly BindableProperty CommandProperty =
		BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(MenuItem));

	public ICommand? Command
	{
		get => (ICommand?)GetValue(CommandProperty);
		set => SetValue(CommandProperty, value);
	}

	public static readonly BindableProperty IsEnabledProperty =
		BindableProperty.Create(nameof(IsEnabled), typeof(bool), typeof(MenuItem), true);

	public bool IsEnabled
	{
		get => (bool)GetValue(IsEnabledProperty);
		set => SetValue(IsEnabledProperty, value);
	}

	public static readonly BindableProperty TextColorProperty =
		BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(MenuItem), null);

	public Color? TextColor
	{
		get => (Color?)GetValue(TextColorProperty);
		set => SetValue(TextColorProperty, value);
	}

	public bool UseMenuResultModel { get; set; }

	public event EventHandler? Clicked;

	internal void FireClicked(object result) => Clicked?.Invoke(result, EventArgs.Empty);

	protected override void OnParentSet()
	{
		base.OnParentSet();
		if (Parent is BindableObject parent)
		{
			BindingContext = parent.BindingContext;
			parent.BindingContextChanged += OnParentBindingContextChanged;
		}
	}

	void OnParentBindingContextChanged(object? sender, EventArgs e)
	{
		if (sender is BindableObject parent)
		{
			BindingContext = parent.BindingContext;
		}
	}
}