using HRMapp.Data.Model;

namespace HRMapp.ViewModels.EmployeeFormViewModel
{
    public interface IEmployeeService
    {
        Task<Employee> GetEmployeeByIdAsync(int id);
        Task CreateEmployeeAsync(Employee employee);
        Task UpdateEmployeeAsync(Employee employee);
        Task DeleteEmployeeAsync(int id);
        Task<Contract>? GetContractDetail(int contractId);
        Task UpdateContractAsync(Contract contract);
    }
}