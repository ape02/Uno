using static System.Console;

namespace GameEngine;

public class Game
{

    private int PlayerAmount;
    private string[] _players = default!;
    
    
    
    public void Start()
    {
        DisplayMainMenu(); 
    }

    private void DisplayMainMenu()
    {
        string Title = "Main Menu";
        string[] Items = { "New Game", "Load Game", "Exit" };
        // if .json file is not empty insert "Continue" on the first position
        Menu menu = new Menu(Title, Items);
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
        string[] Items = { "2", "3", "4", "5", "6", "7", "Back"};
        Menu menu = new Menu(Title, Items);
        int index = menu.Run();
        if (index != Items.Length - 1)
        {
            PlayerAmount = int.Parse(Items[index]);
            _players = new string[PlayerAmount];
            for (int i = 0; i < PlayerAmount; i++)
            {
                _players[i] = $"Player {i + 1}";
            }
            string[] BackAndStartGame = { "Back", "Start Game" };
            Title = "Choose nickname for players";
            Items = _players.Concat(BackAndStartGame).ToArray();
            menu = new Menu(Title, Items);
            index = menu.Run();
            WriteLine(index + 1);
            // ChooseNickname();
            // choose nicknames for each player;
        }
        else
        {
            DisplayMainMenu();
        }
    }

    private void ChooseNickname()
    {
        throw new NotImplementedException();
    }

    private void LoadGame()
    {
        throw new NotImplementedException();
    }

    private void ExitGame()
    {
        string Title = "Are you sure?";
        string[] Items = { "Yes", "No" };
        Menu menu = new Menu(Title, Items);
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
}

