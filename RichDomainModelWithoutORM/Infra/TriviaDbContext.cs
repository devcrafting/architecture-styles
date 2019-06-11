using System.Linq;
using Microsoft.EntityFrameworkCore;
using RichDomainModelWithoutORM.Domain;

namespace RichDomainModelWithoutORM.Infra
{
    public class TriviaDbContext : DbContext
    {
        public TriviaDbContext(DbContextOptions<TriviaDbContext> options)
            : base(options)
        { }

        public DbSet<Game> Games { get; set; }
        public DbSet<Question> Question { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var sport = new Category(1, "Sports");
            modelBuilder.Entity<Category>().HasData(sport);

            modelBuilder.Entity<Question>(q =>
                {
                    q.HasData(
                        Enumerable.Range(1, 100)
                            .Select(i => new Question(i, sport.Id, $"Sport {i}", $"sport {i}"))
                    );
                });
        }
    }
}
