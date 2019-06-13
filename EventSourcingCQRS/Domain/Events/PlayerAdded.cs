namespace EventSourcingCQRS.Domain.Events
{
    public struct PlayerAdded : IDomainEvent
    {
        public readonly string PlayerId;
        public readonly string PlayerName;
        public readonly int Place;
        public readonly bool IsInPenaltyBox;
        public readonly int GoldCoins;

        public PlayerAdded(string playerId, string playerName)
        {
            PlayerId = playerId;
            PlayerName = playerName;
            Place = 0;
            IsInPenaltyBox = false;
            GoldCoins = 0;
        }
    }
}
