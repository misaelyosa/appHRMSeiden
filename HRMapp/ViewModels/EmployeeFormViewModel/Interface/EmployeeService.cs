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
            var employee = await context.Employees
                .Include(e => e.Contracts)
                .Include(e => e.Courses)
                .Include(e => e.LogEmployees)
                .FirstOrDefaultAsync(e => e.employee_id == id);

            if (employee != null)
            {
                foreach (var contract in employee.Contracts)
                {
                    var listTunjangan = await context.Tunjangan
                        .Where(t => t.contract_id == contract.contract_id)
                        .ToListAsync();

                    context.Tunjangan.RemoveRange(listTunjangan);
                }

                context.Contracts.RemoveRange(employee.Contracts);
                context.Course.RemoveRange(employee.Courses);
                context.LogEmployees.RemoveRange(employee.LogEmployees);
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
        public async Task DeleteContractAsync(Contract contract)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            if (contract != null)
            {
                context.Contracts.Remove(contract);
                await context.SaveChangesAsync();
            }
        }

        //Tunjangan
        public async Task CreateTunjanganAsync(Tunjangan tunjangan)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            context.Tunjangan.Add(tunjangan);
            await context.SaveChangesAsync();
        }
        public async Task <Tunjangan> GetTunjanganMK(int contractId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            return await context.Tunjangan.Where(e => e.contract_id == contractId && 
            e.tunjangan_name.Contains("MK", StringComparison.OrdinalIgnoreCase)).FirstOrDefaultAsync();
        }        
        public async Task <Tunjangan> GetTunjanganOther(int contractId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            return await context.Tunjangan.Where(e => e.contract_id == contractId &&
            !e.tunjangan_name.Contains("MK", StringComparison.OrdinalIgnoreCase)).FirstOrDefaultAsync();
        }
    }
}
