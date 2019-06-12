using System.Collections.Generic;
using System.Linq;
using RichDomainModelWithoutORM.Domain;

namespace RichDomainModelWithoutORM.Tests
{
    internal class InMemoryGameRepository : IGameRepository
    {
        public Game LastGameSaved;

        private List<Game> _games = new List<Game>();

        public InMemoryGameRepository(params Game[] games)
        {
            _games.AddRange(games);
        }

        public Game Get(string gameId) =>
            _games.Single(x => x.Id == gameId);

        public List<Game> GetGames()
        {
            throw new System.NotImplementedException();
        }

        public void Save(string gameId, params object[] events)
        {
        }
    }
}
