namespace EventSourcingCQRS.Domain
{
    public class GameQuestion
    {
        public GameQuestion(Question question, bool notUsed)
        {
            Question = question;
            NotUsed = notUsed;
        }

        public int Id { get; }
        public Question Question { get; }
        public bool NotUsed { get; }
    }
}
