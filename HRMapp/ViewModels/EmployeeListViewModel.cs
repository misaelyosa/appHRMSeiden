using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HRMapp.Data.Database;
using HRMapp.Data.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMapp.ViewModels
{
    public partial class EmployeeListViewModel : ObservableObject
    {
        IDbContextFactory<AppDbContext> _dbContextFactory;

        [ObservableProperty]
        private ObservableCollection<Employee> employees = new();

        public EmployeeListViewModel(IDbContextFactory<AppDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
            LoadEmployeeAsync();
        }

        [RelayCommand]
        private async Task LoadEmployeeAsync()
        {
            using var dbContext = _dbContextFactory.CreateDbContext();

            var employeeList = await dbContext.Employees
                .Include(e => e.Department)
                .Include(d => d.Job)
                .Include(c => c.Factory)
                .ToListAsync();

            employees = new ObservableCollection<Employee>(employeeList);
            OnPropertyChanged(nameof(Employees));
        }

    }
}
