using HRMapp.Pages;

namespace HRMapp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(EmployeeDetailPage), typeof(EmployeeDetailPage));
            Routing.RegisterRoute(nameof(ManageEmployee), typeof(ManageEmployee));
        }
    }
}
