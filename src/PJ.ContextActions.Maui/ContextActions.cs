namespace PJ.ContextActions.Maui;

public static class ContextActions
{
	public static readonly BindableProperty ContextActionsProperty =
		BindableProperty.CreateAttached("ContextActions", typeof(List<MenuItem>), typeof(CollectionView), null, defaultValueCreator: ValueCreator);

	static object ValueCreator(BindableObject bindable) => new List<MenuItem>();

	public static void SetContextActions(CollectionView cv, IEnumerable<MenuItem> items) => cv.SetValue(ContextActionsProperty, items);

	public static List<MenuItem> GetContextActions(CollectionView cv) => (List<MenuItem>)cv.GetValue(ContextActionsProperty);

}
