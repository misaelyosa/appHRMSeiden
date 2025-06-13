using CommunityToolkit.Maui.Views;
using HRMapp.ViewModels.EmployeeFormViewModel;

namespace HRMapp.Pages.EmployeeForms.Popups;

public partial class NewEducation : Popup
{
	public NewEducation(object bindingContext)
	{
		InitializeComponent();
		BindingContext = bindingContext;
	}
}