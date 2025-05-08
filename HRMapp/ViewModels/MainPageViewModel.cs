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
