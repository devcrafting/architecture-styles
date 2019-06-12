namespace RichDomainModelWithoutORM.Domain.Events
{
    public struct CurrentPlayerChanged : IDomainEvent
    {
        public readonly string PlayerId;

        public CurrentPlayerChanged(string playerId)
        {
            PlayerId = playerId;
        }
    }
}
