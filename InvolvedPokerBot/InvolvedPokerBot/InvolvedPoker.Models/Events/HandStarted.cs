using InvolvedPoker.Models.Models;

namespace InvolvedPoker.Models.Events;

public class HandStarted
{
    public Card FirstCard { get; set; }
    public Card SecondCard { get; set; }
}