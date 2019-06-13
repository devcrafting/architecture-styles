using System.Collections.Generic;
using EventSourcingCQRS.Domain;
using Microsoft.AspNetCore.Mvc;

namespace EventSourcingCQRS.Infra
{
    public class GameController : Controller
    {
        private readonly GameServices gameServices;

        public GameController(GameServices gameServices)
        {
            this.gameServices = gameServices;
        }

        [HttpPost]
        public string NewGame(string name, IEnumerable<string> categories)
        {
            return gameServices.StartNewGame(name, categories);
        }

        public List<Game> Games()
        {
            return gameServices.GetGames();
        }

        [HttpPost]
        public void AddPlayer(string gameId, string playerName)
        {
            gameServices.AddPlayer(gameId, playerName);
        }

        [HttpPost]
        public string Move(string gameId, string playerId)
        {
            return gameServices.Move(gameId, playerId).Text;
        }

        [HttpPost]
        public bool Answer(string gameId, string playerId, string answer)
        {
            return gameServices.Answer(gameId, playerId, answer);
        }
    }
}
