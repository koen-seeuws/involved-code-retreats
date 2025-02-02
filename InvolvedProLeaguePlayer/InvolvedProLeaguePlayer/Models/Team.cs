namespace InvolvedProLeaguePlayer.Models;

public record Team(int TeamSpirit, int TotalWages, List<Player> PlayerStates, List<Player> BenchedPlayerStates, List<Substitution> Substitutions);