using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HRMapp.Data.Database;
using HRMapp.Pages;
using HRMapp.ViewModels.SessionViewModel.Interface;
using Microsoft.EntityFrameworkCore;
using Plugin.Maui.Calendar.Models;
using System.Collections.ObjectModel;

namespace HRMapp.ViewModels
{
    public partial class MainPageViewModel : ObservableObject
    {
        private readonly IDbContextFactory<AppDbContext> _dbContextFactory;
        private readonly ISessionService _sessionService;

        public EventCollection ContractEndEvent { get; set; } = new();
        public ObservableCollection<string> ContractEndPerMonth { get; set; } = new();

        public MainPageViewModel(IDbContextFactory<AppDbContext> dbContextFactory, ISessionService sessionService)
        {
            _dbContextFactory = dbContextFactory;
            _sessionService = sessionService;
        }

        public async Task LoadContractEndPerMonthAsync(DateTime shownDate)
        {
            ContractEndPerMonth.Clear();

            using var context = await _dbContextFactory.CreateDbContextAsync();
            var contracts = await context.Contracts
                .Include(c => c.Employee)
                .Where(c =>
                    c.end_date.Month == shownDate.Month &&
                    c.end_date.Year == shownDate.Year)
                .ToListAsync();

            foreach (var contract in contracts)
            {
                ContractEndPerMonth.Add($"Akhir Kontrak: {contract.Employee.name} ({contract.end_date:dd MMM yyyy})");
            }
        }

        //proxy shown date calendar
        private DateTime _calendarCurrentDate = DateTime.Today;
        public DateTime CalendarCurrentDate
        {
            get => _calendarCurrentDate;
            set
            {
                if (_calendarCurrentDate != value)
                {
                    _calendarCurrentDate = value;
                    OnPropertyChanged(nameof(CalendarCurrentDate));
                    _ = LoadContractEndPerMonthAsync(value); //detect perubahan selectedmonth
                }
            }
        }

        public async Task LoadContractEndDatesAsync()
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();

            var contracts = await context.Contracts
                .Include(c => c.Employee)
                .ToListAsync();

            ContractEndEvent.Clear();

            foreach (var contract in contracts)
            {
                var endDate = contract.end_date.ToDateTime(new TimeOnly(0, 0));
                var eventText = $"Akhir Kontrak: {contract.Employee.name}";

                if (ContractEndEvent.ContainsKey(endDate))
                {
                    var eventList = ContractEndEvent[endDate] as ObservableCollection<object>;
                    eventList?.Add(eventText);
                }
                else
                {
                    ContractEndEvent[endDate] = new ObservableCollection<object> { eventText };
                }
            }
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
