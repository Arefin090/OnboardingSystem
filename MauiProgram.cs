using Microcharts.Maui;
using CommunityToolkit.Maui.Markup;
using Microsoft.Extensions.Logging;
using OnboardingSystem.Authentication;
using CommunityToolkit.Maui;

namespace OnboardingSystem;

public static class MauiProgram
{

	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()	
			.UseMauiCommunityToolkit()
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
        builder.Services.AddTransient<DashboardPage>();
        builder.Services.AddTransient<ManagementPage>();
        builder.Services.AddTransient<UserListPage>();
        builder.Services.AddSingleton<AppShell>();

        return builder.Build();
    }
}