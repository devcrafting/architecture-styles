using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using RichDomainModelWithoutORM.Domain;

namespace RichDomainModelWithoutORM.Infra
{
    public class GameController : Controller
    {
        private readonly GameServices gameServices;

        public GameController(GameServices gameServices)
        {
            this.gameServices = gameServices;
        }

        [HttpPost]
        public int NewGame(string name, IEnumerable<string> categories)
        {
            return gameServices.StartNewGame(name, categories).Id;
        }

        public List<Game> Games()
        {
            return gameServices.GetGames();
        }

        [HttpPost]
        public void AddPlayer(int gameId, string playerName)
        {
            gameServices.AddPlayer(gameId, playerName);
        }

        [HttpPost]
        public string Move(int gameId, int playerId)
        {
            return gameServices.Move(gameId, playerId).Text;
        }

        [HttpPost]
        public bool Answer(int gameId, int playerId, string answer)
        {
            return gameServices.Answer(gameId, playerId, answer);
        }
    }
}