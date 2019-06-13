using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using EventSourcingCQRS.Domain.Events;

namespace EventSourcingCQRS.Domain
{
    public class Game
    {
        private class State
        {
            public string Id { get; private set; }
            // Note we don't need Name anymore to take decision : public string Name { get; }
            public readonly List<Player> Players = new List<Player>();
            public Player CurrentPlayer { get; private set; }
            public QuestionsDeck QuestionsDeck { get; private set; }
            
            public void Apply(GameStarted gameStarted)
            {
                Id = gameStarted.GameId;
                QuestionsDeck = new QuestionsDeck(gameStarted.GameCategories);
            }

            public void Apply(PlayerAdded playerAdded)
            {
                Players.Add(new Player(playerAdded.PlayerId, playerAdded.PlayerName));
            }
        }

        private readonly State _state = new State();
        
        public Game(IEnumerable<IDomainEvent> history)
        {
            var state = (dynamic) _state;
            foreach (var @event in history) state.Apply((dynamic) @event);
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
            if (!_state.Players.Any())
                yield return new CurrentPlayerChanged(playerId);
        }

        public IEnumerable<IDomainEvent> Move(IRollDice dice, string playerId)
        {
            CheckPlayable();
            CheckPlayerTurn(playerId);
            _state.CurrentPlayer.CheckCanMove();

            var diceRoll = dice.Roll();
            if (_state.CurrentPlayer.CannotGoOutOfPenaltyBox(diceRoll))
            {
                yield return NextPlayerTurn();
            }
            else
            {
                foreach (var @event in _state.CurrentPlayer.Move(diceRoll, _state.QuestionsDeck)) // Meh!! Missing yield! return from F# :/
                    yield return @event;
            }
        }

        public IEnumerable<IDomainEvent> Answer(string playerId, string answer)
        {
            CheckPlayable();
            CheckPlayerTurn(playerId);
            yield return _state.CurrentPlayer.Answer(answer);
            yield return NextPlayerTurn();
        }

        private void CheckPlayable()
        {
            if (_state.Players.Count < 2)
                throw new Exception($"Game cannot be played with {_state.Players.Count} players, at least 2 required");
        }

        private void CheckPlayerTurn(string playerId)
        {
            if (_state.CurrentPlayer.Id != playerId)
                throw new Exception($"It is not {playerId} turn!");
        }

        private CurrentPlayerChanged NextPlayerTurn() =>
            new CurrentPlayerChanged(_state.Players[(_state.Players.FindIndex(p => p.Id == _state.CurrentPlayer.Id) + 1) % _state.Players.Count].Id);
    }
}
