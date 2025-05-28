using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HRMapp.Data.Database;
using HRMapp.Data.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMapp.ViewModels.EmployeeFormViewModel
{
    public partial class CreateEmployeeViewModel : ObservableObject
    {
        private readonly IEmployeeService _employeeService;
        private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

        [ObservableProperty]
        private Employee newEmployee = new();

        public ObservableCollection<Department> Departments { get; set; } = new();
        public ObservableCollection<Job> Jobs { get; set; } = new();
        public ObservableCollection<Factory> Factories { get; set; } = new();
        public ObservableCollection<City> Cities { get; set; } = new();
        public ObservableCollection<Province> Provinces { get; set; } = new();
        public ObservableCollection<Education> Educations { get; set; } = new();
        public ObservableCollection<Religion> Religions { get; set; } = new();
        public ObservableCollection<String> Genders { get; set; } = new();
        public ObservableCollection<String> EmployeeStatus { get; set; } = new();

        [ObservableProperty]
        private Department selectedDepartment;
        [ObservableProperty]
        private Job selectedJob;
        [ObservableProperty]
        private Factory selectedFactory;
        [ObservableProperty]
        private City selectedCity;        
        [ObservableProperty]
        private Province selectedProvince;
        [ObservableProperty]
        private Education selectedEducation;
        [ObservableProperty]
        private Religion selectedReligion;
        [ObservableProperty]
        private String selectedGender;
        [ObservableProperty]
        private DateOnly selectedBirthdate;
        [ObservableProperty]
        private DateOnly selectedGraduationDate;
        [ObservableProperty]
        private DateOnly selectedHireDate;
        [ObservableProperty]
        private string selectedEmployeeStatus;


        [ObservableProperty]
        private string newCityName;
        [ObservableProperty]
        private string newProvinceName;

        [ObservableProperty]
        private string newEducationType;
        [ObservableProperty]
        private string newEducationMajor;
        [ObservableProperty]
        private string? generatedNip;

        public DateTime SelectedHireDateTime
        {
            get => SelectedHireDate.ToDateTime(TimeOnly.MinValue);
            set => SelectedHireDate = DateOnly.FromDateTime(value);
        }

        public DateTime SelectedBirthdateDateTime
        {
            get => SelectedBirthdate.ToDateTime(TimeOnly.MinValue);
            set => SelectedBirthdate = DateOnly.FromDateTime(value);
        }

        public DateTime SelectedGraduationDateTime
        {
            get => SelectedGraduationDate.ToDateTime(TimeOnly.MinValue);
            set => SelectedGraduationDate = DateOnly.FromDateTime(value);
        }

        public CreateEmployeeViewModel(IEmployeeService employeeService, IDbContextFactory<AppDbContext> dbContextFactory)
        {
            _employeeService = employeeService;
            _dbContextFactory = dbContextFactory;
            LoadDropdown();
        }

        private async Task LoadDropdown()
        {
            if (Genders.Any())
            {
                Genders.Clear();
            } 
            Genders.Add("laki-laki");
            Genders.Add("perempuan");

            SelectedBirthdate = DateOnly.FromDateTime(DateTime.Today);
            SelectedHireDate = DateOnly.FromDateTime(DateTime.Today);
            selectedGraduationDate = DateOnly.FromDateTime(DateTime.Today);

            if (EmployeeStatus.Any())
            {
                EmployeeStatus.Clear();
            }
            EmployeeStatus.Add("Active");
            EmployeeStatus.Add("Non Active");

            using var context = await _dbContextFactory.CreateDbContextAsync();
            Departments.Clear();
            foreach (var d in context.Departments.ToList())
                Departments.Add(d);

            Jobs.Clear();
            foreach (var j in context.Jobs.ToList())
                Jobs.Add(j);

            Factories.Clear();
            foreach (var f in context.Factories.ToList())
                Factories.Add(f);

            Cities.Clear();
            foreach (var c in context.Cities.Include(c => c.Provinces).ToList())
                Cities.Add(c);

            Provinces.Clear();
            foreach (var p in context.Provinces.ToList())
                Provinces.Add(p);

            Educations.Clear();
            foreach (var e in context.Educations.ToList())
                Educations.Add(e);

            Religions.Clear();
            foreach (var r in context.Religions.ToList())
                Religions.Add(r);
        }

        partial void OnSelectedCityChanged(City city)
        {
            if (SelectedCity != null)
            {
                SelectedProvince = Provinces.FirstOrDefault(p => p.province_id == SelectedCity.province_id);
            }
        }

        private async Task GetGeneratedNip(Factory factory)
        {
            if (factory != null)
            {
                GeneratedNip = await _employeeService.AutoGenerateNip(factory.name);
            }
        }

        partial void OnSelectedFactoryChanged(Factory factory)
        {
            if(SelectedFactory != null)
            {
                _ = GetGeneratedNip(factory);
            }
        }

        [RelayCommand]
        private async Task AddNewCityProvince()
        {
            if(!string.IsNullOrEmpty(NewCityName) && !string.IsNullOrEmpty(NewProvinceName))
            {
                await _employeeService.AddNewCityProvince(NewCityName, NewProvinceName);
                await LoadDropdown();
                SelectedCity = Cities.FirstOrDefault(c => c.city_name.Equals(NewCityName, StringComparison.OrdinalIgnoreCase));
                SelectedProvince = SelectedCity.Provinces;
                return;
            }
        }

        [RelayCommand]
        private async Task AddNewEducation()
        {
            await _employeeService.AddNewEducation(NewEducationType, NewEducationMajor);
            await LoadDropdown();
            string combinedDisplayName = string.IsNullOrWhiteSpace(NewEducationMajor)
                                            ? NewEducationType
                                            : $"{NewEducationType} - {NewEducationMajor}";
            SelectedEducation = Educations.FirstOrDefault(e => e.DisplayName.Equals(combinedDisplayName, StringComparison.OrdinalIgnoreCase));
        }

        [RelayCommand]
        private async Task SubmitAsync()
        {
            if (SelectedDepartment == null)
            {
                await Application.Current.MainPage.DisplayAlert("Data Departemen Kosong", "Pilih departemen dari dropdown.", "OK");
                return;
            }
            if (SelectedJob == null)
            {
                await Application.Current.MainPage.DisplayAlert("Data Jabatan Kosong", "Pilih jabatan karyawan dari dropdown.", "OK");
                return;
            }
            if (SelectedReligion == null)
            {
                await Application.Current.MainPage.DisplayAlert("Data Agama Kosong", "Pilih agama karyawan dari dropdown.", "OK");
                return;
            }
            if (SelectedEducation == null)
            {
                await Application.Current.MainPage.DisplayAlert("Data Pendidikan Kosong", "Pilih pendidikan terakhir karyawan dari dropdown.", "OK");
                return;
            }
            if (SelectedEmployeeStatus == null)
            {
                await Application.Current.MainPage.DisplayAlert("Data Status Karyawan Kosong", "Pilih status karyawan dari dropdown.", "OK");
                return;
            }
            if (SelectedGender == null)
            {
                await Application.Current.MainPage.DisplayAlert("Data Jenis Kelamin Kosong", "Pilih jenis kelamin karyawan dari dropdown.", "OK");
                return;                
            }
            if (SelectedFactory == null)
            {
                await Application.Current.MainPage.DisplayAlert("Data Pabrik Kosong", "Pilih pabrik lokasi karyawan dari dropdown.", "OK");
                return;
            }
            if (SelectedCity == null)
            {
                await Application.Current.MainPage.DisplayAlert("Data Kota Kosong", "Pilih kota asal karyawan dari dropdown atau tambahkan data baru.", "OK");
                return;
            }
            using var context = await _dbContextFactory.CreateDbContextAsync();
            var existingNik = await context.Employees
                .Include(e => e.Department)
                .Include(e => e.Job)
                .FirstOrDefaultAsync(e => e.nik == NewEmployee.nik);
            if (existingNik != null)
            {       
                var previousContract = await context.Contracts.FirstOrDefaultAsync(c => c.employee_id == existingNik.employee_id);
                if (previousContract != null)
                {
                    await Application.Current.MainPage.DisplayAlert("Data NIK sama Ditemukan", $"Ditemukan data karyawan dengan NIK : {existingNik.nik} \n" +
                        $"Bergabung pada tanggal {existingNik.hire_date} di departemen {existingNik.Department.name} menjabat sebagai {existingNik.Job.job_name}. \n" +
                        $"Detail kontrak sebelumnya : \n" +
                        $"Terkontrak mulai dari {previousContract.contract_date} hingga {previousContract.end_date} selama {previousContract.contract_duration} bulan.\n" +
                        $"Status karyawan : {existingNik.employee_status.ToUpper()}.", "OK");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Data NIK sama Ditemukan", $"Ditemukan data karyawan dengan NIK : {existingNik.nik} \n" +
                       $"Bergabung pada tanggal {existingNik.hire_date} di departemen {existingNik.Department.name} menjabat sebagai {existingNik.Job.job_name}.\n" +
                       $"Status karyawan : {existingNik.employee_status.ToUpper()}.", "OK");
                }
            }
            var insertEmployee = new Employee
            {
                name = NewEmployee.name,
                nip = GeneratedNip,
                nik = NewEmployee.nik,
                phone_number = NewEmployee.phone_number,
                email = NewEmployee.email,
                birthplace = NewEmployee.birthplace,
                birthdate = SelectedBirthdate,
                address = NewEmployee.address,
                marital_status = NewEmployee.marital_status,
                graduation_date = SelectedGraduationDate,
                hire_date = SelectedHireDate,
                gender = SelectedGender,

                department_id = SelectedDepartment.department_id,
                job_id = SelectedJob.job_id,
                religion_id = SelectedReligion.religion_id,
                city_id = SelectedCity.city_id,
                factory_id = SelectedFactory.factory_id,
                education_id = SelectedEducation.education_id,
                employee_status = SelectedEmployeeStatus
            };

            if (SelectedFactory.name.ToLower() == "seidensticker 1" && insertEmployee.nip.Any(char.IsLetter))
            {
                await Application.Current.MainPage.DisplayAlert("NIP salah", "NIP seidensticker 1 tidak boleh berisi huruf.", "OK");
            } else if (SelectedFactory.name.ToLower() == "seidensticker 2" && !insertEmployee.nip.Any(char.IsLetter))
            {
                await Application.Current.MainPage.DisplayAlert("NIP salah", "NIP seidensticker 2 tidak boleh berisi angka saja.", "OK");
            } else
            {
                await Application.Current.MainPage.DisplayAlert("Data berhasil ditambahkan", $"Data karyawan {insertEmployee.name} berhasil ditambahkan.", "OK");
                await _employeeService.CreateEmployeeAsync(insertEmployee);
                await Shell.Current.GoToAsync("..");
            }
        }
        

        [RelayCommand]
        private async Task Back()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
