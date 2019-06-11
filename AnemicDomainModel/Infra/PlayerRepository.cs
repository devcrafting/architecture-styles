using System.Linq;
using AnemicDomainModel.Domain;
using Microsoft.EntityFrameworkCore;

namespace AnemicDomainModel.Infra
{
    public class PlayerRepository : IPlayerRepository
    {
        private readonly TriviaDbContext dbContext;

        public PlayerRepository(TriviaDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public Player GetCurrentPlayer(int gameId) =>
            dbContext.Players
                .Include(p => p.LastQuestion)
                .Single(p => p.Game.Id == gameId && p.Game.CurrentPlayer.Id == p.Id);
    }
}
