using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace PJ.ContextActions.Sample.Pages;

public class TextColorIsEnabledViewModel : INotifyPropertyChanged
{
	bool _isBehaviorEnabled = true;

	public bool IsBehaviorEnabled
	{
		get => _isBehaviorEnabled;
		set
		{
			if (_isBehaviorEnabled == value)
				return;
			_isBehaviorEnabled = value;
			OnPropertyChanged();
			OnPropertyChanged(nameof(ToggleButtonText));
		}
	}

	public string ToggleButtonText => IsBehaviorEnabled
		? "Disable Context Menu"
		: "Enable Context Menu";

	public ICommand ToggleCommand { get; }

	public TextColorIsEnabledViewModel()
	{
		ToggleCommand = new Command(() => IsBehaviorEnabled = !IsBehaviorEnabled);
	}

	public event PropertyChangedEventHandler? PropertyChanged;

	void OnPropertyChanged([CallerMemberName] string? name = null)
		=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
