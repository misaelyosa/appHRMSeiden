using HRMapp.Data.Database;
using HRMapp.Pages;
using HRMapp.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UraniumUI;

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
			})
			.UseUraniumUI()
			.UseUraniumUIMaterial();

        // Register AppDbContext with MySQL
        builder.Services.AddDbContextFactory<AppDbContext>(options =>
            options.UseMySql("server=localhost;user=root;password=;database=hrmseiden;AllowZeroDateTime=True;ConvertZeroDateTime=True",
                             new MySqlServerVersion(new Version(10, 4, 28))));

        // Register ViewModels
        builder.Services.AddSingleton<EmployeeListViewModel>();
        builder.Services.AddTransient<EmployeeDetailViewModel>();
		builder.Services.AddSingleton<MainPageViewModel>();

        // Register Pages
        builder.Services.AddTransient<ManageEmployee>();
        builder.Services.AddTransient<EmployeeDetailPage>();
		builder.Services.AddSingleton<MainPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
