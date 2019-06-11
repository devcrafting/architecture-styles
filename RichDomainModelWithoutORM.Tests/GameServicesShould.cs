using System;
using System.Collections.Generic;
using System.Linq;
using NFluent;
using RichDomainModel.Domain;
using Xunit;

namespace RichDomainModelWithoutORM.Tests
{
    public class GameServicesShould
    {
        [Fact]
        public void StartNewGame()
        {
            var gameRepository = new InMemoryGameRepository();
            var questionsRepository = new FakeQuestionsRepository();
            var gameServices = new GameServices(gameRepository, questionsRepository, null);

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
            var gameServices = new GameServices(gameRepository, questionsRepository, null);

            Check.ThatCode(() => gameServices.StartNewGame("test", new string[] { }))
                .Throws<Exception>();
        }

        [Fact]
        public void AddFirstPlayer()
        {
            var game = GetGame();
            var gameRepository = new InMemoryGameRepository(game);
            var gameServices = new GameServices(gameRepository, null, null);

            gameServices.AddPlayer(game.Id, "player1");

            Check.That(game.Players.Select(x => x.Name)).ContainsExactly("player1");
            Check.That(game.CurrentPlayer.Name).IsEqualTo("player1");
        }

        [Fact]
        public void AddAnotherPlayerWithoutChangingCurrentPlayer()
        {
            var player1 = new Player("player1");
            var game = GetGame(player1);
            var gameRepository = new InMemoryGameRepository(game);
            var gameServices = new GameServices(gameRepository, null, null);

            gameServices.AddPlayer(game.Id, "player2");

            Check.That(game.Players.Select(x => x.Name)).ContainsExactly("player1", "player2");
            Check.That(game.CurrentPlayer).IsEqualTo(player1);
        }

        [Fact]
        public void MoveCurrentPlayer()
        {
            var player1 = new Player("player1");
            var player2 = new Player("player2");
            Game game = GetGame(player1, player2);
            var gameRepository = new InMemoryGameRepository(game);
            var fakeDice = new FakeDice(2);
            var gameServices = new GameServices(gameRepository, null, fakeDice);

            var question = gameServices.Move(game.Id, player1.Id);

            var gameQuestion = game.Categories.Single(x => x.Id == question.CategoryId).Questions.First();
            Check.That(question.Text).IsEqualTo(gameQuestion.Question.Text);
            Check.That(gameQuestion.NotUsed).IsFalse();
            Check.That(game.CurrentPlayer.IsInPenaltyBox).IsFalse();
            Check.That(game.CurrentPlayer.LastQuestion).IsEqualTo(question);
            Check.That(game.CurrentPlayer.Place).IsEqualTo(2);
        }

        [Fact]
        public void FailToMoveCurrentPlayerWhenLessThan2Players()
        {
            var player1 = new Player("player1");
            var game = GetGame(player1);
            var gameRepository = new InMemoryGameRepository(game);
            var gameServices = new GameServices(gameRepository, null, null);

            Check.ThatCode(() => gameServices.Move(game.Id, player1.Id))
                .Throws<Exception>().WithMessage("Game cannot be played with 1 players, at least 2 required");
        }

        [Fact]
        public void FailToMovePlayerWhenNotTheCurrentPlayer()
        {
            var player1 = new Player(1, "player1");
            var player2 = new Player(2, "player2");
            var game = GetGame(player1, player2);
            var gameRepository = new InMemoryGameRepository(game);
            var gameServices = new GameServices(gameRepository, null, null);

            Check.ThatCode(() => gameServices.Move(game.Id, player2.Id))
                .Throws<Exception>().WithMessage("It is not 2 turn!");
        }

        [Fact]
        public void FailToMovePlayerWhenLastQuestionNotAnswered()
        {
            var player1 = new Player(1, "player1", new Question(1, 1, "some question", "its answer"));
            var player2 = new Player(2, "player2");
            var game = GetGame(player1, player2);
            var gameRepository = new InMemoryGameRepository(game);
            var gameServices = new GameServices(gameRepository, null, null);

            Check.ThatCode(() => gameServices.Move(game.Id, player1.Id))
                .Throws<Exception>().WithMessage("Player already moved, need to answer now");
        }

        [Fact]
        public void DoNotMoveCurrentPlayerIfInPenaltyBoxAndRollEvenDice()
        {
            var player1 = new Player(1, "player1", true);
            var player2 = new Player(2, "player2");
            var game = GetGame(player1, player2);
            var gameRepository = new InMemoryGameRepository(game);
            var fakeDice = new FakeDice(2);
            var gameServices = new GameServices(gameRepository, null, fakeDice);

            var question = gameServices.Move(game.Id, player1.Id);

            Check.That(question).IsNull();
            Check.That(game.CurrentPlayer).IsEqualTo(player2);
        }

        [Fact]
        public void MoveCurrentPlayerIfInPenaltyBoxButRollOddDice()
        {
            var player1 = new Player(1, "player1", true);
            var player2 = new Player(2, "player2");
            var game = GetGame(player1, player2);
            var gameRepository = new InMemoryGameRepository(game);
            var fakeDice = new FakeDice(3);
            var gameServices = new GameServices(gameRepository, null, fakeDice);

            var question = gameServices.Move(game.Id, player1.Id);

            var gameQuestion = game.Categories.Single(x => x.Id == question.CategoryId).Questions.First();
            Check.That(question.Text).IsEqualTo(gameQuestion.Question.Text);
            Check.That(gameQuestion.NotUsed).IsFalse();
            Check.That(game.CurrentPlayer.IsInPenaltyBox).IsFalse();
            Check.That(game.CurrentPlayer.LastQuestion).IsEqualTo(question);
            Check.That(game.CurrentPlayer.Place).IsEqualTo(3);
        }

        [Fact]
        public void AnswerCorrectly()
        {
            var player1 = new Player(1, "player1", new Question(1, 1, "some question", "its answer"));
            var player2 = new Player(2, "player2");
            var game = GetGame(player1, player2);
            var gameRepository = new InMemoryGameRepository(game);
            var gameServices = new GameServices(gameRepository, null, null);

            var answer = gameServices.Answer(game.Id, player1.Id, "its answer");

            Check.That(answer).IsTrue();
            Check.That(player1.LastQuestion).IsNull();
            Check.That(player1.GoldCoins).IsEqualTo(1);
            Check.That(player1.IsInPenaltyBox).IsFalse();
            Check.That(game.CurrentPlayer).IsEqualTo(player2);
        }

        [Fact]
        public void AnswerIncorrectly()
        {
            var player1 = new Player(1, "player1", new Question(1, 1, "some question", "its answer"));
            var player2 = new Player(2, "player2");
            var game = GetGame(player1, player2);
            var gameRepository = new InMemoryGameRepository(game);
            var gameServices = new GameServices(gameRepository, null, null);

            var answer = gameServices.Answer(game.Id, player1.Id, "bad answer");

            Check.That(answer).IsFalse();
            Check.That(player1.LastQuestion).IsNull();
            Check.That(player1.GoldCoins).IsEqualTo(0);
            Check.That(player1.IsInPenaltyBox).IsTrue();
            Check.That(game.CurrentPlayer).IsEqualTo(player2);
        }

        [Fact]
        public void FailToAnswerWhenNotEnoughPlayers()
        {
            var player1 = new Player(1, "player1");
            var game = GetGame(player1);
            var gameRepository = new InMemoryGameRepository(game);
            var gameServices = new GameServices(gameRepository, null, null);

            Check.ThatCode(() => gameServices.Answer(game.Id, player1.Id, "answer"))
                .Throws<Exception>().WithMessage("Game cannot be played with 1 players, at least 2 required");
        }

        [Fact]
        public void FailToAnswerWhenNotTheCurrentPlayer()
        {
            var player1 = new Player(1, "player1");
            var player2 = new Player(2, "player2");
            var game = GetGame(player1, player2);
            var gameRepository = new InMemoryGameRepository(game);
            var gameServices = new GameServices(gameRepository, null, null);

            Check.ThatCode(() => gameServices.Answer(game.Id, player2.Id, "bad answer"))
                .Throws<Exception>().WithMessage("It is not 2 turn!");
        }

        private static Game GetGame(params Player[] players)
        {
            return new Game(
                1, "test", players.ToList(), players.FirstOrDefault(),
                new List<GameCategory> {
                    new GameCategory(1, "some category", new List<GameQuestion> {
                            new GameQuestion(new Question(1, 1, "some question", "its answer"), true)})}
            );
        }
    }
}
