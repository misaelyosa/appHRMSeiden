using CommunityToolkit.Maui.Views;

namespace HRMapp.Pages.EmployeeForms.Popups;

public partial class NewJob : Popup
{
	public NewJob(object bindingContext)
	{
		InitializeComponent();
		BindingContext = bindingContext;
	}
}