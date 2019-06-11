using System.Collections.Generic;

namespace RichDomainModelWithoutORM.Domain
{
    public interface IGameRepository
    {
        void Save(Game game);
        List<Game> GetGames();
        Game Get(int gameId);
    }
}
