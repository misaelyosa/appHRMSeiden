using HRMapp.Pages;
using HRMapp.Pages.EmployeeForms;
using HRMapp.Pages.Session;
using System.Diagnostics;

namespace HRMapp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            try
            {
                InitializeComponent();
                Routing.RegisterRoute(nameof(EmployeeDetailPage), typeof(EmployeeDetailPage));
                Routing.RegisterRoute(nameof(ManageEmployee), typeof(ManageEmployee));
                Routing.RegisterRoute("MainPage", typeof(MainPage));

                //Employee form
                Routing.RegisterRoute("EmployeeForms/Edit", typeof(EditEmployeeForm));
                Routing.RegisterRoute(nameof(CreateEmployeeForm), typeof(CreateEmployeeForm));

                Routing.RegisterRoute("GeneratePKWTPage", typeof(GeneratePKWTPage));
                Routing.RegisterRoute("CreateContractForm", typeof(CreateContractForm));

                //Session route 
                Routing.RegisterRoute("LoginPage", typeof(LoginPage));
                Routing.RegisterRoute("SignupPage", typeof(SignupPage));
                Routing.RegisterRoute("ResetPasswordPage", typeof(ResetPasswordPage));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ AppShell initialization error: {ex}");
                throw;
            }
        }
    }
}
