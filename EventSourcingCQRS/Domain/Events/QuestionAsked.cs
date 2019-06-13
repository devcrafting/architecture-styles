namespace EventSourcingCQRS.Domain.Events
{
    public struct QuestionAsked : IDomainEvent
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
