using CommunityToolkit.Maui.Views;

namespace HRMapp.Pages.EmployeeForms.Popups;

public partial class NewReligion : Popup

{
	public NewReligion(object bindingContext)
	{
		InitializeComponent();
		BindingContext = bindingContext;
	}
}