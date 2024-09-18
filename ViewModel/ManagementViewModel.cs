using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using OnboardingSystem.Models;

namespace OnboardingSystem.ViewModel;

public class ManagementViewModel : INotifyPropertyChanged
{
    public ObservableCollection<Dictionary<String, String>> StaffMembers { get; set; }
    public event PropertyChangedEventHandler? PropertyChanged;
    public ManagementViewModel()
    {
        // Populate the collection with initial data
        StaffMembers = new ObservableCollection<Dictionary<String, String>>
        {
            new Dictionary<String, String>()
            {
                {"StaffId" , "M1234"},
                { "Name", "Sara Who"}, 
                { "Role", "Customer Service"}, 
                { "Phone", "0000"},
                { "Address", "0000"},
                { "Branch", "Frankston"},
            },
            // new Staff { StaffId = "M2345", Name = "Max What", Role = "Inventory Manager", Phone = "0000", Address = "...", Branch = "Frankston" },
            // new Staff { StaffId = "M1234", Name = "Sara Who", Role = "Customer Service", Phone = "0000", Address = "...", Branch = "Dandenong" },
            // new Staff { StaffId = "M2345", Name = "Max What", Role = "Inventory Manager", Phone = "0000", Address = "...", Branch = "Frankston" },
            // new Staff { StaffId = "M1234", Name = "Sara Who", Role = "Customer Service", Phone = "0000", Address = "...", Branch = "Dandenong" },
            // new Staff { StaffId = "M2345", Name = "Max What", Role = "Inventory Manager", Phone = "0000", Address = "...", Branch = "Frankston" },
            // new Staff { StaffId = "M1234", Name = "Sara Who", Role = "Customer Service", Phone = "0000", Address = "...", Branch = "Dandenong" },
            // new Staff { StaffId = "M2345", Name = "Max What", Role = "Inventory Manager", Phone = "0000", Address = "...", Branch = "Frankston" },
            // new Staff { StaffId = "M1234", Name = "Sara Who", Role = "Customer Service", Phone = "0000", Address = "...", Branch = "Dandenong" },
            // new Staff { StaffId = "M2345", Name = "Max What", Role = "Inventory Manager", Phone = "0000", Address = "...", Branch = "Frankston" },
            // new Staff { StaffId = "M1234", Name = "Sara Who", Role = "Customer Service", Phone = "0000", Address = "...", Branch = "Dandenong" },
            // new Staff { StaffId = "M2345", Name = "Max What", Role = "Inventory Manager", Phone = "0000", Address = "...", Branch = "Frankston" },
            // new Staff { StaffId = "M1234", Name = "Sara Who", Role = "Customer Service", Phone = "0000", Address = "...", Branch = "Dandenong" },
            // new Staff { StaffId = "M2345", Name = "Max What", Role = "Inventory Manager", Phone = "0000", Address = "...", Branch = "Frankston" }, 
            // new Staff { StaffId = "M1234", Name = "Sara Who", Role = "Customer Service", Phone = "0000", Address = "...", Branch = "Dandenong" },
            // new Staff { StaffId = "M2345", Name = "Max What", Role = "Inventory Manager", Phone = "0000", Address = "...", Branch = "Frankston" },
            // new Staff { StaffId = "M1234", Name = "Sara Who", Role = "Customer Service", Phone = "0000", Address = "...", Branch = "Dandenong" },
            // new Staff { StaffId = "M2345", Name = "Max What", Role = "Inventory Manager", Phone = "0000", Address = "...", Branch = "Frankston" },
            // new Staff { StaffId = "M1234", Name = "Sara Who", Role = "Customer Service", Phone = "0000", Address = "...", Branch = "Dandenong" },
            // new Staff { StaffId = "M2345", Name = "Max What", Role = "Inventory Manager", Phone = "0000", Address = "...", Branch = "Frankston" },
            // new Staff { StaffId = "M1234", Name = "Sara Who", Role = "Customer Service", Phone = "0000", Address = "...", Branch = "Dandenong" },
            // new Staff { StaffId = "M2345", Name = "Max What", Role = "Inventory Manager", Phone = "0000", Address = "...", Branch = "Frankston" },
            // new Staff { StaffId = "M1234", Name = "Sara Who", Role = "Customer Service", Phone = "0000", Address = "...", Branch = "Dandenong" },
            // new Staff { StaffId = "M2345", Name = "Max What", Role = "Inventory Manager", Phone = "0000", Address = "...", Branch = "Frankston" },
            // new Staff { StaffId = "M1234", Name = "Sara Who", Role = "Customer Service", Phone = "0000", Address = "...", Branch = "Dandenong" },
            // new Staff { StaffId = "M2345", Name = "Max What", Role = "Inventory Manager", Phone = "0000", Address = "...", Branch = "Frankston" },
            // new Staff { StaffId = "M1234", Name = "Sara Who", Role = "Customer Service", Phone = "0000", Address = "...", Branch = "Dandenong" },
            // new Staff { StaffId = "M2345", Name = "Max What", Role = "Inventory Manager", Phone = "0000", Address = "...", Branch = "Frankston" },
            // Add more members as needed
        };
    }



    // protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    // {
    //     PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    // }
}