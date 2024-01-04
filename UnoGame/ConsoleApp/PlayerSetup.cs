using static System.Console;
using Domain;
using GameEngine;
using MenuSystem;

namespace Uno_Items;

public class PlayerSetup
{
    // private UnoGameEngine _gameEngine = new ();
    //
    // public static UnoGameEngine GameEngine => _gameEngine;
    public bool PlayersConfigured;
    //
    // public PlayerSetup(UnoGameEngine gameEngine)
    // {
    //     _gameEngine = gameEngine;
    // }
    
    public void ConfigurePlayers()
    {
        PlayersConfigured = false;
        GameState gameState = UnoGameEngine.State;
        gameState.Players.Clear();
        List<object> items = new List<object> { "2", "3", "4", "5", "6", "7", "Back" };
        Menu menu = new Menu("Choose amount of players in game");
        menu.AddMenuItems(items);
        int index = menu.Draw();
        if (index != items.Count - 1)
        {
            items.Clear();
            items.Add(0);
            var playerAmount = index + 2;
            for (int i = 0; i < playerAmount; i++)
            {
                var player = new Player($"Player {i + 1}");
                player.Id = Guid.NewGuid();
                gameState.Players.Add(player);
                items.Add(i + 1);
            }
            items.Add("Back");
        }
        else
        {
            return;
        }
        UnoGameEngine.State.Players = gameState.Players;
        ChooseAi(gameState);
    }

    private void ChooseAi(GameState gameState)
    {
        Menu menu = new Menu("Choose amount of AI's in game");
        List<object> items = new List<object>();
        for (int i = 0; i <= gameState.Players.Count; i++)
        {
            items.Add(i);
        }
        items.Add("Back");
        menu.AddMenuItems(items);
        int index = menu.Draw();
        if (index != items.Count - 1)
        {
            gameState.Players.ForEach(p => p.Type = PlayerType.Human);
            
            for (int i = gameState.Players.Count - 1; i >= gameState.Players.Count - index; i--)
            {
                gameState.Players[i].Type = PlayerType.AI;
            }
            UnoGameEngine.State.Players = gameState.Players;
            ChoosePlayer(gameState);
        }
        else
        {
            ConfigurePlayers();
        }
    }

    private void ChoosePlayer(GameState state)
     {
         object[] BackAndStartGame = { "Start Game", "Back" };
         string title = "Choose nickname for players";
         List<object> items = state.Players.Concat(BackAndStartGame).ToList();
         Menu menu = new Menu(title);
         menu.AddMenuItems(items);
         int index = menu.Draw();
         
         if (index < items.Count - 2)
         {
             Clear();
             string? nickname;
             do
             {
                 WriteLine($"Enter nickname for {state.Players[index]} and press Enter");
                 nickname = ReadLine();
                 if (!string.IsNullOrWhiteSpace(nickname)) continue;
                 Clear();
                 Write("Name can't be empty!");
                 Thread.Sleep(2000);
                 Clear();
             } while (string.IsNullOrWhiteSpace(nickname));
    
             state.Players[index].Nickname = nickname;
             UnoGameEngine.State.Players[index].Nickname = nickname;
             ChoosePlayer(state);
         }
         else if(index == items.Count - 1)
         {
             ChooseAi(state);
         }
         else
         {
             PlayersConfigured = true;
         }
     }
}