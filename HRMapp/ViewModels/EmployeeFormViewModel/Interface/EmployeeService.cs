using HRMapp.Data.Database;
using HRMapp.Data.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UraniumUI.Extensions;

namespace HRMapp.ViewModels.EmployeeFormViewModel.Interface
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public EmployeeService(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        /// <summary>
        /// GetEmployeeByIdAsync used for the form on the EditEmployeeForm view.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// 
        public async Task<List<Employee>> GetAllEmployeeAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Employees
                .AsNoTracking()
                .Include(e => e.Department)
                .Include(e => e.Job)
                .Include(e => e.Factory)
                .OrderBy(e => e.employee_id)
                .ToListAsync();
        }

        public async Task<Employee?> GetEmployeeByIdAsync(int id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            return await context.Employees
                .Include(e => e.Department)
                .Include(e => e.Job)
                .Include(e => e.Factory)
                .Include(e => e.City)
                    .ThenInclude(c => c.Provinces)
                .Include(e => e.Education)
                .Include(e => e.Religion)
                .FirstOrDefaultAsync(e => e.employee_id == id);
        }

        public async Task CreateEmployeeAsync(Employee employee)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            context.Employees.Add(employee);
            await context.SaveChangesAsync();
        }

        public async Task<string?> AutoGenerateNip(string selectedFactory)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var factory = await context.Factories.FirstOrDefaultAsync(f => f.name == selectedFactory);

            if (factory != null)
            {
                if (factory.name.ToLower() == "seidensticker 1")
                {
                    var maxNip = await context.Employees
                        .Where(e => e.Factory.name.ToLower() == "seidensticker 1")
                        .OrderByDescending(e => e.nip)
                        .Select(e => e.nip)
                        .FirstOrDefaultAsync();
                    int nextNumber = string.IsNullOrEmpty(maxNip) ? 1 : int.Parse(maxNip) + 1;
                    Debug.WriteLine(nextNumber);
                    return nextNumber.ToString("D5");
                }
                else
                {
                    var maxNip = await context.Employees
                        .Where(e => e.Factory.name.ToLower() == "seidensticker 2")
                        .OrderByDescending(e => e.nip)
                        .Select(e => e.nip)
                        .FirstOrDefaultAsync();

                    int nextNumber = string.IsNullOrEmpty(maxNip) ? 1 : int.Parse(maxNip.Substring(1)) + 1;
                    Debug.WriteLine(nextNumber);
                    return $"B{nextNumber:D4}";
                }
            }
            return null;
        }

        public async Task UpdateEmployeeAsync(Employee employee)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            context.Employees.Update(employee);
            await context.SaveChangesAsync();
        }


        public async Task DeleteEmployeeAsync(int id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var employee = await context.Employees
                .Include(e => e.Contracts)
                .Include(e => e.Courses)
                .Include(e => e.LogEmployees)
                .FirstOrDefaultAsync(e => e.employee_id == id);

            if (employee != null)
            {
                foreach (var contract in employee.Contracts)
                {
                    var listTunjangan = await context.Tunjangan
                        .Where(t => t.contract_id == contract.contract_id)
                        .ToListAsync();

                    context.Tunjangan.RemoveRange(listTunjangan);
                }

                context.Contracts.RemoveRange(employee.Contracts);
                context.Course.RemoveRange(employee.Courses);
                context.LogEmployees.RemoveRange(employee.LogEmployees);
                context.Employees.Remove(employee);

                await context.SaveChangesAsync();
            }
        }

        public async Task<List<Contract>> GetContractsByEmployeeIdAsync(int employeeId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            return await context.Contracts
                .Where(c => c.employee_id == employeeId)
                .OrderBy(c => c.contract_date) 
                .ToListAsync();
        }
        public async Task<Contract>? GetContractDetail(int contractId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Contracts.FindAsync(contractId);
        }
        public async Task UpdateContractAsync(Contract contract)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            context.Contracts.Update(contract);
            await context.SaveChangesAsync();
        }
        public async Task CreateContractAsync(Contract contract)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            context.Contracts.Add(contract);
            await context.SaveChangesAsync();
        }
        public async Task DeleteContractAsync(Contract contract)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            if (contract != null)
            {
                var listTunjangan = await context.Tunjangan
                    .Where(t => t.contract_id == contract.contract_id)
                    .ToListAsync();

                context.Tunjangan.RemoveRange(listTunjangan);
                
                context.Contracts.Remove(contract);
                await context.SaveChangesAsync();
            }
        }
        public async Task<Contract?> GetLastIndexContractDate(int contractIndex, int employeeId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            if (contractIndex > 0)
            {
                var latestContract = await context.Contracts.Where(c => c.employee_id == employeeId)
                    .OrderByDescending(c => c.contract_date)
                    .FirstOrDefaultAsync();

                return latestContract;
            }

            return null;
        }

        //Tunjangan
        public async Task<int> GetContractCountByEmployeeIdAsync(int employeeId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Contracts
                .Where(c => c.employee_id == employeeId)
                .CountAsync();
        }
        public async Task CreateTunjanganAsync(Tunjangan tunjangan)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            context.Tunjangan.Add(tunjangan);
            await context.SaveChangesAsync();
        }
        public async Task UpdateTunjanganAsync(Tunjangan tunjangan)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            bool isMK = tunjangan.tunjangan_name?.Contains("MK", StringComparison.OrdinalIgnoreCase) ?? false;

            var existingTunjangan = await context.Tunjangan
            .FirstOrDefaultAsync(t => t.contract_id == tunjangan.contract_id &&
                               (isMK
                                   ? t.tunjangan_name.ToLower().Contains("mk") : !t.tunjangan_name.ToLower().Contains("mk")));

            if (existingTunjangan != null)
            {
                existingTunjangan.amount = tunjangan.amount;
            }
            else
            {
                context.Tunjangan.Add(tunjangan);
            }

            await context.SaveChangesAsync();
        }
        public async Task <Tunjangan> GetTunjanganMK(int contractId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            return await context.Tunjangan.Where(e => e.contract_id == contractId && 
            e.tunjangan_name.ToLower().Contains("mk")).FirstOrDefaultAsync();
        }        
        public async Task <Tunjangan> GetTunjanganOther(int contractId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            return await context.Tunjangan.Where(e => e.contract_id == contractId &&
            !e.tunjangan_name.ToLower().Contains("mk")).FirstOrDefaultAsync();
        }

        //CUTIIII
        
        //reset jumlah cuti semua karyawan tiap ganti tahun
        public async Task ResetCutiIfNewYear()
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            int currentYear = DateTime.Now.Year;
            var cutiResetKey = $"cuti_reset_{currentYear}";

            if (!Preferences.ContainsKey(cutiResetKey))
            {
                var employeesToReset = await context.Employees
                    .Where(e => e.yearly_cuti_left < 12)
                    .ToListAsync();

                foreach (var emp in employeesToReset)
                {
                    emp.yearly_cuti_left = 12;
                }

                await context.SaveChangesAsync();

                Preferences.Set(cutiResetKey, true);
            }
        }

        public async Task<List<Cuti>> GetCutiByEmpId(int empId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            return await context.Cuti.Where(c => c.employee_id == empId).OrderBy(c => c.cuti_start_date).ToListAsync();
        }
        public async Task<string> LoadJatahCuti(int empId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var emp = await context.Employees.FirstOrDefaultAsync(e => e.employee_id == empId);

            return emp.yearly_cuti_left.ToString();
        }
        public async Task CreateCuti(Cuti newCuti)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            try
            {
                if (newCuti != null)
                {
                    var employee = await context.Employees.FirstOrDefaultAsync(e => e.employee_id == newCuti.employee_id);

                    if (employee == null)
                    {
                        await Application.Current.MainPage.DisplayAlert("Error", "Karyawan tidak ditemukan.", "OK");
                        return;
                    }

                    if (employee.yearly_cuti_left < newCuti.cuti_day_count)
                    {
                        await Application.Current.MainPage.DisplayAlert("Error", $"Jumlah sisa cuti karyawan ({employee.yearly_cuti_left} hari) kurang.", "OK");
                        return;
                    }

                    var overlappingCuti = await context.Cuti
                        .Where(c => c.employee_id == newCuti.employee_id &&
                    (
                        (newCuti.cuti_start_date >= c.cuti_start_date && newCuti.cuti_start_date < c.cuti_end_date) ||
                        (newCuti.cuti_end_date > c.cuti_start_date && newCuti.cuti_end_date <= c.cuti_end_date) ||
                        (newCuti.cuti_start_date <= c.cuti_start_date && newCuti.cuti_end_date >= c.cuti_end_date)
                    ))
                        .FirstOrDefaultAsync();

                    if (overlappingCuti != null)
                    {
                        await Application.Current.MainPage.DisplayAlert("Error", "Tanggal cuti yang diajukan bertabrakan dengan cuti yang sudah ada.", "OK");
                        return;
                    }

                    employee.yearly_cuti_left -= newCuti.cuti_day_count;

                    context.Add(newCuti);
                    await context.SaveChangesAsync();
                    await Application.Current.MainPage.DisplayAlert($"Cuti berhasil ditambahkan", $"Cuti dari tanggal {newCuti.cuti_start_date} - {newCuti.cuti_end_date} berhasil ditambahkan\n" +
                        $"Sisa cuti karyawan {employee.name} tahun {DateTime.Now.Year} sisa {employee.yearly_cuti_left} hari.", "OK");
                } else
                {
                    await Application.Current.MainPage.DisplayAlert("Invalid Input", $"Tidak ada data cuti yang ditambahkan", "OK");
                }
            }
            catch (Exception ex)
{
            Debug.WriteLine($"[ERROR] Failed to create cuti: {ex}");
            await Application.Current.MainPage.DisplayAlert("Error", $"Failed to create cuti: {ex.Message}", "OK");
            }

        }
        public async Task<Cuti> fetchExistingCuti(int cutiId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var existing = await context.Cuti.FindAsync(cutiId);

            if (existing != null)
            {
                //var employee = await context.Employees.FirstOrDefaultAsync(e => e.employee_id == existing.employee_id);

                return existing;
            }
            return null;


        }
        public async Task UpdateCuti(Cuti updatedCuti)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var existing = await context.Cuti
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.cuti_id == updatedCuti.cuti_id);

            if (existing == null)
                throw new Exception("Data cuti tidak ditemukan.");

            // Validate overlap
            var overlaps = await context.Cuti
                .Where(c => c.employee_id == updatedCuti.employee_id &&
                            c.cuti_id != updatedCuti.cuti_id &&
                            ((updatedCuti.cuti_start_date >= c.cuti_start_date && updatedCuti.cuti_start_date <= c.cuti_end_date) ||
                             (updatedCuti.cuti_end_date >= c.cuti_start_date && updatedCuti.cuti_end_date <= c.cuti_end_date) ||
                             (updatedCuti.cuti_start_date <= c.cuti_start_date && updatedCuti.cuti_end_date >= c.cuti_end_date)))
                .AnyAsync();

            if (overlaps)
                throw new Exception("Tanggal cuti bertabrakan dengan cuti lain.");

            int oldDuration = existing.cuti_day_count;
            int newDuration = updatedCuti.cuti_day_count;
            int difference = oldDuration - newDuration;

            // Fetch employee cuti info and update yearly cuti left
            var employee = await context.Employees.FirstOrDefaultAsync(e => e.employee_id == updatedCuti.employee_id);
            if (employee == null)
                throw new Exception("Karyawan tidak ditemukan.");

            employee.yearly_cuti_left += difference;
            if (employee.yearly_cuti_left < 0)
                throw new Exception("Durasi cuti melebihi sisa jatah cuti.");

            context.Cuti.Update(updatedCuti);
            await context.SaveChangesAsync();
        }


        public async Task DeleteCuti(int cutiId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var cuti = await context.Cuti.FindAsync(cutiId);
            if(cuti != null)
            {
                var emp = await context.Employees.FirstOrDefaultAsync(e => e.employee_id == cuti.employee_id);

                emp.yearly_cuti_left += cuti.cuti_day_count;

                context.Cuti.Remove(cuti);
                await context.SaveChangesAsync();

                await Application.Current.MainPage.DisplayAlert("Berhasil delete", $"Data cuti {emp.name} tanggal {cuti.cuti_start_date} - {cuti.cuti_end_date} berhasil terhapus.", "OK");
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Gagal menghapus data cuti.", "OK");
            }
        }

        //====================================== MANAGE REFERENCE / MASTER DATA ==============================

        //City Province
        public async Task<List<City>> GetCityProvince()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Cities.Include(c => c.Provinces).ToListAsync();
        }
        //add city province 
        public async Task AddNewCityProvince(string newCity, string newProvince)
        {
            string Normalize(string input)
            {
                return Regex.Replace(input ?? "", "[^a-zA-Z]", "").ToLower().Trim();
            }

            using var context = await _contextFactory.CreateDbContextAsync();

            if (!string.IsNullOrEmpty(newCity) && !string.IsNullOrEmpty(newProvince))
            {
                var existingcity = await context.Cities.FirstOrDefaultAsync(c => c.city_name.ToLower() == newCity.ToLower());
                if (existingcity != null)
                {
                    await Application.Current.MainPage.DisplayAlert("Data kota sudah ada", $"Data kota {newCity} sudah ada sebelumnya, silahkan pilih dari dropdown", "OK");
                    return;
                } 
                else
                {
                    var provinces = await context.Provinces.ToListAsync();
                    var existingProvince = provinces.FirstOrDefault(p => Normalize(p.province_name) == Normalize(newProvince));

                    if (existingProvince == null)
                    {
                        var insertProvince = new Province
                        {
                            province_name = newProvince
                        };

                        context.Provinces.Add(insertProvince);
                        await context.SaveChangesAsync();

                        var provincesAfter = await context.Provinces.ToListAsync();
                        var newprovince = provincesAfter.FirstOrDefault(p => Normalize(p.province_name) == Normalize(newProvince));
                        var insertCity = new City
                        {
                            city_name = newCity,
                            province_id = newprovince.province_id
                        };

                        context.Cities.Add(insertCity);
                        await context.SaveChangesAsync();
                        await Application.Current.MainPage.DisplayAlert("Data berhasil ditambahkan", $"Data kota {insertCity.city_name} dan provinsi {insertCity.Provinces.province_name} telah berhasil ditambahkan. Silahkan pilih melalui dropdown.", "OK");
                        Debug.WriteLine(insertCity.city_name, insertCity.province_id);
                    }
                    else
                    {
                        var insertCity = new City
                        {
                            city_name = newCity,
                            province_id = existingProvince.province_id
                        };

                        context.Cities.Add(insertCity);
                        await context.SaveChangesAsync();
                        await Application.Current.MainPage.DisplayAlert("Data berhasil ditambahkan", $"Data kota {insertCity.city_name} dan provinsi {insertCity.Provinces.province_name} telah berhasil ditambahkan. Silahkan pilih melalui dropdown.", "OK");
                        Debug.WriteLine(insertCity.city_name, insertCity.province_id);
                    }
                }
            }
        }
        public async Task DeleteCityProvince(int cityId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var city = await context.Cities.FindAsync(cityId);
            if (city != null)
            {
                try
                {
                    context.Cities.Remove(city);
                    await context.SaveChangesAsync();
                    await Application.Current.MainPage.DisplayAlert("Berhasil", $"Data kota '{city.city_name}' berhasil dihapus.", "OK");
                }
                catch (Exception e)
                {
                    await Application.Current.MainPage.DisplayAlert("Gagal menghapus Data kota.", $"{e.Message}", "OK");
                }
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Data tidak ditemukan", "Data kota tidak ditemukan.", "OK");
            }
        }
        //fetch edit
        public async Task<City?> fetchExistingCityClicked(int id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var city = context.Cities.Include(c => c.Provinces).FirstOrDefault(c => c.city_id == id);

            return city;
        }
          //Edit city province

        public async Task EditCityProvince(int cityId, string updatedCityName, string updatedProvinceName)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync();

                var city = await context.Cities.FirstOrDefaultAsync(c => c.city_id == cityId);
                if (city == null)
                {
                    await Application.Current.MainPage.DisplayAlert("Data tidak ditemukan", "Kota tidak ditemukan.", "OK");
                    return;
                }

                updatedCityName = updatedCityName.Trim();
                updatedProvinceName = updatedProvinceName.Trim();

                //check no change
                var currentProvince = await context.Provinces.FirstOrDefaultAsync(p => p.province_id == city.province_id);
                if (city.city_name.Equals(updatedCityName, StringComparison.OrdinalIgnoreCase) &&
                    currentProvince?.province_name.Equals(updatedProvinceName, StringComparison.OrdinalIgnoreCase) == true)
                {
                    await Application.Current.MainPage.DisplayAlert("Tidak ada perubahan", "Nama kota dan provinsi tidak berubah.", "OK");
                    return;
                }

                bool isDuplicate = await context.Cities
                    .AnyAsync(c => c.city_id != cityId && c.city_name.ToLower() == updatedCityName.ToLower());
                if (isDuplicate)
                {
                    await Application.Current.MainPage.DisplayAlert("Data kota sudah ada", $"Kota {updatedCityName} sudah ada.", "OK");
                    return;
                }

                // Find or create province
                var province = await context.Provinces
                    .FirstOrDefaultAsync(p => p.province_name.ToLower() == updatedProvinceName.ToLower());

                if (province == null)
                {
                    province = new Province { province_name = updatedProvinceName };
                    context.Provinces.Add(province);
                    await context.SaveChangesAsync();
                }

                // Update city
                city.city_name = updatedCityName;
                city.province_id = province.province_id;

                await context.SaveChangesAsync();

                await Application.Current.MainPage.DisplayAlert("Sukses", $"Data {updatedCityName} telah diperbarui.", "OK");
                Debug.WriteLine($"Updated city: {updatedCityName}, Province: {province.province_name}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR] {ex}");
                await Application.Current.MainPage.DisplayAlert("Terjadi kesalahan", "Gagal menyimpan perubahan.", "OK");
            }
        }

        //Education
        public async Task<List<Education>> GetEducation()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Educations.ToListAsync();
        }

        public async Task AddNewEducation(string newEdType, string newMajor)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            if (!string.IsNullOrEmpty(newEdType) && !string.IsNullOrEmpty(newMajor))
            {
                var existingEdType = await context.Educations.FirstOrDefaultAsync(e => e.education_type.ToLower().Trim() == newEdType.ToLower().Trim());
                var existingMajor = await context.Educations.FirstOrDefaultAsync(e => e.major.ToLower().Trim()   == newMajor.ToLower().Trim());

                if (existingEdType == null && existingMajor == null)
                {
                    var newEducation = new Education
                    {
                        education_type = newEdType,
                        major = newMajor
                    };

                    context.Educations.Add(newEducation);
                    await context.SaveChangesAsync();
                    await Application.Current.MainPage.DisplayAlert("Data berhasil ditambahkan", $"Data tingkat pendidikan {newEdType} dengan jurusan {newMajor} telah berhasil ditambahkan. Silahkan pilih melalui dropdown.", "OK");
                } 
                else if (existingEdType != null && existingMajor == null)
                {
                    var newEducation = new Education
                    {
                        education_type = existingEdType.education_type,
                        major = newMajor
                    };

                    context.Educations.Add(newEducation);
                    await context.SaveChangesAsync();
                    await Application.Current.MainPage.DisplayAlert("Data berhasil ditambahkan", $"Data tingkat pendidikan {newEdType} dengan jurusan {newMajor} telah berhasil ditambahkan. Silahkan pilih melalui dropdown.", "OK");
                }
            }
            else
            {
                var newEducation = new Education
                {
                    education_type = newEdType,
                    major = null
                };

                context.Educations.Add(newEducation);
                await context.SaveChangesAsync();
                await Application.Current.MainPage.DisplayAlert("Data berhasil ditambahkan", $"Data tingkat pendidikan {newEdType} dengan jurusan {newMajor} telah berhasil ditambahkan. Silahkan pilih melalui dropdown.", "OK");
            }
        }
        public async Task DeleteEducation(int eduId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var edu = await context.Educations.FindAsync(eduId);
            if (edu != null)
            {
                try
                {
                    context.Educations.Remove(edu);
                    await context.SaveChangesAsync();
                    await Application.Current.MainPage.DisplayAlert("Berhasil", $"Data Pendidikan '{edu.DisplayName}' berhasil dihapus.", "OK");
                }
                catch (Exception e)
                {
                    await Application.Current.MainPage.DisplayAlert("Gagal menghapus Data Pendidikan.", $"{e.Message}", "OK");
                }
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Data tidak ditemukan", "Data pendidikan tidak ditemukan.", "OK");
            }
        }
        //edit 
        public async Task<Education?> fetchExistingEducationClicked(int id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var edu = context.Educations.FirstOrDefault(c => c.education_id == id);

            return edu;
        }
        public async Task EditEducation(int educationId, string updatedEdType, string updatedMajor)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            if (string.IsNullOrWhiteSpace(updatedEdType))
            {
                await Application.Current.MainPage.DisplayAlert("Invalid Input", "Tingkat pendidikan tidak boleh kosong.", "OK");
                return;
            }

            updatedEdType = updatedEdType.Trim();
            updatedMajor = updatedMajor?.Trim();

            var education = await context.Educations.FirstOrDefaultAsync(e => e.education_id == educationId);
            if (education == null)
            {
                await Application.Current.MainPage.DisplayAlert("Data tidak ditemukan", "Data pendidikan tidak ditemukan.", "OK");
                return;
            }

            if (education.education_type.Equals(updatedEdType, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(education.major ?? "", updatedMajor ?? "", StringComparison.OrdinalIgnoreCase))
            {
                await Application.Current.MainPage.DisplayAlert("Tidak ada perubahan", "Tingkat pendidikan dan jurusan tidak berubah.", "OK");
                return;
            }

            bool isDuplicate = await context.Educations.AnyAsync(e =>
                e.education_id != educationId &&
                e.education_type.ToLower() == updatedEdType.ToLower() &&
                (e.major ?? "").ToLower() == (updatedMajor ?? "").ToLower());

            if (isDuplicate)
            {
                await Application.Current.MainPage.DisplayAlert("Data duplikat", "Data pendidikan dengan jurusan tersebut sudah ada.", "OK");
                return;
            }
            
            education.education_type = updatedEdType;
            education.major = updatedMajor;

            await context.SaveChangesAsync();
            await Application.Current.MainPage.DisplayAlert("Sukses", $"Data pendidikan telah diperbarui ke {updatedEdType} - {updatedMajor}.", "OK");
        }


        //DEPARTMENT
        public async Task<List<Department>> GetDepartment()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Departments.ToListAsync();
        }

        public async Task AddNewDepartment(string newDept)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            if (!string.IsNullOrEmpty(newDept))
            {
                var existingDept = await context.Departments.FirstOrDefaultAsync(j => j.name.ToLower() == newDept.ToLower());

                if (existingDept == null)
                {
                    try
                        {
                        var newDepartment = new Department
                        {
                            name = newDept
                        };
                        context.Departments.Add(newDepartment);
                        await context.SaveChangesAsync();
                        await Application.Current.MainPage.DisplayAlert("Data berhasil ditambahkan", $"Data departemen {newDept} berhasil ditambahkan", "OK");
                    } catch (Exception e)
                    {
                        await Application.Current.MainPage.DisplayAlert("Gagal menambahkan data.",$"{e}", "OK");
                    }
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Data duplikat.", "Ditemukan data yang sama dengan input.", "OK");
                }
            }
        }
        public async Task DeleteDepartment(int deptId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var dept = await context.Departments.FindAsync(deptId);
            if (dept != null)
            {
                try
                {
                    context.Departments.Remove(dept);
                    await context.SaveChangesAsync();
                    await Application.Current.MainPage.DisplayAlert("Berhasil", $"Department '{dept.name}' berhasil dihapus.", "OK");
                }
                catch (Exception e)
                {
                    await Application.Current.MainPage.DisplayAlert("Gagal menghapus department.", $"{e.Message}", "OK");
                }
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Data tidak ditemukan", "Department tidak ditemukan.", "OK");
            }
        }
        //update
        public async Task<Department?> fetchExistingDepartmentClicked(int id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var dept = context.Departments.FirstOrDefault(c => c.department_id == id);

            return dept;
        }
        public async Task EditDepartment(int departmentId, string updatedDept)
        {
            if (string.IsNullOrWhiteSpace(updatedDept))
            {
                await Application.Current.MainPage.DisplayAlert("Invalid Input", "Nama departemen tidak boleh kosong.", "OK");
                return;
            }

            using var context = await _contextFactory.CreateDbContextAsync();

            var department = await context.Departments.FirstOrDefaultAsync(d => d.department_id == departmentId);
            if (department == null)
            {
                await Application.Current.MainPage.DisplayAlert("Data tidak ditemukan", "Departemen tidak ditemukan.", "OK");
                return;
            }

            if (department.name.Equals(updatedDept.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                await Application.Current.MainPage.DisplayAlert("Tidak ada perubahan", "Nama departemen tidak berubah.", "OK");
                return;
            }

            bool isDuplicate = await context.Departments.AnyAsync(d =>
                d.department_id != departmentId &&
                d.name.ToLower() == updatedDept.Trim().ToLower());

            if (isDuplicate)
            {
                await Application.Current.MainPage.DisplayAlert("Data duplikat", "Departemen dengan nama tersebut sudah ada.", "OK");
                return;
            }

            department.name = updatedDept.Trim();
            await context.SaveChangesAsync();

            await Application.Current.MainPage.DisplayAlert("Sukses", $"Departemen telah diperbarui menjadi {updatedDept}.", "OK");
        }


        //JOBS
        public async Task<List<Job>> GetJob()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Jobs.ToListAsync();
        }
        public async Task AddNewJob(string newJob)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            if (!string.IsNullOrEmpty(newJob))
            {
                var existingJob = await context.Jobs.FirstOrDefaultAsync(j => j.job_name.ToLower() == newJob.ToLower());

                if (existingJob == null)
                {
                    try
                    {
                        var newJobs = new Job
                        {
                           job_name  =  newJob
                        };
                        context.Jobs.Add(newJobs);
                        await context.SaveChangesAsync();
                        await Application.Current.MainPage.DisplayAlert("Data berhasil ditambahkan", $"Data jabatan {newJob} berhasil ditambahkan", "OK");
                    }
                    catch (Exception e)
                    {
                        await Application.Current.MainPage.DisplayAlert("Gagal menambahkan data.", $"{e}", "OK");
                    }
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Data duplikat.", "Ditemukan data yang sama dengan input.", "OK");
                }
            }
        }

        public async Task DeleteJob(int jobId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var job = await context.Jobs.FindAsync(jobId);
            if (job != null)
            {
                try
                {
                    context.Jobs.Remove(job);
                    await context.SaveChangesAsync();
                    await Application.Current.MainPage.DisplayAlert("Berhasil", $"Jabatan '{job.job_name}' berhasil dihapus.", "OK");
                }
                catch (Exception e)
                {
                    await Application.Current.MainPage.DisplayAlert("Gagal menghapus jabatan.", $"{e.Message}", "OK");
                }
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Data tidak ditemukan", "Jabatan tidak ditemukan.", "OK");
            }
        }
        //edit
        public async Task<Job?> fetchExistingJobClicked(int id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var job = context.Jobs.FirstOrDefault(c => c.job_id == id);

            return job;
        }
        public async Task EditJob(int jobId, string updatedJob)
        {
            if (string.IsNullOrWhiteSpace(updatedJob))
            {
                await Application.Current.MainPage.DisplayAlert("Invalid Input", "Nama jabatan tidak boleh kosong.", "OK");
                return;
            }

            using var context = await _contextFactory.CreateDbContextAsync();

            var job = await context.Jobs.FirstOrDefaultAsync(j => j.job_id == jobId);
            if (job == null)
            {
                await Application.Current.MainPage.DisplayAlert("Data tidak ditemukan", "Jabatan tidak ditemukan.", "OK");
                return;
            }

            if (job.job_name.Equals(updatedJob.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                await Application.Current.MainPage.DisplayAlert("Tidak ada perubahan", "Nama jabatan tidak berubah.", "OK");
                return;
            }

            bool isDuplicate = await context.Jobs.AnyAsync(j =>
                j.job_id != jobId &&
                j.job_name.ToLower() == updatedJob.Trim().ToLower());

            if (isDuplicate)
            {
                await Application.Current.MainPage.DisplayAlert("Data duplikat", "Jabatan dengan nama tersebut sudah ada.", "OK");
                return;
            }

            job.job_name = updatedJob.Trim();
            await context.SaveChangesAsync();

            await Application.Current.MainPage.DisplayAlert("Sukses", $"Jabatan telah diperbarui menjadi {updatedJob}.", "OK");
        }



        //Religion
        public async Task<List<Religion>> GetReligion()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Religions.ToListAsync();
        }
        public async Task AddNewReligion(string newReligion)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            if (!string.IsNullOrEmpty(newReligion))
            {
                var existingReligion = await context.Religions.FirstOrDefaultAsync(r => r.religion_name.ToLower() == newReligion.ToLower());

                if (existingReligion == null)
                {
                    try
                    {
                        var newReligionEntry = new Religion
                        {
                            religion_name = newReligion
                        };
                        context.Religions.Add(newReligionEntry);
                        await context.SaveChangesAsync();
                        await Application.Current.MainPage.DisplayAlert("Data berhasil ditambahkan", $"Data agama {newReligion} berhasil ditambahkan", "OK");
                    }
                    catch (Exception e)
                    {
                        await Application.Current.MainPage.DisplayAlert("Gagal menambahkan data.", $"{e.Message}", "OK");
                    }
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Data duplikat.", "Ditemukan data agama yang sama.", "OK");
                }
            }
        }
        public async Task DeleteReligion(int religionId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var religion = await context.Religions.FindAsync(religionId);
            if (religion != null)
            {
                try
                {
                    context.Religions.Remove(religion);
                    await context.SaveChangesAsync();
                    await Application.Current.MainPage.DisplayAlert("Berhasil", $"Agama '{religion.religion_name}' berhasil dihapus.", "OK");
                }
                catch (Exception e)
                {
                    await Application.Current.MainPage.DisplayAlert("Gagal menghapus agama.", $"{e.Message}", "OK");
                }
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Data tidak ditemukan", "Agama tidak ditemukan.", "OK");
            }
        }
        //edit
        public async Task<Religion?> fetchExistingReligion(int id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var relg = context.Religions.FirstOrDefault(c => c.religion_id == id);

            return relg;
        }
        public async Task EditReligion(int religionId, string updatedReligion)
        {
            if (string.IsNullOrWhiteSpace(updatedReligion))
            {
                await Application.Current.MainPage.DisplayAlert("Invalid Input", "Nama agama tidak boleh kosong.", "OK");
                return;
            }

            using var context = await _contextFactory.CreateDbContextAsync();

            var religion = await context.Religions.FirstOrDefaultAsync(r => r.religion_id == religionId);
            if (religion == null)
            {
                await Application.Current.MainPage.DisplayAlert("Data tidak ditemukan", "Agama tidak ditemukan.", "OK");
                return;
            }

            if (religion.religion_name.Equals(updatedReligion.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                await Application.Current.MainPage.DisplayAlert("Tidak ada perubahan", "Nama agama tidak berubah.", "OK");
                return;
            }

            bool isDuplicate = await context.Religions.AnyAsync(r =>
                r.religion_id != religionId &&
                r.religion_name.ToLower() == updatedReligion.Trim().ToLower());

            if (isDuplicate)
            {
                await Application.Current.MainPage.DisplayAlert("Data duplikat", "Agama dengan nama tersebut sudah ada.", "OK");
                return;
            }

            religion.religion_name = updatedReligion.Trim();
            await context.SaveChangesAsync();

            await Application.Current.MainPage.DisplayAlert("Sukses", $"Agama telah diperbarui menjadi {updatedReligion}.", "OK");
        }




        //Factory
        public async Task<List<Factory>> GetFactory()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Factories.ToListAsync();
        }
        public async Task AddNewFactory(string name, string address, int? personnelCapacity)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(address))
            {
                var existingFactory = await context.Factories.FirstOrDefaultAsync(f => f.name.ToLower() == name.ToLower());

                if (existingFactory == null)
                {
                    try
                    {
                        var newFactoryEntry = new Factory
                        {
                            name = name,
                            address = address,
                            personnel_capacity = personnelCapacity
                        };

                        context.Factories.Add(newFactoryEntry);
                        await context.SaveChangesAsync();

                        await Application.Current.MainPage.DisplayAlert("Data berhasil ditambahkan", $"Pabrik '{name}' berhasil ditambahkan.", "OK");
                    }
                    catch (Exception e)
                    {
                        await Application.Current.MainPage.DisplayAlert("Gagal menambahkan data.", $"{e.Message}", "OK");
                    }
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Data duplikat.", $"Pabrik dengan nama '{name}' sudah ada.", "OK");
                }
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Input tidak valid", "Nama dan alamat pabrik wajib diisi.", "OK");
            }
        }

        public async Task DeleteFactory(int factoryId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var factory = await context.Factories.FindAsync(factoryId);
            if (factory != null)
            {
                try
                {
                    context.Factories.Remove(factory);
                    await context.SaveChangesAsync();
                    await Application.Current.MainPage.DisplayAlert("Berhasil", $"Pabrik '{factory.name}' berhasil dihapus.", "OK");
                }
                catch (Exception e)
                {
                    await Application.Current.MainPage.DisplayAlert("Gagal menghapus pabrik.", $"{e.Message}", "OK");
                }
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Data tidak ditemukan", "Pabrik tidak ditemukan.", "OK");
            }
        }

        //EDIT
        public async Task<Factory?> fetchExistingFactory(int id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var factory = context.Factories.FirstOrDefault(c => c.factory_id == id);

            return factory;
        }
        public async Task EditFactory(int factoryId, string updatedName, string updatedAddress, int? updatedPersonnelCapacity)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            if (string.IsNullOrWhiteSpace(updatedName) || string.IsNullOrWhiteSpace(updatedAddress))
            {
                await Application.Current.MainPage.DisplayAlert("Input tidak valid", "Nama dan alamat pabrik wajib diisi.", "OK");
                return;
            }

            var factory = await context.Factories.FirstOrDefaultAsync(f => f.factory_id == factoryId);
            if (factory == null)
            {
                await Application.Current.MainPage.DisplayAlert("Data tidak ditemukan", "Pabrik tidak ditemukan.", "OK");
                return;
            }

            updatedName = updatedName.Trim();
            updatedAddress = updatedAddress.Trim();

            var duplicateFactory = await context.Factories
                .FirstOrDefaultAsync(f => f.factory_id != factoryId && f.name.ToLower().Trim() == updatedName.ToLower());

            if (duplicateFactory != null)
            {
                await Application.Current.MainPage.DisplayAlert("Data duplikat", $"Pabrik dengan nama '{updatedName}' sudah ada.", "OK");
                return;
            }

            if (factory.name.Equals(updatedName, StringComparison.OrdinalIgnoreCase) &&
                factory.address.Equals(updatedAddress, StringComparison.OrdinalIgnoreCase) &&
                factory.personnel_capacity == updatedPersonnelCapacity)
            {
                await Application.Current.MainPage.DisplayAlert("Tidak ada perubahan", "Data pabrik tidak mengalami perubahan.", "OK");
                return;
            }

            factory.name = updatedName;
            factory.address = updatedAddress;
            factory.personnel_capacity = updatedPersonnelCapacity;

            try
            {
                await context.SaveChangesAsync();
                await Application.Current.MainPage.DisplayAlert("Berhasil", $"Pabrik '{updatedName}' berhasil diperbarui.", "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Gagal memperbarui data", $"{ex.Message}", "OK");
            }
        }

    }
}
