using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls;

namespace OnboardingSystem
{
	public partial class AddUserPopup : Popup
	{
		public AddUserPopup()
		{
			InitializeComponent();
		}

		private void OnSubmitClicked(object sender, EventArgs e)
		{
			// Collect form data
			string username = UsernameEntry.Text;
			string firstName = FirstNameEntry.Text;
			string lastName = LastNameEntry.Text;
			string phone = PhoneEntry.Text;
			string role = RolePicker.SelectedItem?.ToString();
			string password = PasswordEntry.Text;

			// Validate the form
			if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(firstName) ||
				string.IsNullOrWhiteSpace(lastName) || string.IsNullOrWhiteSpace(phone) ||
				string.IsNullOrWhiteSpace(role) || string.IsNullOrWhiteSpace(password))
			{
				Application.Current.MainPage.DisplayAlert("Error", "All fields are required.", "OK");
				return;
			}

			if (!int.TryParse(phone, out int value)) {
				Application.Current.MainPage.DisplayAlert("Error", "The Phone input field must be numerical.", "OK");
				return;
			}

			// Return the collected data as a result
			Close(new { username, firstName, lastName, phone, role, password });
		}
	}
}