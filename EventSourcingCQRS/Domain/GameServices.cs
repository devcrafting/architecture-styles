using System.Collections.Generic;
using System.Linq;
using EventSourcingCQRS.Domain.Events;

namespace EventSourcingCQRS.Domain
{
    public class GameServices
    {
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

        public string StartNewGame(string name, IEnumerable<string> categories)
        {
            var gameStarted = Game.StartNew(questionRepository, name, categories);
            gameRepository.Save(gameStarted.GameId, gameStarted);
            return gameStarted.GameId;
        }

        public List<Game> GetGames()
        {
            return gameRepository.GetGames();
        }

        public void AddPlayer(string gameId, string playerName)
        {
            var game = gameRepository.Get(gameId);
            var events = game.AddPlayer(playerName);
            gameRepository.Save(gameId, events.ToArray());
        }

        public QuestionAsked Move(string gameId, string playerId)
        {
            var game = gameRepository.Get(gameId);
            var events = game.Move(dice, playerId).ToArray();
            gameRepository.Save(gameId, events);
            return events.OfType<QuestionAsked>().FirstOrDefault();
        }

        public bool Answer(string gameId, string playerId, string answer)
        {
            var game = gameRepository.Get(gameId);
            var events = game.Answer(playerId, answer).ToArray();
            gameRepository.Save(gameId, events);
            // Could give better information to client than just a bool => can return events including if game won
            return events.OfType<GoldCoinEarned>().Any();
        }
    }
}
