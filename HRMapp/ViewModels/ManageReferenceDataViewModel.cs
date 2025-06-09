using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HRMapp.Data.Model;
using HRMapp.ViewModels.EmployeeFormViewModel;
using HRMapp.ViewModels.EmployeeFormViewModel.Interface;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMapp.ViewModels
{
    public partial class ManageReferenceDataViewModel : ObservableObject
    {
        private readonly IEmployeeService _employeeService;

        [ObservableProperty]
        private ObservableCollection<Department> departments = new();
        [ObservableProperty]
        private ObservableCollection<Job> jobs = new();
        [ObservableProperty]
        private ObservableCollection<Religion> religions = new();
        [ObservableProperty]
        private ObservableCollection<Factory> factories = new();
        [ObservableProperty]
        private ObservableCollection<City> cityProvinces = new();
        [ObservableProperty]
        private ObservableCollection<Education> educations = new();

        public ManageReferenceDataViewModel(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
            _ = LoadAll();
        }

        private async Task LoadAll()
        {
            LoadDept();
            LoadJob();
            LoadRelg();
            LoadFactories();
            LoadCityProv();
            LoadEducation();
        }

        private async Task LoadDept() { Departments = new ObservableCollection<Department>(await _employeeService.GetDepartment()); }
        private async Task LoadJob() { Jobs = new ObservableCollection<Job>(await _employeeService.GetJob()); }
        private async Task LoadRelg() { Religions = new ObservableCollection<Religion>(await _employeeService.GetReligion()); }
        private async Task LoadFactories() { Factories = new ObservableCollection<Factory>(await _employeeService.GetFactory()); }
        private async Task LoadCityProv() { CityProvinces = new ObservableCollection<City>(await _employeeService.GetCityProvince()); }
        private async Task LoadEducation() { Educations = new ObservableCollection<Education>(await _employeeService.GetEducation());}

        //CRUD DEPARTMENT
        [ObservableProperty]
        private string newDepartment;
        [RelayCommand]
        private async Task AddNewDepartment()
        {
            await _employeeService.AddNewDepartment(NewDepartment);
            LoadDept();
        }

        //CRUD JOB
        [ObservableProperty]
        private string newJob;
        [RelayCommand]
        private async Task AddNewJob()
        {
            await _employeeService.AddNewJob(NewJob);
            LoadJob();
        }
        [RelayCommand]
        private async Task DeleteJob(Job job)
        {
            if (job == null)
                return;

            bool confirm = await Application.Current.MainPage.DisplayAlert("Konfirmasi", $"Hapus jabatan '{job.job_name}'?", "Ya", "Batal");
            if (confirm)
            {
                await _employeeService.DeleteJob(job.job_id);
                LoadJob();
            }
        }

        //CRUD EDUCATION
        [ObservableProperty]
        private string newEducationType;
        [ObservableProperty]
        private string newEducationMajor;
        [RelayCommand]
        private async Task AddNewEducation()
        {
            await _employeeService.AddNewEducation(NewEducationType, NewEducationMajor);
            string combinedDisplayName = string.IsNullOrWhiteSpace(NewEducationMajor)
                                            ? NewEducationType
                                            : $"{NewEducationType} - {NewEducationMajor}";
            LoadEducation();
        }

        //CRUD AGAMA
        [ObservableProperty]
        private string newRelg;
        [RelayCommand]
        private async Task AddNewReligion()
        {
            await _employeeService.AddNewReligion(NewRelg);
            LoadRelg();
        }
        [RelayCommand]
        private async Task DeleteReligion(Religion agama)
        {
            if (agama == null)
                return;

            bool confirm = await Application.Current.MainPage.DisplayAlert("Konfirmasi", $"Hapus data agama '{agama.religion_name}'?", "Ya", "Batal");
            if (confirm)
            {
                await _employeeService.DeleteReligion(agama.religion_id);
                LoadRelg();
            }
        }

        //CRUD CityProv
        [ObservableProperty]
        private string newCityName;
        [ObservableProperty]
        private string newProvinceName;
        [RelayCommand]
        private async Task AddNewCityProvince()
        {
            if (!string.IsNullOrEmpty(NewCityName) && !string.IsNullOrEmpty(NewProvinceName))
            {
                await _employeeService.AddNewCityProvince(NewCityName, NewProvinceName);
                LoadCityProv();
            }
        }

        //CRUD FACTORY
        [ObservableProperty]
        private string factoryName;
        [ObservableProperty]
        private string factoryAddress;
        [ObservableProperty]
        private string factoryCapacity;
        [RelayCommand]
        private async Task AddNewFactory()
        {
            if (!string.IsNullOrEmpty(FactoryName) && !string.IsNullOrEmpty(FactoryAddress))
            { 
                await _employeeService.AddNewFactory(FactoryName, FactoryAddress, int.Parse(FactoryCapacity));
                LoadFactories();
            }
        }
        [RelayCommand]
        private async Task DeleteFactory(Factory factory)
        {
            if (factory == null)
                return;

            bool confirm = await Application.Current.MainPage.DisplayAlert("Konfirmasi", $"Hapus pabrik '{factory.name}'?", "Ya", "Batal");
            if (confirm)
            {
                await _employeeService.DeleteFactory(factory.factory_id);
                LoadFactories();
            }
        }

    }
}
