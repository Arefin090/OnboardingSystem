using OnboardingSystem.Models.Menu;

namespace OnboardingSystem;
public static class MenuInitializer {
    public static List<AppShellItem> menuItems = new List<AppShellItem> {
        // Staff Table
        new AppShellItem { 
            Icon = "group_96dp_icon.png", 
            Title = "Staff", 
            TableName = "Staff", 
            ColumnDefinitions = new List<ColumnDefinitions> {
                new ColumnDefinitions { Name = "StaffId", Key=true, Type = "INT", Constraint = "AUTO_INCREMENT" },
                new ColumnDefinitions { Name = "Name", Key=false, Type = "VARCHAR(100)" },
                new ColumnDefinitions { Name = "Role", Key=false, Type = "VARCHAR(50)" },
                new ColumnDefinitions { Name = "PhoneNumber", Key=false, Type = "VARCHAR(20)" },
                new ColumnDefinitions { Name = "Branch", Key=false, Type = "VARCHAR(50)" },
            }
        },
        // Products Table
        new AppShellItem { 
            Icon = "group_96dp_icon.png", 
            Title = "Products", 
            TableName = "Products", 
            ColumnDefinitions = new List<ColumnDefinitions> {
                new ColumnDefinitions { Name = "ProductId", Key=true, Type = "INT", Constraint = "AUTO_INCREMENT" },
                new ColumnDefinitions { Name = "ProductName", Key=false, Type = "VARCHAR(100)" },
                new ColumnDefinitions { Name = "Details", Key=false, Type = "VARCHAR(255)" },
                new ColumnDefinitions { Name = "Gender", Key=false, Type = "VARCHAR(10)" },
                new ColumnDefinitions { Name = "Price", Key=false, Type = "DECIMAL(10,2)" },
                new ColumnDefinitions { Name = "Stock", Key=false, Type = "INT" },
            }
        },

        // Sales Table
        new AppShellItem { 
            Icon = "group_96dp_icon.png", 
            Title = "Sales", 
            TableName = "Sales", 
            ColumnDefinitions = new List<ColumnDefinitions> {
                new ColumnDefinitions { Name = "SaleId", Key=true, Type = "INT", Constraint = "AUTO_INCREMENT" },
                new ColumnDefinitions { Name = "ProductId", Key=false, Type = "INT", Constraint = "FOREIGN KEY REFERENCES Products(ProductId)" },
                new ColumnDefinitions { Name = "Qty", Key=false, Type = "INT" },
                new ColumnDefinitions { Name = "Branch", Key=false, Type = "VARCHAR(50)" },
                new ColumnDefinitions { Name = "Date", Key=false, Type = "DATE" },
            }
        }
    };
};
