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

                Debug.WriteLine($"Factory : " + SelectedFactory.name);
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

        //LOG EMPLOYEE
        private List<string> GetChanges()
        {
            var changes = new List<string>();

            if (Employee.name != Employee.name) changes.Add($"Full Name: {Employee.name} → {Employee.name}");
            if (Employee.email != Employee.email) changes.Add($"Email: {Employee.email} → {Employee.email}");
            if (Employee.phone_number != Employee.phone_number) changes.Add($"Phone: {Employee.phone_number} → {Employee.phone_number}");

            if (Employee.gender != SelectedGender)
                changes.Add($"Gender: {Employee.gender} → {SelectedGender}");

            if (Employee.City?.city_name != SelectedCity?.city_name)
                changes.Add($"City: {Employee.City?.city_name} → {SelectedCity?.city_name}");

            if (Employee.Department?.name != SelectedDepartment?.name)
                changes.Add($"Department: {Employee.Department?.name} → {SelectedDepartment?.name}");

            if (Employee.Job?.job_name != SelectedJob?.job_name)
                changes.Add($"Job: {Employee.Job?.job_name} → {SelectedJob?.job_name}");

            if (Employee.Factory?.name != SelectedFactory?.name)
                changes.Add($"Factory: {Employee.Factory?.name} → {SelectedFactory?.name}");

            if (Employee.Education?.education_type != SelectedEducation?.education_type ||
                Employee.Education?.major != SelectedEducation?.major)
                changes.Add($"Education: {Employee.Education?.education_type} {Employee.Education?.major} → {SelectedEducation?.education_type} {SelectedEducation?.major}");

            if (Employee.Religion?.religion_name != SelectedReligion?.religion_name)
                changes.Add($"Religion: {Employee.Religion?.religion_name} → {SelectedReligion?.religion_name}");

            if (Employee.birthdate != SelectedBirthdate)
                changes.Add($"Birthdate: {Employee.birthdate} → {SelectedBirthdate}");

            if (Employee.graduation_date != SelectedGraduationDate)
                changes.Add($"Graduation Date: {Employee.graduation_date} → {SelectedGraduationDate}");

            return changes;
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

                var changes = GetChanges();
                if (changes.Count > 0)
                {
                    var log = new LogEmployee
                    {
                        employee_id = Employee.employee_id,
                        data_changed = string.Join(", ", changes),
                        updated_by = "admin",
                        updated_at = DateTime.Now,
                        deleted_at = DateTime.MinValue
                    };

                    using var context = await _contextFactory.CreateDbContextAsync();
                    context.LogEmployees.Add(log);
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
