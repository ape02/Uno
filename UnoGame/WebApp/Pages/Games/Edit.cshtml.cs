using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Domain;
using Domain.Database;
using Helpers;
using Player = Domain.Database.Player;

namespace WebApp.Pages_Games
{
    public class EditModel : PageModel
    {
        private readonly DAL.AppDbContext _context;

        public EditModel(DAL.AppDbContext context)
        {
            _context = context;
        }
        [BindProperty] public List<Player> Players { get; set; } = new ();
        [BindProperty] public Game Game { get; set; } = default!;
        [BindProperty] public bool ShuffleIncluded { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            var game = await GetGame(id);
            if (game == null)
            {
                return NotFound();
            }
            Game = game;
            Players = game.Players!.ToList();
            ShuffleIncluded = JsonSerializer.Deserialize<GameState>(Game.State, JsonHelper.JsonSerializerOptions)!.ShuffleCardIncluded;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            var state = JsonSerializer.Deserialize<GameState>(Game.State, JsonHelper.JsonSerializerOptions);
            for (int i = 0; i < Players.Count; i++)
            {
                var player = Players[i];
                var existingPlayer = _context.Players.Find(player.Id);

                if (existingPlayer != null)
                {
                    existingPlayer.Nickname = player.Nickname;
                    existingPlayer.PlayerType = player.PlayerType;
                    state!.Players[i].Nickname = player.Nickname;
                    state.Players[i].Type = player.PlayerType;
                }
            }
            Game.UpdatedAtDt = DateTime.Now;
            var shuffleCard = state!.Deck.FirstOrDefault(c => c.CardValue == ECardValue.Shuffle);
            if (ShuffleIncluded && shuffleCard == null)
            {
                state.Deck.Add(new GameCard(ECardValue.Shuffle, ECardColor.Wild));
            }
            else if (ShuffleIncluded == false && shuffleCard != null)
            {
                state.Deck.Remove(shuffleCard);
            }
            state.ShuffleCardIncluded = ShuffleIncluded;
            state.GameName = Game.GameName;
            Game.State = JsonSerializer.Serialize(state, JsonHelper.JsonSerializerOptions);
            if (!ModelState.IsValid)
            {
                return Page();
            }
            _context.Attach(Game).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GameExists(Game.Id))
                {
                    return NotFound();
                }
                throw;
            }
            return RedirectToPage("./Index");
        }

        private bool GameExists(Guid id)
        {
            return _context.Games.Any(e => e.Id == id);
        }

        private async Task<Game> GetGame(Guid? id)
        {
            var game =  await _context.Games.FirstOrDefaultAsync(m => m.Id == id);
            await _context.Players
                .Where(p => p.GameId == game!.Id)
                .DefaultIfEmpty()
                .ToListAsync();
            return game!;
        }
    }
}
