using System;
using System.Collections.Generic;
using System.Linq;

namespace AnemicDomainModel.Domain
{
    public class GameServices
    {
        // Repositories are very thin (leaky) abstraction of the ORM in this case, leaky because we rely on:
        // - entities tracking of changes
        // - eager loading defined in GetXXX methods (less impact if we do a real agregate repository)
        private readonly IGameRepository gameRepository;
        private readonly IQuestionRepository questionRepository;
        private readonly IRollDice dice;

        public GameServices(
            IGameRepository gameRepository,
            IQuestionRepository questionRepository,
            IRollDice dice)
        {
            this.gameRepository = gameRepository;
            this.questionRepository = questionRepository;
            this.dice = dice;
        }

        public Game StartNewGame(string name, IEnumerable<string> categories)
        {
            if (!categories.Any())
                throw new Exception("You should choose at least one questions' category");

            var game = new Game { Name = name, Categories = new List<GameCategory>() };
            foreach (var categoryName in categories)
            {
                var questions = questionRepository.GetRandomForCategory(categoryName, 50)
                    .Select(q => new GameQuestion { Question = q, NotUsed = true })
                    .ToList();
                var category = new GameCategory { Name = categoryName, Questions = questions };
                game.Categories.Add(category);
            }
            gameRepository.Save(game);
            return game;
        }

        public List<Game> GetGames()
        {
            return gameRepository.GetGames();
        }

        public void AddPlayer(int gameId, string playerName)
        {
            // NB: we load Game as an agregate root (i.e always with the same eager loading strategy behind Get method)
            // It is clearly not really common in "Anemic Domain Model"-oriented architecture
            // More often there are several GetWithXXX methods on the repository
            var game = gameRepository.Get(gameId);
            var player = new Player
                {
                    Name = playerName,
                    Place = 0,
                    IsInPenaltyBox = false,
                    GoldCoins = 0
                };
            if (!game.Players.Any())
                game.CurrentPlayer = player; 

            game.Players.Add(player);
            // NB: we can just call Save because we know that ORM has tracking
            // Else we would need to call DAO of each object to avoid update of the whole object graph (because we would not know what changed)
            // => much more calls to data access layer, cluttering services
            gameRepository.Save(game);
        }

        public Question Move(int gameId, int playerId)
        {
            // We use Game repository, but it is not rare to see Player repository or GameQuestion repository
            // => accessing directly entities out of the Game agregate
            var game = gameRepository.Get(gameId);
            CheckPlayable(game);
            CheckPlayerTurn(playerId, game);
            if (game.CurrentPlayer.LastQuestion != null)
                throw new Exception("Player already moved, need to answer now");

            var diceRoll = dice.Roll();
            GameQuestion questionToAsk = null;
            if (game.CurrentPlayer.IsInPenaltyBox && diceRoll % 2 == 0)
            {
                NextPlayerTurn(game);
            }
            else
            {
                game.CurrentPlayer.IsInPenaltyBox = false;
                game.CurrentPlayer.Place = (game.CurrentPlayer.Place + diceRoll) % 12;
                questionToAsk = game.Categories[game.CurrentPlayer.Place % game.Categories.Count]
                    .Questions.First(x => x.NotUsed);
                questionToAsk.NotUsed = false;
                game.CurrentPlayer.LastQuestion = questionToAsk.Question;
            }
            gameRepository.Save(game);
            return questionToAsk?.Question;
        }

        public bool Answer(int gameId, int playerId, string answer)
        {
            var game = gameRepository.Get(gameId);
            CheckPlayable(game);
            CheckPlayerTurn(playerId, game);
            var goodAnswer = game.CurrentPlayer.LastQuestion.Answer == answer;
            if (goodAnswer)
                game.CurrentPlayer.GoldCoins++;
            else
                game.CurrentPlayer.IsInPenaltyBox = true;
            game.CurrentPlayer.LastQuestion = null;
            NextPlayerTurn(game);
            gameRepository.Save(game);
            return goodAnswer;
        }

        private static void CheckPlayable(Game game)
        {
            if (game.Players.Count < 2)
                throw new Exception($"Game cannot be played with {game.Players.Count} players, at least 2 required");
        }

        private static void CheckPlayerTurn(int playerId, Game game)
        {
            if (game.CurrentPlayer.Id != playerId)
                throw new Exception($"It is not {playerId} turn!");
        }

        private static void NextPlayerTurn(Game game)
        {
            game.CurrentPlayer = game.Players[(game.Players.IndexOf(game.CurrentPlayer) + 1) % game.Players.Count];
        }
    }
}