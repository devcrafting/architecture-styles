using System;
using System.Collections.Generic;
using System.Linq;
using NFluent;
using RichDomainModelWithoutORM.Domain;
using RichDomainModelWithoutORM.Domain.Events;
using Xunit;

namespace RichDomainModelWithoutORM.Tests
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
            var game = GetGame();

            var events = game.AddPlayer("player1").ToArray();

            Check.That(events).CountIs(2);
            var playerId = events.OfType<PlayerAdded>().First().PlayerId;
            Check.That(events).ContainsExactly(new PlayerAdded(playerId, "player1"), new CurrentPlayerChanged(playerId));
        }

        [Fact]
        public void AddAnotherPlayerWithoutChangingCurrentPlayer()
        {
            var player1 = new Player("player1");
            var game = GetGame(player1);

            var events = game.AddPlayer("player2").ToArray();

            Check.That(events).CountIs(1);
            var playerId = events.OfType<PlayerAdded>().First().PlayerId;
            Check.That(events).ContainsExactly(new PlayerAdded(playerId, "player2"));
        }

        [Fact]
        public void MoveCurrentPlayer()
        {
            var player1 = new Player("player1");
            var player2 = new Player("player2");
            var game = GetGame(player1, player2);
            var fakeDice = new FakeDice(2);

            var events = game.Move(fakeDice, player1.Id);

            var question = game.QuestionDeck.Categories.First().Questions.First().Question;
            Check.That(events).ContainsExactly(new Moved(player1.Id, 2), new QuestionAsked(question.Id, question.Text));
        }

        [Fact]
        public void FailToMoveCurrentPlayerWhenLessThan2Players()
        {
            var player1 = new Player("player1");
            var game = GetGame(player1);

            Check.ThatCode(() => game.Move(null, player1.Id).ToArray())
                .Throws<Exception>().WithMessage("Game cannot be played with 1 players, at least 2 required");
        }

        [Fact]
        public void FailToMovePlayerWhenNotTheCurrentPlayer()
        {
            var player1 = new Player("1", "player1");
            var player2 = new Player("2", "player2");
            var game = GetGame(player1, player2);
            
            Check.ThatCode(() => game.Move(null, player2.Id).ToArray())
                .Throws<Exception>().WithMessage("It is not 2 turn!");
        }

        [Fact]
        public void FailToMovePlayerWhenLastQuestionNotAnswered()
        {
            var player1 = new Player("1", "player1", false, 0, 0, new Question(1, "some question", "its answer"));
            var player2 = new Player("2", "player2");
            var game = GetGame(player1, player2);
            
            Check.ThatCode(() => game.Move(null, player1.Id).ToArray())
                .Throws<Exception>().WithMessage("Player already moved, need to answer now");
        }

        [Fact]
        public void DoNotMoveCurrentPlayerIfInPenaltyBoxAndRollEvenDice()
        {
            var player1 = new Player("1", "player1", true, 0, 0, null);
            var player2 = new Player("2", "player2");
            var game = GetGame(player1, player2);
            var fakeDice = new FakeDice(2);
            var events = game.Move(fakeDice, player1.Id);

            Check.That(events).ContainsExactly(new CurrentPlayerChanged(player2.Id));
        }

        [Fact]
        public void MoveCurrentPlayerIfInPenaltyBoxButRollOddDice()
        {
            var player1 = new Player("1", "player1", true, 0, 0, null);
            var player2 = new Player("2", "player2");
            var game = GetGame(player1, player2);
            var fakeDice = new FakeDice(3);

            var events = game.Move(fakeDice, player1.Id);

            var question = game.QuestionDeck.Categories.First().Questions.First().Question;
            Check.That(events).ContainsExactly(
                new GetOutOfPenaltyBox(player1.Id), new Moved(player1.Id, 3), new QuestionAsked(question.Id, question.Text));
        }

        [Fact]
        public void AnswerCorrectly()
        {
            var player1 = new Player("1", "player1", false, 0, 0, new Question(1, "some question", "its answer"));
            var player2 = new Player("2", "player2");
            var game = GetGame(player1, player2);

            var events = game.Answer(player1.Id, "its answer");

            Check.That(events).ContainsExactly(new GoldCoinEarned(player1.Id, 1), new CurrentPlayerChanged(player2.Id));
        }

        [Fact]
        public void AnswerIncorrectly()
        {
            var player1 = new Player("1", "player1", false, 0, 0, new Question(1, "some question", "its answer"));
            var player2 = new Player("2", "player2");
            var game = GetGame(player1, player2);

            var events = game.Answer(player1.Id, "bad answer");

            Check.That(events).ContainsExactly(new GoneToPenaltyBox(player1.Id), new CurrentPlayerChanged(player2.Id));
        }

        [Fact]
        public void FailToAnswerWhenNotEnoughPlayers()
        {
            var player1 = new Player("1", "player1");
            var game = GetGame(player1);

            Check.ThatCode(() => game.Answer(player1.Id, "answer").ToArray())
                .Throws<Exception>().WithMessage("Game cannot be played with 1 players, at least 2 required");
        }

        [Fact]
        public void FailToAnswerWhenNotTheCurrentPlayer()
        {
            var player1 = new Player("1", "player1");
            var player2 = new Player("2", "player2");
            var game = GetGame(player1, player2);

            Check.ThatCode(() => game.Answer(player2.Id, "bad answer").ToArray())
                .Throws<Exception>().WithMessage("It is not 2 turn!");
        }

        private static Game GetGame(params Player[] players)
        {
            return new Game(
                "some id", "test", players.ToList(), players.FirstOrDefault(),
               new QuestionsDeck(new List<GameCategory> {
                    new GameCategory(1, "some category", new List<GameQuestion> {
                            new GameQuestion(new Question(1, "some question", "its answer"), true)})})
            );
        }
    }
}
