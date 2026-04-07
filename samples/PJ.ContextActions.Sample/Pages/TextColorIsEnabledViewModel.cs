using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace PJ.ContextActions.Sample.Pages;

public class TextColorIsEnabledViewModel : INotifyPropertyChanged
{
	bool _isPremiumEnabled = false;

	public bool IsPremiumEnabled
	{
		get => _isPremiumEnabled;
		set
		{
			if (_isPremiumEnabled == value)
				return;
			_isPremiumEnabled = value;
			OnPropertyChanged();
			OnPropertyChanged(nameof(TogglePremiumText));
		}
	}

	public string TogglePremiumText => IsPremiumEnabled
		? "Disable Premium Feature"
		: "Enable Premium Feature";

	public ICommand TogglePremiumCommand { get; }

	public TextColorIsEnabledViewModel()
	{
		TogglePremiumCommand = new Command(() => IsPremiumEnabled = !IsPremiumEnabled);
	}

	public event PropertyChangedEventHandler? PropertyChanged;

	void OnPropertyChanged([CallerMemberName] string? name = null)
		=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
