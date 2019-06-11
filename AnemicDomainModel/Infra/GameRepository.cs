using System.Collections.Generic;
using System.Linq;
using AnemicDomainModel.Domain;
using Microsoft.EntityFrameworkCore;

namespace AnemicDomainModel.Infra
{
    public class GameRepository : IGameRepository
    {
        private readonly TriviaDbContext dbContext;

        public GameRepository(TriviaDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public Game Get(int gameId)
        {
            return dbContext.Games
                .Include(x => x.Players)
                .Include(x => x.Categories)
                    .ThenInclude(x => x.Questions)
                        .ThenInclude(x => x.Question)
                .Single(x => x.Id == gameId);
        }

        public List<Game> GetGames()
        {
            return dbContext.Games.ToList();
        }

        public void Save(Game game)
        {
            dbContext.Attach(game);
            dbContext.SaveChanges();
        }
    }
}
