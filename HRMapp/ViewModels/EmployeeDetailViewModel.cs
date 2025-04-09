using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HRMapp.Data.Database;
using HRMapp.Data.Model;
using HRMapp.ViewModels.EmployeeFormViewModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
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
    private Tunjangan tunjangan;

    [ObservableProperty]
    private bool isRefreshing;

    [ObservableProperty]
    private int contractCount;

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
        await LoadLogsAsync();
    }

    public async Task LoadContracts()
    {
        using var dbContext = _dbContextFactory.CreateDbContext();

        Contractz = new ObservableCollection<Contract>(await dbContext.Contracts
                    .Where(e => e.employee_id == EmployeeId)
                    .ToListAsync());

        ContractCount = Contractz.Count();
        OnPropertyChanged(nameof(ContractCount));
        Debug.WriteLine(ContractCount);

        if (Contractz.Any())
        {
            var firstContract = Contractz.First();
            Tunjangan = await dbContext.Tunjangan
                .FirstOrDefaultAsync(t => t.contract_id == firstContract.contract_id);
        }
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
        bool confirm = await Shell.Current.DisplayAlert("Confirm Delete", $"Are you sure you want to delete {Employee?.name}?", "Yes", "Cancel");
        if (!confirm || Employee == null)
            return;

        try
        {
            await _employeeService.DeleteEmployeeAsync(Employee.employee_id);
            await Shell.Current.DisplayAlert("Success", "Employee deleted successfully.", "OK");
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error deleting employee: {ex.Message}");
            await Shell.Current.DisplayAlert("Error", "Failed to delete employee.", "OK");
        }
    }
}
    