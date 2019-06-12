namespace RichDomainModelWithoutORM.Domain.Events
{
    public struct GetOutOfPenaltyBox
    {
        public readonly string PlayerId;

        public GetOutOfPenaltyBox(string playerId)
        {
            PlayerId = playerId;
        }
    }
}
