using OnboardingSystem.Models.Menu;

namespace OnboardingSystem;
public class MenuInitializer {
    public List<AppShellItem> menuItems = new List<AppShellItem> {
        new AppShellItem { Icon = "group_96dp_icon.png", Title = "Staff", TableName = "staff", 
            ColumnDefinitions = new List<ColumnDefinitions> {
                new ColumnDefinitions { Name = "id", Key=true, Type = "INT", Constraint = "AUTO_INCREMENT" },
                new ColumnDefinitions { Name = "firstname", Key=false, Type = "VARCHAR(40)" },
                new ColumnDefinitions { Name = "lastname", Key=false, Type = "VARCHAR(40)" },
                new ColumnDefinitions { Name = "address", Key=false, Type = "VARCHAR(60)" },
            }
         },
        new AppShellItem { Icon = "group_96dp_icon.png", Title = "Product", TableName = "product", 
            ColumnDefinitions = new List<ColumnDefinitions> {
                new ColumnDefinitions { Name = "id", Key=true, Type = "INT", Constraint = "AUTO_INCREMENT" },
                new ColumnDefinitions { Name = "name", Key=false, Type = "VARCHAR(40)" },
                new ColumnDefinitions { Name = "description", Key=false, Type = "VARCHAR(255)" },
                new ColumnDefinitions { Name = "price", Key=false, Type = "FLOAT" },
            }
         }
    };
};
