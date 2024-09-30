using Microcharts.Maui;
using CommunityToolkit.Maui.Markup;
using Microsoft.Extensions.Logging;
using OnboardingSystem.Authentication;
using OnboardingSystem.Global.Menu;
using OnboardingSystem.ViewModels;
using OnboardingSystem.Management;
using CommunityToolkit.Maui;

namespace OnboardingSystem;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMicrocharts()
			.UseMauiCommunityToolkitMarkup()
			.UseMauiCommunityToolkit()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

        builder.Services.AddSingleton<IAuthenticationService, AuthenticationService>();
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddSingleton<HttpClient>();
        builder.Services.AddSingleton<IUserService, UserService>();
        builder.Services.AddTransient<DashboardPage>();
        builder.Services.AddTransient<ManagementPage>();
        builder.Services.AddTransient<UserListPage>(sp => 
            new UserListPage(
                sp.GetRequiredService<IAuthenticationService>(),
                sp.GetRequiredService<IUserService>()
            )
        );
        builder.Services.AddSingleton<AppShell>();

        return builder.Build();
    }
}