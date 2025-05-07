using HRMapp.Pages.Session;
using HRMapp.ViewModels.SessionViewModel;
using HRMapp.ViewModels.SessionViewModel.Interface;
using System.Diagnostics;

namespace HRMapp
{
    public partial class App : Application
    {
        public App(IServiceProvider serviceProvider)
        {
            InitializeComponent();

            _ = InitAsync(serviceProvider);
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                System.Diagnostics.Debug.WriteLine("Unhandled Exception: " + e.ExceptionObject);
            };

            TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                System.Diagnostics.Debug.WriteLine("Unobserved Task Exception: " + e.Exception);
            };
        }

        private async Task InitAsync(IServiceProvider serviceProvider)
        {
            var sessionService = serviceProvider.GetRequiredService<ISessionService>();
            bool isLoggedIn = await sessionService.IsUserLoggedInAsync();

            if (isLoggedIn)
            {
                MainPage = new AppShell();
                await Shell.Current.GoToAsync("//MainPage");
            }
            else
            {
                var loginVm = new LoginViewModel(sessionService);
                var loginPage = new LoginPage(loginVm);
                MainPage = new NavigationPage(loginPage);
            }
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}