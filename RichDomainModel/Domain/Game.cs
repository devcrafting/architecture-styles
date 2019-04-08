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
    }
}