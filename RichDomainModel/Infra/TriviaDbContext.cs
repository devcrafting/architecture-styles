using System.Linq;
using RichDomainModel.Domain;
using Microsoft.EntityFrameworkCore;

namespace RichDomainModel.Infra
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