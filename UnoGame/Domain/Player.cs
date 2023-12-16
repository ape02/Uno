using Helpers;

namespace Domain;

public class Player
{
    public PlayerType Type { get; set; } = PlayerType.Human;
    public string Nickname { get; set; }
    public List<GameCard> Cards { get; set; } = new();
    public int Points { get; set; }

    public Player(string nickname)
    {
        Nickname = nickname;
        CountPoints();
    }

    public override string ToString()
    {
        return $"{Nickname} ({Type})";
    }

    public List<GameCard> GetCards()
    {
        return Cards;
    }
    
    public GameCard GetCardAtIndex(int index)
    {
        return Cards[index];
    }
    
    public void DeleteCardAtIndex(int index)
    {
        Cards.RemoveAt(index);
        CountPoints();
    }

    public void TakeCard(GameCard card)
    {
        Cards.Add(card);
        CountPoints();
    }

    public void CountPoints()
    {
        string[] values = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
        string[] twentyValues = {"Skip", "Reverse", "+2", "+4"};
        int points = 0;
        foreach (GameCard playerCard in Cards)
        {
            if (values.Contains(playerCard.CardValue.Description()))
            {
                int intValue = int.Parse(playerCard.CardValue.Description());
                points += intValue;
            }
            else if (twentyValues.Contains(playerCard.CardValue.Description()))
            {
                points += 20;
            }
            else
            {
                points += 50;
            }
        }
        Points = points;
    }
}