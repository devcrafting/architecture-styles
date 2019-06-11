using System.Linq;
using AnemicDomainModel.Domain;
using Microsoft.EntityFrameworkCore;

namespace AnemicDomainModel.Infra
{
    public class TriviaDbContext : DbContext
    {
        public TriviaDbContext(DbContextOptions<TriviaDbContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Game> Games { get; set; }
        public DbSet<Question> Question { get; set; }
        public DbSet<Player> Players { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Game>().HasMany(game => game.Players).WithOne(player => player.Game);
            var sport = new Category { Id = 1, Name = "Sports" };
            modelBuilder.Entity<Category>().HasData(sport);

            modelBuilder.Entity<Question>(q =>
                {
                    q.HasData(
                        Enumerable.Range(1, 100)
                            .Select(i => new Question { Id = i, CategoryId = sport.Id, Text = $"Sport {i}", Answer = $"sport {i}" })
                    );
                });
        }
    }
}
