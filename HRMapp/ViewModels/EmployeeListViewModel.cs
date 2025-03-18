using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HRMapp.Data.Database;
using HRMapp.Data.Model;
using HRMapp.Pages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HRMapp.ViewModels
{
    public partial class EmployeeListViewModel : ObservableObject
    {
        private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

        [ObservableProperty]
        private ObservableCollection<Employee> employees = new();

        [ObservableProperty]
        private ObservableCollection<Employee> filteredEmployees = new();

        [ObservableProperty]
        private string searchText = string.Empty;

        [ObservableProperty]
        private Employee selectedEmployee;

        public ICommand LoadMoreCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand EmployeeSelectedCommand { get; }

        public EmployeeListViewModel(IDbContextFactory<AppDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
            LoadMoreCommand = new AsyncRelayCommand(LoadEmployeeAsync);
            SearchCommand = new RelayCommand(FilterEmployees);
            EmployeeSelectedCommand = new AsyncRelayCommand(NavigateToEmployeeDetail);
        }

        private int _pageSize = 20;
        private int _currentPage = 1;
        private bool _isLoading = false;

        [RelayCommand]
        private async Task LoadEmployeeAsync()
        {
            if (_isLoading) return;
            _isLoading = true;

            try
            {
                using var dbContext = _dbContextFactory.CreateDbContext();

                var employeeList = await dbContext.Employees
                    .AsNoTracking()
                    .Include(e => e.Department)
                    .Include(e => e.Job)
                    .Include(e => e.Factory)
                    .OrderBy(e => e.employee_id)
                    .Skip((_currentPage - 1) * _pageSize)
                    .Take(_pageSize)
                    .ToListAsync();

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (_currentPage == 1)
                        Employees.Clear();
                    foreach (var emp in employeeList)
                        Employees.Add(emp);
                });

                FilterEmployees();

                _currentPage++;

                //employees = new ObservableCollection<Employee>(employeeList);
                //OnPropertyChanged(nameof(Employees));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading employees: {ex.Message}");
            }
            finally
            {
                _isLoading = false;
            }
        }

        private void FilterEmployees()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                // If search text is empty, show full list
                FilteredEmployees = new ObservableCollection<Employee>(Employees);
            }
            else
            {
                var lowerText = SearchText.ToLower();
                FilteredEmployees = new ObservableCollection<Employee>(
                    Employees.Where(e =>
                        e.name.ToLower().Contains(lowerText) ||
                        e.nip.ToLower().Contains(lowerText) ||
                        (e.Department != null && e.Department.name.ToLower().Contains(lowerText)) ||
                        (e.Job != null && e.Job.job_name.ToLower().Contains(lowerText))
                    )
                );
            }

        }

        private async Task NavigateToEmployeeDetail()
        {
            if (SelectedEmployee == null) return;

            Debug.WriteLine($"Navigating to EmployeeDetailPage with ID: {SelectedEmployee.employee_id}");

            await Shell.Current.GoToAsync($"/{nameof(EmployeeDetailPage)}?employeeId={SelectedEmployee.employee_id}");

        }
    }
}
