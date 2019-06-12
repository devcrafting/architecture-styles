namespace RichDomainModelWithoutORM.Domain.Events
{
    public class QuestionAsked
    {
        public readonly int QuestionId;
        public readonly string Text;

        public QuestionAsked(int questionId, string text)
        {
            QuestionId = questionId;
            Text = text;
        }
    }
}
