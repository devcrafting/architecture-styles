using System;
using System.Collections.Generic;
using System.Linq;
using EventSourcingCQRS.Domain;
using EventSourcingCQRS.Domain.Events;
using NFluent;
using Xunit;

namespace EventSourcingCQRS.Tests
{
    public class GameShould
    {
        [Fact]
        public void StartNewGame()
        {
            var questionsRepository = new FakeQuestionsRepository();

            var gameStarted = Game.StartNew(questionsRepository, "test", new [] { "Sports", "Science" });
            
            Check.That(gameStarted.Name).IsEqualTo("test");
            Check.That(gameStarted.GameCategories.Select(x => (x.Name, x.Questions.Count)))
                .ContainsExactly(("Sports", 50), ("Science", 50));
        }

        [Fact]
        public void FailToStartNewGameWithoutCategories()
        {
            var questionsRepository = new FakeQuestionsRepository();

            Check.ThatCode(() => Game.StartNew(questionsRepository, "test", new string[] { }))
                .Throws<Exception>();
        }

        [Fact]
        public void AddFirstPlayer()
        {
            var game = GetGame(new GameStarted("gameId", "name", GameCategories));

            var events = game.AddPlayer("player1").ToArray();

            Check.That(events).CountIs(2);
            var playerId = events.OfType<PlayerAdded>().First().PlayerId;
            Check.That(events).ContainsExactly(new PlayerAdded(playerId, "player1"), new CurrentPlayerChanged(playerId));
        }

        [Fact]
        public void AddAnotherPlayerWithoutChangingCurrentPlayer()
        {
            var game = GetGame(
                new GameStarted("gameId", "name", GameCategories),
                new PlayerAdded("p1Id", "player1"));

            var events = game.AddPlayer("player2").ToArray();

            Check.That(events).CountIs(1);
            var playerId = events.OfType<PlayerAdded>().First().PlayerId;
            Check.That(events).ContainsExactly(new PlayerAdded(playerId, "player2"));
        }

        [Fact]
        public void MoveCurrentPlayer()
        {
            const string playerId = "p1Id";
            var game = GetGame(
                new GameStarted("gameId", "name", GameCategories),
                new PlayerAdded(playerId, "player1"),
                new CurrentPlayerChanged(playerId),
                new PlayerAdded("p2Id", "player2"));
            var fakeDice = new FakeDice(2);

            var events = game.Move(fakeDice, playerId);

            var question = GameCategories.First().Questions.First().Question;
            Check.That(events).ContainsExactly(
                new Moved(playerId, 2),
                new QuestionAsked(question.Id, question.Text, question.Answer));
        }

        [Fact]
        public void FailToMoveCurrentPlayerWhenLessThan2Players()
        {
            const string playerId = "p1Id";
            var game = GetGame(
                new GameStarted("gameId", "name", GameCategories),
                new PlayerAdded(playerId, "player1"));

            Check.ThatCode(() => game.Move(null, playerId).ToArray())
                .Throws<Exception>().WithMessage("Game cannot be played with 1 players, at least 2 required");
        }

        [Fact]
        public void FailToMovePlayerWhenNotTheCurrentPlayer()
        {
            const string player1Id = "1";
            const string player2Id = "2";
            var game = GetGame(
                new GameStarted("gameId", "name", GameCategories),
                new PlayerAdded(player1Id, "player1"),
                new CurrentPlayerChanged(player1Id),
                new PlayerAdded(player2Id, "player2"));

            Check.ThatCode(() => game.Move(null, player2Id).ToArray())
                .Throws<Exception>().WithMessage("It is not 2 turn!");
        }

        [Fact]
        public void FailToMovePlayerWhenLastQuestionNotAnswered()
        {
            const string playerId = "p1Id";
            var game = GetGame(
                new GameStarted("gameId", "name", GameCategories),
                new PlayerAdded(playerId, "player1"),
                new CurrentPlayerChanged(playerId),
                new PlayerAdded("p2Id", "player2"),
                new Moved(playerId, 2),
                new QuestionAsked(1, "question", "answer"));

            Check.ThatCode(() => game.Move(null, playerId).ToArray())
                .Throws<Exception>().WithMessage("Player already moved, need to answer now");
        }

        [Fact]
        public void DoNotMoveCurrentPlayerIfInPenaltyBoxAndRollEvenDice()
        {
            const string player1Id = "p1Id";
            const string player2Id = "p2Id";
            var game = GetGame(
                new GameStarted("gameId", "name", GameCategories),
                new PlayerAdded(player1Id, "player1"),
                new CurrentPlayerChanged(player1Id),
                new PlayerAdded(player2Id, "player2"),
                new Moved(player1Id, 2),
                new QuestionAsked(1, "question", "answer"),
                new GoneToPenaltyBox(player1Id));
            var fakeDice = new FakeDice(2);
            var events = game.Move(fakeDice, player1Id);

            Check.That(events).ContainsExactly(new CurrentPlayerChanged(player2Id));
        }

        [Fact]
        public void MoveCurrentPlayerIfInPenaltyBoxButRollOddDice()
        {
            const string player1Id = "p1Id";
            const string player2Id = "p2Id";
            var game = GetGame(
                new GameStarted("gameId", "name", GameCategories),
                new PlayerAdded(player1Id, "player1"),
                new CurrentPlayerChanged(player1Id),
                new PlayerAdded(player2Id, "player2"),
                new Moved(player1Id, 2),
                new QuestionAsked(1, "question", "answer"),
                new GoneToPenaltyBox(player1Id)
                /* Bypass Player2 turn */);
            var fakeDice = new FakeDice(3);

            var events = game.Move(fakeDice, player1Id);

            var question = GameCategories.First().Questions.First().Question;
            Check.That(events).ContainsExactly(
                new GetOutOfPenaltyBox(player1Id),
                new Moved(player1Id, 5),
                new QuestionAsked(question.Id, question.Text, question.Answer));
        }

        [Fact]
        public void AnswerCorrectly()
        {
            const string player1Id = "p1Id";
            const string player2Id = "p2Id";
            var game = GetGame(
                new GameStarted("gameId", "name", GameCategories),
                new PlayerAdded(player1Id, "player1"),
                new CurrentPlayerChanged(player1Id),
                new PlayerAdded(player2Id, "player2"),
                new Moved(player1Id, 2),
                new QuestionAsked(1, "question", "answer"));

            var events = game.Answer(player1Id, "answer");

            Check.That(events).ContainsExactly(
                new GoldCoinEarned(player1Id, 1),
                new CurrentPlayerChanged(player2Id));
        }

        [Fact]
        public void AnswerIncorrectly()
        {
            const string player1Id = "p1Id";
            const string player2Id = "p2Id";
            var game = GetGame(
                new GameStarted("gameId", "name", GameCategories),
                new PlayerAdded(player1Id, "player1"),
                new CurrentPlayerChanged(player1Id),
                new PlayerAdded(player2Id, "player2"),
                new Moved(player1Id, 2),
                new QuestionAsked(1, "question", "answer"));

            var events = game.Answer(player1Id, "bad answer");

            Check.That(events).ContainsExactly(
                new GoneToPenaltyBox(player1Id),
                new CurrentPlayerChanged(player2Id));
        }

        [Fact]
        public void FailToAnswerWhenNotEnoughPlayers()
        {
            const string playerId = "p1Id";
            var game = GetGame(
                new GameStarted("gameId", "name", GameCategories),
                new PlayerAdded(playerId, "player1"));

            Check.ThatCode(() => game.Answer(playerId, "answer").ToArray())
                .Throws<Exception>().WithMessage("Game cannot be played with 1 players, at least 2 required");
        }

        [Fact]
        public void FailToAnswerWhenNotTheCurrentPlayer()
        {
            const string player1Id = "p1Id";
            const string player2Id = "p2Id";
            var game = GetGame(
                new GameStarted("gameId", "name", GameCategories),
                new PlayerAdded(player1Id, "player1"),
                new CurrentPlayerChanged(player1Id),
                new PlayerAdded("p2Id", "player2"),
                new Moved(player1Id, 2),
                new QuestionAsked(1, "question", "answer"));

            Check.ThatCode(() => game.Answer(player2Id, "bad answer").ToArray())
                .Throws<Exception>().WithMessage("It is not p2Id turn!");
        }

        private static Game GetGame(params IDomainEvent[] pastEvents) => new Game(pastEvents);

        private List<GameCategory> GameCategories => new List<GameCategory>()
        {
            new GameCategory("sports", new List<GameQuestion>()
            {
                new GameQuestion(new Question(1, "question", "answer"), true)
            })
        };
    }
}
