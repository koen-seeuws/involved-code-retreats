namespace InvolvedPoker.Models.Models;

public class Card 
{
    public Card(CardSuit suit, CardType type)
    {
        this.Suit = suit;
        this.Type = type;
    }

    public CardSuit Suit { get; }

    public CardType Type { get; }
}