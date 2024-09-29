using OnboardingSystem.Models.Menu;


using System.Net;
using System.Net.Http.Json;

namespace OnboardingSystem;
public static class MenuInitializer {
    /*
     This is the dynamic array used to initialize a collection of table inside the database
     */
    public static List<AppShellItem> menu = new List<AppShellItem> {
        new AppShellItem { Icon = "table_view_96dp.png", Title = "Staff", TableName = "staff", 
            ColumnDefinitions = new List<ColumnDefinitions> {
                new ColumnDefinitions { Name = "id", Key=true, Type = "INT", Constraint = "AUTO_INCREMENT" },
                new ColumnDefinitions { Name = "firstname", Key=false, Type = "VARCHAR(40)" },
                new ColumnDefinitions { Name = "lastname", Key=false, Type = "VARCHAR(40)" },
                new ColumnDefinitions { Name = "address", Key=false, Type = "VARCHAR(60)" },
            }
         },
        new AppShellItem { Icon = "table_view_96dp.png", Title = "Product", TableName = "product", 
            ColumnDefinitions = new List<ColumnDefinitions> {
                new ColumnDefinitions { Name = "id", Key=true, Type = "INT", Constraint = "AUTO_INCREMENT" },
                new ColumnDefinitions { Name = "name", Key=false, Type = "VARCHAR(40)" },
                new ColumnDefinitions { Name = "description", Key=false, Type = "VARCHAR(255)" },
                new ColumnDefinitions { Name = "price", Key=false, Type = "FLOAT(2)" },
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

    public async static void CreateTables()
    {
        HttpClient client = new HttpClient();
        client.BaseAddress = new Uri(Constants.API_BASE_URL);
            
        var tableSchemas = new List<TableSchema>();

        foreach(var item in menu)
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

        }
    }
    public static AppShellItem? GetItemByTableName(string tableName)
    {
        var item = menu.Find(item => item.TableName == tableName);
        return item;
    }
};
