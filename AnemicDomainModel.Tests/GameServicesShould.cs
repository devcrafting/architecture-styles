using System;
using System.Collections.Generic;
using System.Linq;
using AnemicDomainModel.Domain;
using NFluent;
using Xunit;

namespace AnemicDomainModel.Tests
{
    public class GameServicesShould
    {
        [Fact]
        public void StartNewGame()
        {
            var gameRepository = new InMemoryGameRepository();
            var questionsRepository = new FakeQuestionsRepository();
            var gameServices = new GameServices(gameRepository, questionsRepository);

            var game = gameServices.StartNewGame("test", new [] { "Sports", "Science" });

            Check.That(game.Name).IsEqualTo("test");
            Check.That(game.Categories.Select(x => (x.Name, x.Questions.Count)))
                .ContainsExactly(("Sports", 50), ("Science", 50));
            Check.That(game.Players).IsNull();
            Check.That(game.CurrentPlayer).IsNull();
        }

        [Fact]
        public void FailToStartNewGameWithoutCategories()
        {
            var gameRepository = new InMemoryGameRepository();
            var questionsRepository = new FakeQuestionsRepository();
            var gameServices = new GameServices(gameRepository, questionsRepository);

            Check.ThatCode(() => gameServices.StartNewGame("test", new string[] { }))
                .Throws<Exception>();
        }
    }
}