using static System.Console;

namespace MenuSystem;

public class Menu
{
    private string Title { get; set; }
    private string SubTitle { get; set; } = default!;
    private List<MenuItem> MenuItems = new();
    
    private int SelectedIndex;

    public Menu(string menuTitle)
    {
        Title = menuTitle;
    }

    public void AddMenuItems<T>(List<T> items)
    {
        foreach (var menuItem in items.Select(item => new MenuItem(item)))
        {
            MenuItems.Add(menuItem);
        }
    }

    public int Draw()
    {
        ConsoleKey pressedKey;

        do
        {
            Clear();
            DisplayItems();
            
            ConsoleKeyInfo keyInfo = ReadKey(true);
            pressedKey = keyInfo.Key;
            
            if (pressedKey == ConsoleKey.UpArrow)
            {
                SelectedIndex = (SelectedIndex == 0 ? MenuItems.Count - 1 : SelectedIndex - 1);
            } else if (pressedKey == ConsoleKey.DownArrow)
            {
                SelectedIndex = (SelectedIndex == MenuItems.Count - 1 ? 0 : SelectedIndex + 1);
            }
        } while (pressedKey != ConsoleKey.Enter);

        return SelectedIndex;
    }
    
    private void DisplayItems()
    {
        if (!string.IsNullOrEmpty(Title))
        {
            WriteLine(Title);
            WriteLine(MenuItem.Separator);
        }
        for (int i = 0; i < MenuItems.Count; i++)
        {
            object currentItem = MenuItems[i];
            string prefix = (SelectedIndex == i) ? "->" : "  ";
            currentItem = $"{prefix} {currentItem}";
            WriteLine(currentItem);
        }
        WriteLine(MenuItem.Separator);
    }
}