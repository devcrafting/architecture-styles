using System.Collections.Generic;
using AnemicDomainModel.Domain;
using Microsoft.AspNetCore.Mvc;

namespace AnemicDomainModel.Infra
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
            return gameServices.NewGame(name, categories);
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
            return gameServices.Move(gameId, playerId);
        }

        [HttpPost]
        public bool Answer(int gameId, int playerId, string answer)
        {
            return gameServices.Answer(gameId, playerId, answer);
        }
    }
}