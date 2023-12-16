using System.Runtime.Serialization;
using System.Text.Json;
using Domain;
using Helpers;

namespace DAL;

public class GameRepositoryFileSystem : IGameRepository
{
    private const string SaveLocation = "savedGames";
    
    public void SaveGame(Guid id, GameState state)
    {
        var content = JsonSerializer.Serialize(state, JsonHelper.JsonSerializerOptions);

        var fileName = Path.ChangeExtension(state.Id.ToString(), ".json");

        if (!Path.Exists(SaveLocation))
        {
            Directory.CreateDirectory(SaveLocation);
        }

        File.WriteAllText(Path.Combine(SaveLocation, fileName), content);

    }

    public List<(GameState, DateTime)> GetSavedGames()
    {
        if (!Directory.Exists(SaveLocation))
        {
            return new List<(GameState, DateTime)>();
        }
        var data = Directory.EnumerateFiles(SaveLocation);

        List<(GameState, DateTime)> savedGames = new List<(GameState, DateTime)>();
        foreach (var fileName in data)
        {
            var lastTime = File.GetLastWriteTime(fileName);
            var jsonStr = File.ReadAllText(fileName);
            var deserialized = JsonSerializer.Deserialize<GameState>(jsonStr, JsonHelper.JsonSerializerOptions);
            savedGames.Add((deserialized, lastTime));
        }
        return savedGames;
    }
    
    public GameState LoadGame(Guid id)
    {
        var fileName = Path.ChangeExtension(id.ToString(), ".json");

        var jsonStr = File.ReadAllText(Path.Combine(SaveLocation, fileName));
        var res = JsonSerializer.Deserialize<GameState>(jsonStr, JsonHelper.JsonSerializerOptions);
        if (res == null) throw new SerializationException($"Cannot deserialize {jsonStr}");

        return res;

    }
}