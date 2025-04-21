using HRMapp.ViewModels.EmployeeFormViewModel;
using System.Threading.Tasks;

namespace HRMapp.Pages.EmployeeForms;

public partial class GeneratePKWTPage : ContentPage
{
	private GeneratePKWTPageViewModel _viewModel; 
	public GeneratePKWTPage(GeneratePKWTPageViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
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