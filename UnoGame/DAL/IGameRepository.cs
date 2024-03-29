﻿using Domain;
using static System.Console;

namespace DAL;

public interface IGameRepository
{
    void SaveGame(Guid id, GameState state);
    
    List<(GameState, DateTime)> GetSavedGames();
    
    GameState LoadGame(Guid id);
}