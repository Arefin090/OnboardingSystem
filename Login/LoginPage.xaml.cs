using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using OnboardingSystem.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace OnboardingSystem
{
    public partial class LoginPage : ContentPage, INotifyPropertyChanged
    {
        private IAuthenticationService _authService;

        private string _username = "";
        private string _password = "";
        private bool _hasError;
        private string _errorMessage = "";
        private string _loggedInUsername = "";

        public string LoggedInUsername
		{
            get => _loggedInUsername;
            private set => SetProperty(ref _loggedInUsername, value);
        }

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public bool HasError
        {
            get => _hasError;
            set => SetProperty(ref _hasError, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public LoginPage()
        {
            InitializeComponent();
            BindingContext = this;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _authService = ServiceHelper.GetService<IAuthenticationService>();
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            try
            {
                var (isValid, errorMessage) = await ValidateUserAsync(Username, Password);

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

        protected virtual async Task<(bool isValid, string errorMessage)> ValidateUserAsync(string username, string password)
        {
            return await _authService.ValidateUserAsync(username, password);
        }

        protected virtual async Task HandleSuccessfulLoginAsync()
        {
			LoggedInUsername = Username;
            await Shell.Current.GoToAsync($"//{nameof(DashboardPage)}");
        }

        private async Task ShowErrorAsync(string message)
        {
            HasError = true;
            ErrorMessage = message;
            await Task.Delay(4000);
            HasError = false;
            ErrorMessage = string.Empty;
			LoggedInUsername = string.Empty;
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}