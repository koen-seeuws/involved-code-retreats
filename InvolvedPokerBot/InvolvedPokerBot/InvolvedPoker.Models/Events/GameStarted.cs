namespace InvolvedPoker.Models.Events;

public class GameStarted
{
    public int StartStack { get; set; }
    public List<string> PlayerNames { get; set; }
}