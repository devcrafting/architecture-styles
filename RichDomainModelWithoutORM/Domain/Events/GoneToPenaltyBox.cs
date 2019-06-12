namespace RichDomainModelWithoutORM.Domain.Events
{
    public class GoneToPenaltyBox
    {
        public readonly string PlayerId;

        public GoneToPenaltyBox(string playerId)
        {
            PlayerId = playerId;
        }
    }
}
