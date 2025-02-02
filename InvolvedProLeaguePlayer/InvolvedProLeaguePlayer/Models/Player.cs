namespace InvolvedProLeaguePlayer.Models;

public record Player(int PlayerId, string Name, int GeneralRating, int Stamina, bool Injured, int Goals, int Assists, int YellowCards, bool RedCard, PlayStyle? PlayStyle);