using CommunityToolkit.Maui.Views;
using HRMapp.ViewModels.EmployeeFormViewModel;

namespace HRMapp.Pages.EmployeeForms.Popups;


public partial class NewCityProvince : Popup
{
    public NewCityProvince(object bindingContext)
    {
        InitializeComponent();
        BindingContext = bindingContext;
    }

}
