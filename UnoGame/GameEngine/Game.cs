using DAL;
using Domain;
using MenuSystem;
using static System.Console;

namespace GameEngine;

public class Game
{
    private GameState state = UnoGameEngine.State;
    private static bool hasWinner;
    public void Run()
    {
        int choice;
        while (true)
        {
             UnoGameEngine.CheckDeck();
            GameField field = new GameField(state);
            var currentPlayer = state.Players[state.PlayerIndex];

            if (currentPlayer.Type == PlayerType.AI)
            {
                field.Draw();
                
                int aiChoice;
                var matchingCards = currentPlayer
                    .GetCards()
                    .Where(UnoGameEngine.ValidateCard) 
                    .OrderByDescending(card => card.CardValue);
                aiChoice = currentPlayer.GetCards().IndexOf(matchingCards.First());
                choice = aiChoice;
                Processor.ProcessCard(choice);
                continue;
            }
            choice = field.Draw();
            if (choice == -1)
            {
                int pauseChoice;
    
                do
                {
                    pauseChoice = Display.PauseMenu();
                    switch (pauseChoice)
                    {
                        case 0:
                            continue;
                        case 1:
                            // Save game here
                            GameRepositoryFileSystem gameRepository = new GameRepositoryFileSystem();
                
                            if (string.IsNullOrEmpty(state.GameName))
                            {
                                string? gameName;
                                do
                                {
                                    WriteLine("Write a name for the game and press enter:");
                                    gameName = ReadLine();
                                    if (!string.IsNullOrWhiteSpace(gameName)) continue;
                                    Clear();
                                    Write("Name can't be empty!");
                                    Thread.Sleep(2000);
                                    Clear();
                                } while (string.IsNullOrWhiteSpace(gameName));

                                state.GameName = gameName;
                            }
                
                            gameRepository.SaveGame(state.Id, state);
                            WriteLine("Game is saved!");
                            WriteLine("Press any key to continue...");
                            ReadLine();
                            break;
                        case 2:
                            return;
                    }
                } while (pauseChoice != 0);
            } else if (choice == currentPlayer.GetCards().Count)
            {
                UnoGameEngine.Take(currentPlayer, 1);
            }
            else
            {
                UnoGameEngine.ValidateCard(currentPlayer.GetCardAtIndex(choice));
                Processor.ProcessCard(choice);
            }

            if (!(state.Players.Any(player => player.Points >= 500) ||
                  state.Players.Any(player => player.GetCards().Count == 0))) continue;
            hasWinner = true;
            break;
        }
        
    }

    public static void GetWinner()
    {
        if (!hasWinner) return;
        List<Player> sortedPlayers = UnoGameEngine.State.Players
            .OrderBy(player => player.GetCards().Count)
            .ThenBy(player => player.Points)
            .ToList();
        Player winner = sortedPlayers[0];
        WriteLine($"The Winner is: {winner}");
        WriteLine("Press any key to continue...");
        ReadKey(true);
    }
}