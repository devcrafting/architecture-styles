using System.Collections.Generic;

namespace EventSourcingCQRS.Domain
{
    public interface IGameRepository
    {
        void Save(string gameId, params object[] events);
        List<Game> GetGames();
        Game Get(string gameId);
    }
}
