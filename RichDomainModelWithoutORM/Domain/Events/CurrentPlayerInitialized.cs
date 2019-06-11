namespace RichDomainModelWithoutORM.Domain.Events
{
    public class CurrentPlayerInitialized
    {
        public readonly string PlayerId;

        public CurrentPlayerInitialized(string playerId)
        {
            PlayerId = playerId;
        }
    }
}
