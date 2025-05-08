using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HRMapp.Pages;
using HRMapp.ViewModels.SessionViewModel.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMapp.ViewModels.SessionViewModel
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly ISessionService _sessionService;
        
        public LoginViewModel(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        [ObservableProperty]
        public string username;
        [ObservableProperty]
        public string password; 

        [RelayCommand]
        public async Task LoginAsync()
        {
            bool success = await _sessionService.LoginAsync(Username, Password);
            
            if (success)
            {
                Application.Current.MainPage = new AppShell(_sessionService);
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Login gagal", "username atau password salah", "OK");
            }
        }
    }
}
