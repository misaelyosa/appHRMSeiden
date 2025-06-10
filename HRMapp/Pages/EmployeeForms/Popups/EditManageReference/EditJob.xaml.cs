using CommunityToolkit.Maui.Views;
using HRMapp.ViewModels.EmployeeFormViewModel;

namespace HRMapp.Pages.EmployeeForms.Popups.EditManageReference;

public partial class EditJob : Popup
{
	public EditJob(object bindingContext)
	{
		InitializeComponent();
		BindingContext = bindingContext;
	}
}