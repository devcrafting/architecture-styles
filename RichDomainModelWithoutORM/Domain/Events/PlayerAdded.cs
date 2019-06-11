namespace RichDomainModelWithoutORM.Domain.Events
{
    public class PlayerAdded
    {
        public readonly string PlayerId;
        public readonly string PlayerName;
        public readonly int Place = 0;
        public readonly bool IsInPenaltyBox = false;
        public readonly int GoldCoins = 0;

        public PlayerAdded(string playerId, string playerName)
        {
            PlayerId = playerId;
            PlayerName = playerName;
        }
    }
}
