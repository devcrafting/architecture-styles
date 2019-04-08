using System;
using System.Collections.Generic;
using System.Linq;

namespace RichDomainModel.Domain
{
    public class Game
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public List<Player> Players { get; private set; }
        public Player CurrentPlayer { get; set; }
        public List<GameCategory> Categories { get; private set; }

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
                    .Select(q => new GameQuestion { Question = q, NotUsed = true })
                    .ToList();
                var category = new GameCategory { Name = categoryName, Questions = questions };
                game.Categories.Add(category);
            }
            return game;
        }

        internal void AddPlayer(string playerName)
        {
            var player = new Player
                {
                    Name = playerName,
                    Place = 0,
                    IsInPenaltyBox = false,
                    GoldCoins = 0
                };
            if (!this.Players.Any())
                this.CurrentPlayer = player; 

            this.Players.Add(player);
        }

        internal GameQuestion Move(IRollDice dice, int playerId)
        {
            CheckPlayable();
            CheckPlayerTurn(playerId);
            if (this.CurrentPlayer.LastQuestion != null)
                throw new Exception("Player already moved, need to answer now");

            var diceRoll = dice.Roll();
            GameQuestion questionToAsk = null;
            if (this.CurrentPlayer.IsInPenaltyBox && diceRoll % 2 == 0)
            {
                NextPlayerTurn();
            }
            else
            {
                this.CurrentPlayer.IsInPenaltyBox = false;
                this.CurrentPlayer.Place = (this.CurrentPlayer.Place + diceRoll) % 12;
                questionToAsk = this.Categories[this.CurrentPlayer.Place % this.Categories.Count]
                    .Questions.First(x => x.NotUsed);
                questionToAsk.NotUsed = false;
                this.CurrentPlayer.LastQuestion = questionToAsk.Question;
            }
            return questionToAsk;
        }

        internal bool Answer(int playerId, string answer)
        {
            CheckPlayable();
            CheckPlayerTurn(playerId);
            var goodAnswer = this.CurrentPlayer.LastQuestion.Answer == answer;
            if (goodAnswer)
                this.CurrentPlayer.GoldCoins++;
            else
                this.CurrentPlayer.IsInPenaltyBox = true;
            this.CurrentPlayer.LastQuestion = null;
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