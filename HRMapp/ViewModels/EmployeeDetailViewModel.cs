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
    [ObservableProperty]
    private string jatahCuti; 
    public async Task LoadCuti()
    {
        Cutis = new ObservableCollection<Cuti>(await _employeeService.GetCutiByEmpId(EmployeeId));
        JatahCuti = await _employeeService.LoadJatahCuti(EmployeeId);
    }

    [ObservableProperty]
    public string cutiDuration;
    [ObservableProperty]
    public DateOnly cutiStartDate;
    [ObservableProperty]
    public string cutiReason;
    [ObservableProperty]
    private string cutiEndDateDisplay;
    [ObservableProperty]
    public Cuti selectedCuti;
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
                cuti_end_date = CutiStartDate.AddDays(cutiDur-1),
                reason = CutiReason,
                employee_id = EmployeeId,
                created_by = Preferences.Get("username", "admin"),
                created_at = DateTime.Now
            };

            await _employeeService.CreateCuti(newCuti);
            await LoadCuti();
        }   
    }

    [RelayCommand]
    public async Task DeleteCuti(Cuti cuti)
    {
        if (cuti == null)
            return;

        bool confirm = await Application.Current.MainPage.DisplayAlert("Konfirmasi", $"Hapus cuti '{cuti.cuti_start_date}'?", "Ya", "Batal");
        if (confirm)
        {
            await _employeeService.DeleteCuti(cuti.cuti_id);
            await LoadCuti();
        }
    }


    [RelayCommand] 
    public async Task OnOpenEditCutiPopup(Cuti cuti)
    {
        if (cuti == null)
        {
            return;
        }

        var existing = await _employeeService.fetchExistingCuti(cuti.cuti_id);
        if (existing != null)
        {
            SelectedCuti = existing;
            CutiDuration = existing.cuti_day_count.ToString();
            CutiStartDateProxy =  existing.cuti_start_date.ToDateTime(TimeOnly.MinValue);
            CutiReason = existing.reason;
        }

        var popup = new EditCuti(this);
        Shell.Current.ShowPopup(popup);
    }

    [RelayCommand]
    public async Task EditCuti()
    {
        if (string.IsNullOrWhiteSpace(CutiDuration) || !int.TryParse(CutiDuration, out int cutiDur) || cutiDur <= 0)
        {
            await Application.Current.MainPage.DisplayAlert("Invalid Input", "Masukkan jumlah hari cuti yang valid.", "OK");
            return;
        }

        var updateCuti = new Cuti
        {
            cuti_id = SelectedCuti.cuti_id,
            employee_id = SelectedCuti.employee_id,
            cuti_day_count = cutiDur,
            cuti_start_date = DateOnly.FromDateTime(CutiStartDateProxy),
            cuti_end_date = DateOnly.FromDateTime(CutiStartDateProxy.AddDays(cutiDur-1)),
            updated_at = DateTime.Now,
            updated_by = Preferences.Get("username", "admin")
        };

        try
        {
            await _employeeService.UpdateCuti(updateCuti);
            await LoadCuti();
            await Shell.Current.DisplayAlert("Berhasil", "Data cuti berhasil diperbarui.", "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Gagal", ex.Message, "OK");
        }
        ;
    }
}
    