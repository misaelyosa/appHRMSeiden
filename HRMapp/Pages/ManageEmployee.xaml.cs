namespace HRMapp.Pages;
using HRMapp.ViewModels;

public partial class ManageEmployee : ContentPage
{
	public ManageEmployee(EmployeeListViewModel viewModel)
	{
		InitializeComponent();

		BindingContext = viewModel;

		viewModel.LoadEmployeeCommand.Execute(null);
	}
}