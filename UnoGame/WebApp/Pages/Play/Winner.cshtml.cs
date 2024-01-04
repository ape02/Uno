using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Play;

public class Winner : PageModel
{

    public string GameWinner { get; set; } = default!;

    public void OnGet(string winner)
    {
        GameWinner = winner;
    }
}