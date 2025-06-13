using CommunityToolkit.Maui.Views;
using HRMapp.ViewModels.EmployeeFormViewModel;

namespace HRMapp.Pages.EmployeeForms.Popups.EditManageReference;

public partial class EditFactory : Popup
{
	public EditFactory(object bindingContext)
	{
		InitializeComponent();
		BindingContext = bindingContext;
	}
}