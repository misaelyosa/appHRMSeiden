using HRMapp.Data.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;

namespace HRMapp.Data.Database
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            var connectionString = "server=localhost;user=root;password=;database=hrmseiden;AllowZeroDateTime=True;ConvertZeroDateTime=True";
            var serverVersion = new MySqlServerVersion(new Version(10, 4, 28));

            optionsBuilder.UseMySql(connectionString, serverVersion);

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
