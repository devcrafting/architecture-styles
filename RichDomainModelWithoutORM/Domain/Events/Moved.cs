namespace RichDomainModelWithoutORM.Domain.Events
{
    public struct Moved
    {
        public readonly string PlayerId;
        public readonly int NewPlace;

        public Moved(string playerId, int newPlace)
        {
            PlayerId = playerId;
            NewPlace = newPlace;
        }
    }
}