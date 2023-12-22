using static System.Console;
using Domain;
using Helpers;

namespace GameEngine;

public class UnoGameEngine
{
    public static GameState State = new ();

    public Player GetActivePlayer()
    {
        return State.Players[State.PlayerIndex];
    }
    
    public int GetActivePlayerNumber()
    {
        return State.PlayerIndex;
    }

    public static void Start()
    {
        GenerateDeck();
        ShuffleDeck();
        State.CurrentCard = State.Deck[0];
        while (State.CurrentCard.CardColor == ECardColor.Wild)
        {
            ShuffleDeck();
            State.CurrentCard = State.Deck[0];
        }
        GiveCardsToPlayers();
        State.Deck.RemoveAt(0);
    }
    public static void CheckDeck()
    {
        if (State.Deck.Count == 0)
        {
            if (State.BeatenDeck.Count != 0)
            {
                int count = 0;
                while (count != 3)
                {
                    for (int j = 0; j <= 3; j++)
                    {
                        var dots = new string('.', j);
                        Clear();
                        WriteLine($"Shuffling beaten cards{dots}");
                        Thread.Sleep(300);
                    }

                    count++;
                }

                State.Deck = new List<GameCard>(State.BeatenDeck);
                State.BeatenDeck.Clear();
                ShuffleDeck();
            }
        }
    }
    
     public static void Take(Player player, int cardsCount)
     {
         if (State.Deck.Count == 0)
         {
             if (State.BeatenDeck.Count != 0) return;
             WriteLine("Players have all cards!");
             Thread.Sleep(1500);
             return;
         }

         for (int i = 0; i < cardsCount; i++)
         {
             try
             {
                 player.TakeCard(State.Deck[0]);
                 State.Deck.RemoveAt(0);
             }
             catch (Exception)
             {
                 WriteLine($"Nothing to take, deck is empty. Taken cards:{i + 1}");
                 Thread.Sleep(2000);
                 CheckDeck();
                 return;
             }
         }
     }
     
     
     
    public static bool ValidateCard(GameCard chosenCard)
    {
        if (chosenCard.CardColor == ECardColor.Wild)
        {
            return true;
        }
        //chosen card is the same as on the table
        if (chosenCard.CardValue == State.CurrentCard.CardValue 
            && chosenCard.CardColor == State.CurrentCard.CardColor) return false;
        //chosen card has no same value and color
        if (chosenCard.CardValue != State.CurrentCard.CardValue 
            && chosenCard.CardColor != State.CurrentCard.CardColor) return false;
        //check chosen card value is not empty value
        
        // State.CurrentCard = chosenCard;
        return true;
    }
    public static void GiveCardsToPlayers()
     {
         if (State.Players.All(p => p.Cards.Count > 0))
         {
             return;
         }
         foreach (Player player in State.Players)
         {
             for (int i = 0; i < 7; i++)
             {
                 if (State.Deck.Count == 0)
                 {
                     break;
                 }
                 GameCard card = State.Deck[0];
                 player.TakeCard(card);
                 State.Deck.RemoveAt(0);
             }
         }
     }

     private static void ShuffleDeck()
     {
         if (State.Deck.Count < 2)
         {
             return;
         }
         Random random = new Random();
         int deckSize = State.Deck.Count;

         for (int i = deckSize - 1; i > 0; i--)
         {
             int j = random.Next(0, i + 1);
             
             (State.Deck[i], State.Deck[j]) = (State.Deck[j], State.Deck[i]);
         }
     }
     
     private static void GenerateDeck()
     {
         if (State.Deck.Count != 0) return;
         var colors = Enum.GetValues(typeof(ECardColor))
             .Cast<ECardColor>()
             .Where(c => c.Description() != "Wild");
         var values = Enum.GetValues(typeof(ECardValue))
             .Cast<ECardValue>()
             .Where(e => e.Description() != "Shuffle" 
                         && e.Description() != "" 
                         && e.Description() != "+4" 
                         && e.Description() != "+0")
             .ToArray();
         foreach (ECardColor color in colors)
         {
             foreach (ECardValue value in values)
             {
                 var card = new GameCard(value, color);
                 State.Deck.Add(card);
                 if (value != ECardValue.Value0)
                 {
                     State.Deck.Add(card);
                 }
             }
         }
         var wildOnlyColor = new GameCard(ECardValue.ValueNull, ECardColor.Wild);
         var wildTakeFour = new GameCard(ECardValue.ValueTakeFour, ECardColor.Wild);
         var shuffleCard = new GameCard(ECardValue.Shuffle, ECardColor.Wild);
         for (int i = 0; i < 4; i++)
         {
             State.Deck.Add(wildOnlyColor);
             State.Deck.Add(wildTakeFour);
         }
         if (State.ShuffleCardIncluded)
         {
             State.Deck.Add(shuffleCard);
         }
     }
}