using System.Collections.Generic;
using System.Linq;

namespace RichDomainModelWithoutORM.Domain
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

        public Question Move(string gameId, string playerId)
        {
            var game = gameRepository.Get(gameId);
            var questionToAsk = game.Move(dice, playerId);
            gameRepository.Save(gameId);
            return questionToAsk?.Question;
        }

        public bool Answer(string gameId, string playerId, string answer)
        {
            var game = gameRepository.Get(gameId);
            var goodAnswer = game.Answer(playerId, answer);
            gameRepository.Save(gameId);
            return goodAnswer;
        }
    }
}
