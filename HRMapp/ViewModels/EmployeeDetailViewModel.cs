using CommunityToolkit.Mvvm.ComponentModel;
using HRMapp.Data.Database;
using HRMapp.Data.Model;
using Microsoft.EntityFrameworkCore;

[QueryProperty(nameof(EmployeeId), "employeeId")]
public partial class EmployeeDetailViewModel : ObservableObject
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

    [ObservableProperty]
    private Employee employee;

    [ObservableProperty]
    private int employeeId; 

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
    }
}
