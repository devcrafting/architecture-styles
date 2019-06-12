namespace RichDomainModelWithoutORM.Domain.Events
{
    internal class CurrentPlayerChanged
    {
        public readonly string PlayerId;

        public CurrentPlayerChanged(string playerId)
        {
            PlayerId = playerId;
        }
    }
}
