using System;
using System.Collections.Generic;
using System.Linq;

namespace RichDomainModelWithoutORM.Domain
{
    public class Game
    {
        public int Id { get; }
        public string Name { get; }
        public List<Player> Players { get; }
        public Player CurrentPlayer { get; private set; }
        public List<GameCategory> Categories { get; }

        public Game(int id, string name, List<Player> players, Player currentPlayer, List<GameCategory> categories)
        {
            Id = id;
            Name = name;
            Players = players;
            CurrentPlayer = currentPlayer;
            Categories = categories;
        }

        // Factory method
        internal static Game StartNew(IQuestionRepository questionRepository, string name, IEnumerable<string> categories)
        {
            if (!categories.Any())
                throw new Exception("You should choose at least one questions' category");

            var game = new Game(0, name, null, null, new List<GameCategory>());
            foreach (var categoryName in categories)
            {
                var questions = questionRepository.GetRandomForCategory(categoryName, 50)
                    .Select(q => new GameQuestion(q, true))
                    .ToList();
                var category = new GameCategory(categoryName, questions);
                game.Categories.Add(category);
            }
            return game;
        }

        internal void AddPlayer(string playerName)
        {
            var player = new Player(playerName);
            if (!this.Players.Any())
                this.CurrentPlayer = player;

            this.Players.Add(player);
        }

        internal GameQuestion Move(IRollDice dice, int playerId)
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

        internal bool Answer(int playerId, string answer)
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

        private void CheckPlayerTurn(int playerId)
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
