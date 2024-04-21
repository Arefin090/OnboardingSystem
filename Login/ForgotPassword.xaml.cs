namespace OnboardingSystem;

public partial class ForgotPassword : ContentPage
{
	public ForgotPassword()
	{
		InitializeComponent();
	}


	private void SendEmail(object sender, EventArgs e)
    {
		var change_text = user_email.Text;
		testing.Text = $"{change_text}.";

		// if (Email.Default.IsComposeSupported)
		// {
		// 	string subject = "New Password";
		// 	string body = "Here is your temporary password: (???)";
		// 	string[] recipients = new[] { $"{change_text}" };

		// 	var message = new EmailMessage
		// 	{
		// 		Subject = subject,
		// 		Body = body,
		// 		BodyFormat = EmailBodyFormat.PlainText,
		// 		To = new List<string>(recipients)
		// 	};

		// 	Email.Default.ComposeAsync(message);
		// }

		//Navigation.PopAsync();
    }
}