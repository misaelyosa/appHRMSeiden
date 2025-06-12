using CommunityToolkit.Maui.Views;
using HRMapp.Pages.EmployeeForms.Popups.CutiPopup;
using System.Diagnostics;

namespace HRMapp.Pages;

public partial class EmployeeDetailPage : ContentPage
{
    private readonly EmployeeDetailViewModel _viewModel;
    public EmployeeDetailPage(EmployeeDetailViewModel viewModel)
    {
        Debug.WriteLine("EmployeeDetailPage Constructor Called");
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        Debug.WriteLine($"EmployeeDetailPage Appeared, EmployeeId: {_viewModel.EmployeeId}");
        Shell.SetFlyoutBehavior(this, FlyoutBehavior.Disabled);
        await _viewModel.LoadEmployeeDetails();
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

    private async void OnOpenCreateCutiPopup(object sender, EventArgs e)
    {
        if (BindingContext is EmployeeDetailViewModel vm)
        {
            vm.CutiDuration = string.Empty;
            vm.CutiReason = string.Empty;
            vm.CutiStartDate = default;

            var popup = new NewCuti(vm);
            this.ShowPopup(popup);
        }
    } 
}