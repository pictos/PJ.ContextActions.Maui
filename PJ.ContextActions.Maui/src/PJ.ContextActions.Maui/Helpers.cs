using static System.Reflection.BindingFlags;

namespace PJ.ContextActions.Maui;
static class Helpers
{
#if WINDOWS
	static Type? itemTemplateContextType;

	public static object ToCollectionViewModel(this object item)
	{
		itemTemplateContextType ??= item.GetType();

		var propertyInfo = itemTemplateContextType.GetProperty("Item", Instance | Public);

		Assert(propertyInfo is not null);

		var value = propertyInfo.GetValue(item, null);

		Assert(value is not null);

		return value;
	}
#endif
}
