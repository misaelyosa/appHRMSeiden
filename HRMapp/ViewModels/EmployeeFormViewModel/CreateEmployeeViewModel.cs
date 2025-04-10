using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HRMapp.Data.Database;
using HRMapp.Data.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMapp.ViewModels.EmployeeFormViewModel
{
    public partial class CreateEmployeeViewModel : ObservableObject
    {
        private readonly IEmployeeService _employeeService;
        private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

        [ObservableProperty]
        private Employee newEmployee = new();

        public CreateEmployeeViewModel(IEmployeeService employeeService, IDbContextFactory<AppDbContext> dbContextFactory)
        {
            _employeeService = employeeService;
            _dbContextFactory = dbContextFactory;
        }

        [RelayCommand]
        private async Task SubmitAsync()
        {

        }
    }
}
