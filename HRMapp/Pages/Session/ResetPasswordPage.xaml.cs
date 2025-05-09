using HRMapp.ViewModels.SessionViewModel;

namespace HRMapp.Pages.Session;

public partial class ResetPasswordPage : ContentPage
{
	private readonly ResetPasswordViewModel _viewModel;
	public ResetPasswordPage(ResetPasswordViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
	}

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        Shell.SetNavBarIsVisible(this, false);
    }

    protected override void OnNavigatedFrom(NavigatedFromEventArgs args)
    {
        base.OnNavigatedFrom(args);
        Shell.SetNavBarIsVisible(this, true);
    }
}