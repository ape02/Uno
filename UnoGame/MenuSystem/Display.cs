using System.Runtime.InteropServices;
using DAL;
using static System.Console;
using Domain;

namespace MenuSystem;

public static class Display
{
    // public static void Continue()
    // {
    //     Menu menu = new Menu("Main Menu");
    //     List<object> items = new List<object> { "New Game", "Load Game", "Exit" };
    //
    //     menu.AddMenuItems(items);
    //
    //     int index = menu.Draw();
    // }
    
    public static string MainMenu()
    {
        Menu menu = new Menu("Main Menu");
        List<string> items = new List<string> { "New Game", "Load Game", "Options", "Exit" };
        IGameRepository gameRepository = new GameRepositoryFileSystem();
        if (gameRepository.GetSavedGames().Count != 0)
        {
            items.Insert(0, "Continue");
        }
        menu.AddMenuItems(items);
        int index = menu.Draw();
        return items[index];
    }
    
    public static int LoadGameMenu(List<string> savedGames)
    {
        Menu menu = new Menu("Saved Games");
        
        IGameRepository gameRepository = new GameRepositoryFileSystem();
        var savedGameList = gameRepository.GetSavedGames();
        savedGames.Add("Back");
        menu.AddMenuItems(savedGames);
        
        int index = menu.Draw();
        return index;
    }
    
    public static void Exit()
    {
        string title = "Are you sure?";
        List<object> items = new List<object>{ "Yes", "No" };

        Menu menu = new Menu(title);
        menu.AddMenuItems(items);
    
        int index = menu.Draw();

        if (index == 0)
        {
            Environment.Exit(0);
        }
    }
    
    public static int PauseMenu()
     {
         string Title = "Pause";
         List<object> items = new List<object>{ "Resume Game", "Save game", "Exit" };
         Menu menu = new Menu(Title);
         menu.AddMenuItems(items);
         int index = menu.Draw();
         return index;
     }

    public static int OptionsMenu(bool enabledOption)
    {
        string Title = "Options";
        var option = enabledOption ? "Disable Shuffle Card" : "Enable Shuffle Card";
        List<object> items = new List<object>{ option, "About Shuffle Card", "Back" };
        Menu menu = new Menu(Title);
        menu.AddMenuItems(items);
        int index = menu.Draw();
        return index;
    }
}