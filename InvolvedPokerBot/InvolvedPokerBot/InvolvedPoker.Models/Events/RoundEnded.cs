using InvolvedPoker.Models.Models;

namespace InvolvedPoker.Models.Events;

public class RoundEnded
{
    public List<PlayerActionAndNameDto> RoundActions { get; set; }
}