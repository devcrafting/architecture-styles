using System;
using System.Collections.Generic;
using System.Linq;
using RichDomainModelWithoutORM.Domain.Events;

namespace RichDomainModelWithoutORM.Domain
{
    public class Game
    {
        public string Id { get; }
        public string Name { get; }
        public List<Player> Players { get; }
        public Player CurrentPlayer { get; }
        public List<GameCategory> Categories { get; }

        public Game(string id, string name, List<Player> players, Player currentPlayer, List<GameCategory> categories)
        {
            Id = id;
            Name = name;
            Players = players;
            CurrentPlayer = currentPlayer;
            Categories = categories;
        }

        // Factory method
        public static GameStarted StartNew(IQuestionRepository questionRepository, string name, IEnumerable<string> categories)
        {
            if (!categories.Any())
                throw new Exception("You should choose at least one questions' category");

            var gameCategories = (from categoryName in categories
                let questions = questionRepository.GetRandomForCategory(categoryName, 50)
                    .Select(q => new GameQuestion(q, true))
                    .ToList()
                select new GameCategory(categoryName, questions)).ToList();
            return new GameStarted(Guid.NewGuid().ToString(), name, gameCategories);
        }

        public IEnumerable<IDomainEvent> AddPlayer(string playerName)
        {
            var playerId = Guid.NewGuid().ToString();
            yield return new PlayerAdded(playerId, playerName);
            if (!Players.Any())
                yield return new CurrentPlayerChanged(playerId);
        }

        public IEnumerable<IDomainEvent> Move(IRollDice dice, string playerId)
        {
            CheckPlayable();
            CheckPlayerTurn(playerId);
            CurrentPlayer.CheckCanMove();

            var diceRoll = dice.Roll();
            if (CurrentPlayer.CannotGoOutOfPenaltyBox(diceRoll))
            {
                yield return NextPlayerTurn();
            }
            else
            {
                foreach (var @event in CurrentPlayer.Move(diceRoll, Categories)) // Meh!! Missing yield! return from F# :/
                    yield return @event;
            }
        }

        public IEnumerable<IDomainEvent> Answer(string playerId, string answer)
        {
            CheckPlayable();
            CheckPlayerTurn(playerId);
            yield return CurrentPlayer.Answer(answer);
            yield return NextPlayerTurn();
        }

        private void CheckPlayable()
        {
            if (Players.Count < 2)
                throw new Exception($"Game cannot be played with {Players.Count} players, at least 2 required");
        }

        private void CheckPlayerTurn(string playerId)
        {
            if (CurrentPlayer.Id != playerId)
                throw new Exception($"It is not {playerId} turn!");
        }

        private CurrentPlayerChanged NextPlayerTurn() =>
            new CurrentPlayerChanged(Players[(Players.FindIndex(p => p.Id == CurrentPlayer.Id) + 1) % this.Players.Count].Id);
    }
}
