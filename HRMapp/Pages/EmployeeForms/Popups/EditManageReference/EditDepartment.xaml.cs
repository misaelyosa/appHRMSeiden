using CommunityToolkit.Maui.Views;
using HRMapp.ViewModels.EmployeeFormViewModel;

namespace HRMapp.Pages.EmployeeForms.Popups.EditManageReference;

public partial class EditDepartment : Popup
{
	public EditDepartment(object bindingContext)
	{
		InitializeComponent();
		BindingContext = bindingContext;
	}
}