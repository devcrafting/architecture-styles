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

        [Fact]
        public void AddFirstPlayer()
        {
            var game = new Game {
                Id = 1,
                Name = "test",
                Players = new List<Player>(),
            };
            var gameRepository = new InMemoryGameRepository(game);
            var gameServices = new GameServices(gameRepository, null);

            gameServices.AddPlayer(game.Id, "player1");

            Check.That(game.Players.Select(x => x.Name)).ContainsExactly("player1");
            Check.That(game.CurrentPlayer.Name).IsEqualTo("player1");
        }

        [Fact]
        public void AddAnotherPlayerWithoutChangingCurrentPlayer()
        {
            var player1 = new Player { Name = "player1" };
            var game = new Game
            {
                Id = 1,
                Name = "test",
                Players = new List<Player> { player1 },
                CurrentPlayer = player1
            };
            var gameRepository = new InMemoryGameRepository(game);
            var gameServices = new GameServices(gameRepository, null);

            gameServices.AddPlayer(game.Id, "player2");

            Check.That(game.Players.Select(x => x.Name)).ContainsExactly("player1", "player2");
            Check.That(game.CurrentPlayer).IsEqualTo(player1);
        }

        [Fact]
        public void MoveCurrentPlayer()
        {
            var player1 = new Player { Name = "player1" };
            var player2 = new Player { Name = "player2" };
            var game = new Game
            {
                Id = 1,
                Name = "test",
                Players = new List<Player> { player1, player2 },
                CurrentPlayer = player1,
                Categories = new List<GameCategory> {
                    new GameCategory {
                        Id = 1,
                        Questions = new List<GameQuestion> {
                            new GameQuestion { NotUsed = true, Question = new Question { CategoryId = 1, Text = "test question"} } } }}
            };
            var gameRepository = new InMemoryGameRepository(game);
            var gameServices = new GameServices(gameRepository, null);

            var question = gameServices.Move(game.Id, player1.Id);

            var gameQuestion = game.Categories.Single(x => x.Id == question.CategoryId).Questions.First();
            Check.That(question.Text).IsEqualTo(gameQuestion.Question.Text);
            Check.That(gameQuestion.NotUsed).IsFalse();
            Check.That(game.CurrentPlayer.IsInPenaltyBox).IsFalse();
            Check.That(game.CurrentPlayer.LastQuestion).IsEqualTo(question);
            Check.That(game.CurrentPlayer.Place).IsNotEqualTo(0);
        }

        [Fact]
        public void FailToMoveCurrentPlayerWhenLessThan2Players()
        {
            var player1 = new Player { Name = "player1" };
            var game = new Game
            {
                Id = 1,
                Name = "test",
                Players = new List<Player> { player1 }
            };
            var gameRepository = new InMemoryGameRepository(game);
            var gameServices = new GameServices(gameRepository, null);

            Check.ThatCode(() => gameServices.Move(game.Id, player1.Id))
                .Throws<Exception>().WithMessage("Game cannot be played with 1 players, at least 2 required");
        }

        [Fact]
        public void FailToMovePlayerWhenNotTheCurrentPlayer()
        {
            var player1 = new Player { Id = 1, Name = "player1" };
            var player2 = new Player { Id = 2, Name = "player2" };
            var game = new Game
            {
                Id = 1,
                Name = "test",
                Players = new List<Player> { player1, player2 },
                CurrentPlayer = player1
            };
            var gameRepository = new InMemoryGameRepository(game);
            var gameServices = new GameServices(gameRepository, null);

            Check.ThatCode(() => gameServices.Move(game.Id, player2.Id))
                .Throws<Exception>().WithMessage("It is not 2 turn!");
        }

        [Fact]
        public void FailToMovePlayerWhenLastQuestionNotAnswered()
        {
            var player1 = new Player { Id = 1, Name = "player1", LastQuestion = new Question() };
            var player2 = new Player { Id = 2, Name = "player2" };
            var game = new Game
            {
                Id = 1,
                Name = "test",
                Players = new List<Player> { player1, player2 },
                CurrentPlayer = player1
            };
            var gameRepository = new InMemoryGameRepository(game);
            var gameServices = new GameServices(gameRepository, null);

            Check.ThatCode(() => gameServices.Move(game.Id, player1.Id))
                .Throws<Exception>().WithMessage("Player already moved, need to answer now");
        }
    }
}