using Android.Views;

namespace PJ.ContextActions.Maui;

sealed class MenuItemClickListener : Java.Lang.Object, IMenuItemOnMenuItemClickListener
{
	readonly CommandBag bag;

	public MenuItemClickListener(CommandBag bag)
	{
		this.bag = bag;
	}

	public bool OnMenuItemClick(IMenuItem item)
	{
		if (item is null)
		{
			return false;
		}

		var menuItem = bag.item;
		var element = bag.cvItem;

		menuItem.FireClicked(element);

		if (menuItem.Command?.CanExecute(element) is true)
		{
			menuItem.Command.Execute(element);
		}

		return true;
	}
}