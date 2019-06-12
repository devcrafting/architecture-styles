namespace RichDomainModelWithoutORM.Domain.Events
{
    public struct GoneToPenaltyBox
    {
        public readonly string PlayerId;

        public GoneToPenaltyBox(string playerId)
        {
            PlayerId = playerId;
        }
    }
}
