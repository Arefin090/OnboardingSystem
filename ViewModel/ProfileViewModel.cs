using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace OnboardingSystem.ViewModel
{
    public class ProfileViewModel : INotifyPropertyChanged
    {
        // private string? _name;
        // private string? _position;
        // private string? _email;
        // private string? _password;
        // private DateTime? _dateOfBirth;
        // private string? _country;
        private bool _isLoading = false;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged(nameof(IsLoading)); // Notify the UI to update the loading indicator
            }
        }

        // public string? Name
        // {
        //     get => _name;
        //     set
        //     {
        //         if (_name != value)
        //         {
        //             _name = value ?? string.Empty;  // Ensuring it's never null
        //             OnPropertyChanged();
        //         }
        //     }
        // }

        // public string? Position
        // {
        //     get => _position;
        //     set
        //     {
        //         if (_position != value)
        //         {
        //             _position = value ?? string.Empty;
        //             OnPropertyChanged();
        //         }
        //     }
        // }

        // public string? Email
        // {
        //     get => _email;
        //     set
        //     {
        //         if (_email != value)
        //         {
        //             _email = value ?? string.Empty;
        //             OnPropertyChanged();
        //         }
        //     }
        // }

        // public string? Password
        // {
        //     get => _password;
        //     set
        //     {
        //         if (_password != value)
        //         {
        //             _password = value ?? string.Empty;
        //             OnPropertyChanged();
        //         }
        //     }
        // }

        // public DateTime? DateOfBirth
        // {
        //     get => _dateOfBirth;
        //     set
        //     {
        //         if (_dateOfBirth != value)
        //         {
        //             _dateOfBirth = value ?? DateTime.Today;
        //             OnPropertyChanged();
        //         }
        //     }
        // }

        // public string? Country
        // {
        //     get => _country;
        //     set
        //     {
        //         if (_country != value)
        //         {
        //             _country = value ?? string.Empty;
        //             OnPropertyChanged();
        //         }
        //     }
        // }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
