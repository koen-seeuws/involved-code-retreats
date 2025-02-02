using InvolvedPoker.Models.Models;

namespace InvolvedPoker.Models.Events;

public class HandEnded
{
    public Dictionary<string, List<Card>> ShowDownCards { get; set; }
}