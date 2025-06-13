using CommunityToolkit.Maui.Views;

namespace HRMapp.Pages.EmployeeForms.Popups.CutiPopup;

public partial class NewCuti : Popup
{
	public NewCuti(object bindingContext)
	{
		InitializeComponent();
		BindingContext = bindingContext;
	}
}