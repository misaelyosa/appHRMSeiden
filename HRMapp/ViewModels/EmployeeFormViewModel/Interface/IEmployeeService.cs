using HRMapp.Data.Model;

namespace HRMapp.ViewModels.EmployeeFormViewModel
{
    public interface IEmployeeService
    {
        Task<Employee> GetEmployeeByIdAsync(int id);
        Task UpdateEmployeeAsync(Employee employee);
        Task DeleteEmployeeAsync(int id);
    }
}