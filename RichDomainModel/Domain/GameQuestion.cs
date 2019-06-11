namespace RichDomainModel.Domain
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
        public bool NotUsed { get; private set; }

        public void Use()
        {
            NotUsed = false;
        }
    }
}
