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
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		builder.Services.AddSingleton<IAuthenticationService, AuthenticationService>();
        builder.Services.AddSingleton<HttpClient>();
		builder.Services.AddSingleton<IUserService, UserService>();
		builder.Services.AddSingleton<HttpClient>();
		builder.Services.AddTransient<UserListPage>(sp => 
    new UserListPage(
        sp.GetRequiredService<IAuthenticationService>(),
        sp.GetRequiredService<IUserService>()
    )
);
        MenuInitializer.CreateTables();
		Console.WriteLine("ASdasd");
        // Register your pages
        builder.Services.AddTransient<LoginPage>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
