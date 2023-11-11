using System.Text.Json;
using static System.Console;
using PlayerSystem;
using GameEngine;
using Frame;

namespace MenuSystem;

public class Menu
{

    private int _playerAmount;
    private List<Player> _players { get; } = new List<Player>();
    private Game game = default!;
    
    
    
    public void Run()
    {
        DisplayMainMenu(); 
    }

    private void DisplayMainMenu()
    {
        string Title = "Main Menu";
        List<object> items = new List<object>{ "New Game", "Load Game", "Exit" };
        string savedGames = File.ReadAllText("SavedGames.json");
        
        if (!string.IsNullOrEmpty(savedGames))
        {
            items.Insert(0, "Continue");
        }
        
        MenuFrame menu = new MenuFrame(Title, items);
        int index = menu.Run();

        if (items.Count == 4)
        {
            switch (index)
            {
                case 0:
                    ContinueLastSavedGame();
                    break;
                case 1:
                    NewGame();
                    break;
                case 2:
                    LoadGame();
                    break;
                case 3:
                    ExitGame();
                    break;
            }
        }
        else
        {
            switch (index)
            {
                case 0:
                    NewGame();
                    break;
                case 1:
                    LoadGame();
                    break;
                case 2:
                    ExitGame();
                    break;
            }
        }
        
    }

    private void ContinueLastSavedGame()
    {
        List<Game> existingGames;
        
        try
        {
            string content = File.ReadAllText("SavedGames.json");
            existingGames = JsonSerializer.Deserialize<List<Game>>(content) ?? new List<Game>();
        }
        catch (Exception ex)
        {
            existingGames = new List<Game>();
        }
        
        string nextId = File.ReadAllText("next-id.txt");
        int id = int.Parse(nextId) - 1;
        Game foundGame = existingGames.FirstOrDefault(game => game.Id == id.ToString())!;
        int gameState = foundGame.Continue();
        HandleGame(gameState, foundGame);
    }

    private void NewGame()
    {
        _players.Clear();
        string Title = "Choose amount of players in game";
        List<object> items = new List<object>{ "2", "3", "4", "5", "6", "7", "Back"};
        MenuFrame menu = new MenuFrame(Title, items);
        int index = menu.Run();
        if (index != items.Count - 1)
        {
            _playerAmount = index + 2;
            for (int i = 0; i < _playerAmount; i++)
            {
                _players.Add(new Player($"Player {i + 1}"));
            }
            ChoosePlayer();
        }
        else
        {
            DisplayMainMenu();
        }
    }

    private void ChoosePlayer()
    {
        object[] BackAndStartGame = { "Start Game", "Back" };
        string Title = "Choose nickname for players";
        List<object> Items = _players.Concat(BackAndStartGame).ToList();
        MenuFrame menu = new MenuFrame(Title, Items);
        int index = menu.Run();
        if (index < Items.Count - 2)
        {
            NicknameForPlayerUnderIndex(index);
        }
        else if(index == Items.Count - 1)
        {
            NewGame();
        }
        else
        {
            Game game = new Game(_players);
            int gameState = game.Start();
            HandleGame(gameState, game);
        }
    }

    private void HandleGame(int gameState, Game currentGame)
    {
        if (gameState == -1)
        {
            int chosenOption = DisplayPauseMenu();
            switch (chosenOption)
            {
                case 0:
                    gameState = currentGame.Continue();
                    HandleGame(gameState, currentGame);
                    break;
                case 1:
                    if (!string.IsNullOrEmpty(currentGame.Id))
                    {
                        currentGame.SaveGame();
                        WriteLine("Game is saved!");
                        WriteLine("Press any key...");
                        ReadLine();
                        HandleGame(gameState, currentGame);
                    }
                    WriteLine("Write a name for the game:");
                    string? gameName = ReadLine();
                    currentGame.GameName = !string.IsNullOrEmpty(gameName) ? gameName : $"Game played on {DateTime.Now}";
                    currentGame.SaveGame();
                    WriteLine("Your game is saved!");
                    ReadLine();
                    DisplayMainMenu();
                    break;
                case 2:
                    DisplayMainMenu();
                    break;
            }
        }
        else if (gameState == -3)
        {
            Clear();
            List<Player> sortedPlayers = currentGame.Players
                .OrderByDescending(player => player.GetCards().Count)
                .ThenByDescending(player => player.Points)
                .ToList();
            Player? winner = sortedPlayers.FirstOrDefault();
            WriteLine($"The Winner is: {winner}");
            WriteLine("Press any key to continue...");
            ReadKey(true);
            DisplayMainMenu();
        }
    }

    private int DisplayPauseMenu()
    {
        string Title = "Pause";
        List<object> items = new List<object>{ "Resume Game", "Save game", "Exit" };
        MenuFrame menu = new MenuFrame(Title, items);
        int index = menu.Run();
        return index;
    }

    private void NicknameForPlayerUnderIndex(int index)
    {
        Clear();
        string? nickname;
    
        do
        {
            WriteLine($"Enter nickname for {_players[index]} and press Enter");
            nickname = ReadLine();

            if (string.IsNullOrWhiteSpace(nickname))
            {
                ShowWarning();
            }
        } while (string.IsNullOrWhiteSpace(nickname));

        _players[index] = new Player(nickname);
        ChoosePlayer();
    }

    private void LoadGame()
    {
        try
        {
            string data = File.ReadAllText("SavedGames.json");
            var jsonOptions = new JsonSerializerOptions()
            {
                WriteIndented = true,
                AllowTrailingCommas = true,
            };
            List<Game> savedGames = JsonSerializer.Deserialize<List<Game>>(data, jsonOptions)!;
            List<object> items = new List<object>();
       
            foreach (var game in savedGames)
            {
                items.Add(game.GameName);
            }
            items.Add("Back");

            MenuFrame menu = new MenuFrame("Load Game", items);
            int index = menu.Run();

            if (index == items.Count - 1)
            {
                DisplayMainMenu();
            }
            else
            {
                Game loadedGame = savedGames[index];
                int gameState = loadedGame.Continue();
                HandleGame(gameState, loadedGame);
            }
        }
        catch (Exception e)
        {
            Clear();
            WriteLine("Nothing to load!");
            Thread.Sleep(1000);
            DisplayMainMenu();
        }
    }

    private void ExitGame()
    {
        string Title = "Are you sure?";
        List<object> Items = new List<object>{ "Yes", "No" };
        MenuFrame menu = new MenuFrame(Title, Items);
        int index = menu.Run();
        switch (index)
        {
            case 0:
                Environment.Exit(0);
                break;
            case 1:
                DisplayMainMenu();
                break;
        }
    }


    private void ShowWarning()
    {
        Clear();
        Write("Name can't be empty!");
        Thread.Sleep(2000);
        Clear();
    }
}

