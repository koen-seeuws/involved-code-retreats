namespace InvolvedProLeaguePlayer.Models.States;

public record GameState
{
    public int GameId { get; init; }
    public int Minute { get; init; }
    public bool PlayingHome { get; set; }
    public int HomeScore { get; init; }
    public int AwayScore { get; init; }
    public TeamState TeamState { get; init; } = null!;


    public bool ShouldChangeTactic(out GameTactic? gameTactic)
    {
        var difference = HomeScore - AwayScore;
        if (!PlayingHome)
            difference *= -1;

        if (Minute <= 15)
        {
            gameTactic = GameTactic.UltraAttacking;
        } else if (Minute <= 30)
        {
            gameTactic = GameTactic.Attacking;
        }
        else
        {
            const byte aggression = 3;
            switch (difference)
            {
                case <= aggression-2:
                    gameTactic = GameTactic.UltraAttacking;
                    break;
                case aggression-1:
                    gameTactic = GameTactic.Attacking;
                    break;
                case aggression:
                    gameTactic = GameTactic.Neutral;
                    break;
                case aggression+1:
                    gameTactic = GameTactic.Defensive;
                    break;
                case >= aggression + 2:
                    gameTactic = GameTactic.UltraDefensive;
                    break;
            }
        }


        return gameTactic != TeamState.GameTactic;
    }

    public bool ShouldSubstitute(out Player? playerOut, out Player? playerIn)
    {
        //Injured
        playerOut =
            TeamState.PlayerStates.Find(p => p.Injured);
        
        if (playerOut != null)
        {
            var outStyle = (int?)playerOut.PlayStyle ?? 0;
            
            playerIn = TeamState.BenchedPlayerStates
                .Where(p => !p.Injured)
                .OrderBy(p =>
                {
                    var pStyle = (int?)p.PlayStyle ?? 0;
                    var styleDifference = outStyle - pStyle;
                    if (styleDifference < 0)
                        styleDifference *= -1;
                    return styleDifference;
                })
                .ThenByDescending(p => p.Stamina)
                .FirstOrDefault();
            if (playerIn != null) return true;
        }

        playerIn = null;
        return false;
    }
}