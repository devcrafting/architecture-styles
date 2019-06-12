namespace RichDomainModelWithoutORM.Domain.Events
{
    public struct GoneToPenaltyBox : IDomainEvent
    {
        public readonly string PlayerId;

        public GoneToPenaltyBox(string playerId)
        {
            PlayerId = playerId;
        }
    }
}
