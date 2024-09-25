namespace OnboardingSystem.Models.Menu;

public class AppShellItem
{
    public String Title { get; set; }
    public String Icon { get; set; }
    public String TableName {get;set;}
    public List<ColumnDefinitions> ColumnDefinitions { get;set;}
}