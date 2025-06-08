using CommunityToolkit.Mvvm.ComponentModel;
using HRMapp.Data.Model;
using HRMapp.ViewModels.EmployeeFormViewModel;
using HRMapp.ViewModels.EmployeeFormViewModel.Interface;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMapp.ViewModels
{
    public partial class ManageReferenceDataViewModel : ObservableObject
    {
        private readonly IEmployeeService _employeeService;

        [ObservableProperty]
        private ObservableCollection<Department> departments = new();
        [ObservableProperty]
        private ObservableCollection<Job> jobs = new();
        [ObservableProperty]
        private ObservableCollection<Religion> religions = new();
        [ObservableProperty]
        private ObservableCollection<Factory> factories = new();

        public ManageReferenceDataViewModel(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
            _ = LoadAll();
        }

        private async Task LoadAll()
        {
            Departments = new ObservableCollection<Department>(await _employeeService.GetDepartment());
            Jobs = new ObservableCollection<Job>(await _employeeService.GetJob());
            Religions = new ObservableCollection<Religion>(await _employeeService.GetReligion());
            Factories = new ObservableCollection<Factory>(await _employeeService.GetFactory());
         }

    }
}
