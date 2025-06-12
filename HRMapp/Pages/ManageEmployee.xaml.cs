namespace HRMapp.Pages;
using HRMapp.ViewModels;
using System.Diagnostics;

public partial class ManageEmployee : ContentPage
{
    private readonly EmployeeListViewModel _viewModel;
	public ManageEmployee(EmployeeListViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
		BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await _viewModel.LoadDepartmentAsync();
        await _viewModel.LoadJobsAsync();
        await _viewModel.LoadFactoryAsync();
        Debug.WriteLine($"SearchText: '{_viewModel.SearchText}'");
        Debug.WriteLine($"Department: {_viewModel.SelectedDepartment}, Factory: {_viewModel.SelectedFactory}, Job: {_viewModel.SelectedJob}");

        if (string.IsNullOrEmpty(_viewModel.SearchText) && (_viewModel.SelectedDepartment == "none" 
            && _viewModel.SelectedFactory == "none" && _viewModel.SelectedJob == "none"))
        {
            Debug.WriteLine("kondisi 1"); 
            await _viewModel.RefreshData();
        } else if(!string.IsNullOrEmpty(_viewModel.SearchText) && (_viewModel.SelectedDepartment == "none"
            || _viewModel.SelectedFactory == "none" || _viewModel.SelectedJob == "none"))
        {
            Debug.WriteLine("kondisi 2"); 
            _viewModel.SelectedDepartment = "none";
            _viewModel.SelectedJob = "none";
            _viewModel.SelectedFactory = "none";
            await _viewModel.SearchFilter();
        } else if (string.IsNullOrEmpty(_viewModel.SearchText) && (_viewModel.SelectedDepartment != "none"
            || _viewModel.SelectedFactory != "none" || _viewModel.SelectedJob != "none"))
        {
            Debug.WriteLine("kondisi 3"); 
            await _viewModel.ApplyFilter();
        } else
        {
            Debug.WriteLine("kondisi 4"); 
            await _viewModel.RefreshData();
        }

            employeeListDg.SelectedItem = null;
    }
}