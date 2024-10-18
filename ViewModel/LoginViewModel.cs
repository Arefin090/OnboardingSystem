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
        private bool _isLoading;

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

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    OnPropertyChanged(nameof(IsLoading));
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
            if (IsLoading) return; // Prevent multiple login attempts

            try
            {
                IsLoading = true;
                var (isValid, errorMessage) = await _authService.ValidateUserAsync(Username, Password);

                // Hide loading indicator immediately after receiving response
                IsLoading = false;

                if (isValid)
                {
                    await HandleSuccessfulLoginAsync();
                }
                else
                {
                    ShowError(errorMessage);
                }
            }
            catch (Exception ex)
            {
                IsLoading = false;
                ShowError(Constants.GENERIC_ERROR);
            }
        }

        private async Task HandleSuccessfulLoginAsync()
        {
            var appShell = new AppShell();
            Application.Current.MainPage = appShell;
        
            await Shell.Current.GoToAsync($"//{nameof(DashboardPage)}");
        }

        private void ShowError(string message)
        {
            HasError = true;
            ErrorMessage = message;
            Device.StartTimer(TimeSpan.FromSeconds(4), () =>
            {
                HasError = false;
                ErrorMessage = string.Empty;
                return false; // Don't repeat the timer
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}