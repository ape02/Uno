using System.Text.Json;
using Domain;
using Domain.Database;
using Helpers;
namespace DAL;

public class EfGameRepository : IGameRepository
{
    private readonly AppDbContext _ctx;

    public EfGameRepository(AppDbContext ctx)
    {
        _ctx = ctx;
    }

    public void SaveGame(Guid id, GameState state)
    {
        // is it already in db?
        var game = _ctx.Games.FirstOrDefault(g => g.Id == state.Id);
        if (game == null)
        {
            game = new Game()
            {
                Id = state.Id,
                State = JsonSerializer.Serialize(state, JsonHelper.JsonSerializerOptions),
                Players = state.Players.Select(p => new Domain.Database.Player()
                {
                    Id = p.Id,
                    NickName = p.Nickname,
                    PlayerType = p.Type
                }).ToList()
            };
            _ctx.Games.Add(game);
        }
        else
        {
            game.UpdatedAtDt = DateTime.Now;
            game.State = JsonSerializer.Serialize(state, JsonHelper.JsonSerializerOptions);
        }

        var changeCount = _ctx.SaveChanges();
        Console.WriteLine("SaveChanges: " + changeCount);
    }

    public List<(GameState, DateTime)> GetSavedGames()
    {
        return _ctx.Games
            .OrderByDescending(g => g.UpdatedAtDt)
            .ToList()
            .Select(g => (JsonSerializer.Deserialize<GameState>(g.State, JsonHelper.JsonSerializerOptions)!, g.UpdatedAtDt))
            .ToList();
    }

    public GameState LoadGame(Guid id)
    {
        var game = _ctx.Games.First(g => g.Id == id);
        return JsonSerializer.Deserialize<GameState>(game.State, JsonHelper.JsonSerializerOptions)!;
    }

}