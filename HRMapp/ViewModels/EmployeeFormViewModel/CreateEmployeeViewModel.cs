using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HRMapp.Data.Database;
using HRMapp.Data.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private City selectedProvince;
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
        private string selectedEmployeeStatus;

        [ObservableProperty]
        private string newDepartment;
        [ObservableProperty]
        private string newJob;
        [ObservableProperty]
        private string newReligion;
        [ObservableProperty]
        private string newCityName;
        [ObservableProperty]
        private string newProvinceName;
        [ObservableProperty]
        private string newFactoryName;
        [ObservableProperty]
        private string newFactoryAddress;
        [ObservableProperty]
        private int newFactoryCapacity;
        [ObservableProperty]
        private string newEducationType;
        [ObservableProperty]
        private string newEducationMajor;

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

        public CreateEmployeeViewModel(IEmployeeService employeeService, IDbContextFactory<AppDbContext> dbContextFactory)
        {
            _employeeService = employeeService;
            _dbContextFactory = dbContextFactory;
            LoadDropdown();
        }

        private async Task LoadDropdown()
        {
            Genders.Add("laki-laki");
            Genders.Add("perempuan");

            EmployeeStatus.Add("Active");
            EmployeeStatus.Add("Non Active");

            using var context = await _dbContextFactory.CreateDbContextAsync();
            Departments = new(context.Departments.ToList());
            Jobs = new(context.Jobs.ToList());
            Factories = new(context.Factories.ToList());
            Cities = new(context.Cities.Include(c => c.Provinces).ToList());
            Provinces = new(context.Provinces.ToList());
            Educations = new(context.Educations.ToList());
            Religions = new(context.Religions.ToList());
        }

        [RelayCommand]
        private async Task SubmitAsync()
        {
            var insertEmployee = new Employee
            {
                name = NewEmployee.name,
                nip = NewEmployee.nip,
                Department = SelectedDepartment
            };

            await _employeeService.CreateEmployeeAsync(insertEmployee);
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private async Task Back()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
