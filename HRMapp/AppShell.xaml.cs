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
            Routing.RegisterRoute("EmployeeForms/Edit", typeof(EditEmployeeForm));

        }
    }
}
