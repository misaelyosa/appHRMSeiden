using HRMapp.Data.Database;
using HRMapp.Pages;
using HRMapp.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HRMapp;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

        // Register AppDbContext with MySQL
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseMySql("server=localhost;user=root;password=;database=hrmseiden",
                             new MySqlServerVersion(new Version(10, 4, 28))));

        // Register ViewModels
        builder.Services.AddSingleton<EmployeeListViewModel>();
        builder.Services.AddTransient<EmployeeDetailViewModel>();

        // Register Pages
        builder.Services.AddSingleton<ManageEmployee>();
        builder.Services.AddTransient<EmployeeDetailPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
