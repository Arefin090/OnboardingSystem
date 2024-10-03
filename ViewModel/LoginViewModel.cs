using System.ComponentModel;
using System.Windows.Input;
using OnboardingSystem.Authentication;
using OnboardingSystem.Global.Menu;

namespace OnboardingSystem.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly IAuthenticationService _authService;
        private string _username = "";
        private string _password = "";
        private bool _hasError;
        private string _errorMessage = "";

        public string Username
        {
            get => _username;
            set
            {
                if (_username != value)
                {
                    _username = value;
                    OnPropertyChanged(nameof(Username));
                }
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                if (_password != value)
                {
                    _password = value;
                    OnPropertyChanged(nameof(Password));
                }
            }
        }

        public bool HasError
        {
            get => _hasError;
            set
            {
                if (_hasError != value)
                {
                    _hasError = value;
                    OnPropertyChanged(nameof(HasError));
                }
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                if (_errorMessage != value)
                {
                    _errorMessage = value;
                    OnPropertyChanged(nameof(ErrorMessage));
                }
            }
        }

        public ICommand LoginCommand { get; }

       public LoginViewModel()
    {
        _authService = ServiceHelper.GetService<IAuthenticationService>();
        LoginCommand = new Command(OnLoginClicked);
    }

        private async void OnLoginClicked()
        {
            try
            {
                var (isValid, errorMessage) = await _authService.ValidateUserAsync(Username, Password);

                if (isValid)
                {
                    await HandleSuccessfulLoginAsync();
                }
                else
                {
                    await ShowErrorAsync(errorMessage);
                }
            }
            catch (Exception ex)
            {
                await ShowErrorAsync(Constants.GENERIC_ERROR);
            }
        }

        private async Task HandleSuccessfulLoginAsync()
        {
            var appShell = new AppShell();
            Application.Current.MainPage = appShell;
        
            await Shell.Current.GoToAsync($"//{nameof(DashboardPage)}");
        }

        private async Task ShowErrorAsync(string message)
        {
            HasError = true;
            ErrorMessage = message;
            await Task.Delay(4000);
            HasError = false;
            ErrorMessage = string.Empty;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}