using static System.Console;
using PlayerSystem;
using GameEngine;
using Frame;

namespace MenuSystem;

public class Menu
{

    private int _playerAmount;
    private Player[] _players = default!;
    
    
    
    public void Run()
    {
        DisplayMainMenu(); 
    }

    private void DisplayMainMenu()
    {
        string Title = "Main Menu";
        string[] items = { "New Game", "Load Game", "Exit" };
        // if .json file is not empty insert "Continue" to the first position
        MenuFrame menu = new MenuFrame(Title, items);
        int index = menu.Run();

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

    private void NewGame()
    {
        string Title = "Choose amount of players in game";
        string[] items = { "2", "3", "4", "5", "6", "7", "Back"};
        MenuFrame menu = new MenuFrame(Title, items);
        int index = menu.Run();
        if (index != items.Length - 1)
        {
            _playerAmount = int.Parse(items[index]);
            _players = new Player[_playerAmount];
            for (int i = 0; i < _playerAmount; i++)
            {
                _players[i] = new Player($"Player {i + 1}");
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
        object[] Items = _players.Concat(BackAndStartGame).ToArray();
        MenuFrame menu = new MenuFrame(Title, Items);
        int index = menu.Run();
        if (index < Items.Length - 2)
        {
            NicknameForPlayerUnderIndex(index);
        }
        else if(index == Items.Length - 1)
        {
            NewGame();
        }
        else
        {
            Game game = new Game(_players);
            game.Start();
        }
        
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
        throw new NotImplementedException();
    }

    private void ExitGame()
    {
        string Title = "Are you sure?";
        string[] Items = { "Yes", "No" };
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

