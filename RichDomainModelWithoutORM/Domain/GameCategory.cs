using System.Collections.Generic;

namespace RichDomainModelWithoutORM.Domain
{
    public class GameCategory
    {
        public GameCategory(int id, string name, List<GameQuestion> questions) : this(name, questions)
        {
            Id = id;
        }

        public GameCategory(string name, List<GameQuestion> questions)
        {
            Name = name;
            Questions = questions;
        }

        public int Id { get; }
        public string Name { get; }
        public List<GameQuestion> Questions { get; }
    }
}
