using System.Collections.Generic;

namespace RichDomainModelWithoutORM.Domain.Events
{
    public struct GameStarted
    {
        public readonly string GameId;
        public readonly string Name;
        public readonly IEnumerable<GameCategory> GameCategories;

        public GameStarted(string gameId, string name, IEnumerable<GameCategory> gameCategories)
        {
            GameId = gameId;
            Name = name;
            GameCategories = gameCategories;
        }
    }
}
