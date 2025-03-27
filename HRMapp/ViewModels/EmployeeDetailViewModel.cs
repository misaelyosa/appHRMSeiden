using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HRMapp.Data.Database;
using HRMapp.Data.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Diagnostics;

[QueryProperty(nameof(EmployeeId), "employeeId")]
public partial class EmployeeDetailViewModel : ObservableObject
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

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

    public EmployeeDetailViewModel(IDbContextFactory<AppDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
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

            LoadContracts();
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

    [RelayCommand]
    private async Task RefreshData()
    {
        IsRefreshing = true;
        await LoadContracts();
        await Task.Delay(100);
        IsRefreshing = false;
    }
}
    