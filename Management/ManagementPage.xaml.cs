
using OnboardingSystem.ViewModel;

namespace OnboardingSystem.Management;

public partial class ManagementPage : ContentPage
{
    public ManagementPage()
    {
        InitializeComponent();
        BindingContext = new ManagementViewModel();

        // Programmatically modify UI or add labels dynamically if needed
        AddColumnHeaders();
        
        // Programmatically modify the column definitions of the Grid
        SetColumnDefinitions(9);
    }   
        // Function to dynamically add labels
        // Method to dynamically add labels to the header grid
        private async void AddColumnHeaders()
        {
            // Create a list of headers you want to display
            var headers = new List<string> { "Select", "Staff ID", "Name", "Role", "Phone", "Address", "Branch", "Address", "Branch" };
            // Add labels dynamically to the Grid
            for (int i = 0; i < headers.Count; i++)
            {
                var label = new Label
                {
                    Text = headers[i],
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Center
                };
                // Add the label to the grid at the correct column
                HeaderGrid.Children.Add(label);
                HeaderGrid.SetColumn(label, i);
            }
            
        }
        // Method to dynamically set the column definitions
        private void SetColumnDefinitions(int numOfItems)
        {
            var columnWidths = new List<GridLength>();
            for (int i = 0; i < numOfItems; i++)
            {
                columnWidths.Add(new GridLength(1, GridUnitType.Star)); // Defines Column Size
            }

            // Clear any existing column definitions (optional)
            HeaderGrid.ColumnDefinitions.Clear();

            // Dynamically add column definitions based on the columnWidths list
            foreach (var width in columnWidths)
            {
                HeaderGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = width });
            }
        }
        private async void AddColumnRows()
        {
            
        }
}