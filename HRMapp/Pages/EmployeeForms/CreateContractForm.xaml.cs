using HRMapp.Data.Model;
using HRMapp.ViewModels.EmployeeFormViewModel;

namespace HRMapp.Pages.EmployeeForms;

public partial class CreateContractForm : ContentPage
{
    private readonly CreateContractViewModel _viewModel; 
	public CreateContractForm(CreateContractViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if(BindingContext is CreateContractViewModel _viewModel)
        {
            await _viewModel.InitializeAsync();
        }
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