using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Timers;


namespace OnboardingSystem
{
    public partial class LoginPage : ContentPage, INotifyPropertyChanged
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
                OnPropertyChanged();
            }
        }

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        public bool HasError
        {
            get => _hasError;
            set
            {
                _hasError = value;
                OnPropertyChanged();
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged();
            }
        }

        public LoginPage()
        {
            InitializeComponent();
            BindingContext = this;
            _errorTimer = new System.Timers.Timer(4000); // 4 seconds
            _errorTimer.Elapsed += ErrorTimer_Elapsed;
        }

        private async void ToMain(object sender, EventArgs e)
        {
            try
            {
                // Validate user asynchronously
                var (isValid, errorMessage) = await UserAuthenticator.ValidateUserAsync(Username, Password);

                if (isValid)
                {
                    // Set logged-in email and navigate to the MainPage asynchronously
                    LoggedInEmail = Username;
                    await Shell.Current.GoToAsync($"//{nameof(DashboardPage)}");
                }
                else
                {
                    // Display error message if login fails
                    HasError = true;
                    ErrorMessage = errorMessage;
                    _errorTimer.Start();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "An unexpected error occurred.";
                HasError = true;
            }
        }

        private void ErrorTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.Dispatch(() =>
            {
                _errorTimer.Stop();
                HasError = false;
                ErrorMessage = string.Empty;
                LoggedInEmail = string.Empty;
            });
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _errorTimer.Dispose();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
