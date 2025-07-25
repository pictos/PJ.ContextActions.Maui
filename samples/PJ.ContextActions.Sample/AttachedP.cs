using System.Windows.Input;

namespace PJ.ContextActions.Sample;

public static class AttachedP
{
	public static readonly BindableProperty ContextActionsProperty =
		BindableProperty.CreateAttached("ContextActions", typeof(IEnumerable<MenuItem>), typeof(CollectionView), Enumerable.Empty<MenuItem>());

	public static void SetContextActions(CollectionView cv, IEnumerable<MenuItem> items) => cv.SetValue(ContextActionsProperty, items);

	public static IEnumerable<MenuItem> GetContextActions(CollectionView cv) => (IEnumerable<MenuItem>)cv.GetValue(ContextActionsProperty);
}

public class MenuItem
{
	public string? Text { get; set; }

	public string? Icon { get; set; }

	public ICommand? Command { get; set; }
}
