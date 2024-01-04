using Domain;
using static System.Console;

namespace GameEngine;

public static class Processor
{
    public static void ProcessCard(int chosenIndex, bool inWeb)
         {
             Player currentPlayer = UnoGameEngine.State.Players[UnoGameEngine.State.PlayerIndex];
             GameCard cardInHand = currentPlayer.GetCardAtIndex(chosenIndex);
             bool validated = UnoGameEngine.ValidateCard(cardInHand);
    
             if (cardInHand.CardColor == ECardColor.Wild && !inWeb)
             {
                 ProcessWild(currentPlayer, chosenIndex, cardInHand, null);
                 return;
             }
    
             if (!validated && !inWeb)
             {
                 ShowWarning();
                 return;
             }
             
             switch (cardInHand.CardValue)
             {
                 case ECardValue.ValueTakeTwo:
                     // TakeTwo(chosenIndex);
                     int nextIndex = (UnoGameEngine.State.PlayerIndex + 1) % UnoGameEngine.State.Players.Count;
                     Player nextPlayer = UnoGameEngine.State.Players[nextIndex];
                     UnoGameEngine.Take(nextPlayer, 2, inWeb);
                     UnoGameEngine.State.Players[UnoGameEngine.State.PlayerIndex].DeleteCardAtIndex(chosenIndex);
                     UnoGameEngine.State.PlayerIndex = (UnoGameEngine.State.PlayerIndex + 2) % UnoGameEngine.State.Players.Count;
                     break;
                 case ECardValue.ValueSkip:
                     Skip(chosenIndex);
                     break;
                 case ECardValue.ValueReverse:
                     Reverse(chosenIndex);
                     break;
                 default:
                     currentPlayer.DeleteCardAtIndex(chosenIndex);
                     UnoGameEngine.State.PlayerIndex = (UnoGameEngine.State.PlayerIndex + 1) % UnoGameEngine.State.Players.Count;
                     break;
             }
             if (UnoGameEngine.State.CurrentCard.CardValue != ECardValue.Empty)
             {
                 UnoGameEngine.State.BeatenDeck.Add(UnoGameEngine.State.CurrentCard);
             }
             UnoGameEngine.State.CurrentCard = cardInHand;
         }
    
    public static void ProcessWild(Player currentPlayer, int chosenIndex, GameCard cardInHand, ECardColor? webChosenColor)
    {
        if (cardInHand.CardValue == ECardValue.ValueTakeFour)
        {
            // TakeFour(chosenIndex);
            int nextIndex = (UnoGameEngine.State.PlayerIndex + 1) % UnoGameEngine.State.Players.Count;
            Player nextPlayer = UnoGameEngine.State.Players[nextIndex];
            UnoGameEngine.Take(nextPlayer, 4, webChosenColor != null);
        }
        UnoGameEngine.State.BeatenDeck.Add(UnoGameEngine.State.CurrentCard);
        UnoGameEngine.State.BeatenDeck.Add(cardInHand);
        currentPlayer.DeleteCardAtIndex(chosenIndex);
        if (cardInHand.CardValue == ECardValue.Shuffle)
        {
            var allCards = UnoGameEngine.State.Players.SelectMany(p => p.GetCards()).ToList();
            UnoGameEngine.State.Players.ForEach(player => player.Cards.Clear());
            Random random = new Random();
            for (int i = allCards.Count - 1; i > 0; i--)
            {
                int j = random.Next(0, i + 1);
                (allCards[i], allCards[j]) = (allCards[j], allCards[i]);
            }
            int cardsPerPlayer = allCards.Count / UnoGameEngine.State.Players.Count;
            foreach (var player in UnoGameEngine.State.Players)
            {
                player.Cards.AddRange(allCards.GetRange(0, cardsPerPlayer));
                allCards.RemoveRange(0, cardsPerPlayer);
            }

            if (webChosenColor == null)
            {
                int count = 0;
                while (count != 3)
                {
                    for (int j = 0; j <= 3; j++)
                    {
                        var dots = new string('.', j);
                        Clear();
                        WriteLine($"Shuffling players cards{dots}");
                        Thread.Sleep(300);
                    }

                    count++;
                }
            }
        }

        if (webChosenColor == null)
        {
            ECardColor chosenColor = ChooseCardColor();
            UnoGameEngine.State.CurrentCard = new GameCard(ECardValue.Empty, chosenColor);
        }
        else
        {
            UnoGameEngine.State.CurrentCard = new GameCard(ECardValue.Empty, webChosenColor.Value);
        }
        UnoGameEngine.State.PlayerIndex = (UnoGameEngine.State.PlayerIndex + 2) % UnoGameEngine.State.Players.Count;
    }

    private static void ShowWarning()
    {
        Clear();
        WriteLine("You can't pick this card!");
        Thread.Sleep(2000);
    }

    private static void Skip(int chosenIndex)
    {
        UnoGameEngine.State.Players[UnoGameEngine.State.PlayerIndex].DeleteCardAtIndex(chosenIndex);
        UnoGameEngine.State.PlayerIndex = (UnoGameEngine.State.PlayerIndex + 2) % UnoGameEngine.State.Players.Count;
        
    }

    private static void Reverse(int chosenIndex)
    {
        if (UnoGameEngine.State.Players.Count == 2)
        {
            Skip(chosenIndex);
            return;
        }
        UnoGameEngine.State.Players[UnoGameEngine.State.PlayerIndex].DeleteCardAtIndex(chosenIndex);
        UnoGameEngine.State.Players.Reverse();
        UnoGameEngine.State.PlayerIndex = UnoGameEngine.State.Players.Count - 1 - UnoGameEngine.State.PlayerIndex % UnoGameEngine.State.Players.Count;
        UnoGameEngine.State.PlayerIndex = (UnoGameEngine.State.PlayerIndex + 1) % UnoGameEngine.State.Players.Count;
         
    }

    private static ECardColor ChooseCardColor()
    {
        var colors = Enum.GetValues(typeof(ECardColor))
            .Cast<ECardColor>()
            .Where(color => color != ECardColor.Wild)
            .ToArray();
        int selectedIndex = 0;
        ConsoleKey pressedKey;
        do
        {
            Clear();
            var currentPlayer = UnoGameEngine.State.Players[UnoGameEngine.State.PlayerIndex];
            if (currentPlayer.Type != PlayerType.AI) ShowPlayersCards();
            WriteLine("Choose a color to change on:");
            for (int i = 0; i < colors.Length; i++)
            {
                GameCard currentColor = new GameCard(ECardValue.Empty, (ECardColor)colors.GetValue(i)!);
                string prefix = selectedIndex == i ? "->" : "  ";
                string line = currentPlayer.Type == PlayerType.AI ? $"  {currentColor}" : $"{prefix} {currentColor}";
                WriteLine(line);
            }

            if (currentPlayer.Type == PlayerType.AI)
            { 
                Thread.Sleep(4000);
                var maxColor = currentPlayer
                    .GetCards()
                    .GroupBy(c => c.CardColor)
                    .Select(entry => new { CardColor = entry.Key, Count = entry.Count() })
                    .MaxBy(entry => entry.Count);
                
                WriteLine($"{currentPlayer} has chosen color {maxColor!.CardColor}");
                Thread.Sleep(1500);
                return maxColor!.CardColor;
            }
             
            ConsoleKeyInfo keyInfo = ReadKey(true);
            pressedKey = keyInfo.Key;
             
            switch (pressedKey)
            {
                case ConsoleKey.UpArrow:
                    selectedIndex = (selectedIndex == 0 ? colors.Length - 1 : selectedIndex - 1);
                    break;
                case ConsoleKey.DownArrow:
                    selectedIndex = (selectedIndex == colors.Length - 1 ? 0 : selectedIndex + 1);
                    break;
            }
        } while (pressedKey != ConsoleKey.Enter);

        return (ECardColor)colors.GetValue(selectedIndex)!;
    }

    private static void ShowPlayersCards()
    {
        List<GameCard> cards = UnoGameEngine.State.Players[UnoGameEngine.State.PlayerIndex].GetCards();
        WriteLine($"{UnoGameEngine.State.Players[UnoGameEngine.State.PlayerIndex]} cards:");
        foreach (GameCard card in cards)
        {
            WriteLine($"  {card}");
        }
    }
}