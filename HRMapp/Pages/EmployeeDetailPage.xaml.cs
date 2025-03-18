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

        await _viewModel.LoadEmployeeDetails();
    }
}