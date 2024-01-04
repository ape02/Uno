using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DAL;
using Domain.Database;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Internal;

namespace WebApp.Pages_Games
{
    public class IndexModel : PageModel
    {
        private readonly DAL.AppDbContext _context;

        public IndexModel(DAL.AppDbContext context)
        {
            _context = context;
        }

        public List<SelectListItem> Players { get; set; } = default!;
        [BindProperty]
        public Guid SelectedPlayerId { get; set; }
        public List<Game> Game { get; set; } = new ();

        public async Task OnGetAsync()
        {
            var games = await _context.Games.ToListAsync();
            foreach (var game in games)
            { 
                await _context.Players
                    .Where(p => p.GameId == game.Id)
                    .DefaultIfEmpty()
                    .ToListAsync();
                
            }
            Game = games;
        }

        public IActionResult OnPost()
        {
            var gameId = Guid.Parse(Request.Form["GameId"]!);
            var playerId = SelectedPlayerId;
            return RedirectToPage("../Play/Index", new {playerId, gameId});
        }
    }
}
