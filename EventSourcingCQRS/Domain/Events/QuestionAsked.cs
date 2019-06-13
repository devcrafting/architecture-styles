namespace EventSourcingCQRS.Domain.Events
{
    public struct QuestionAsked : IDomainEvent
    {
        public readonly string Answer;
        public readonly int QuestionId;
        public readonly string Text;

        public QuestionAsked(int questionId, string text, string answer)
        {
            Answer = answer;
            QuestionId = questionId;
            Text = text;
        }
    }
}
