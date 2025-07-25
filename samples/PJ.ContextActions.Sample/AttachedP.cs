using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PJ.ContextActions.Sample;

public class MyCV : CollectionView
{
	public static readonly BindableProperty ContextActionsProperty =
		BindableProperty.Create("ContextActions", typeof(ObservableCollection<MenuItem>),
			typeof(CollectionView),
			new ObservableCollection<MenuItem>(),
			propertyChanged: OnContextActionsChanged);

	

	public ObservableCollection<MenuItem> ContextActions
	{
		set => SetValue(ContextActionsProperty, value);
		get => (ObservableCollection<MenuItem>)GetValue(ContextActionsProperty);
	}

	static void OnContextActionsChanged(BindableObject bindable, object oldValue, object newValue)
	{
		if (bindable is MyCV myCV && newValue is List<MenuItem> menuItems)
		{
			foreach (var item in menuItems)
			{
				item.BindingContext = myCV.BindingContext;
			}

			// Keep MenuItems' BindingContext in sync if MyCV's BindingContext changes
			myCV.BindingContextChanged -= MyCV_BindingContextChanged;
			myCV.BindingContextChanged += MyCV_BindingContextChanged;
		}
	}

	static void MyCV_BindingContextChanged(object? sender, EventArgs e)
	{
		if (sender is MyCV myCV)
		{
			var menuItems = myCV.ContextActions;
			if (menuItems != null)
			{
				foreach (var item in menuItems)
				{
					item.BindingContext = myCV.BindingContext;
				}
			}
		}
	}
}


public static class AttachedP
{
	public static readonly BindableProperty ContextActionsProperty =
		BindableProperty.CreateAttached("ContextActions", typeof(List<MenuItem>), typeof(CollectionView), Enumerable.Empty<MenuItem>().ToList());

	public static void SetContextActions(CollectionView cv, IEnumerable<MenuItem> items) => cv.SetValue(ContextActionsProperty, items);

	public static IEnumerable<MenuItem> GetContextActions(CollectionView cv) => (IEnumerable<MenuItem>)cv.GetValue(ContextActionsProperty);
}


// TODO: HErdar BindingCOntext do `Parent`
public class MenuItem : Element
{
	public string? Text { get; set; }

	public string? Icon { get; set; }

	public static readonly BindableProperty CommandProperty =
		BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(MenuItem));

	public ICommand? Command
	{
		get => (ICommand?)GetValue(CommandProperty);
		set => SetValue(CommandProperty, value);
	}

	public event EventHandler? Clicked;

	internal void FireClicked() => Clicked?.Invoke(this, EventArgs.Empty);

	// Inherit BindingContext from parent (e.g., MyCV)
	protected override void OnParentSet()
	{
		base.OnParentSet();
		if (Parent is BindableObject parent)
		{
			BindingContext = parent.BindingContext;
			parent.BindingContextChanged += Parent_BindingContextChanged;
		}
	}

	void Parent_BindingContextChanged(object? sender, EventArgs e)
	{
		if (sender is BindableObject parent)
		{
			BindingContext = parent.BindingContext;
		}
	}
}
