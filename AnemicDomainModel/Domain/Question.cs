namespace AnemicDomainModel.Domain
{
    public class Question
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string Answer { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}