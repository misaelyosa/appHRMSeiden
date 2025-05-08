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

            MainPage = new ContentPage
            {
                Content = new ActivityIndicator
                {
                    IsRunning = true,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center
                }
            };

            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                System.Diagnostics.Debug.WriteLine("Unhandled Exception: " + e.ExceptionObject);
            };

            TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                System.Diagnostics.Debug.WriteLine("Unobserved Task Exception: " + e.Exception);
            };
            _ = InitAsync(serviceProvider);
        }

        private async Task InitAsync(IServiceProvider serviceProvider)
        {
            try
            { 
                var sessionService = serviceProvider.GetRequiredService<ISessionService>();
                bool isLoggedIn = await sessionService.IsUserLoggedInAsync();

                if (isLoggedIn)
                {
                    MainPage = new AppShell(sessionService);
                    //await Shell.Current.GoToAsync("//MainPage");
                }
                else
                {
                    var loginVm = new LoginViewModel(sessionService);
                    var loginPage = new LoginPage(loginVm);
                    MainPage = new NavigationPage(loginPage);
                }
            } catch (Exception ex)
            {
                Debug.WriteLine($"Exception during InitAsync: {ex}");

                MainPage = new ContentPage
                {
                    Content = new Label
                    {
                        Text = "Startup failed. Please restart the app.",
                        TextColor = Colors.Red,
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.Center
                    }
                };
            }
        }
    }
}