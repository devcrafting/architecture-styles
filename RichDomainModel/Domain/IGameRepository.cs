using System.Collections.Generic;

namespace RichDomainModel.Domain
{
    public interface IGameRepository
    {
        void Save(Game game);
        List<Game> GetGames();
        Game Get(int gameId);
    }
}