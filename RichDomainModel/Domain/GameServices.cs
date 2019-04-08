using System;
using System.Collections.Generic;
using System.Linq;

namespace RichDomainModel.Domain
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
            var game = Game.StartNew(questionRepository, name, categories);
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
            game.AddPlayer(playerName);
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
            var questionToAsk = game.Move(dice, playerId);
            gameRepository.Save(game);
            return questionToAsk?.Question;
        }

        public bool Answer(int gameId, int playerId, string answer)
        {
            var game = gameRepository.Get(gameId);
            var goodAnswer = game.Answer(playerId, answer);
            gameRepository.Save(game);
            return goodAnswer;
        }
    }
}