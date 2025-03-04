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
            //One to many Employee-Contract
            modelBuilder.Entity<Employee>()
                .HasMany(e => e.Contracts)
                .WithOne(c => c.Employee)
                .HasForeignKey(c => c.employee_id);

            //1 : N employee-log 
            modelBuilder.Entity<Employee>()
                .HasMany(e => e.LogEmployees)
                .WithOne(c => c.Employee)
                .HasForeignKey(c => c.employee_id);

            modelBuilder.Entity<Employee>()
                .HasOne(c => c.Religion)
                .WithMany()
                .HasForeignKey(c => c.religion_id);

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Department)
                .WithMany()
                .HasForeignKey(e => e.department_id);

            //1:N Province-City
            modelBuilder.Entity<City>()
                .HasOne(c => c.Provinces)
                .WithMany()
                .HasForeignKey(c => c.province_id);

            modelBuilder.Entity<Employee>()
                .HasOne(c => c.Education)
                .WithMany()
                .HasForeignKey(c => c.education_id);

            modelBuilder.Entity<Employee>()
                .HasOne(c => c.City)
                .WithMany()
                .HasForeignKey(c => c.city_id);

            modelBuilder.Entity<Job>()
                .HasOne(e => e.Department)
                .WithMany(c => c.Jobs)
                .HasForeignKey(e => e.department_id);

            modelBuilder.Entity<Department>()
                .HasOne(d => d.Factory)
                .WithMany()
                .HasForeignKey(d => d.factory_id);
        }
    }
}
