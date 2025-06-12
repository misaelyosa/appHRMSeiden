using HRMapp.Pages;
using HRMapp.Pages.EmployeeForms;
using HRMapp.Pages.Session;
using HRMapp.ViewModels.SessionViewModel;
using HRMapp.ViewModels.SessionViewModel.Interface;
using System.Diagnostics;

namespace HRMapp
{
    public partial class AppShell : Shell
    {
        private readonly ISessionService _sessionService;
        public AppShell(ISessionService sessionService)
        {
            try
            {
                InitializeComponent();
                _sessionService = sessionService;
                BindingContext = this;

                Routing.RegisterRoute(nameof(EmployeeDetailPage), typeof(EmployeeDetailPage));
                Routing.RegisterRoute(nameof(ManageEmployee), typeof(ManageEmployee));
                Routing.RegisterRoute("MainPage", typeof(MainPage));
                Routing.RegisterRoute("ManageReferenceDataPage", typeof(ManageReferenceDataPage));
                Routing.RegisterRoute("ManageSessionUser", typeof(ManageSessionUser));

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
                Debug.WriteLine($"AppShell initialization error: {ex}");
                throw;
            }
        }
        public string Username => _sessionService.Username;
        public void RefreshUsernameBinding()
        {
            OnPropertyChanged(nameof(Username));
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            await _sessionService.LogoutAsync();
            var loginVm = new LoginViewModel(_sessionService);
            Application.Current.MainPage = new NavigationPage(new LoginPage(loginVm));
        }
    }
}
