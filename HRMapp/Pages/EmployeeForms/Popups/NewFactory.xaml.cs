using CommunityToolkit.Maui.Views;

namespace HRMapp.Pages.EmployeeForms.Popups;

public partial class NewFactory : Popup
{
	public NewFactory(object bindingContext)
	{
		InitializeComponent();
		BindingContext = bindingContext;
	}
}