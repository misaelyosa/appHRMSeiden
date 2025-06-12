using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HRMapp.Data.Database;
using HRMapp.Data.Model;
using HRMapp.Pages;
using HRMapp.Pages.EmployeeForms;
using HRMapp.ViewModels.EmployeeFormViewModel;
using HRMapp.ViewModels.EmployeeFormViewModel.Interface;
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
        private ObservableCollection<string> factoryName = new();

        [ObservableProperty]
        private string searchText = string.Empty;

        [ObservableProperty]
        private Employee selectedEmployee;

        [ObservableProperty]
        private string selectedDepartment;

        [ObservableProperty]
        private string selectedJob;        
        
        [ObservableProperty]
        private string selectedFactory;

        [ObservableProperty]
        private bool isRefreshing;

        [ObservableProperty]
        private bool isFilterVisible;

        public string FilterToggleText => IsFilterVisible ? "▲ Hide" : "▼ Show";


        public EmployeeListViewModel(IDbContextFactory<AppDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
            OnSelectedEmployeeChanged(SelectedEmployee);

            IsFilterVisible = true; 
            ToggleFilter();
        }

        [RelayCommand]
        public async Task LoadEmployeeAsync()
        {
            using var dbContext = _dbContextFactory.CreateDbContext();

            var employeeList = await dbContext.Employees
                .AsNoTracking()
                .Include(e => e.Department)
                .Include(e => e.Job)
                .Include(e => e.Factory)
                .OrderBy(e => e.employee_id)
                .ToListAsync();

            Employees = new ObservableCollection<Employee>(employeeList);
            OnPropertyChanged(nameof(Employees));

        }

        [RelayCommand]
        public async Task RefreshData()
        {
            IsRefreshing = true;
            await ResetFilter();
            await Task.Delay(100);
            IsRefreshing = false;
        }

        [RelayCommand]
        private async Task NavigateToCreateForm()
        {
            await Shell.Current.GoToAsync(nameof(CreateEmployeeForm));
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

            if (string.IsNullOrEmpty(SelectedDepartment))
                SelectedDepartment = DepartmentsName.FirstOrDefault();
        }

        [RelayCommand]
        public async Task LoadJobsAsync()
        {
            using var dbContext = _dbContextFactory.CreateDbContext();

            var jobsName = await dbContext.Jobs
                .Select(c => c.job_name)
                .ToListAsync();
            jobsName.Insert(0, "none");

            JobsName = new ObservableCollection<string>(jobsName);
            OnPropertyChanged(nameof(JobsName));

            if (string.IsNullOrEmpty(SelectedJob))
                SelectedJob = JobsName.FirstOrDefault();
        }        
        
        [RelayCommand]
        public async Task LoadFactoryAsync()
        {
            using var dbContext = _dbContextFactory.CreateDbContext();

            var factoryName = await dbContext.Factories
                .Select(c => c.name)
                .ToListAsync();
            factoryName.Insert(0, "none");

            FactoryName = new ObservableCollection<string>(factoryName);
            OnPropertyChanged(nameof(FactoryName));

            if (string.IsNullOrEmpty(SelectedFactory))
                SelectedFactory = FactoryName.FirstOrDefault();
        }

        //Filter Command
        [RelayCommand]
        public async Task SearchFilter()
        {
            using var dbContext = _dbContextFactory.CreateDbContext();

            if (string.IsNullOrWhiteSpace(SearchText))
            {
                LoadEmployeeAsync();
            }
            else
            {
                SelectedDepartment = "none";
                SelectedJob = "none";
                SelectedFactory = "none";

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
        public async Task ApplyFilter()
        {
            SearchText = string.Empty;
            using var dbContext = _dbContextFactory.CreateDbContext();

            var query = dbContext.Employees
                .AsNoTracking()
                .Include(e => e.Department)
                .Include(e => e.Job)
                .Include(e => e.Factory)
                .AsQueryable();

            if (!string.IsNullOrEmpty(SelectedDepartment) && SelectedDepartment != "none")
            {
                query = query.Where(e => e.Department != null && e.Department.name == SelectedDepartment);
            }

            if (!string.IsNullOrEmpty(SelectedJob) && SelectedJob != "none")
            {
                query = query.Where(e => e.Job != null && e.Job.job_name == SelectedJob);
            }

            if (!string.IsNullOrEmpty(SelectedFactory) && SelectedFactory != "none")
            {
                query = query.Where(e => e.Factory != null && e.Factory.name == SelectedFactory);
            }

            var filteredEmployees = await query.OrderBy(e => e.nip).ToListAsync();

            Employees = new ObservableCollection<Employee>(filteredEmployees);
            OnPropertyChanged(nameof(Employees));
        }

        [RelayCommand]
        private async Task ResetFilter()
        {
            SelectedDepartment = "none";
            SelectedJob = "none";
            SelectedFactory = "none";
            SearchText = string.Empty;

            await LoadEmployeeAsync();
        } 

        [RelayCommand]        
        private void ToggleFilter()
        {
            IsFilterVisible = !IsFilterVisible;
            OnPropertyChanged(nameof(FilterToggleText));
        }
    }
}
