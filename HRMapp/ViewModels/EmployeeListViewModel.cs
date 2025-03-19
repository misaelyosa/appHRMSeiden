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
        private string searchText = string.Empty;

        [ObservableProperty]
        private Employee selectedEmployee;

        
        public ICommand EmployeeSelectedCommand { get; }

        public EmployeeListViewModel(IDbContextFactory<AppDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
            LoadEmployeeAsync();
            EmployeeSelectedCommand = new AsyncRelayCommand(NavigateToEmployeeDetail);
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

        private async Task NavigateToEmployeeDetail()
        {
            if (SelectedEmployee == null) return;

            Debug.WriteLine($"Navigating to EmployeeDetailPage with ID: {SelectedEmployee.employee_id}");

            await Shell.Current.GoToAsync($"/{nameof(EmployeeDetailPage)}?employeeId={SelectedEmployee.employee_id}");

        }
    }
}
