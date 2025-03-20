using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HRMapp.Data.Database;
using HRMapp.Pages;
using Microsoft.EntityFrameworkCore;

namespace HRMapp.ViewModels
{
    public partial class MainPageViewModel : ObservableObject
    {
        private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

        public MainPageViewModel(IDbContextFactory<AppDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        [RelayCommand]
        private async Task NavigateToManageEmployee()
        {
            await Shell.Current.GoToAsync(nameof(ManageEmployee));
        }
    
    }
}
