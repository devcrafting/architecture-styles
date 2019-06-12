namespace RichDomainModelWithoutORM.Domain.Events
{
    internal struct GetOutOfPenaltyBox
    {
        public readonly string PlayerId;

        public GetOutOfPenaltyBox(string playerId)
        {
            PlayerId = playerId;
        }
    }
}
