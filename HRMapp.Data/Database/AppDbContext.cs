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
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; } 
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
        public DbSet<Session> Session { get; set; }
        public DbSet<Course> Course { get; set; }

        public DbSet<Tunjangan> Tunjangan { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = "server=localhost;user=root;password=;database=hrmseiden;AllowZeroDateTime=True;ConvertZeroDateTime=True";
            var serverVersion = new MySqlServerVersion(new Version(10, 4, 28));
            optionsBuilder.UseMySql(connectionString, serverVersion);
        }

        //Define table relationship
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

            modelBuilder.Entity<Employee>()
                .HasOne(c => c.Job)
                .WithMany()
                .HasForeignKey(c => c.job_id);

            //modelBuilder.Entity<Job>()
            //    .HasOne(e => e.Department)
            //    .WithMany(c => c.Jobs)
            //    .HasForeignKey(e => e.department_id);

            //modelBuilder.Entity<Department>()
            //    .HasOne(d => d.Factory)
            //    .WithMany()
            //    .HasForeignKey(d => d.factory_id);

            modelBuilder.Entity<Employee>()
                .HasOne(c => c.Factory)
                .WithMany()
                .HasForeignKey(c => c.factory_id);

            modelBuilder.Entity<Course>()
                .HasOne(c => c.Employee)
                .WithMany(e => e.Courses)
                .HasForeignKey(c => c.employee_id);

            modelBuilder.Entity<Tunjangan>()
                .HasOne(c => c.Contract)
                .WithMany()
                .HasForeignKey(c => c.contract_id);
            modelBuilder.Entity<Session>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.user_id);

            modelBuilder
                .Entity<Session>()
                .Property(s => s.status)
                .HasConversion<string>();
        }
    }
}
