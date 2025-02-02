using System.Text.Json.Serialization.Metadata;
using InvolvedPoker.Models.Events;
using InvolvedPoker.Models.Models;

namespace InvolvedPokerBot;

public class KoenPokerBot
{
    public List<string> Names { get; set; }
    public List<Card> CommunityCards;
    public List<Card> MyCards;

    public Tuple<PlayerAction, List<Card>> DetermineAction(TurnStarted turnStarted)
    {
        var (score, cards) = ScoreCardCombos();

        var action = ActionEnum.CheckOrCall;

        var raise = 0;

        if (score < 1)
            action = ActionEnum.Fold;
        if (score >= 6)
        {
            action = ActionEnum.Raise + 10;
            raise = turnStarted.MinimumAmountToRaise;
        }

        if (score >= 7 && CommunityCards.Count >= 5)
            action = ActionEnum.AllIn;

        var playerAction = new PlayerAction { Action = action, Amount = raise };
        return new Tuple<PlayerAction, List<Card>>(playerAction, cards);
    }

    private Tuple<int, List<Card>> ScoreCardCombos()
    {
        var combos = GenerateAllCombos();
        var scores = new Dictionary<int, List<Card>>();

        foreach (var tempCombo in combos)
        {
            var combo = tempCombo.ToList();
            var score = 0;

            var pairs = CountPairs(combo.ToList());
          
                    score += pairs;
                    if(pairs > 2)
                        score += 2;
            

            if (IsStraight(combo)) score += 3;
            if (IsFlush(combo)) score += 4;
            if (IsStraightFlush(combo)) score += 7;
            if (IsRoyalFlush(combo)) score += 8;

            scores.Add(score, combo);
        }

        var maxScore = scores.Keys.Max();
        var bestCombo = scores[maxScore];

        return new Tuple<int, List<Card>>(maxScore, bestCombo);
    }

    private bool IsRoyalFlush(List<Card> cards)
    {
        return IsStraightFlush(cards) && cards.Last().Type == CardType.Ace;
    }

    private bool IsStraightFlush(List<Card> hand)
    {
        return IsFlush(hand) && IsStraight(hand);
    }


    private bool IsStraight(List<Card> cards)
    {
        var orderedTypes = cards.OrderBy(c => c.Type).Select(c => c.Type).ToList();
        var lowest = orderedTypes.First();
        foreach (var cardType in orderedTypes)
        {
            if (cardType != lowest)
                return false;
            lowest++;
        }

        return true;
    }

    private bool IsFlush(List<Card> cards)
    {
        return cards.All(c => c.Suit == CardSuit.Club) || cards.All(c => c.Suit == CardSuit.Diamond) ||
               cards.All(c => c.Suit == CardSuit.Heart) || cards.All(c => c.Suit == CardSuit.Spade);
    }

    private int CountPairs(List<Card> cards)
    {
        var count = 0;
        foreach (var cardType in Enum.GetValues<CardType>())
        {
            var tempCount = cards.Count(c => c.Type == cardType);
            if (tempCount > count)
                count = tempCount;
        }

        return count;
    }


    private List<Card[]> GenerateAllCombos()
    {
        var allCombos = new List<Card[]>();

        var amountOfCommunityCards = CommunityCards.Count;

        switch (amountOfCommunityCards)
        {
            //Preflop & Flop
            case 0 or 3:
                allCombos.Add(MyCards.Concat(CommunityCards).ToArray());
                break;
            //Turn
            case 4:
            {
                for (var i = 0; i < amountOfCommunityCards; i++)
                {
                    var combo1 = CommunityCards.Concat(new List<Card>() { MyCards.ElementAt(0) }).ToArray();
                    combo1[i] = MyCards.ElementAt(1);
                    allCombos.Add(combo1);

                    var combo2 = CommunityCards.Concat(new List<Card>() { MyCards.ElementAt(1) }).ToArray();
                    combo2[i] = MyCards.ElementAt(0);
                    allCombos.Add(combo2);
                }

                break;
            }
            //River
            case 5:
            {
                foreach (var myCard in MyCards)
                {
                    for (var i = 0; i < amountOfCommunityCards; i++)
                    {
                        var combo = CommunityCards.ToArray();
                        combo[i] = myCard;
                        allCombos.Add(combo);
                    }
                }

                for (var i = 0; i < amountOfCommunityCards - 1; i++)
                {
                    var combo = CommunityCards.ToArray();
                    combo[i] = MyCards.ElementAt(0);
                    combo[i + 1] = MyCards.ElementAt(1);
                    allCombos.Add(combo);
                }

                break;
            }
        }

        //One of myCards
        return allCombos;
    }
}