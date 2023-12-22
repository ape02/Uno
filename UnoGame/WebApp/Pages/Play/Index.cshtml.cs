using DAL;
using Domain;
using GameEngine;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Play;

public class Index : PageModel
{
    private readonly DAL.AppDbContext _context;

    private readonly IGameRepository _gameRepository = default!; 
    // private GameOptions gameOptions = new GameOptions();
    public UnoGameEngine Engine { get; set; } = default!;


    public Index(AppDbContext context)
    {
        _context = context;
        _gameRepository = new EfGameRepository(_context);
    }
    
    [BindProperty(SupportsGet = true)]
    public Guid GameId { get; set; }

    [BindProperty(SupportsGet = true)]
    public Guid PlayerId { get; set; }

    public void OnGet()
    {
        var gameState = _gameRepository.LoadGame(GameId);

        Engine = new UnoGameEngine();
        UnoGameEngine.State = gameState;


    }
}