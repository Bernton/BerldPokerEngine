using BerldPokerEngine.Poker;
using Xunit;

namespace BerldPokerEngine.Tests
{
    public class EngineTests
    {
        private static readonly List<List<Card>?> singleWildPlayer = new() { null };

        [Fact]
        public void Should_Throw_On_NotEnoughCards()
        {
            List<List<Card>?> holeCards = new();

            for (int i = 0; i < 24; i++)
            {
                List<Card> cards = new()
                {
                    Card.Create(i * 2),
                    Card.Create(i * 2 + 1)
                };

                holeCards.Add(cards);
            }

            Assert.Throws<ArgumentException>(() => Engine.Evaluate(null, holeCards));
        }

        [Fact]
        public void Should_Throw_On_Duplicate_BoardCards()
        {
            List<Card> boardCards = new()
            {
                Card.Card8c,
                Card.CardAs,
                Card.Card2h,
                Card.CardTc,
                Card.CardAs
            };

            Assert.Throws<ArgumentException>(() => Engine.Evaluate(boardCards, singleWildPlayer));
        }

        [Fact]
        public void Should_Throw_On_DuplicateHoleCards()
        {
            List<List<Card>?> holeCards = new()
            {
                new() { Card.CardAs, Card.CardAd },
                new() { Card.Card7s},
                new(),
                new() { Card.Card8s, Card.Card7s }
            };

            Assert.Throws<ArgumentException>(() => Engine.Evaluate(null, holeCards));
        }

        [Fact]
        public void Should_Throw_On_DuplicateAllCards()
        {
            List<Card> boardCards = new()
            {
                Card.CardTd,
                Card.Card9d,
                Card.CardJs
            };

            List<List<Card>?> holeCards = new()
            {
                new() { Card.Card3s, Card.Card3d },
                new() { Card.CardJs }
            };

            Assert.Throws<ArgumentException>(() => Engine.Evaluate(boardCards, holeCards));
        }

        [Fact]
        public void Should_EvaluateCorrectly_On_XxXxXxXxXx_AcTc_3c3d()
        {
            List<List<Card>?> holeCards = new()
            {
                new() { Card.CardAc, Card.CardTc },
                new() { Card.Card3c, Card.Card3d }
            };

            List<Player> playerStats = Engine.Evaluate(null, holeCards);

            Assert.Equal(1712304.0, playerStats.Sum(c => c.TotalEquity));
            Assert.Equal(856031.0, playerStats[0].TotalEquity);
            Assert.Equal(856273.0, playerStats[1].TotalEquity);
        }

        [Fact]
        public void Should_EvaluateCorrectly_On_XxXxXxXxXx_KcKd_Kh2c()
        {
            List<List<Card>?> holeCards = new()
            {
                new() { Card.CardKc, Card.CardKd },
                new() { Card.CardKh, Card.Card2c }
            };

            List<Player> playerStats = Engine.Evaluate(null, holeCards);

            Assert.Equal(1712304.0, playerStats.Sum(c => c.TotalEquity));
            Assert.Equal(1625383.0, playerStats[0].TotalEquity);
            Assert.Equal(86921.0, playerStats[1].TotalEquity);
        }
    }
}