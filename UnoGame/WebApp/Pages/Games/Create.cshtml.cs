using Domain;
using Domain.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using Helpers;
using GameEngine;

namespace WebApp.Pages_Games;

public class CreateModel : PageModel
{
    private readonly DAL.AppDbContext _context;

    public CreateModel(DAL.AppDbContext context)
    {
        _context = context;
    }
    
    [BindProperty] public Domain.Database.Game Game { get; set; } = default!;
    [BindProperty] public bool ShuffleIncluded { get; set; }
    public async Task<IActionResult> OnPost()
    {
        Game.CreatedAtDt = DateTime.Now;
        Game.UpdatedAtDt = DateTime.Now;
        GameState state = new GameState
        {
            GameName = Game.GameName,
            ShuffleCardIncluded = ShuffleIncluded
        };
        UnoGameEngine.State = state;
        UnoGameEngine.GenerateDeck();
        UnoGameEngine.ShuffleDeck();
        UnoGameEngine.DefineCurrentCard();
        Game.Players = new List<Domain.Database.Player>();
        Game.Id = Guid.NewGuid();
        state.Id = Game.Id;
        Game.State = JsonSerializer.Serialize(UnoGameEngine.State, JsonHelper.JsonSerializerOptions);
        _context.Games.Add(Game);
        await _context.SaveChangesAsync();
        return RedirectToPage("./Index");
    }
}
