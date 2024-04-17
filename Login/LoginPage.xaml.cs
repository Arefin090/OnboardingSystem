namespace OnboardingSystem;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
	}

	private async void ToMain(object sender, EventArgs e) //on click button
	{
		await Shell.Current.GoToAsync($"//{nameof(MainPage)}"); //redirect to main page
	}
}