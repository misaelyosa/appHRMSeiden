using CommunityToolkit.Maui.Views;

namespace HRMapp.Pages.EmployeeForms.Popups;

public partial class NewDepartment : Popup
{
	public NewDepartment(object bindingContext)
	{
		InitializeComponent();
		BindingContext = bindingContext;
	}
}