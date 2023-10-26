
namespace MenuSystem;

public class Menu
{
    private const string Separator = "========================";
    private const string InsertPlayers = "Insert amount of players in the game (2-7):";
    private const string NicknameForPlayer = "Nickname for Player ";
    private string Title { get; set; } = "Main menu";
    private string? MenuContent;
    private List<MenuItem> _menuItems = new() { new MenuItem("n", "New Game"), 
        new MenuItem("l", "Load Game"), new MenuItem("e", "Exit game") };
    
    private bool _exitGame;
    
    
    private int? PlayerCount = null!;
    private int playerIndex = 1;

    private List<string> _players = null!;
    


    public void DrawMenu()
    {
        while (!_exitGame)
        {
            Console.Clear();
            Console.WriteLine(Title);
            Console.WriteLine(Separator);
            if (MenuContent != null)
            {
                if (MenuContent.Contains(NicknameForPlayer))
                {
                    Console.WriteLine(MenuContent + (playerIndex));
                }
                else
                {
                    Console.WriteLine(MenuContent);
                }
            }
            foreach (var menuItem in _menuItems)
            {
                Console.WriteLine(menuItem.Shortcut + ' ' + menuItem.Label);   
            }
            Console.WriteLine(Separator);
            Console.Write("Your choice:");
            ProcessUserInput(Console.ReadLine());
            
        }
    }

    private void ProcessUserInput(string? userInput)
    {
        if (_menuItems.Any(item => item.Shortcut == userInput!.Trim()))
        {
            switch (userInput)
            {
                case "n":
                    ShowNewGameMenu();
                    break;
                case "l" :
                    ShowLoadGameMenu();
                    break;
                case "b" :
                    goBack();
                    break;
                case "e":
                    Console.Clear();
                    Console.Write("Bye!");
                    _exitGame = true;
                    break;
                default:
                    ShowWarning();
                    DrawMenu();
                    break;
            }   
        }
        else
        {
            ProcessNewGame(userInput);
        }
    }

    private void ProcessNewGame(string? userInput)
    {
        if (MenuContent != null && PlayerCount == null)
        {
            if (int.TryParse(userInput, out int parsedPlayerCount))
            {
                PlayerCount = parsedPlayerCount;
                _players = new List<string>(parsedPlayerCount);
            }
            // Game.PlayerCount = playerCount;
            ShowNewGameMenu();
        }
        
        if (PlayerCount != null && PlayerCount != playerIndex)
        {
            _players.Add("");
            _players[playerIndex - 1] = userInput!;
            playerIndex++;
            DrawMenu();
        }

        if (PlayerCount != playerIndex) return;
        Console.Clear();
        Console.WriteLine("OK! YOU GOT IT! REALIZE BACK FUNCTION AND GO CREATE THE GAME!");
        Console.ReadLine();
    }

    private void goBack()
    {
        switch (Title)
        {
            case "New Game":
                if (MenuContent == "Insert amount of players in the game (2-7):")
                {
                    MenuContent = null;
                    PlayerCount = 0;
                    ShowMainMenu();
                }
                if (MenuContent!.Contains("Nickname for player"))
                {
                    playerIndex = 1;
                    _players.Clear();
                    PlayerCount = null;
                    MenuContent = null;
                    // MenuContent = "Insert amount of players in the game (2-7):";
                    ShowNewGameMenu();
                }
                break;
            case "Load game":
                // Not implemented
                break;
        }
    }

    private static void ShowWarning()
    {
        Console.Clear();
        Console.Write("Didn't work. Try again!");
        Thread.Sleep(2000);
    }


    private void ShowMainMenu()
    {
        Title = "Main Menu";
        _menuItems = new List<MenuItem>()
        {
            new MenuItem("n", "New Game"),
            new MenuItem("l", "Load Game"), new MenuItem("e", "Exit game")
        };
        DrawMenu();
    }

    private void ShowNewGameMenu()
    {
        Title = "New Game";
        _menuItems = new List<MenuItem>()
            { new MenuItem("b", "Back") };
        if (MenuContent == null)
        {
            MenuContent = "Insert amount of players in the game (2-7):";
        }
        else if (MenuContent != null)
        {
            MenuContent = "Nickname for player - ";
        }
        DrawMenu();
    }

    private void ShowLoadGameMenu()
    {
        Title = "Load Game";
        // Here appear the saved games. You should read them from csv file as menu items;
    }

}
