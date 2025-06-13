using CommunityToolkit.Maui.Views;

namespace HRMapp.Pages.EmployeeForms.Popups.CutiPopup;

public partial class EditCuti : Popup
{
    public EditCuti(object bindingContext)
    {
        InitializeComponent();
        BindingContext = bindingContext;
    }
}