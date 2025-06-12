using CommunityToolkit.Mvvm.ComponentModel;
using HRMapp.ViewModels.EmployeeFormViewModel;
using HRMapp.ViewModels.EmployeeFormViewModel.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMapp.ViewModels.Cuti
{
    public partial class ManageCutiViewModel : ObservableObject
    {
        private readonly IEmployeeService _employeeService;
        public ManageCutiViewModel(IEmployeeService employeeService)
        {
            employeeService = _employeeService;
        }
    }
}
