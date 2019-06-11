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
        public Player CurrentPlayer { get; private set; }
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
        internal static GameStarted StartNew(IQuestionRepository questionRepository, string name, IEnumerable<string> categories)
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

        internal IEnumerable<object> AddPlayer(string playerName)
        {
            var playerId = Guid.NewGuid().ToString();
            yield return new PlayerAdded(playerId, playerName);
            if (!Players.Any())
                yield return new CurrentPlayerInitialized(playerId);
        }

        internal GameQuestion Move(IRollDice dice, string playerId)
        {
            CheckPlayable();
            CheckPlayerTurn(playerId);
            CurrentPlayer.CheckCanMove();

            var diceRoll = dice.Roll();
            GameQuestion questionToAsk = null;
            if (CurrentPlayer.CannotGoOutOfPenaltyBox(diceRoll))
            {
                NextPlayerTurn();
            }
            else
            {
                questionToAsk = CurrentPlayer.Move(diceRoll, Categories);
            }
            return questionToAsk;
        }

        internal bool Answer(string playerId, string answer)
        {
            CheckPlayable();
            CheckPlayerTurn(playerId);
            var goodAnswer = CurrentPlayer.Answer(answer);
            NextPlayerTurn();
            return goodAnswer;
        }

        private void CheckPlayable()
        {
            if (this.Players.Count < 2)
                throw new Exception($"Game cannot be played with {this.Players.Count} players, at least 2 required");
        }

        private void CheckPlayerTurn(string playerId)
        {
            if (this.CurrentPlayer.Id != playerId)
                throw new Exception($"It is not {playerId} turn!");
        }

        private void NextPlayerTurn()
        {
            this.CurrentPlayer = this.Players[(this.Players.IndexOf(this.CurrentPlayer) + 1) % this.Players.Count];
        }
    }
}
