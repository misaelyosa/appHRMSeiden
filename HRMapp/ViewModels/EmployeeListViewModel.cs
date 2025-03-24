using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HRMapp.Data.Database;
using HRMapp.Data.Model;
using HRMapp.Pages;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace HRMapp.ViewModels
{
    public partial class EmployeeListViewModel : ObservableObject
    {
        private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

        [ObservableProperty]
        private ObservableCollection<Employee> employees = new();

        [ObservableProperty]
        private ObservableCollection<string> departmentsName = new();

        [ObservableProperty]
        private ObservableCollection<string> jobsName = new();

        [ObservableProperty]
        private string searchText = string.Empty;

        [ObservableProperty]
        private Employee selectedEmployee;

        [ObservableProperty]
        private string selectedDepartment;

        [ObservableProperty]
        private string selectedJob;

        [ObservableProperty]
        private bool isRefreshing;

        [ObservableProperty]
        private bool isFilterVisible;
        public string FilterToggleText => IsFilterVisible ? "▲ Hide" : "▼ Show";


        public EmployeeListViewModel(IDbContextFactory<AppDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
            LoadDepartmentAsync();
            LoadJobsAsync();
            LoadEmployeeAsync();
            SearchFilter();
            OnSelectedEmployeeChanged(SelectedEmployee);
            RefreshData();

            IsFilterVisible = true; 
            ToggleFilter();
            ApplyFilter();
        }

        [RelayCommand]
        private async Task LoadEmployeeAsync()
        {
            using var dbContext = _dbContextFactory.CreateDbContext();

            var employeeList = await dbContext.Employees
                .AsNoTracking()
                .Include(e => e.Department)
                .Include(e => e.Job)
                .Include(e => e.Factory)
                .OrderBy(e => e.employee_id)
                .ToListAsync();

            employees = new ObservableCollection<Employee>(employeeList);
            OnPropertyChanged(nameof(Employees));

        }

        [RelayCommand]
        private async Task RefreshData()
        {
            IsRefreshing = true;
            await LoadEmployeeAsync();
            await Task.Delay(100);
            IsRefreshing = false;
        }

        private async Task NavigateToEmployeeDetail(int employeeId)
        {
            Debug.WriteLine($"Navigating to EmployeeDetailPage with ID: {employeeId}");

            await Shell.Current.GoToAsync($"/{nameof(EmployeeDetailPage)}?employeeId={employeeId}");

        }

        partial void OnSelectedEmployeeChanged(Employee value)
        {
            if (value != null)
            {
                var selectedEmpId = value.employee_id;

                NavigateToEmployeeDetail(selectedEmpId);
            }
        }

        //PICKER DATA
        [RelayCommand]
        public async Task LoadDepartmentAsync()
        {
            using var dbContext = _dbContextFactory.CreateDbContext();

            var departments = await dbContext.Departments
                .Select(c => c.name)
                .ToListAsync();
            departments.Insert(0, "none");

            DepartmentsName = new ObservableCollection<string>(departments);
            OnPropertyChanged(nameof(DepartmentsName));

            SelectedDepartment = DepartmentsName.FirstOrDefault();
        }

        [RelayCommand]
        private async Task LoadJobsAsync()
        {
            using var dbContext = _dbContextFactory.CreateDbContext();

            var jobsName = await dbContext.Jobs
                .Select(c => c.job_name)
                .ToListAsync();
            jobsName.Insert(0, "none");

            JobsName = new ObservableCollection<string>(jobsName);
            OnPropertyChanged(nameof(JobsName));

            SelectedJob = JobsName.FirstOrDefault();
        }

        //Filter Command
        [RelayCommand]
        private async Task SearchFilter()
        {
            using var dbContext = _dbContextFactory.CreateDbContext();

            if (string.IsNullOrWhiteSpace(SearchText))
            {
                LoadEmployeeAsync();
            }
            else
            {
                var lowerText = SearchText.ToLower();

                var filteredEmployeeList = await dbContext.Employees
                    .AsNoTracking()
                    .Include(e => e.Department)
                    .Include(e => e.Job)
                    .Include(e => e.Factory)
                    .Where(e =>
                        e.name.ToLower().Contains(lowerText) ||
                        e.nip.ToLower().Contains(lowerText) ||
                        e.nik.ToLower().Contains(lowerText) ||
                        (e.Department != null && e.Department.name.ToLower().Contains(lowerText)) ||
                        (e.Job != null && e.Job.job_name.ToLower().Contains(lowerText))
                    )
                    .ToListAsync();

                Employees = new ObservableCollection<Employee>(filteredEmployeeList);
                OnPropertyChanged(nameof(Employees));
            }
        }

        [RelayCommand]
        private async Task ApplyFilter()
        {
            using var dbContext = _dbContextFactory.CreateDbContext();

            var query = dbContext.Employees
                .AsNoTracking()
                .Include(e => e.Department)
                .Include(e => e.Job)
                .Include(e => e.Factory)
                .AsQueryable();

            if (!string.IsNullOrEmpty(SelectedDepartment) && SelectedDepartment != "none")
            {
                query = query.Where(e => e.Department != null && e.Department.name == selectedDepartment);
            }

            if (!string.IsNullOrEmpty(selectedJob) && selectedJob != "none")
            {
                query = query.Where(e => e.Job != null && e.Job.job_name == selectedJob);
            }

            var filteredEmployees = await query.OrderBy(e => e.employee_id).ToListAsync();

            Employees = new ObservableCollection<Employee>(filteredEmployees);
            OnPropertyChanged(nameof(Employees));
        }

        [RelayCommand]        
        private void ToggleFilter()
        {
            IsFilterVisible = !IsFilterVisible;
            OnPropertyChanged(nameof(FilterToggleText));
        }
    }
}
