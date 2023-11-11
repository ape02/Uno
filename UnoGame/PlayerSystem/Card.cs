using System.Drawing;

namespace PlayerSystem;

public class Card
{
    public string Value { get; }
    public string Color { get; }

    public Card(string color, string value)
    {
        this.Value = value;
        this.Color = color;
    }

    public override string ToString()
    {
        return (string.IsNullOrWhiteSpace(Value)) ? $"{Color}" : $"{Color}, {Value}";
    }
}