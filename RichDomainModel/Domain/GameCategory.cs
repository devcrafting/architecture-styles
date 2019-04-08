using System.Collections.Generic;

namespace RichDomainModel.Domain
{
    public class GameCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<GameQuestion> Questions { get; set; }
    }
}