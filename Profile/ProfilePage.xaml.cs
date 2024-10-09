using System;
using System.ComponentModel;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using OnboardingSystem.Authentication;
using OnboardingSystem.Management.Components;
using OnboardingSystem.ViewModel;

namespace OnboardingSystem;

public partial class ProfilePage : ContentPage
{
	 private string _id = "";
	private string _username = "";
	private string _password = "";
	private string _firstName = "";
	private string _lastName = "";
	private string _role = "";
	private string _phone = "";

	private bool isEditModeEnabled = false;  // This will track if the fields are editable

	private HttpClient _httpClient;
	private readonly IAuthenticationService _authenticationService;
	private ProfileViewModel _viewModel = new ProfileViewModel();

	public ProfilePage(IAuthenticationService authenticationService)
	{
		InitializeComponent();
		// Ensure all fields are non-editable when the page loads
        SetFieldsEditable(false);
		SaveButton.IsVisible = false;
		_httpClient = new HttpClient(); // HttpClient instance
		_authenticationService = authenticationService;
		BindingContext = _viewModel;
		
        //LoadUserProfile();  // Call to load the profile data on page load
	}

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        await LoadUserProfile();
    }

	// Properties to bind user data
	public string Username
	{
		get => _username;
		set
		{
			_username = value;
			OnPropertyChanged(nameof(Username)); // Notifying UI when value changes
		}
	}

	public string FirstName
	{
		get => _firstName;
		set
		{
			_firstName = value;
			OnPropertyChanged(nameof(FirstName));
		}
	}

	public string LastName
	{
		get => _lastName;
		set
		{
			_lastName = value;
			OnPropertyChanged(nameof(LastName));
		}
	}

	public string Phone
	{
		get => _phone;
		set
		{
			_phone = value;
			OnPropertyChanged(nameof(Phone));
		}
	}

	// Method to decode the JWT and extract the username
	private string? GetUsernameFromToken(string token)
	{
		var handler = new JwtSecurityTokenHandler();
		var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

		// Extract the "unique_name" field (which holds the username)
		return jsonToken?.Claims.First(claim => claim.Type == "unique_name")?.Value;
	}

	 // Method to load user profile
	private async Task LoadUserProfile()
	{
		try
		{
			_viewModel.IsLoading = true; // Show loading overlay

			// Retrieve the token securely
			var token = await _authenticationService.GetValidTokenAsync();

			// Extract username from the token
			var username = GetUsernameFromToken(token);

			// Set the Authorization header with the token
			_httpClient.DefaultRequestHeaders.Authorization =
				new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

			// Make the GET request to fetch user profile
			var response = await _httpClient.GetAsync($"{Constants.API_BASE_URL}{Constants.GET_USERS_ENDPOINT}/{username}");
			
			if (response.IsSuccessStatusCode)
			{
				// Parse the response
				var jsonResponse = await response.Content.ReadAsStringAsync();
				var user = JsonConvert.DeserializeObject<User>(jsonResponse);

				// Bind the user data to the UI fields
				 _id = user.Id.ToString();
				UsernameEntry.Text = user.Username;
				FirstNameEntry.Text = user.FirstName;
				LastNameEntry.Text = user.LastName;
				PhoneEntry.Text = user.Phone;
				 RoleEntry.Text = user.Role; 

				//password fetching is unnecessary thus didn't
				//PasswordEntry.Text = user.Password; 
			}
			else
			{
				// Handle the case where the request failed (e.g., display an error)
				await DisplayAlert("Error", "Failed to load profile data", "OK");
			}
		}
		catch (Exception ex)
		{
			await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
		}
		finally 
		{
			_viewModel.IsLoading = false; // Hide loading overlay regardless of success or failure
		}
	}

		// Class to deserialize user data (this should match your User model from the backend)
        public class User
        {
			 public int Id { get; set; }
			 public string? Username { get; set; }
            public string? FirstName { get; set; }
            public string? LastName { get; set; }
            public string? Phone { get; set; }
            public string? Password { get; set; }
            public string? Role { get; set; }
        }

	// Event handler for the edit icon button click
    private void OnEditClicked(object sender, EventArgs e)
    {
        // Toggle the edit mode
        isEditModeEnabled = !isEditModeEnabled;

        // Set the Entry fields to be editable or not based on isEditModeEnabled
        SetFieldsEditable(isEditModeEnabled);

        // Show the Save button if edit mode is enabled, hide it otherwise
        SaveButton.IsVisible = isEditModeEnabled;
    }

	private void OnPressed(object sender, EventArgs e)
		{
			EditButton.ScaleTo(1.2, 100); // Scale the button up to 1.2 times its original size
		}

		// Event handler for when the button is released (simulating hover exit)
		private void OnReleased(object sender, EventArgs e)
		{
			EditButton.ScaleTo(1.0, 100); // Scale the button back to its original size
		}
	// Event handler for Save button click (optional)
   // Event handler for save button click
        private async void OnSaveClicked(object sender, EventArgs e)
        {
            try
            {
                var token = await _authenticationService.GetValidTokenAsync();
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var updateUser = new
                {	
					Id = int.Parse(_id),
                    Username = UsernameEntry.Text,
                    FirstName = FirstNameEntry.Text,
                    LastName = LastNameEntry.Text,
                    Phone = PhoneEntry.Text,
					Role = RoleEntry.Text
                };

                var jsonContent = JsonConvert.SerializeObject(updateUser);
				Console.WriteLine($"Payload: {jsonContent}"); // Log the payload
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

                // Send PUT request to update user data
                var response = await _httpClient.PutAsync($"{Constants.API_BASE_URL}{Constants.UPDATE_USER_ENDPOINT}", content);

                if (response.IsSuccessStatusCode)
                {
                    await DisplayAlert("Success", "Profile updated successfully", "OK");
                    SetFieldsEditable(false);
                    SaveButton.IsVisible = false;
                }
                else
                {
					 var errorContent = await response.Content.ReadAsStringAsync();
    				Console.WriteLine($"Error: {errorContent}");
                    await DisplayAlert("Error", "Failed to save profile changes", "OK");
					
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }


	// Helper method to find all Entry fields
    private IEnumerable<Entry> FindEntryFields()
    {
        return new List<Entry> { UsernameEntry, FirstNameEntry, LastNameEntry, PhoneEntry };
    }
	 // Method to set all Entry fields as editable or non-editable
    private void SetFieldsEditable(bool isEnabled)
    {
		
        UsernameEntry.IsEnabled = false;
        FirstNameEntry.IsEnabled = isEnabled;
        LastNameEntry.IsEnabled = isEnabled;
        PhoneEntry.IsEnabled = isEnabled;
		RoleEntry.IsEnabled = false;
    }

}