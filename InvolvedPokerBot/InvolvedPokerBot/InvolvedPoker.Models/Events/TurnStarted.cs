using InvolvedPoker.Models.Models;

namespace InvolvedPoker.Models.Events;

public class TurnStarted
{
    public int CurrentStack { get; set; }
    public int MyCurrentBet { get; set; }
    public int CurrentMaxBet { get; set; }
    public int AmountToCall { get; set; }
    public int MinimumAmountToRaise { get; set; }
    public int SmallBlind { get; set; }
    public Pot MainPot { get; set; }
    public List<Pot> SidePots { get; set; }
    public List<PlayerActionAndNameDto> PreviousActions { get; set; }
}