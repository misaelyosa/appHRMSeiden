//using CommunityToolkit.Mvvm.ComponentModel;
//using HRMapp.Data.Database;
//using HRMapp.Data.Model;
//using System.Collections.ObjectModel;

//namespace HRMapp.ViewModels
//{
//    public partial class EmployeeViewModel : ObservableObject
//    {
//        [ObservableProperty]
//        private ObservableCollection<Employee> employees;

//        public EmployeeViewModel()
//        {
//            LoadEmployees();
//        }

//        //private void LoadEmployees()
//        //{
//        //    using (var context = new AppDbContext())
//        //    {
//        //        var employeesFromDb = context.Employees.ToList();
//        //        employees = new ObservableCollection<Employee>(employeesFromDb);
//        //    }
//        //}
//    }
//}
