using static System.Console;

namespace GameEngine;

public class Menu
{
    private string Title;
    private string Separator;
    private string[] Items;
    private int SelectedIndex;


    public Menu(string title, string[] items)
    {
        Title = title;
        Items = items;
        Separator = "===================";
        SelectedIndex = 0;
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
            string currentItem = Items[i];
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
        ResetColor();
        WriteLine(Separator);
    }



    // private const string Separator = "========================";
    // private const string InsertPlayers = "Insert amount of players in the game (2-7):";
    // private const string NicknameForPlayer = "Nickname for Player ";
    // private string Title { get; set; } = "Main menu";
    // private string? _menuContent;
    // private List<MenuItem> _menuItems = new() { new MenuItem("n", "New Game"), 
    //     new MenuItem("l", "Load Game"), new MenuItem("e", "Exit game") };
    //
    // private bool _exitGame;
    //
    //
    //
    // public void DrawMenu()
    // {
    //     while (!_exitGame)
    //     {
    //         Console.Clear();
    //         Console.WriteLine(Title);
    //         Console.WriteLine(Separator);
    //         if (_menuContent != null)
    //         {
    //             Console.WriteLine(_menuContent);
    //         }
    //         foreach (var menuItem in _menuItems)
    //         {
    //             Console.WriteLine(menuItem.Shortcut + ' ' + menuItem.Label);   
    //         }
    //         Console.WriteLine(Separator);
    //         Console.Write("Your choice:");
    //         ProcessUserInput(Console.ReadLine());
    //         
    //     }
    // }
    //
    // private void ProcessUserInput(string? userInput)
    // {
    //     if (_menuItems.Any(item => item.Shortcut == userInput!.Trim()))
    //     {
    //         switch (userInput)
    //         {
    //             case "n":
    //                 ShowNewGameMenu();
    //                 break;
    //             case "l" :
    //                 ShowLoadGameMenu();
    //                 break;
    //             case "b" :
    //                 ShowPreviousMenu();
    //                 break;
    //             case "e":
    //                 Console.Clear();
    //                 Console.Write("Bye!");
    //                 _exitGame = true;
    //                 break;
    //             default:
    //                 ShowWarning();
    //                 DrawMenu();
    //                 break;
    //         }   
    //     }
    //     else
    //     {
    //         ProcessInGameMenu(userInput);
    //     }
    // }
    //
    //
    // private void ShowPreviousMenu()
    // {
    //     switch (Title)
    //     {
    //         case "New Game":
    //             if (_menuContent == "Insert amount of players in the game (2-7):")
    //             {
    //                 _menuContent = null;
    //                 PlayerCount = 0;
    //                 ShowMainMenu();
    //             }
    //             if (_menuContent!.Contains("Nickname for player"))
    //             {
    //                 playerIndex = 1;
    //                 _players.Clear();
    //                 PlayerCount = null;
    //                 _menuContent = null;
    //                 // MenuContent = "Insert amount of players in the game (2-7):";
    //                 ShowNewGameMenu();
    //             }
    //             break;
    //         case "Load game":
    //             // Not implemented
    //             break;
    //     }
    // }
    //
    // private static void ShowWarning()
    // {
    //     Console.Clear();
    //     Console.Write("Didn't work. Try again!");
    //     Thread.Sleep(2000);
    // }
    //
    //
    // private void ShowMainMenu()
    // {
    //     Title = "Main Menu";
    //     _menuItems = new List<MenuItem>()
    //     {
    //         new MenuItem("n", "New Game"),
    //         new MenuItem("l", "Load Game"), new MenuItem("e", "Exit game")
    //     };
    //     DrawMenu();
    // }
    //
    // private void ShowNewGameMenu()
    // {
    //     Title = "New Game";
    //     _menuItems = new List<MenuItem>()
    //         { new MenuItem("b", "Back") };
    //     if (_menuContent == null)
    //     {
    //         _menuContent = "Insert amount of players in the game (2-7):";
    //     }
    //     else if (_menuContent != null)
    //     {
    //         _menuContent = "Nickname for player - ";
    //     }
    //     DrawMenu();
    // }
    //
    // private void ShowLoadGameMenu()
    // {
    //     Title = "Load Game";
    //     // Here appear the saved games. You should read them from csv file as menu items;
    // }

}
