using HRMapp.Data.Model;
using System.Threading.Tasks;

namespace HRMapp.ViewModels.EmployeeFormViewModel
{
    public interface IEmployeeService
    {
        Task<List<Employee>> GetAllEmployeeAsync();
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

        Task<List<Cuti>> GetCutiByEmpId(int empId);
        Task CreateCuti(Cuti newCuti);

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
        Task AddNewReligion(string newReligion);
        Task AddNewFactory(string name, string address, int? personnelCapacity);

        Task DeleteJob(int jobId);
        Task DeleteFactory(int factoryId);
        Task DeleteReligion(int religionId);
        Task DeleteDepartment(int deptId);
        Task DeleteEducation(int eduId);
        Task DeleteCityProvince(int cityId);

        //edit
        Task<City?> fetchExistingCityClicked(int id);
        Task EditCityProvince(int cityId, string updatedCityName, string updatedProvinceName);

        Task<Education?> fetchExistingEducationClicked(int id);
        Task EditEducation(int educationId, string updatedEdType, string updatedMajor);

        Task<Department?> fetchExistingDepartmentClicked(int id);
        Task EditDepartment(int departmentId, string updatedDept);

        Task<Job?> fetchExistingJobClicked(int id);
        Task EditJob(int jobId, string updatedJob);

        Task<Religion?> fetchExistingReligion(int id);
        Task EditReligion(int religionId, string updatedReligion);

        Task<Factory?> fetchExistingFactory(int id);
        Task EditFactory(int factoryId, string updatedName, string updatedAddress, int? updatedPersonnelCapacity);

    }
}