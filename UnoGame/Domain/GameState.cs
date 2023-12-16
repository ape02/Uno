namespace Domain;

public class GameState
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? GameName { get; set; }
    public List<GameCard> Deck { get; set; } = new ();
    public List<GameCard> BeatenDeck { get; set; } = new ();
    public GameCard CurrentCard { get; set; } = default!;
    public int PlayerIndex { get; set; } = 0;
    public List<Player> Players { get; set; } = new ();

    public bool ShuffleCardIncluded { get; set; }
}
