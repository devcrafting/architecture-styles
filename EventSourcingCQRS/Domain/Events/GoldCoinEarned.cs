namespace EventSourcingCQRS.Domain.Events
{
    public struct GoldCoinEarned : IDomainEvent
    {
        public readonly string PlayerId;
        public readonly int GoldCoins;

        public GoldCoinEarned(string playerId, int goldCoins)
        {
            PlayerId = playerId;
            GoldCoins = goldCoins;
        }
    }
}
