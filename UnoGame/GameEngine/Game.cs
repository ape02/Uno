using System.Text.Json;
using static System.Console;
using PlayerSystem;
using Frame;

namespace GameEngine;

public class Game
{
    public string? Id { get; set; }
    public string? GameName { get; set; }

    public int PlayerIndex { get; set; }
    public List<Player> Players { get; set; }
    public Card CurrentCard { get; set; } = default!;
    public List<Card> Deck { get; set; }
    
    private const string filePath = "SavedGames.json";
    

    public Game(List<Player> players)
    {
        Players = players;
        Deck = GenerateDeck();
        GameName = null;
    }

    public int Start()
    {
        ShuffleDeck();
        GivePlayersCards();
        CurrentCard = Deck[0];
        return ShowGameField();
    }

    public int Continue()
    {
        return ShowGameField();
    }

    public void SaveGame()
    {
        Id ??= GetNewId();
        
        List<Game> existingGames;
        
        try
        {
            string content = File.ReadAllText(filePath);
            existingGames = JsonSerializer.Deserialize<List<Game>>(content) ?? new List<Game>();
        }
        catch (Exception ex)
        {
            existingGames = new List<Game>();
        }

        if (existingGames.Any(game => game.Id == Id))
        {
            UpdateGameById(Id);
            return;
        }
        
        existingGames.Add(this);
        
        try
        {
            string content = JsonSerializer.Serialize(existingGames, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, content);
        }
        catch (Exception ex)
        { 
            WriteLine($"Error writing games to file: {ex.Message}");
        }
    }

    private static string? GetNewId()
    {
        string content = File.ReadAllText("next-id.txt");
        int id = (int.TryParse(content, out int newId) ? newId : 0) + 1;
        File.WriteAllText("next-id.txt", id.ToString());
        return content;
    }

    private void UpdateGameById(string? id)
    {
        if (id == null)
        {
            return;
        }
        
        List<Game> existingGames;
        try
        {
            string content = File.ReadAllText(filePath);
            existingGames = JsonSerializer.Deserialize<List<Game>>(content) ?? new List<Game>();
        }
        catch (Exception ex)
        {
            existingGames = new List<Game>();
        }
        
        List<Game> updatedGames = new List<Game>();

        if (existingGames.Count != 0)
        {
            foreach (var game in existingGames)
            {
                if (game.Id == Id)
                {
                    updatedGames.Add(this);
                    continue;
                }
                updatedGames.Add(game);
            }
        }
        else
        {
            return;
        }
        
        try
        {
            string content = JsonSerializer.Serialize(updatedGames, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, content);
        }
        catch (Exception ex)
        { 
            WriteLine($"Error writing games to file: {ex.Message}");
        }
    }

    private int ShowGameField()
    {
        while (!(Players.Any(player => player.Points >= 500) || Players.Any(player => player.GetCards().Count == 0)))
        {
            // CheckDeck()
            GameFrame gameField = new GameFrame(Players[PlayerIndex], CurrentCard);
            int chosenIndex = gameField.Run();
            if (chosenIndex == -1)
            {
                return chosenIndex;
            }
            if (chosenIndex == Players[PlayerIndex].GetCards().Count)
            {
                TakeCard(PlayerIndex);
            }
            else
            {
                ProcessCard(chosenIndex);
            }
        }
        return -3;
    }

    private void ProcessCard(int chosenIndex)
    {
        Card cardInHand = Players[PlayerIndex].GetCardAtIndex(chosenIndex);
        bool validated = ValidateCard(cardInHand);

        if (cardInHand.Color == "Wild")
        {
            ProcessWild(chosenIndex, cardInHand);
            return;
        }

        if (!validated)
        {
            ShowWarning();
            ShowGameField();
        }
        
        switch (cardInHand.Value)
        {
            case "+2":
                TakeTwo(chosenIndex);
                return;
            case "Skip":
                Skip(chosenIndex);
                return;
            case "Reverse":
                Reverse(chosenIndex);
                return;
            default:
                Players[PlayerIndex].DeleteCardAtIndex(chosenIndex);
                PlayerIndex = (PlayerIndex + 1) % Players.Count;
                break;
        }
    }

    private void ProcessWild(int chosenIndex, Card cardInHand)
    {
        if (cardInHand.Value == "+4")
        {
            TakeFour();
        }

        Players[PlayerIndex].DeleteCardAtIndex(chosenIndex);
        PlayerIndex = (PlayerIndex + 2) % Players.Count;
        string chosenColor = ChooseCardColor();
        CurrentCard = new Card(chosenColor, "");
    }

    private void TakeFour()
    {
        for (int i = 0; i < 4; i++)
        {
            int index = (PlayerIndex + 1 == Players.Count) ? 0 : PlayerIndex + 1;
            Players[index].AddCard(Deck[0]);
            Deck.RemoveAt(0);
        }
    }

    private void TakeTwo(int chosenIndex)
    {
        for (int i = 0; i < 2; i++)
        {
            int index = (PlayerIndex + 1) % Players.Count;
            Players[index].AddCard(Deck[0]);
            Deck.RemoveAt(0);
        }

        Players[PlayerIndex].DeleteCardAtIndex(chosenIndex);
        PlayerIndex = (PlayerIndex + 2) % Players.Count;
        
    }

    private void Skip(int chosenIndex)
    {
        Players[PlayerIndex].DeleteCardAtIndex(chosenIndex);
        PlayerIndex = (PlayerIndex + 2) % Players.Count;
       
    }

    private void Reverse(int chosenIndex)
    {
        if (Players.Count == 2)
        {
            Skip(chosenIndex);
            return;
        }
        Players[PlayerIndex].DeleteCardAtIndex(chosenIndex);
        Players.Reverse();
        PlayerIndex = Players.Count - 1 - PlayerIndex % Players.Count;
        PlayerIndex = (PlayerIndex + 1) % Players.Count;
        
    }

    private string ChooseCardColor()
    {
        String[] colors = { "Green", "Yellow", "Red", "Blue"};
        int SelectedIndex = 0;
        ConsoleKey pressedKey;
        do
        {
            Clear();
            ShowPlayersCards();
            WriteLine("Choose a color to change on:");
            for (int i = 0; i < colors.Length; i++)
            {
                object currentItem = colors[i];
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
            
            ConsoleKeyInfo keyInfo = ReadKey(true);
            pressedKey = keyInfo.Key;
            
            switch (pressedKey)
            {
                case ConsoleKey.UpArrow:
                    SelectedIndex = (SelectedIndex == 0 ? colors.Length - 1 : SelectedIndex - 1);
                    break;
                case ConsoleKey.DownArrow:
                    SelectedIndex = (SelectedIndex == colors.Length - 1 ? 0 : SelectedIndex + 1);
                    break;
            }
        } while (pressedKey != ConsoleKey.Enter);

        return colors[SelectedIndex];
    }

    private void ShowPlayersCards()
    {
        List<Card> cards = Players[PlayerIndex].GetCards();
        WriteLine("Your cards:");
        foreach (Card card in cards)
        {
            WriteLine($"  {card}");
        }
    }

    private void TakeCard(int localIndex)
    {
        Players[localIndex].AddCard(Deck[0]);
        Deck.RemoveAt(0);
    }

    private void ShowWarning()
    {
        Clear();
        WriteLine("You can't pick this card!");
        Thread.Sleep(2000);
    }

    private bool ValidateCard(Card chosenCard)
    {
        if (chosenCard.Value == CurrentCard.Value && chosenCard.Color == CurrentCard.Color) return false;
        if (chosenCard.Value != CurrentCard.Value && chosenCard.Color != CurrentCard.Color) return false;
        CurrentCard = chosenCard;
        return true;

    }

    private void GivePlayersCards()
    {
        foreach (Player player in Players)
        {
            for (int i = 0; i < 7; i++)
            {
                if (Deck.Count == 0)
                {
                    break;
                }
                Card card = Deck[0];
                player.AddCard(card);
                Deck.RemoveAt(0);
            }
        }
    }

    private void ShuffleDeck()
    {
        Random random = new Random();
        int deckSize = Deck.Count;

        for (int i = deckSize - 1; i > 0; i--)
        {
            int j = random.Next(0, i + 1);
            
            (Deck[i], Deck[j]) = (Deck[j], Deck[i]);
        }
    }

    private List<Card> GenerateDeck()
    {
        string[] colors = { "Red", "Blue", "Green", "Yellow" };
        string[] values = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "Skip", "Reverse", "+2" };

        List<Card> deck = new();
        foreach (string color in colors)
        {
            foreach (string value in values)
            {
                deck.Add(new Card(color, value));
                if (value != "0")
                {
                    deck.Add(new Card(color, value));
                }
            }
        }
        
        deck.Add(new Card("Wild", "+0"));
        deck.Add(new Card("Wild", "+4"));
        return deck;
    }
}