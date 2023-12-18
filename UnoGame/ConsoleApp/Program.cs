using DAL;
using Domain;
using GameEngine;
using MenuSystem;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Uno_Items;
using static System.Console;


bool shuffleIncluded = false;

var connectionString = "DataSource=<%temppath%>uno.db;Cache=Shared";
connectionString = connectionString.Replace("<%temppath%>", Path.GetTempPath());

var contextOptions = new DbContextOptionsBuilder<AppDbContext>()
    .UseSqlite(connectionString)
    .EnableDetailedErrors()
    .EnableSensitiveDataLogging()
    .Options;
using var db = new AppDbContext(contextOptions);
// apply all the migrations
db.Database.Migrate();

// =========MAIN=========
while (true)
{
    var chosenItem = Display.MainMenu();
    // IGameRepository gameRepository = new GameRepositoryFileSystem();
    IGameRepository gameRepository = new EfGameRepository(db);
    var savedGameList = gameRepository.GetSavedGames();
    WriteLine(savedGameList.Count);
    ReadLine();
    
      switch (chosenItem)
    {
        case "Continue":
            var orderedGames = savedGameList
                .OrderBy(g => g.Item2);
            var gameToContinue = orderedGames.ElementAt(0).Item1;
            UnoGameEngine.State = gameRepository.LoadGame(gameToContinue.Id);
            NewGame();
            break;
        case "New Game":
            UnoGameEngine.State = new GameState
            {
                ShuffleCardIncluded = shuffleIncluded
            };
            PlayerSetup playerSetup = new PlayerSetup();
            playerSetup.ConfigurePlayers();
            if (playerSetup.PlayersConfigured)
            {
                NewGame();
            }
            break;
        case "Load Game":
            var items = savedGameList.Select((s, i) => i + 1 + " - " 
                                                       + s.Item1.GameName 
                                                       + " - " + s.Item2).ToList();
            if (items.Count == 0)
            {
                Clear();
                WriteLine("Nothing to load! There are no saved games!");
                Thread.Sleep(2000);
                break;
            }
            int index = Display.LoadGameMenu(items);
            if (index == savedGameList.Count) break;
            var gameToLoad = savedGameList.ElementAt(index).Item1;
            UnoGameEngine.State = gameRepository.LoadGame(gameToLoad.Id);
            NewGame();
            break;
        case "Options":
            bool showOptionsMenu = true;

            while (showOptionsMenu)
            {
                int chosenOption = Display.OptionsMenu(shuffleIncluded);

                switch (chosenOption)
                {
                    case 0:
                        shuffleIncluded = !shuffleIncluded;
                        break;
                    case 1:
                        Clear();
                        WriteLine("The Shuffle Card has a special ability:");
                        WriteLine("When played, it collects all players' cards, shuffles them, and distributes them back to the players.");
                        WriteLine("Press any key to continue...");
                        ReadKey();
                        break;
                    case 2:
                        showOptionsMenu = false;
                        break;
                }
            }
            break;
        case "Exit":
            Display.Exit();
            break;
        default:
            Display.MainMenu();
            break;
    }  
}

void NewGame()
{
    UnoGameEngine.Start();
    Game game = new Game(); 
    game.Run();
    Game.GetWinner();
}