namespace AnemicDomainModel.Domain
{
    public class Player
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Place { get; set; }
        public bool IsInPenaltyBox { get; set; }
        public int GoldCoins { get; set; }
        public Question LastQuestion { get; set; }
    }
}