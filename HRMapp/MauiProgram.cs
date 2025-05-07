using HRMapp.Data.Database;
using HRMapp.Pages;
using HRMapp.Pages.EmployeeForms;
using HRMapp.ViewModels;
using HRMapp.ViewModels.EmployeeFormViewModel;
using HRMapp.ViewModels.EmployeeFormViewModel.Interface;
using InputKit.Handlers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UraniumUI;
using QuestPDF.Infrastructure;
using HRMapp.ViewModels.SessionViewModel;
using HRMapp.Pages.Session;
using HRMapp.ViewModels.SessionViewModel.Interface;

namespace HRMapp;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		QuestPDF.Settings.License = LicenseType.Community;

		var builder = MauiApp.CreateBuilder();
        builder
			.UseMauiApp<App>()
			.ConfigureMauiHandlers(handlers =>
			{
				handlers.AddInputKitHandlers();
			})
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

        builder.Services.AddScoped<IEmployeeService, EmployeeService>();
		builder.Services.AddScoped<ISessionService, SessionService>();

        // Register ViewModels
        builder.Services.AddSingleton<EmployeeListViewModel>();
        builder.Services.AddTransient<EmployeeDetailViewModel>();
		builder.Services.AddSingleton<MainPageViewModel>();
		builder.Services.AddTransient<EditEmployeeViewModel>();
		builder.Services.AddTransient<CreateEmployeeViewModel>();
		builder.Services.AddTransient<GeneratePKWTPageViewModel>();
		builder.Services.AddTransient<CreateContractViewModel>();
		//session
		builder.Services.AddTransient<SignupViewModel>();
		builder.Services.AddTransient<LoginViewModel>();
		builder.Services.AddTransient<ResetPasswordViewModel>();

        // Register Pages
        builder.Services.AddTransient<ManageEmployee>();
        builder.Services.AddTransient<EmployeeDetailPage>();
		builder.Services.AddSingleton<MainPage>();
		builder.Services.AddTransient<EditEmployeeForm>();
		builder.Services.AddTransient<CreateEmployeeForm>();
		builder.Services.AddTransient<GeneratePKWTPage>();
		builder.Services.AddTransient<CreateContractForm>();
		//session
		builder.Services.AddTransient<SignupPage>();
		builder.Services.AddTransient<LoginPage>();
		builder.Services.AddTransient<ResetPasswordPage>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
