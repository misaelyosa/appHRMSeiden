using CommunityToolkit.Maui.Views;
using HRMapp.ViewModels.EmployeeFormViewModel;

namespace HRMapp.Pages.EmployeeForms.Popups.EditManageReference;

public partial class EditReligion : Popup
{
	public EditReligion(object bindingContext)
	{
		InitializeComponent();
		BindingContext = bindingContext;
	}
}