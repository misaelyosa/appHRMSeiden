using HRMapp.Pages;
using HRMapp.Pages.EmployeeForms;

namespace HRMapp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(EmployeeDetailPage), typeof(EmployeeDetailPage));
            Routing.RegisterRoute(nameof(ManageEmployee), typeof(ManageEmployee));

            //Employee form
            Routing.RegisterRoute("EmployeeForms/Edit", typeof(EditEmployeeForm));
            Routing.RegisterRoute(nameof(CreateEmployeeForm), typeof(CreateEmployeeForm));

            Routing.RegisterRoute("GeneratePKWTPage", typeof(GeneratePKWTPage));
            Routing.RegisterRoute("CreateContractForm", typeof(CreateContractForm));

        }
    }
}
