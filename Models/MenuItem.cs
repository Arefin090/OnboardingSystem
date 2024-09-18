namespace OnboardingSystem.Models;

public class AppShellItem : ShellContent
{
    public String Title { get; set; }
    public String Name { get; set; }
    public String Icon { get; set; }
    public MenuItem[] subItems { get; set; }
}