namespace RichDomainModelWithoutORM.Domain
{
    public class Question
    {
        public Question(int id, string text, string answer)
        {
            Id = id;
            Text = text;
            Answer = answer;
        }

        public int Id { get; }
        public string Text { get; }
        public string Answer { get; }
    }
}
