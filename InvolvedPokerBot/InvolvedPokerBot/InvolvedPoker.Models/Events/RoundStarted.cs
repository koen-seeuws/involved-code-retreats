using InvolvedPoker.Models.Models;

namespace InvolvedPoker.Models.Events;

public class RoundStarted
{
    public int CurrentStack { get; set; }
    public List<Card> CommunityCards { get; set; }
    public Pot MainPot { get; set; }
    public List<Pot> SidePots { get; set; }
    public Dictionary<string, int> PlayerStacks { get; set; }
}