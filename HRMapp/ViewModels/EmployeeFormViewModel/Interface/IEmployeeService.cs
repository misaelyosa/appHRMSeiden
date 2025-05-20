using HRMapp.Data.Model;

namespace HRMapp.ViewModels.EmployeeFormViewModel
{
    public interface IEmployeeService
    {
        Task<Employee> GetEmployeeByIdAsync(int id);
        Task CreateEmployeeAsync(Employee employee);
        Task UpdateEmployeeAsync(Employee employee);
        Task DeleteEmployeeAsync(int id);
        Task<string?> AutoGenerateNip(string selectedFactory);
        Task<Contract>? GetContractDetail(int contractId);
        Task UpdateContractAsync(Contract contract);
        Task CreateContractAsync(Contract contract);
        Task DeleteContractAsync(Contract contract);

        Task<int> GetContractCountByEmployeeIdAsync(int employeeId);
        Task CreateTunjanganAsync(Tunjangan tunjangan);
        Task UpdateTunjanganAsync(Tunjangan tunjangan);
        Task <Tunjangan> GetTunjanganMK(int contractId);
        Task <Tunjangan> GetTunjanganOther(int contractId);

        Task AddNewCityProvince(string newCity, string newProvince);
    }
}