using CommunityToolkit.Maui.Views;
using HRMapp.ViewModels.EmployeeFormViewModel;

namespace HRMapp.Pages.EmployeeForms.Popups.EditManageReference;


public partial class EditCityProvince : Popup
{
    public EditCityProvince(object bindingContext)
    {
        InitializeComponent();
        BindingContext = bindingContext;
    }

}
