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

                ////check no change
                //var currentProvince = await context.Provinces.FirstOrDefaultAsync(p => p.province_id == city.province_id);
                //if (city.city_name.Equals(updatedCityName, StringComparison.OrdinalIgnoreCase) &&
                //    currentProvince?.province_name.Equals(updatedProvinceName, StringComparison.OrdinalIgnoreCase) == true)
                //{
                //    await Application.Current.MainPage.DisplayAlert("Tidak ada perubahan", "Nama kota dan provinsi tidak berubah.", "OK");
                //    return;
                //}

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

                await Application.Current.MainPage.DisplayAlert("Sukses", $"Kota {updatedCityName} telah diperbarui.", "OK");
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
                var existingJob = context.Jobs.FirstOrDefaultAsync(j => j.job_name.ToLower() == newJob.ToLower());

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
                } else
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

    }
}
