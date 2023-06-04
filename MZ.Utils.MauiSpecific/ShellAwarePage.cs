using MZ.Utils.ViewModel;

namespace MZ.Utils.MauiSpecific;

public class ShellAwarePage<T> : ContentPage where T : BaseViewModel
{
	protected T? ViewModel => BindingContext as T;
	public ShellAwarePage(T viewModel)
	{
		BindingContext = viewModel;
	}
}