namespace OnboardingSystem;
public partial class LoginPage : ContentPage
{
    private string _username = "";
    private string _password = "";
    private bool _hasError;
    private string _errorMessage = "";
    private System.Timers.Timer _errorTimer;
    private string _loggedInEmail = "";

    public string LoggedInEmail
    {
        get => _loggedInEmail;
        private set
        {
            _loggedInEmail = value;
            OnPropertyChanged(nameof(LoggedInEmail));
        }
    }

    public string Username
    {
        get => _username;
        set
        {
            _username = value;
        }
    }

    public string Password
    {
        get => _password;
        set
        {
            _password = value;
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
        _errorTimer = new System.Timers.Timer(4000); // 4 seconds
        _errorTimer.Elapsed += ErrorTimer_Elapsed;
    }

    private void ToMain(object sender, EventArgs e)
    {
        var (isValid, errorMessage) = UserAuthenticator.ValidateUser(Username, Password);
    if (isValid) // Authentication successful
        {
            LoggedInEmail = Username;
            Shell.Current.GoToAsync($"//{nameof(MainPage)}");
        }
        else
        {
            HasError = true;
            ErrorMessage = errorMessage;
            _errorTimer.Start();
        }
    }

    private void OnTapGestureRecognizerTapped(object sender, TappedEventArgs args)
    {
        Shell.Current.GoToAsync($"//{nameof(ForgotPassword)}");
        //Navigation.PushAsync(new ForgotPassword());
    }

    private void ErrorTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
    {
        _errorTimer.Stop();
        HasError = false;
        ErrorMessage = string.Empty;
        LoggedInEmail = string.Empty;
    }
}
