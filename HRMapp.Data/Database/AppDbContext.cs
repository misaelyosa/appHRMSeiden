using HRMapp.Data.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMapp.Data.Database
{
    public class AppDbContext : DbContext
    {
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Education> Educations { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Province> Provinces { get; set; }
        public DbSet<Religion> Religions { get; set; }
        public DbSet<Factory> Factories { get; set; }
        public DbSet<LogEmployee> LogEmployees { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = "server=localhost;user=root;password=;database=hrmseiden";
            var serverVersion = new MySqlServerVersion(new Version(10, 4, 28));
            optionsBuilder.UseMySql(connectionString, serverVersion);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>()
            .HasMany(e => e.Contracts)
            .WithOne(c => c.Employee)
            .HasForeignKey(c => c.employee_id);

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.LogEmployees)
                .WithOne(c => c.Employee)
                .HasForeignKey(c => c.employee_id);

            modelBuilder.Entity<City>()
                .HasOne(c => c.Province)
                .WithMany()
                .HasForeignKey(c => c.province_id);

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Job)
                .WithMany()
                .HasForeignKey(e => e.job_id);

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Department)
                .WithMany()
                .HasForeignKey(e => e.department_id);

            modelBuilder.Entity<Department>()
                .HasOne(d => d.Factory)
                .WithMany()
                .HasForeignKey(d => d.factory_id);
        }
    }
}
