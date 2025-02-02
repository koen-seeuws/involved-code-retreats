namespace InvolvedProLeaguePlayer.Models.States;

public record TeamState(int TeamSpirit, GameTactic GameTactic, List<Player> PlayerStates, List<Player> BenchedPlayerStates, List<Substitution> Substitutions);
