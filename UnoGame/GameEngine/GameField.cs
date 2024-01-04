using System.Text;
using System.Text.RegularExpressions;
using static System.Console;
using Domain;
using GameEngine;

namespace MenuSystem;

public class GameField
{
    private Player _player;
    private GameCard _currentCard;
    private int SelectedIndex;
    public GameField(GameState state)
    {
        _player = state.Players[state.PlayerIndex];
        _currentCard = state.CurrentCard;
    }

    public int Draw()
    {
        if (_player.Type == PlayerType.AI)
        {
            Clear();
            DisplayItems();
            int startAmount = _player.GetCards().Count;
            while (!_player.Cards.Any(UnoGameEngine.ValidateCard))
            {
                UnoGameEngine.CheckDeck(false);
                UnoGameEngine.Take(_player, 1, false);
                int endAmount = _player.GetCards().Count;
                WriteLine($"{_player} takes {endAmount - startAmount} card(s)!");
                Thread.Sleep(700);
                ClearLastLine();
            }
        }
        else
        {
            ConsoleKey pressedKey;
            
             do
             {
                 Clear();
                 DisplayItems();
                 ConsoleKeyInfo keyInfo = ReadKey(true);
                 pressedKey = keyInfo.Key;
                 
                 switch (pressedKey)
                 {
                     case ConsoleKey.UpArrow:
                         SelectedIndex = (SelectedIndex == 0 ? _player.Cards.Count - 1 : SelectedIndex - 1);
                         break;
                     case ConsoleKey.DownArrow:
                         SelectedIndex = (SelectedIndex == _player.Cards.Count - 1 ? 0 : SelectedIndex + 1);
                         break;
                     case ConsoleKey.P:
                         return -1;
                     case ConsoleKey.T:
                         return _player.Cards.Count;
                 }
             } while (pressedKey != ConsoleKey.Enter);
        }
        return SelectedIndex;
     }

     private void DisplayItems()
     {
         
         DrawFrame(_currentCard.ToString());
         if (_player.Type == PlayerType.AI)
         {
             DrawFrame($"{_player} Points: {_player.Points}");
             WriteLine(" ");
             int count = 0;
             while (count != 3)
             {
                 for (int j = 0; j <= 3; j++)
                 {
                     var dots = new string('.', j);
                     ClearLastLine();
                     WriteLine($"{_player} thinks{dots}");
                     Thread.Sleep(200);
                 }
                 count++;
             }
         }
         else
         {
             for (int i = 0; i < _player.Cards.Count; i++)
             {
                 object currentItem = _player.Cards[i];
                 string prefix = (SelectedIndex == i) ? "->" : "  ";
                 currentItem = $"{prefix} {currentItem}";
                 WriteLine(currentItem);
             } 
             WriteLine(MenuItem.Separator);
             WriteLine("Press T to Take Extra Card");
             WriteLine("Press P to Pause Game");
             DrawFrame($"{_player} Points: {_player.Points}");
         }
     }
     
     private void DrawFrame(string item)
     {
         int frameWidth = 30;
         int frameHeight = 3;

         string topBottomBorder = new string('=', frameWidth);
         
         string plainItem = RemoveAnsiEscapeCodes(item);
         int itemLength = plainItem.Length;
         string bgCode = GameCard.GetBackgroundColorCode(_currentCard.CardColor);
         
         string cardLine = item == _currentCard.ToString() ? $"|\x1b[{bgCode}m{plainItem
             .PadLeft((frameWidth - itemLength) / 2 + itemLength)
             .PadRight(frameWidth - 2)}\x1b[0m|" : 
                 $"|{plainItem
             .PadLeft((frameWidth - itemLength) / 2 + itemLength)
             .PadRight(frameWidth - 2)}|";
         WriteLine(topBottomBorder);
         WriteLine($"{cardLine}");
         WriteLine(topBottomBorder);
     }

     private static string RemoveAnsiEscapeCodes(string input)
     {
         //regex to remove ANSI escape codes
         return Regex.Replace(input, @"\x1B\[[^@-~]*[@-~]", "");
     }
     
     static void ClearLastLine()
     {
         int currentLineCursor = CursorTop;
         SetCursorPosition(0, CursorTop - 1);
         Write(new string(' ', WindowWidth));
         SetCursorPosition(0, currentLineCursor - 1);
     }
}