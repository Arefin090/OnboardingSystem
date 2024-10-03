using OnboardingSystem.Models.Menu;


using System.Net;
using System.Net.Http.Json;

namespace OnboardingSystem;
public class MenuInitializer {
    public static List<AppShellItem> menuItems = new List<AppShellItem> {
        // Staff Table
        new AppShellItem { 
            Icon = "table_view_96dp.png", 
            Title = "Staff", 
            TableName = "Staff", 
            ColumnDefinitions = new List<ColumnDefinitions> {
                new ColumnDefinitions { Name = "StaffId", Key=true, Type = "INT AUTO_INCREMENT", Constraint = "AUTO_INCREMENT" },
                new ColumnDefinitions { Name = "Name", Key=false, Type = "VARCHAR(100)" },
                new ColumnDefinitions { Name = "Role", Key=false, Type = "VARCHAR(50)" },
                new ColumnDefinitions { Name = "PhoneNumber", Key=false, Type = "VARCHAR(20)" },
                new ColumnDefinitions { Name = "Branch", Key=false, Type = "VARCHAR(50)" },
            }
        },
        // Products Table
        new AppShellItem { 
            Icon = "table_view_96dp.png", 
            Title = "Products", 
            TableName = "Products", 
            ColumnDefinitions = new List<ColumnDefinitions> {
                new ColumnDefinitions { Name = "ProductId", Key=true, Type = "INT AUTO_INCREMENT", Constraint = "AUTO_INCREMENT" },
                new ColumnDefinitions { Name = "ProductName", Key=false, Type = "VARCHAR(100)" },
                new ColumnDefinitions { Name = "Details", Key=false, Type = "VARCHAR(255)" },
                new ColumnDefinitions { Name = "Gender", Key=false, Type = "VARCHAR(10)" },
                new ColumnDefinitions { Name = "Price", Key=false, Type = "DECIMAL(10,2)" },
                new ColumnDefinitions { Name = "Stock", Key=false, Type = "INT" },
            }
        },

        // Sales Table
        new AppShellItem { 
            Icon = "table_view_96dp.png", 
            Title = "Sales", 
            TableName = "Sales", 
            ColumnDefinitions = new List<ColumnDefinitions> {
                new ColumnDefinitions { Name = "SaleId", Key=true, Type = "INT AUTO_INCREMENT", Constraint = "AUTO_INCREMENT" },
                new ColumnDefinitions { Name = "ProductId", Key=false, Type = "INT", Constraint = "FOREIGN KEY REFERENCES Products(ProductId)" },
                new ColumnDefinitions { Name = "Qty", Key=false, Type = "INT" },
                new ColumnDefinitions { Name = "Branch", Key=false, Type = "VARCHAR(50)" },
                new ColumnDefinitions { Name = "Date", Key=false, Type = "DATE" },
            }
        }
    };

    private record CreateTableRequest(
        List<TableSchema> menu
    );
    private record TableSchema(
        String TableName,
        List<ColumnDefinitions> ColumnDefinitions
    );

    public static async void CreateTables()
    {
        HttpClient client = new HttpClient();
        client.BaseAddress = new Uri(Constants.API_BASE_URL);
            
        var tableSchemas = new List<TableSchema>();
        foreach(var item in menuItems)
        {
            var tableSchema = new TableSchema(item.TableName, item.ColumnDefinitions);
            tableSchemas.Add(tableSchema);
        }

        var requestData = new CreateTableRequest(tableSchemas);

        var response = await client.PostAsJsonAsync("/api/Management/create-tables", requestData);
        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("Tables created successfully!");
        }
        else
        {
            response.EnsureSuccessStatusCode();
            var errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Error: {response.StatusCode}, Content: {errorContent}");
            throw new Exception($"Error: {response.StatusCode}, Content: {errorContent}");
        }
    }
    public static AppShellItem? GetItemByTableName(string tableName)
    {
        var item = menuItems.Find(item => item.TableName == tableName);
        return item;
    }
};
