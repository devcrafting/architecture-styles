using System.Collections.Generic;

namespace AnemicDomainModel.Domain
{
    public class Game
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Player> Players { get; set; }
        public Player CurrentPlayer { get; set; }
        public List<GameCategory> Categories { get; set; }
    }
}