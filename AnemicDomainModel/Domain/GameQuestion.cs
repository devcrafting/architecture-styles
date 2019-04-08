namespace AnemicDomainModel.Domain
{
    public class GameQuestion
    {
        public int Id { get; set; }
        public Question Question { get; set; }
        public bool NotUsed { get; set; }
    }
}