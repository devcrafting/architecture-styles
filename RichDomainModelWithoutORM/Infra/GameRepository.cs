using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using Dapper;
using RichDomainModelWithoutORM.Domain;

namespace RichDomainModelWithoutORM.Infra
{
    public class GameRepository : IGameRepository
    {
        private static IDbConnection GetConnection()
        {
            return new SQLiteConnection("Data Source=trivia.db");
        }

        public Game Get(int gameId)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                return connection.Query("SELECT Id, Name, CurrentPlayerId FROM Games WHERE Id=@gameId", new { gameId })
                    .Select(g =>
                        new Game((int)g.Id, (string)g.Name, GetPlayers(connection, gameId),
                            new Player((int)g.CurrentPlayerId, null), GetCategories(connection, gameId)))
                    .Single();
            }
        }

        private List<GameCategory> GetCategories(IDbConnection connection, int gameId) =>
            connection.Query("SELECT gc.Id, Name, gq.NotUsed, gq.QuestionId, q.Text as QuestionText, q.Answer as QuestionAnswer " +
                             "FROM GameCategory gc " +
                             "INNER JOIN GameQuestion gq ON gc.Id = gq.GameCategoryId " +
                             "INNER JOIN Question q ON q.Id = gq.QuestionId " +
                             "WHERE GameId = @gameId", new {gameId})
                .GroupBy(x => ((int) x.Id, (string) x.Name))
                .Select(x =>
                    new GameCategory(x.Key.Item1, x.Key.Item2,
                        x.Select(q =>
                                new GameQuestion(
                                    new Question((int) q.QuestionId, 0, (string) q.QuestionText,
                                        (string) q.QuestionAnswer), q.NotUsed > 0))
                            .ToList()))
                .ToList();

        private List<Player> GetPlayers(IDbConnection connection, int gameId) =>
            connection.Query(
                    "SELECT p.Id, Name, Place, IsInPenaltyBox, GoldCoins, LastQuestionId, Text as LastQuestionText, Answer as LastQuestionAnswer " +
                    "FROM Player p " +
                    "INNER JOIN Question q ON p.LastQuestionId = q.Id " +
                    "WHERE GameId = @gameId",
                    new {gameId})
                .Select(p =>
                    new Player((int) p.Id, (string) p.Name, p.IsInPenaltyBox > 0, (int) p.Place, (int) p.GoldCoins,
                        new Question((int) p.LastQuestionId, 0, (string)p.LastQuestionText, (string)p.LastQuestionAnswer)))
                .ToList();

        public List<Game> GetGames()
        {
            return null;
        }

        public void Save(Game game)
        {
        }
    }
}
