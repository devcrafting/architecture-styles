namespace EventSourcingCQRS.Domain
{
    public class GameQuestion
    {
        public GameQuestion(Question question, bool notUsed)
        {
            Question = question;
            NotUsed = notUsed;
        }
        
        public Question Question { get; } // I use Question Read Model here...not really a good idea => to refactor
        public bool NotUsed { get; }
    }
}
