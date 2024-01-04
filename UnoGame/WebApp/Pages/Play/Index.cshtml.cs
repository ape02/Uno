using DAL;
using Domain;
using GameEngine;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Play;

public class Index : PageModel
{
    private readonly AppDbContext _context;

    private readonly IGameRepository _gameRepository = default!; 
    public UnoGameEngine Engine { get; set; } = default!;


    public Index(AppDbContext context)
    {
        _context = context;
        _gameRepository = new EfGameRepository(_context);
    }
    
    [BindProperty(SupportsGet = true, Name = "GameId")]
    public Guid GameId { get; set; }

    [BindProperty(SupportsGet = true, Name = "PlayerId")]
    public Guid PlayerId { get; set; }

    public Player currentPlayer { get; set; } = default!;

    public string Message { get; set; } = null!;
    public bool PickCard { get; set; }
    [BindProperty(SupportsGet = true, Name = "SelectedCardIndex")]
    public int SelectedCardIndex { get; set; }

    public void OnGet(string playerId, string? message, bool pickCard, int selectedCard)
    {
        var gameState = _gameRepository.LoadGame(GameId);
        Engine = new UnoGameEngine();
        UnoGameEngine.State = gameState;
        Message = message;
        PlayerId = Guid.Parse(playerId);
        currentPlayer = gameState.Players.SingleOrDefault(p => p.Id == PlayerId)!;
        PickCard = pickCard;
        SelectedCardIndex = selectedCard;
        if (UnoGameEngine.GetActivePlayer().Type == PlayerType.AI)
        {
            var matchingCards = UnoGameEngine.GetActivePlayer()
                .GetCards()
                .Where(UnoGameEngine.ValidateCard) 
                .OrderByDescending(card => card.CardValue);
            while (matchingCards.Count() == 0)
            {
                UnoGameEngine.Take(UnoGameEngine.GetActivePlayer(), 1, true);
                matchingCards = UnoGameEngine.GetActivePlayer()
                    .GetCards()
                    .Where(UnoGameEngine.ValidateCard) 
                    .OrderByDescending(card => card.CardValue);
            }
            var aiChoice = UnoGameEngine.GetActivePlayer().GetCards().IndexOf(matchingCards.First());
            var card = UnoGameEngine.GetActivePlayer().GetCardAtIndex(aiChoice);
            var validated = UnoGameEngine.ValidateCard(card);
            if (validated && card.CardColor == ECardColor.Wild)
            {
                var maxColor = UnoGameEngine.GetActivePlayer()
                    .GetCards()
                    .GroupBy(c => c.CardColor)
                    .Select(entry => new { CardColor = entry.Key, Count = entry.Count() })
                    .MaxBy(entry => entry.Count);
                gameState = UnoGameEngine.State;
                Processor.ProcessWild(UnoGameEngine.GetActivePlayer(), aiChoice, UnoGameEngine.GetActivePlayer().GetCardAtIndex(aiChoice), maxColor!.CardColor);
                _gameRepository.SaveGame(GameId, gameState);
            }
            else if (validated)
            {
                Processor.ProcessCard(aiChoice, true);
                gameState = UnoGameEngine.State;
                _gameRepository.SaveGame(GameId, gameState);
            }
        }
    }

    public IActionResult OnPostCard(int selectedCard, Guid gameId, Guid playerId)
    {
        var card = UnoGameEngine.GetActivePlayer().GetCardAtIndex(selectedCard);
        var validated = UnoGameEngine.ValidateCard(card);
        if (validated && card.CardColor == ECardColor.Wild)
        {
            var gameState = UnoGameEngine.State;
            _gameRepository.SaveGame(gameId, gameState);
            var redirectToPage = CheckWinner();
            if (redirectToPage != null) return redirectToPage;
            return RedirectToPage("./Index", new {gameId , playerId, pickCard = true, selectedCard});
        }
        if (validated)
        {
            Processor.ProcessCard(selectedCard, true);
            var gameState = UnoGameEngine.State;
            _gameRepository.SaveGame(gameId, gameState);
            var redirectToPage = CheckWinner();
            if (redirectToPage != null) return redirectToPage;
            return RedirectToPage("./Index", new {gameId , playerId});
        }
        return RedirectToPage("./Index", new {gameId, playerId, message = "You can't choose this card!"});
    }

    private IActionResult? CheckWinner()
    {
        if (UnoGameEngine.State.Players.Any(player => player.Points >= 500) ||
            UnoGameEngine.State.Players.Any(player => player.GetCards().Count == 0))
        {
            List<Player> sortedPlayers = UnoGameEngine.State.Players
                .OrderBy(player => player.GetCards().Count)
                .ThenBy(player => player.Points)
                .ToList();
            Player winner = sortedPlayers[0];
            var game = _context.Games.Find(GameId);
            if (game != null)
            {
                _context.Games.Remove(game);
                _context.SaveChangesAsync();
            }
            return RedirectToPage("./Winner", new { winner });
        }
    
        return null;
    }

    public IActionResult OnPostTake()
    {
        var players = UnoGameEngine.State.Players;
        var index = UnoGameEngine.State.PlayerIndex;
        UnoGameEngine.Take(players[index], 1, true);
        var gameId = Guid.Parse(Request.Form["GameId"]!);
        var gameState = UnoGameEngine.State;
        _gameRepository.SaveGame(gameId, gameState);
        return RedirectToPage("./Index", new {gameId, playerId = players[index].Id});
    }
    
    public IActionResult OnPostSave()
    {
        var gameId = Guid.Parse(Request.Form["GameId"]!);
        var gameState = UnoGameEngine.State;
        _gameRepository.SaveGame(gameId, gameState);
        return RedirectToPage("../Games/Index");
    }
    
    public IActionResult OnPostWild(Guid gameId, Guid playerId)
    {
        var selectedColor = Enum.Parse<ECardColor>(Request.Form["selectedColor"]!);
        var gameState = UnoGameEngine.State;
        currentPlayer = gameState.Players.SingleOrDefault(p => p.Id == PlayerId)!;
        Processor.ProcessWild(currentPlayer, SelectedCardIndex, currentPlayer.GetCardAtIndex(SelectedCardIndex), selectedColor);
        var redirectToPage = CheckWinner();
        if (redirectToPage != null) return redirectToPage;
        _gameRepository.SaveGame(gameId, gameState);
        return RedirectToPage("./Index", new {gameId, playerId});
    }
}