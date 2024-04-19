namespace OnboardingSystem;

public partial class LoginPage : ContentPage
{   private List<string> loggedInUsernames = new List<string>(); 
    private string _username;
    private string _password;
    private bool _hasError;
    private string _errorMessage;

    public string Username
    {
        get => _username;
        set
        {
            _username = value;
            OnPropertyChanged(nameof(Username));
        }
    }

    public string Password
    {
        get => _password;
        set
        {
            _password = value;
            OnPropertyChanged(nameof(Password));
        }
    }

    public bool HasError
    {
        get => _hasError;
        set
        {
            _hasError = value;
            OnPropertyChanged(nameof(HasError));
        }
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set
        {
            _errorMessage = value;
            OnPropertyChanged(nameof(ErrorMessage));
        }
    }

    public LoginPage()
    {
        InitializeComponent();
        BindingContext = this;
    }

    private async void ToMain(object sender, EventArgs e)
    {
      
			 loggedInUsernames.Add(Username);
            // Authentication successful
            await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
       
    }

}