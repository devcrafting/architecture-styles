using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using EventSourcingCQRS.Domain.Events;

namespace EventSourcingCQRS.Domain
{
    public class Game
    {
        private string _id;
        // Note we don't need Name anymore to take decision : public string Name { get; }
        private readonly List<Player> _players = new List<Player>();
        private Player _currentPlayer;
        private QuestionsDeck _questionsDeck;
        
        public void Apply(GameStarted gameStarted)
        {
            _id = gameStarted.GameId;
            _questionsDeck = new QuestionsDeck(gameStarted.GameCategories);
        }

        public void Apply(PlayerAdded playerAdded)
        {
            _players.Add(new Player(playerAdded.PlayerId, playerAdded.PlayerName));
        }

        public void Apply(CurrentPlayerChanged currentPlayerChanged)
        {
            _currentPlayer = _players.First(x => x.Id == currentPlayerChanged.PlayerId);
        }

        public void Apply(Moved moved)
        {
            ((dynamic)_currentPlayer).Apply((dynamic)moved);
        }

        public void Apply(QuestionAsked questionAsked)
        {
            ((dynamic)_currentPlayer).Apply((dynamic)questionAsked);
        }

        public void Apply(GoneToPenaltyBox goneToPenaltyBox)
        {
            ((dynamic)_currentPlayer).Apply((dynamic)goneToPenaltyBox);
        }

        public Game(IEnumerable<IDomainEvent> history)
        {
            foreach (var @event in history) Apply((dynamic) @event);
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
            _currentPlayer.CheckCanMove();

            var diceRoll = dice.Roll();
            if (_currentPlayer.CannotGoOutOfPenaltyBox(diceRoll))
            {
                yield return NextPlayerTurn();
            }
            else
            {
                foreach (var @event in _currentPlayer.Move(diceRoll, _questionsDeck)) // Meh!! Missing yield! return from F# :/
                    yield return @event;
            }
        }

        public IEnumerable<IDomainEvent> Answer(string playerId, string answer)
        {
            CheckPlayable();
            CheckPlayerTurn(playerId);
            yield return _currentPlayer.Answer(answer);
            yield return NextPlayerTurn();
        }

        private void CheckPlayable()
        {
            if (_players.Count < 2)
                throw new Exception($"Game cannot be played with {_players.Count} players, at least 2 required");
        }

        private void CheckPlayerTurn(string playerId)
        {
            if (_currentPlayer.Id != playerId)
                throw new Exception($"It is not {playerId} turn!");
        }

        private CurrentPlayerChanged NextPlayerTurn() =>
            new CurrentPlayerChanged(_players[(_players.FindIndex(p => p.Id == _currentPlayer.Id) + 1) % _players.Count].Id);
    }
}
