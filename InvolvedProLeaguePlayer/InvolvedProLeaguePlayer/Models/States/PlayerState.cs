namespace InvolvedProLeaguePlayer.Models.States;

public record PlayerState(int PlayerId, int GeneralRating, int Stamina, int Goals, int YellowCards, int RedCards, int Wage, PlayStyle? Playstyle);