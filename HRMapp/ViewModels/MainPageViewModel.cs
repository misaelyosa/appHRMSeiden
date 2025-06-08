using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HRMapp.Data.Database;
using HRMapp.Data.Model;
using HRMapp.Pages;
using HRMapp.ViewModels.SessionViewModel.Interface;
using Microsoft.EntityFrameworkCore;
using Plugin.Maui.Calendar.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http.Json;

namespace HRMapp.ViewModels
{
    public partial class MainPageViewModel : ObservableObject
    {
        private readonly IDbContextFactory<AppDbContext> _dbContextFactory;
        private readonly ISessionService _sessionService;

        public EventCollection CalendarEvents { get; set; } = new();
        public ObservableCollection<ContractEndItem> ContractEndPerMonth { get; set; } = new();
        public ObservableCollection<string> HariLiburNasionalPerMonth { get; set; } = new();
        public HashSet<DateTime> HolidayDates { get; set; } = new();
        [ObservableProperty]
        private ContractEndItem? selectedContractEnd;

        public MainPageViewModel(IDbContextFactory<AppDbContext> dbContextFactory, ISessionService sessionService)
        {
            _dbContextFactory = dbContextFactory;
            _sessionService = sessionService;
        }

        public async Task RefreshHolidayDataAsync(DateTime selectedDate, bool forceReload)
        {
            await LoadHariLibur(selectedDate.Year, forceReload);
            await UpdateHariLiburNasionalPerMonth(selectedDate);
        }

        public async Task LoadContractEndPerMonthAsync(DateTime shownDate)
        {
            ContractEndPerMonth.Clear();

            using var context = await _dbContextFactory.CreateDbContextAsync();
            var contracts = await context.Contracts
                .Include(c => c.Employee)
                .Include(c => c.Employee.Department)
                .Include(c => c.Employee.Job)
                .Where(c =>
                    c.end_date.Month == shownDate.Month &&
                    c.end_date.Year == shownDate.Year)
                .ToListAsync();

            foreach (var contract in contracts)
            {
                ContractEndPerMonth.Add(new ContractEndItem
                {
                    DisplayText = $"{contract.end_date:dd MMMM yyyy} : {contract.Employee.name} ({contract.Employee.Department.name}, {contract.Employee.Job.job_name})",
                    EmployeeId = contract.Employee.employee_id
                });
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
                    Debug.WriteLine($"[DEBUG] CalendarCurrentDate changed to: {value}");
                    _ = LoadContractEndPerMonthAsync(value);
                    _ = RefreshHolidayDataAsync(value, false);
                }
            }
        }

        public async Task LoadContractEndDatesAsync()
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();

            var contracts = await context.Contracts
                .Include(c => c.Employee)
                .ToListAsync();

            CalendarEvents.Clear();

            foreach (var contract in contracts)
            {
                var endDate = contract.end_date.ToDateTime(new TimeOnly(0, 0));
                var item = new CalendarEventItem
                {
                    Title = $"Akhir Kontrak: {contract.Employee.name}",
                    IsHoliday = false
                };

                if (!CalendarEvents.ContainsKey(endDate))
                    CalendarEvents[endDate] = new ObservableCollection<object>();

                (CalendarEvents[endDate] as ObservableCollection<object>)?.Add(item);
            }
        }

        private HashSet<int> loadedYear = new();
        public async Task LoadHariLibur(int year, bool forceReload)
        {
            if (loadedYear.Contains(year) && !forceReload)
            {
                Debug.WriteLine($"[DEBUG] Holidays for {year} already loaded.");
                return;
            }

            if(loadedYear.Contains(year) && forceReload)
            {
                loadedYear.Clear();
                var toRemove = HolidayDates.Where(d => d.Year == year).ToList();
                var count = 0;
                foreach (var d in toRemove)
                {
                    count++;
                    HolidayDates.Remove(d);
                    CalendarEvents.Remove(d);
                }
                HariLiburNasionalPerMonth.Clear();
                Debug.WriteLine($"Forced Reload, removed {year}, removed {count} holidays.");
                count = 0;
            }

            using var client = new HttpClient();
            var url = $"https://api-harilibur.vercel.app/api?year={year}";

            try
            {
                var hariLiburNasional = await client.GetFromJsonAsync<List<HolidayDTO>>(url);
                Debug.WriteLine($"[DEBUG] API returned {hariLiburNasional?.Count ?? 0} holidays");
                if (hariLiburNasional is null) return;

                foreach (var libur in hariLiburNasional)
                {
                    if (!libur.is_national_holiday) //skip libur cultural
                        continue;

                    var formatted = libur.holiday_date
                        .Split('-')
                        .Select(s => s.PadLeft(2, '0'))
                        .ToArray();

                    var normalizedDate = $"{formatted[0]}-{formatted[1]}-{formatted[2]}"; // yyyy-MM-dd

                    if (DateTime.TryParse(normalizedDate, out var tanggalLibur))
                    {
                        var item = new CalendarEventItem
                        {
                            Title = $"🎉 : {libur.holiday_name}",
                            IsHoliday = true
                        };
                        HolidayDates.Add(tanggalLibur.Date);

                        if (!CalendarEvents.ContainsKey(tanggalLibur))
                            CalendarEvents[tanggalLibur] = new ObservableCollection<object>();

                        (CalendarEvents[tanggalLibur] as ObservableCollection<object>)?.Add(item);
                    }

                    loadedYear.Add(year);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to load holidays: {ex.Message}");
            }
        }

        public async Task UpdateHariLiburNasionalPerMonth(DateTime selectedDate)
        {
            HariLiburNasionalPerMonth.Clear();

            foreach (var date in HolidayDates.OrderBy(d => d))
            {
                if (date.Month == selectedDate.Month && date.Year == selectedDate.Year)
                {
                    if (CalendarEvents.TryGetValue(date, out var eventList))
                    {
                        foreach (var item in eventList)
                        {
                            if (item is CalendarEventItem evt && evt.IsHoliday)
                            {
                                HariLiburNasionalPerMonth.Add($"{date:dd MMMM yyyy} - {evt.Title}");
                            }
                        }
                    }
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

        [RelayCommand]
        private async Task NavigateToEmployeeDetail(ContractEndItem selected)
        {
            if (selected is null)
                return;

            await Shell.Current.GoToAsync($"EmployeeDetailPage?employeeId={selected.EmployeeId}");
            SelectedContractEnd = null;
        }
    }
}
