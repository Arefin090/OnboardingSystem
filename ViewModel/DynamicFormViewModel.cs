using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnboardingSystem.ViewModel
{
    public class DynamicFormViewModel : INotifyPropertyChanged
    {
        private string _formTitle;

        public string FormTitle
        {
            get => _formTitle;
            set
            {
                if (_formTitle != value)
                {
                    _formTitle = value;
                    OnPropertyChanged(nameof(_formTitle));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
