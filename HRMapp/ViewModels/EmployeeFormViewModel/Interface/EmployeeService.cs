using HRMapp.Data.Database;
using HRMapp.Data.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMapp.ViewModels.EmployeeFormViewModel.Interface
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public EmployeeService(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        /// <summary>
        /// GetEmployeeByIdAsync used for the form on the EditEmployeeForm view.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Employee?> GetEmployeeByIdAsync(int id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            return await context.Employees
                .Include(e => e.Department)
                .Include(e => e.Job)
                .Include(e => e.Factory)
                .Include(e => e.City)
                    .ThenInclude(c => c.Provinces)
                .Include(e => e.Education)
                .Include(e => e.Religion)
                .FirstOrDefaultAsync(e => e.employee_id == id);
        }

        public async Task CreateEmployeeAsync(Employee employee)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            context.Employees.Add(employee);
            await context.SaveChangesAsync();
        }

        public async Task UpdateEmployeeAsync(Employee employee)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            context.Employees.Update(employee);
            await context.SaveChangesAsync();
        }


        public async Task DeleteEmployeeAsync(int id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var employee = await context.Employees.FindAsync(id);

            if (employee != null)
            {
                context.Employees.Remove(employee);
                await context.SaveChangesAsync();
            }
        }

        public async Task<Contract>? GetContractDetail(int contractId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Contracts.FindAsync(contractId);
        }
        public async Task UpdateContractAsync(Contract contract)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            context.Contracts.Update(contract);
            await context.SaveChangesAsync();
        }
        public async Task CreateContractAsync(Contract contract)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            context.Contracts.Add(contract);
            await context.SaveChangesAsync();
        }
    }

}
