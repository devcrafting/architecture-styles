namespace RichDomainModelWithoutORM.Domain.Events
{
    public struct CurrentPlayerChanged
    {
        public readonly string PlayerId;

        public CurrentPlayerChanged(string playerId)
        {
            PlayerId = playerId;
        }
    }
}
