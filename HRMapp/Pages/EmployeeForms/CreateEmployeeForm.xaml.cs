using CommunityToolkit.Maui.Views;
using HRMapp.Pages.EmployeeForms.Popups;
using HRMapp.ViewModels.EmployeeFormViewModel;

namespace HRMapp.Pages.EmployeeForms;

public partial class CreateEmployeeForm : ContentPage
{
    private CreateEmployeeViewModel _viewModel;
	public CreateEmployeeForm(CreateEmployeeViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
	}

    private void OnOpenPopupClicked(object sender, EventArgs e)
    {
        var popup = new NewCityProvince(this.BindingContext); 
        this.ShowPopup(popup);
    }

    private void OnOpenPopupEduClicked(object sender, EventArgs e)
    {
        var popup = new NewEducation(this.BindingContext);
        this.ShowPopup(popup);
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