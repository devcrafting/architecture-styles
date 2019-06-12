using System;
using System.Collections.Generic;
using System.Linq;
using RichDomainModelWithoutORM.Domain.Events;

namespace RichDomainModelWithoutORM.Domain
{
    public class Game
    {
        private readonly List<Player> _players = new List<Player>();

        public string Id { get; }
        public string Name { get; }
        public IEnumerable<Player> Players => _players;
        public Player CurrentPlayer { get; }
        public QuestionsDeck QuestionDeck { get; }

        public Game(string id, string name, IEnumerable<Player> players, Player currentPlayer, QuestionsDeck questionDeck)
        {
            Id = id;
            Name = name;
            _players.AddRange(players);
            CurrentPlayer = currentPlayer;
            QuestionDeck = questionDeck;
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
            if (!_players.Any())
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
                foreach (var @event in CurrentPlayer.Move(diceRoll, QuestionDeck)) // Meh!! Missing yield! return from F# :/
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
            if (_players.Count < 2)
                throw new Exception($"Game cannot be played with {_players.Count} players, at least 2 required");
        }

        private void CheckPlayerTurn(string playerId)
        {
            if (CurrentPlayer.Id != playerId)
                throw new Exception($"It is not {playerId} turn!");
        }

        private CurrentPlayerChanged NextPlayerTurn() =>
            new CurrentPlayerChanged(_players[(_players.FindIndex(p => p.Id == CurrentPlayer.Id) + 1) % this._players.Count].Id);
    }
}
