using System.Collections.Generic;

namespace AnemicDomainModel.Domain
{
    public interface IGameRepository
    {
        void Save(Game game);
        List<Game> GetGames();
        Game Get(int gameId);
    }
}
