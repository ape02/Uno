using System.Text.Json.Serialization;
using Helpers;
namespace Domain;

public class GameCard
{
    public ECardColor CardColor { get; set; }
    public ECardValue CardValue { get; set; }

    [JsonConstructor]
    public GameCard(ECardValue cardValue, ECardColor cardColor)
    {
        CardValue = cardValue;
        CardColor = cardColor;
    }
    public override string ToString()
    {
        string bgCode = GetBackgroundColorCode(CardColor);
        return $"\x1b[{bgCode}m{CardColor.Description()} {CardValue.Description()}\x1b[0m";
    }
    
    public static string GetBackgroundColorCode(ECardColor color)
    {
        return color switch
        {
            ECardColor.Red => "0;91",
            ECardColor.Green => "0;92",
            ECardColor.Yellow => "0;93",
            ECardColor.Blue => "0;94",
            _ => "0"
        };
    }
    
    public string GetShortenedValue()
    {
        switch (CardValue)
        {
            case ECardValue.Shuffle:
                return "SH";
            case ECardValue.ValueReverse:
                return "R";
            case ECardValue.ValueSkip:
                return "S";
            default:
                return CardValue.Description();
        }
    } 
}
