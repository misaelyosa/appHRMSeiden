using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HRMapp.Data.Database;
using HRMapp.Pages;
using HRMapp.ViewModels.SessionViewModel.Interface;
using Microsoft.EntityFrameworkCore;

namespace HRMapp.ViewModels
{
    public partial class MainPageViewModel : ObservableObject
    {
        private readonly IDbContextFactory<AppDbContext> _dbContextFactory;
        private readonly ISessionService _sessionService;

        public MainPageViewModel(IDbContextFactory<AppDbContext> dbContextFactory, ISessionService sessionService)
        {
            _dbContextFactory = dbContextFactory;
            _sessionService = sessionService;
        }

        [ObservableProperty]
        private bool isAdmin;

        [RelayCommand]
        public async Task CheckIsAdmin()
        {
            IsAdmin = await _sessionService.CheckIsAdmin();
        }

        [RelayCommand]
        public async Task GetResetToken()
        {
            var resetToken = await _sessionService.GetResetToken();
            if (!string.IsNullOrEmpty(resetToken))
            {
                await Clipboard.Default.SetTextAsync(resetToken);
                await Application.Current.MainPage.DisplayAlert("Token Disalin",
                $"Token reset password ({resetToken}) telah disalin ke clipboard. Silakan tempel (paste) di tempat yang dibutuhkan.",
                "OK");
            } 
            else
            {
                await Application.Current.MainPage.DisplayAlert("Token tidak ditemukan",
                "Token reset password tidak ditemukan, coba logout dan coba kembali.",
                "OK");
            }
        }
        
        [RelayCommand]
        private async Task NavigateToManageEmployee()
        {
            await Shell.Current.GoToAsync(nameof(ManageEmployee));
        }

        [RelayCommand]
        private async Task NavigateToRegister()
        {
            await Shell.Current.GoToAsync("SignupPage");
        }
    
    }
}
