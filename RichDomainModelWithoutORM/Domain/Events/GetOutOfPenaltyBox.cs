namespace RichDomainModelWithoutORM.Domain.Events
{
    public struct GetOutOfPenaltyBox : IDomainEvent
    {
        public readonly string PlayerId;

        public GetOutOfPenaltyBox(string playerId)
        {
            PlayerId = playerId;
        }
    }
}
