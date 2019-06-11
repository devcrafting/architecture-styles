namespace RichDomainModel.Domain
{
    public class Question
    {
        public Question(int id, int categoryId, string text, string answer)
        {
            Id = id;
            CategoryId = categoryId;
            Text = text;
            Answer = answer;
        }

        public int Id { get; }
        public string Text { get; }
        public string Answer { get; }
        public int CategoryId { get; }
        public Category Category { get; }
    }
}
