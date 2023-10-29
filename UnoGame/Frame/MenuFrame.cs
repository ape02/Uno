using static System.Console;

namespace Frame;

public class MenuFrame : IFrame
{
    private string Title;
    private string Separator = "===================";
    private Object[] Items;
    private int SelectedIndex;


    public MenuFrame(string title, Object[] items)
    {
        Title = title;
        Items = items;
    }

    public int Run()
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
                SelectedIndex = (SelectedIndex == 0 ? Items.Length - 1 : SelectedIndex - 1);
            } else if (pressedKey == ConsoleKey.DownArrow)
            {
                SelectedIndex = (SelectedIndex == Items.Length - 1 ? 0 : SelectedIndex + 1);
            }
        } while (pressedKey != ConsoleKey.Enter);

        return SelectedIndex;
    }

    private void DisplayItems()
    {
        WriteLine(Title);
        WriteLine(Separator);
        for (int i = 0; i < Items.Length; i++)
        {
            object currentItem = Items[i];
            string prefix;
            if (SelectedIndex == i)
            {
                prefix = "->";
                currentItem = $"{prefix} {currentItem}";
            }
            else
            {
                prefix = "  ";
                currentItem = $"{prefix} {currentItem}";
            }
            WriteLine(currentItem);
        }
        WriteLine(Separator);
    }
}
