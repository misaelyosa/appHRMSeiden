using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HRMapp.Data.Database;
using HRMapp.Data.Model;
using HRMapp.Pages.EmployeeForms.Popups.CutiPopup;
using HRMapp.ViewModels;
using HRMapp.ViewModels.EmployeeFormViewModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Diagnostics;

[QueryProperty(nameof(EmployeeId), "employeeId")]
public partial class EmployeeDetailViewModel : ObservableObject
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;
    private readonly IEmployeeService _employeeService;

    [ObservableProperty]
    private Employee employee;

    [ObservableProperty]
    private int employeeId;

    [ObservableProperty]
    private ObservableCollection<Contract> contractz = new();

    [ObservableProperty]
    private bool isRefreshing;

    [ObservableProperty]
    private int contractCount;

    [ObservableProperty]
    private int contractId;

    [ObservableProperty]
    private ObservableCollection<LogEmployee> logEntries = new();

    public EmployeeDetailViewModel(IDbContextFactory<AppDbContext> dbContextFactory, IEmployeeService employeeService)
    {
        _dbContextFactory = dbContextFactory;
        _employeeService = employeeService;
    }

    public async Task LoadEmployeeDetails()
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        Employee = await dbContext.Employees
            .Include(e => e.Department)
            .Include(e => e.Job)
            .Include(e => e.Factory)
            .Include(e => e.Education)
            .Include(e => e.City)
                .ThenInclude(City => City.Provinces)
            .Include(e => e.Religion)
            .FirstOrDefaultAsync(e => e.employee_id == EmployeeId);

        await LoadContracts();
        await LoadCuti();
        await LoadLogsAsync();
    }

    public async Task LoadContracts()
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        var contracts = await dbContext.Contracts
               .Where(e => e.employee_id == EmployeeId)
               .OrderBy(e => e.contract_date)
               .ToListAsync();

        Contractz = new ObservableCollection<Contract>(contracts); 

    }

    [RelayCommand]
    private async Task NavigateToGeneratePKWT(Contract contract)
    {
        if (contract == null) return;

        var index = Contractz.IndexOf(contract);
        Debug.WriteLine($"Navigating to PKWT page - Employee: {EmployeeId}, Contract: {contract.contract_id}, Index: {index}");

        await Shell.Current.GoToAsync($"GeneratePKWTPage?employeeId={EmployeeId}&contractId={contract.contract_id}&contractIndex={index}");
    }
    
    [RelayCommand]
    private async Task NavigateToCreateContractPage()
    {
        await Shell.Current.GoToAsync($"CreateContractForm?employeeId={EmployeeId}");
    }

    public async Task LoadLogsAsync()
    {
        using var dbcontext = await _dbContextFactory.CreateDbContextAsync();
        var logs = await dbcontext.LogEmployees
            .Where(l => l.employee_id == EmployeeId)
            .OrderByDescending(l => l.updated_at)
            .ToListAsync();

        LogEntries = new ObservableCollection<LogEmployee>(logs);
        OnPropertyChanged(nameof(LogEntries));
    }

    [RelayCommand]
    private async Task RefreshData()
    {
        IsRefreshing = true;
        await LoadContracts();
        await Task.Delay(100);
        IsRefreshing = false;
    }

    [RelayCommand]
    private async Task EditEmployee()
    {
        await Shell.Current.GoToAsync($"EmployeeForms/Edit?employeeId={Employee.employee_id}");
    }

    [RelayCommand]
    private async Task DeleteEmployee()
    {
        if (Employee == null)
            return;

        bool confirmDelete = await Shell.Current.DisplayAlert("Confirm Delete",
            $"Apakah anda yakin akan menghapus data {Employee?.name}?", "Yes", "Cancel");

        if (!confirmDelete)
            return;

        bool confirmRelatedData = await Shell.Current.DisplayAlert("Confirm Deletion",
            "Apakah anda yakin? Semua data yang berhubungan akan terhapus dan tidak dapat di restore", "Yes", "Cancel");
        if (!confirmRelatedData)
            return;

        try
        {
            await _employeeService.DeleteEmployeeAsync(Employee.employee_id);

            await Shell.Current.DisplayAlert("Success", $"Data {Employee?.name} dan semua related data berhasil terhapus.", "OK");
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error deleting employee: {ex.Message}");
            await Shell.Current.DisplayAlert("Error", "Gagal menghapus data karyawan.", "OK");
        }
    }

    [RelayCommand]
    private async Task DeleteContract(Contract contract)
    {
        if (Employee == null)
            return;

        bool confirmDelete = await Shell.Current.DisplayAlert("Confirm Delete",
            $"Apakah anda yakin akan menghapus data kontrak?", "Yes", "Cancel");

        if (!confirmDelete)
            return;

        bool confirmRelatedData = await Shell.Current.DisplayAlert("Confirm Deletion",
            "Apakah anda yakin? Data yang terhapus tidak dapat di restore", "Yes", "Cancel");
        if (!confirmRelatedData)
            return;

        try
        {
            await _employeeService.DeleteContractAsync(contract);

            await Shell.Current.DisplayAlert("Success", $"Data kontrak berhasil terhapus.", "OK");
            await RefreshData();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error deleting employee: {ex.Message}");
            await Shell.Current.DisplayAlert("Error", "Gagal menghapus data kontrak.", "OK");
        }
    }

    //CUTII
    [ObservableProperty]
    public ObservableCollection<Cuti> cutis;
    public async Task LoadCuti()
    {
        Cutis = new ObservableCollection<Cuti>(await _employeeService.GetCutiByEmpId(EmployeeId));
    }

    [ObservableProperty]
    public string cutiDuration;
    [ObservableProperty]
    public DateOnly cutiStartDate;
    [ObservableProperty]
    public string cutiReason;
    [ObservableProperty]
    private string cutiEndDateDisplay; 
    public DateTime CutiStartDateProxy
    {
        get => CutiStartDate.ToDateTime(TimeOnly.MinValue);
        set
        {
            if (CutiStartDate != DateOnly.FromDateTime(value))
            {
                CutiStartDate = DateOnly.FromDateTime(value);
                OnPropertyChanged(nameof(CutiStartDateProxy));
            }
        }
    }

    [RelayCommand]
    public async Task CreateCuti()
    {
        Debug.WriteLine("[DEBUG] CREATE CUTI INVOKED");
        var cutiDur = int.Parse(CutiDuration);
        if (cutiDur > 0 && CutiStartDate != default)
        {
            var newCuti = new Cuti
            {
                cuti_day_count = cutiDur,
                cuti_start_date = CutiStartDate,
                cuti_end_date = CutiStartDate.AddDays(cutiDur),
                reason = CutiReason,
                employee_id = EmployeeId,
                created_by = Preferences.Get("username", "admin"),
                created_at = DateTime.Now
            };

                await _employeeService.CreateCuti(newCuti);
        }   
    }

    [RelayCommand] 
    public async Task OnOpenEditCutiPopup()
    {

    }

    [RelayCommand]
    public async Task EditCuti()
    {

    }
}
    