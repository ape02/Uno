namespace MenuSystem;

public class MenuItem
{
    public string Shortcut { get; set; }
    
    public string Label { get; set; }
    
    public MenuItem(string shortcut, string label)
    {
        Shortcut = shortcut;
        Label = label;
    }
}