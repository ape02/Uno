// using static System.Console;
// using Domain;
// namespace Frame;
//
// public class GameFrame : IFrame
// {
//     private Player Player;
//     private GameCard CurrentCard;
//     private string Separator = new string('=', 30);
//     private List<GameCard> Cards;
//     private int SelectedIndex;
//
//
//     public GameFrame(Player player, GameCard currentCard)
//     {
//         Player = player;
//         CurrentCard = currentCard;
//         Cards = player.GetCards();
//     }
//
//     public int Run()
//     {
//         ConsoleKey pressedKey;
//
//         do
//         {
//             Clear();
//             DisplayItems();
//             
//             ConsoleKeyInfo keyInfo = ReadKey(true);
//             pressedKey = keyInfo.Key;
//             
//             switch (pressedKey)
//             {
//                 case ConsoleKey.UpArrow:
//                     SelectedIndex = (SelectedIndex == 0 ? Cards.Count - 1 : SelectedIndex - 1);
//                     break;
//                 case ConsoleKey.DownArrow:
//                     SelectedIndex = (SelectedIndex == Cards.Count - 1 ? 0 : SelectedIndex + 1);
//                     break;
//                 case ConsoleKey.P:
//                     return -1;
//                 case ConsoleKey.T:
//                     return Cards.Count;
//             }
//         } while (pressedKey != ConsoleKey.Enter);
//
//         return SelectedIndex;
//     }
//
//     private void DisplayItems()
//     {
//         DrawFrame(CurrentCard.ToString()
//         );
//         
//         for (int i = 0; i < Cards.Count; i++)
//         {
//             object currentItem = Cards[i];
//             string prefix = (SelectedIndex == i) ? "->" : "  ";
//             currentItem = $"{prefix} {currentItem}";
//             WriteLine(currentItem);
//         }
//         WriteLine(Separator);
//         WriteLine("Press T to Take Extra Card");
//         WriteLine("Press P to Pause Game");
//         DrawFrame($"{Player} Points: {Player.Points}");
//     }
//     
//     private void DrawFrame(string item)
//     {
//         int frameWidth = 30;
//         int frameHeight = 3;
//
//         string topBottomBorder = new string('=', frameWidth);
//         string cardLine = $"|{item.PadLeft((frameWidth - item.Length) / 2 + item.Length).PadRight(frameWidth - 1)}|";
//
//         WriteLine(topBottomBorder);
//
//         WriteLine(cardLine);
//
//         WriteLine(topBottomBorder);
//     }
// }