using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HRMapp.Pages.Session;
using HRMapp.ViewModels.SessionViewModel.Interface;
using Microsoft.Maui.Controls.PlatformConfiguration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMapp.ViewModels.SessionViewModel
{
    public partial class SignupViewModel : ObservableObject
    {
        private readonly ISessionService _sessionService;
        public SignupViewModel(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        [ObservableProperty]
        private string username;
        [ObservableProperty]
        private string password;
        [ObservableProperty]
        private string authority;

        [RelayCommand]
        public async Task NavigateToLoginPage()
        {
   
            var loginVm = new LoginViewModel(_sessionService);
            var loginPage = new LoginPage(loginVm);

            await Application.Current.MainPage.Navigation.PushAsync(loginPage);
        }

        [RelayCommand]
        public async Task RegisterUserAsync()
        {
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                var token = await _sessionService.RegisterAsync(username, password, authority);
                if (!string.IsNullOrEmpty(token))
                {
                    await Application.Current.MainPage.DisplayAlert("Catat Token untuk Reset Password", $"Token hanya berlaku 1 kali dan akan diperbarui setelah pemakaian" +
                   $" silahkan tekan OK untuk copy dan simpan token berikut : {token}", "OK");
                    await Clipboard.Default.SetTextAsync(token);
                    await Application.Current.MainPage.DisplayAlert("Token Disalin",
                    "Token reset password telah disalin ke clipboard. Silakan tempel (paste) di tempat yang dibutuhkan.",
                    "OK");
                    NavigateToLoginPage();
                }
            }
        }
    }
}
