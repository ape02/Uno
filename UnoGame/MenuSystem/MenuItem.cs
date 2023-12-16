namespace MenuSystem;

public class MenuItem
{
    public const string Separator = "===================";
    private object Name { get; set; }

    public MenuItem(object name)
    {
        Name = name;
    }

    public override string ToString()
    {
        return Name.ToString()!;
    }
}