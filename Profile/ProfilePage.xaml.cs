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

	public ProfilePage()
	{
		InitializeComponent();
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
}