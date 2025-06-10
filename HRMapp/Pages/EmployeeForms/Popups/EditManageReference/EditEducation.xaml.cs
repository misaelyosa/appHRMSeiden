using CommunityToolkit.Maui.Views;
using HRMapp.ViewModels.EmployeeFormViewModel;

namespace HRMapp.Pages.EmployeeForms.Popups.EditManageReference;

public partial class EditEducation : Popup
{
	public EditEducation(object bindingContext)
	{
		InitializeComponent();
		BindingContext = bindingContext;
	}
}