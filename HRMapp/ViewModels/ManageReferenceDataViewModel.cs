using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HRMapp.Data.Model;
using HRMapp.Pages.EmployeeForms.Popups.EditManageReference;
using HRMapp.ViewModels.EmployeeFormViewModel;
using HRMapp.ViewModels.EmployeeFormViewModel.Interface;
using Microsoft.EntityFrameworkCore.Metadata;
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
        [ObservableProperty]
        private int deptId;
        [RelayCommand]
        private async Task AddNewDepartment()
        {
            await _employeeService.AddNewDepartment(NewDepartment);
            LoadDept();
        }
        [RelayCommand]
        private async Task DeleteDepartment(Department department)
        {
            if (department == null)
                return;

            bool confirm = await Application.Current.MainPage.DisplayAlert("Konfirmasi", $"Hapus departemen '{department.name}'?", "Ya", "Batal");
            if (confirm)
            {
                await _employeeService.DeleteDepartment(department.department_id);
                LoadDept();
            }
        }
        [RelayCommand]
        private async Task OpenEditPopupDepartment(Department dept)
        {
            if (dept == null)
            {
                return;
            }
            ;

            var fetchExisting = await _employeeService.fetchExistingDepartmentClicked(dept.department_id);
            if (fetchExisting != null)
            {
                DeptId = dept.department_id;
                NewDepartment = dept.name;

                var popup = new EditDepartment(this);

                await Shell.Current.CurrentPage.ShowPopupAsync(popup);
            }
        }
        [RelayCommand]
        private async Task EditDepartment()
        {
            if (string.IsNullOrWhiteSpace(NewDepartment))
            {
                await App.Current.MainPage.DisplayAlert("Invalid Input", "Please fill in all fields", "OK");
                return;
            }

            await _employeeService.EditDepartment(DeptId, NewDepartment);
            LoadDept();
        }


        //CRUD JOB
        [ObservableProperty]
        private string newJob;
        [ObservableProperty]
        private int jobId;
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
        [RelayCommand]
        private async Task OpenEditPopupJob(Job job)
        {
            if (job == null)
            {
                return;
            };

            var fetchExisting = await _employeeService.fetchExistingJobClicked(job.job_id);
            if (fetchExisting != null)
            {
                JobId = job.job_id;
                NewJob = fetchExisting.job_name;

                var popup = new EditJob(this);

                await Shell.Current.CurrentPage.ShowPopupAsync(popup);
            }
        }
        [RelayCommand]
        private async Task EditJob()
        {
            if (string.IsNullOrWhiteSpace(NewJob))
            {
                await App.Current.MainPage.DisplayAlert("Invalid Input", "Please fill in all fields", "OK");
                return;
            }

            await _employeeService.EditJob(JobId, NewJob);
            LoadJob();
        }

        //CRUD EDUCATION
        [ObservableProperty]
        private string newEducationType;
        [ObservableProperty]
        private string newEducationMajor;
        [ObservableProperty]
        private int eduId;
        [RelayCommand]
        private async Task AddNewEducation()
        {
            await _employeeService.AddNewEducation(NewEducationType, NewEducationMajor);
            string combinedDisplayName = string.IsNullOrWhiteSpace(NewEducationMajor)
                                            ? NewEducationType
                                            : $"{NewEducationType} - {NewEducationMajor}";
            LoadEducation();
        }
        [RelayCommand]
        private async Task DeleteEducation(Education education)
        {
            if (education == null)
                return;

            bool confirm = await Application.Current.MainPage.DisplayAlert("Konfirmasi", $"Hapus pendidikan '{education.DisplayName}'?", "Ya", "Batal");
            if (confirm)
            {
                await _employeeService.DeleteEducation(education.education_id);
                LoadEducation();
            }
        }
        [RelayCommand]
        private async Task OpenEditPopupEducation(Education edu)
        {
            if (edu == null)
            {
                return;
            };

            var fetchExisting = await _employeeService.fetchExistingEducationClicked(edu.education_id);
            if (fetchExisting != null)
            {
                EduId = edu.education_id;
                NewEducationType = fetchExisting.education_type;
                NewEducationMajor = fetchExisting.major;

                var popup = new EditEducation(this);

                await Shell.Current.CurrentPage.ShowPopupAsync(popup);
            }
        }
        [RelayCommand]
        private async Task EditEducation()
        {
            if (string.IsNullOrWhiteSpace(NewEducationType) || string.IsNullOrWhiteSpace(NewEducationMajor))
            {
                await App.Current.MainPage.DisplayAlert("Invalid Input", "Please fill in all fields", "OK");
                return;
            }

            await _employeeService.EditEducation(EduId, NewEducationType, NewEducationMajor);
            LoadEducation();
        }


        //CRUD AGAMA
        [ObservableProperty]
        private string newRelg;
        [ObservableProperty]
        private int relgId;
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
        [RelayCommand]
        private async Task OpenEditPopupReligion(Religion relg)
        {
            if (relg == null)
            {
                return;
            }
;

            var fetchExisting = await _employeeService.fetchExistingReligion(relg.religion_id);
            if (fetchExisting != null)
            {
                RelgId = relg.religion_id;
                NewRelg = relg.religion_name;

                var popup = new EditReligion(this);

                await Shell.Current.CurrentPage.ShowPopupAsync(popup);
            }
        }
        [RelayCommand]
        private async Task EditReligion()
        {
            if (string.IsNullOrWhiteSpace(NewRelg))
            {
                await App.Current.MainPage.DisplayAlert("Invalid Input", "Please fill in all fields", "OK");
                return;
            }

            await _employeeService.EditReligion(RelgId, NewRelg);
            LoadRelg();
        }

        //CRUD CityProv
        [ObservableProperty]
        private string newCityName;
        [ObservableProperty]
        private string newProvinceName;
        [ObservableProperty] 
        private int cityId;
        [RelayCommand]
        private async Task AddNewCityProvince()
        {
            if (!string.IsNullOrEmpty(NewCityName) && !string.IsNullOrEmpty(NewProvinceName))
            {
                await _employeeService.AddNewCityProvince(NewCityName, NewProvinceName);
                LoadCityProv();
            }
        }
        [RelayCommand]
        private async Task DeleteCityProvince(City cityProvince)
        {
            if (cityProvince == null)
                return;

            bool confirm = await Application.Current.MainPage.DisplayAlert("Konfirmasi", $"Hapus kota/provinsi '{cityProvince.city_name}, {cityProvince.Provinces.province_name}'?", "Ya", "Batal");
            if (confirm)
            {
                await _employeeService.DeleteCityProvince(cityProvince.city_id);
                LoadCityProv();
            }
        }
        [RelayCommand]
        private async Task OpenEditPopupCityProvince(City cityProvince)
        {
            if(cityProvince == null)
            {
                return;
            };

            var fetchExisting = await _employeeService.fetchExistingCityClicked(cityProvince.city_id);
            if (fetchExisting != null )
            {
                CityId = fetchExisting.city_id;
                NewCityName = fetchExisting.city_name;
                NewProvinceName = fetchExisting.Provinces.province_name;

                var popup = new EditCityProvince(this);

                await Shell.Current.CurrentPage.ShowPopupAsync(popup);
            }
        }
        [RelayCommand]
        private async Task EditCityProvince()
        {
            if (string.IsNullOrWhiteSpace(NewCityName) || string.IsNullOrWhiteSpace(NewProvinceName))
            {
                await App.Current.MainPage.DisplayAlert("Invalid Input", "Please fill in all fields", "OK");
                return;
            }

            await _employeeService.EditCityProvince(CityId, NewCityName, NewProvinceName);
            LoadCityProv();
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
