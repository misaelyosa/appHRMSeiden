using CommunityToolkit.Mvvm.ComponentModel;
using HRMapp.Data.Database;
using Microsoft.EntityFrameworkCore;
using HRMapp.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore.Internal;
using System.Diagnostics;
using System.Threading.Channels;

namespace HRMapp.ViewModels.EmployeeFormViewModel
{
    [QueryProperty(nameof(EmployeeId), "employeeId")]
    public partial class EditEmployeeViewModel : ObservableObject
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IEmployeeService _employeeService;

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
        private string currentCity;

        [ObservableProperty]
        private string currentProvince;

        [ObservableProperty]
        private string currentFactory;

        [ObservableProperty]
        private string currentJob;

        [ObservableProperty]
        private string currentDepartment;        
        
        [ObservableProperty]
        private string currentReligion;

        [ObservableProperty]
        private Province selectedProvince;

        [ObservableProperty]
        private string selectedEmployeeStatus;

        [ObservableProperty]
        private string currentEducation;

        private Employee initialEmployee;

        //proxy buat convert datetime (compatible with datepicker)
        public DateTime SelectedBirthdateDateTime
        {
            get => selectedBirthdate.ToDateTime(TimeOnly.MinValue);
            set => SelectedBirthdate = DateOnly.FromDateTime(value);
        }

        public DateTime SelectedGraduationDateTime
        {
            get => SelectedGraduationDate.ToDateTime(TimeOnly.MinValue);
            set => SelectedGraduationDate = DateOnly.FromDateTime(value);
        }

        public EditEmployeeViewModel(IEmployeeService employeeService, IDbContextFactory<AppDbContext> contextFactory)
        {
            _employeeService = employeeService;
            _contextFactory = contextFactory;
            LoadDropdown();
        }

        [ObservableProperty]
        private Employee? employee;

        [ObservableProperty]
        private int employeeId;

        [RelayCommand]
        public async Task LoadEmployeeAsync()
        {
            if(EmployeeId > 0)
            {
                Employee = await _employeeService.GetEmployeeByIdAsync(EmployeeId);
            }

            using var context = await _contextFactory.CreateDbContextAsync();
            Departments = new(context.Departments.ToList());
            Jobs = new(context.Jobs.ToList());
            Factories = new(context.Factories.ToList());
            Cities = new(context.Cities.Include(c => c.Provinces).ToList());
            Provinces = new(context.Provinces.ToList());
            Educations = new(context.Educations.ToList());
            Religions = new(context.Religions.ToList());

            initialEmployee = new Employee
            {
                employee_id = Employee.employee_id,
                name = Employee.name,
                email = Employee.email,
                phone_number = Employee.phone_number,
                gender = Employee.gender,
                birthdate = Employee.birthdate,
                graduation_date = Employee.graduation_date,
                City = new City { city_id = Employee.City?.city_id ?? 0, city_name = Employee.City?.city_name },
                Department = new Department { department_id = Employee.Department?.department_id ?? 0, name = Employee.Department?.name },
                Job = new Job { job_id = Employee.Job?.job_id ?? 0, job_name = Employee.Job?.job_name },
                Factory = new Factory { factory_id = Employee.Factory?.factory_id ?? 0, name = Employee.Factory?.name },
                Education = new Education
                {
                    education_id = Employee.Education?.education_id ?? 0,
                    education_type = Employee.Education?.education_type,
                    major = Employee.Education?.major
                },
                Religion = new Religion { religion_id = Employee.Religion?.religion_id ?? 0, religion_name = Employee.Religion?.religion_name }
            };

            if (Employee != null)
            {
                SelectedDepartment = Departments.FirstOrDefault(d => d.department_id == Employee.Department.department_id);
                SelectedJob = Jobs.FirstOrDefault(j => j.job_id == Employee.Job?.job_id);
                SelectedFactory = Factories.FirstOrDefault(f => f.factory_id == Employee.Factory?.factory_id);
                SelectedCity = Cities.FirstOrDefault(c => c.city_id == Employee.City?.city_id);
                SelectedEducation = Educations.FirstOrDefault(e => e.education_id == Employee.Education?.education_id);
                SelectedReligion = Religions.FirstOrDefault(r => r.religion_id == Employee.Religion.religion_id);
                SelectedGender = Employee.gender.ToLower();
                SelectedBirthdate = Employee.birthdate;
                SelectedGraduationDate = Employee.graduation_date ?? DateOnly.FromDateTime(DateTime.Today);
                SelectedEmployeeStatus = Employee.employee_status.ToUpper();

                CurrentDepartment = SelectedDepartment.name;
                CurrentJob = SelectedJob.job_name;
                CurrentFactory = SelectedFactory.name;
                CurrentCity = SelectedCity.city_name;
                CurrentReligion = SelectedReligion.religion_name;
                CurrentProvince = Provinces.FirstOrDefault(e => e.province_id == SelectedCity.province_id).province_name;

                var edType = SelectedEducation.education_type;
                var major = SelectedEducation.major;
                currentEducation =edType + $" - " + major;
            }

            OnPropertyChanged(nameof(Departments));
            OnPropertyChanged(nameof(Jobs));
            OnPropertyChanged(nameof(Factories));
            OnPropertyChanged(nameof(Cities));
            OnPropertyChanged(nameof(Provinces));
            OnPropertyChanged(nameof(Educations));
            OnPropertyChanged(nameof(Religions));
            OnPropertyChanged(nameof(CurrentEducation));
            OnPropertyChanged(nameof(SelectedBirthdateDateTime));
            OnPropertyChanged(nameof(SelectedGraduationDateTime));
        }

        partial void OnSelectedCityChanged(City value)
        {
            if (value != null)
            {
                SelectedProvince = Provinces.FirstOrDefault(p => p.province_id == value.province_id);
            }
        }

        [RelayCommand]
        public async Task SaveAsync()
        {
            if (Employee != null)
            {
                Employee.Department = SelectedDepartment;
                Employee.Job = SelectedJob;
                Employee.Factory = SelectedFactory;
                Employee.City = SelectedCity;
                Employee.Education = SelectedEducation;
                Employee.Religion = SelectedReligion;
                Employee.gender = SelectedGender;
                Employee.birthdate = SelectedBirthdate;
                Employee.employee_status = SelectedEmployeeStatus;
                Employee.graduation_date = SelectedGraduationDate;

                //Log Entries
                var logs = new List<LogEmployee>();
                
                bool AreStringsEqual(string? a, string? b) =>
                string.Equals(a?.Trim(), b?.Trim(), StringComparison.OrdinalIgnoreCase);

                void AddChange(string field, string? oldVal, string? newVal)
                {
                    logs.Add(new LogEmployee
                    {
                        employee_id = Employee.employee_id,
                        field_name = field,
                        old_value = oldVal ?? "-",
                        new_value = newVal ?? "-",
                        updated_by = "admin",
                        updated_at = DateTime.Now,
                        deleted_at = DateTime.MinValue
                    });
                }

                if (!AreStringsEqual(initialEmployee.name, Employee.name))
                    AddChange("Name", initialEmployee.name, Employee.name);

                if (!AreStringsEqual(initialEmployee.email, Employee.email))
                    AddChange("Email", initialEmployee.email, Employee.email);

                if (!AreStringsEqual(initialEmployee.phone_number, Employee.phone_number))
                    AddChange("Phone", initialEmployee.phone_number, Employee.phone_number);

                if (initialEmployee.gender != SelectedGender)
                    AddChange("Gender", initialEmployee.gender, SelectedGender);

                if (initialEmployee.employee_status != SelectedEmployeeStatus)
                    AddChange("Employee Status", initialEmployee.employee_status, SelectedEmployeeStatus);

                if (SelectedCity != null)
                    AddChange("City", initialEmployee.City?.city_name, SelectedCity?.city_name);

                //if (initialEmployee.City.Provinces?.province_id != SelectedProvince?.province_id)
                //    AddChange("Province", initialEmployee.City.Provinces?.province_name, SelectedProvince?.province_name);

                //if (initialEmployee.Department?.department_id != SelectedDepartment?.department_id)
                //    AddChange("Department", initialEmployee.Department?.name, SelectedDepartment?.name);

                //if (initialEmployee.Job?.job_id != SelectedJob?.job_id)
                //    AddChange("Job", initialEmployee.Job?.job_name, SelectedJob?.job_name);

                if (initialEmployee.Factory?.factory_id != SelectedFactory?.factory_id)
                    AddChange("Factory", initialEmployee.Factory?.name, SelectedFactory?.name);

                //if (initialEmployee.Education?.education_id != SelectedEducation?.education_id)
                //{
                //    var oldEdu = $"{initialEmployee.Education?.education_type} - {initialEmployee.Education?.major}";
                //    var newEdu = $"{SelectedEducation?.education_type} - {SelectedEducation?.major}";
                //    AddChange("Education", oldEdu, newEdu);
                //}

                //if (initialEmployee.Religion?.religion_id != SelectedReligion?.religion_id)
                //    AddChange("Religion", initialEmployee.Religion?.religion_name, SelectedReligion?.religion_name);

                if (initialEmployee.birthdate != SelectedBirthdate)
                    AddChange("Birthdate", initialEmployee.birthdate.ToString(), SelectedBirthdate.ToString());

                if (initialEmployee.graduation_date != SelectedGraduationDate)
                    AddChange("Graduation Date", initialEmployee.graduation_date?.ToString(), SelectedGraduationDate.ToString());


                if (logs.Count > 0)
                {
                    using var context = await _contextFactory.CreateDbContextAsync();
                    context.LogEmployees.AddRange(logs);
                    await context.SaveChangesAsync();
                }


                await _employeeService.UpdateEmployeeAsync(Employee);
                await Shell.Current.GoToAsync("..");
            }
        }

        [RelayCommand]
        public async Task Back()
        {
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private async Task LoadDropdown()
        {
            Genders.Add("laki-laki");
            Genders.Add("perempuan");

            EmployeeStatus.Add("ACTIVE");
            EmployeeStatus.Add("NON ACTIVE");
        }
    }
}
