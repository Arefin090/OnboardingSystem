using System;
using System.ComponentModel;

namespace OnboardingSystem;

public partial class ProfilePage : ContentPage, INotifyPropertyChanged
{
	private string _username = "";
	private string _password = "";
	private string _firstName = "";
	private string _lastName = "";
	private string _role = "";
	private string _phone = "";

	private bool isEditModeEnabled = false;  // This will track if the fields are editable

	public ProfilePage()
	{
		InitializeComponent();
		// Ensure all fields are non-editable when the page loads
        SetFieldsEditable(false);
		 SaveButton.IsVisible = false;
	}

	public string Username
	{
		get => _username;
		set => _username = value;
	}

	public string Password
	{
		get => _password;
		set => _password = value;
	}

	public string FirstName
	{
		get => _firstName;
		set => _firstName = value;
	}

	public string LastName
	{
		get => _lastName;
		set => _lastName = value;
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

	// Event handler for Save button click (optional)
    private void OnSaveClicked(object sender, EventArgs e)
    {
        // Hide the Save button and set fields back to non-editable after saving
        isEditModeEnabled = false;
        SetFieldsEditable(false);
        SaveButton.IsVisible = false;  // Hide the Save button after saving
    }

	// Helper method to find all Entry fields
    private IEnumerable<Entry> FindEntryFields()
    {
        return new List<Entry> { UsernameEntry, PasswordEntry, FirstNameEntry, LastNameEntry, PhoneEntry };
    }
	 // Method to set all Entry fields as editable or non-editable
    private void SetFieldsEditable(bool isEnabled)
    {
        UsernameEntry.IsEnabled = isEnabled;
        PasswordEntry.IsEnabled = isEnabled;
        FirstNameEntry.IsEnabled = isEnabled;
        LastNameEntry.IsEnabled = isEnabled;
        PhoneEntry.IsEnabled = isEnabled;
    }

}