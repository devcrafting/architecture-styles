using System.Collections.Generic;

namespace EventSourcingCQRS.Domain
{
    public class GameCategory
    {
        public GameCategory(string name, IEnumerable<GameQuestion> questions)
        {
            Name = name;
            Questions = questions;
        }
        
        public string Name { get; }
        public IEnumerable<GameQuestion> Questions { get; }
    }
}
