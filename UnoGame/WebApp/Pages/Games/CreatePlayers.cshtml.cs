using System.Text.Json;
using Domain;
using GameEngine;
using Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Game = Domain.Database.Game;
using Player = Domain.Database.Player;

namespace WebApp.Pages.Games;

public class CreatePlayers : PageModel
{
    private readonly DAL.AppDbContext _context;

    public CreatePlayers(DAL.AppDbContext context)
    {
        _context = context;
    }

    [BindProperty] public int PlayerCount { get; set; }
    [BindProperty] public List<Player> Players { get; set; } = new ();
    public Game Game { get; set; } = default!;

    public void OnGet(string id, int? playercount)
    {
        var game = _context.Games.First(g => g.Id == Guid.Parse(id));
        Game = game;
        PlayerCount = playercount ?? 0;
        for (int i = 0; i < PlayerCount; i++)
        {
            Players.Add(new Player()
            {
                Game = Game,
                GameId = Game.Id,
                Id = Guid.NewGuid()
            });
        }
    }

    public async Task<IActionResult> OnPost(string id)
    {
        Players = (Players == null) ? new List<Player>() : new List<Player>(Players);
        if (Players.IsNullOrEmpty() || Players.Any(p => p.Nickname.IsNullOrEmpty()))
        {
            return RedirectToPage("./CreatePlayers", new { id, playercount = PlayerCount });
        }
        Game = await _context.Games.FirstAsync(g => g.Id == Guid.Parse(id));
        var state = JsonSerializer.Deserialize<GameState>(Game.State, JsonHelper.JsonSerializerOptions);
        foreach (var dbPlayer in Players)
        {
            var player = new Domain.Player(dbPlayer.Nickname)
            {
                Id = dbPlayer.Id,
                Type = dbPlayer.PlayerType
            };
            state!.Players.Add(player);
            _context.Players.Add(dbPlayer);
        }
        Game.Players = Players;
        UnoGameEngine.State = state!;
        UnoGameEngine.GiveCardsToPlayers();
        Game.State = JsonSerializer.Serialize(UnoGameEngine.State, JsonHelper.JsonSerializerOptions);
        _context.Update(Game);
        await _context.SaveChangesAsync();
        return RedirectToPage("./Index");
    }
}