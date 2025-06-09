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
        Task<Contract?> GetLastIndexContractDate(int contractIndex, int employeeId);
        Task<int> GetContractCountByEmployeeIdAsync(int employeeId);
        Task CreateTunjanganAsync(Tunjangan tunjangan);
        Task UpdateTunjanganAsync(Tunjangan tunjangan);
        Task <Tunjangan> GetTunjanganMK(int contractId);
        Task <Tunjangan> GetTunjanganOther(int contractId);

        //Manage master, reference data
        Task<List<Department>> GetDepartment();
        Task<List<Job>> GetJob();
        Task<List<Religion>> GetReligion();
        Task<List<Factory>> GetFactory();
        Task<List<City>> GetCityProvince();
        Task<List<Education>> GetEducation();
        Task AddNewDepartment(string newDept);
        Task AddNewJob(string newJob);
        Task AddNewCityProvince(string newCity, string newProvince);
        Task AddNewEducation(string newEdType, string newMajor);
    }
}