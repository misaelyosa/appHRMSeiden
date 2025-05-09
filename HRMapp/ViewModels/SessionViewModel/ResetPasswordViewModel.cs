using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HRMapp.Pages.Session;
using HRMapp.ViewModels.SessionViewModel.Interface;
using Microsoft.Maui.Controls.PlatformConfiguration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMapp.ViewModels.SessionViewModel
{
    public partial class ResetPasswordViewModel : ObservableObject
    {
        private readonly ISessionService _sessionService;

        [ObservableProperty]
        private string username;
        [ObservableProperty]
        private string forgotPassToken;

        [ObservableProperty]
        private string newPassword;

        [ObservableProperty]
        public bool isValidate;
        [ObservableProperty]
        public bool isEnterNewPassword;

        private string? userId;

        public ResetPasswordViewModel(ISessionService sessionService)
        {
            _sessionService = sessionService;
            IsEnterNewPassword = false;
            IsValidate = true;
            Debug.WriteLine($"Isenternewpass = {IsEnterNewPassword}");
            Debug.WriteLine($"Isvalidate = {IsValidate}");
        }

        [RelayCommand]
        public async Task ValidateToken()
        {
            if (!string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(ForgotPassToken))
            {
                userId = await _sessionService.ValidateForgotPasswordToken(Username, ForgotPassToken);

                if (!string.IsNullOrEmpty(userId))
                {
                    IsValidate = false; 
                    IsEnterNewPassword = true; 
                }
                else
                {
                    IsValidate = true; 
                    IsEnterNewPassword = false;
                }
            }
        }

        [RelayCommand]
        public async Task ResetPassword()
        {
            if (!string.IsNullOrEmpty(NewPassword) && !string.IsNullOrEmpty(userId))
            {
                var newToken = await _sessionService.ResetPassword(userId, NewPassword);
                if (!string.IsNullOrEmpty(newToken))
                {
                    await Application.Current.MainPage.DisplayAlert("Catat Token baru untuk Reset Password", $"Password berhasil direset dan Token lama telah berhasil diperbaharui" +
                        $" silahkan tekan OK untuk copy dan simpan token berikut : {newToken}", "OK");
                    await Clipboard.Default.SetTextAsync(newToken);
                    await Application.Current.MainPage.DisplayAlert("Token Disalin",
                    "Token reset password telah disalin ke clipboard. Silakan tempel (paste) di tempat yang dibutuhkan.",
                    "OK");

                    var loginVm = new LoginViewModel(_sessionService);
                    var loginPage = new LoginPage(loginVm);
                    await Application.Current.MainPage.Navigation.PushAsync(loginPage);
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Gagal Mereset Password", "Reset password gagal silahkan coba lagi", "OK");
                    var resetPwVm = new ResetPasswordViewModel(_sessionService);
                    var resetPwPage = new ResetPasswordPage(resetPwVm);
                    await Application.Current.MainPage.Navigation.PushAsync(resetPwPage);
                }        
            }
        }
    }
}
