namespace PlayerSystem;

public class Player
{
    public string Nickname { get; set; }
    public List<Card> Cards { get; set; } = new();
    public int Points { get; set; }

    public Player(string nickname)
    {
        Nickname = nickname;
        CountPoints();
    }

    public override string ToString()
    {
        return Nickname;
    }

    public List<Card> GetCards()
    {
        return Cards;
    }
    
    public Card GetCardAtIndex(int index)
    {
        return Cards[index];
    }
    
    public void DeleteCardAtIndex(int index)
    {
        Cards.RemoveAt(index);
        CountPoints();
    }

    public void AddCard(Card card)
    {
        Cards.Add(card);
        CountPoints();
    }

    public void CountPoints()
    {
        string[] values = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
        string[] twentyValues = {"Skip", "Reverse", "+2"};
        int points = 0;
        foreach (Card playerCard in Cards)
        {
            if (values.Contains(playerCard.Value))
            {
                int intValue = int.Parse(playerCard.Value);
                points += intValue;
            }
            else if (twentyValues.Contains(playerCard.Value))
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