using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using Dapper;
using RichDomainModelWithoutORM.Domain;
using RichDomainModelWithoutORM.Domain.Events;

namespace RichDomainModelWithoutORM.Infra
{
    public class GameRepository : IGameRepository
    {
        private static IDbConnection GetConnection()
        {
            return new SQLiteConnection("Data Source=trivia.db");
        }

        public Game Get(string gameId)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                return connection.Query("SELECT Id, Name, CurrentPlayerId FROM Games WHERE Id=@gameId", new { gameId })
                    .Select(g =>
                        new Game((string)g.Id, (string)g.Name, GetPlayers(connection, gameId),
                            new Player((string)g.CurrentPlayerId, null), GetCategories(connection, gameId)))
                    .Single();
            }
        }

        private List<GameCategory> GetCategories(IDbConnection connection, string gameId) =>
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

        private List<Player> GetPlayers(IDbConnection connection, string gameId) =>
            connection.Query(
                    "SELECT p.Id, Name, Place, IsInPenaltyBox, GoldCoins, LastQuestionId, Text as LastQuestionText, Answer as LastQuestionAnswer " +
                    "FROM Player p " +
                    "INNER JOIN Question q ON p.LastQuestionId = q.Id " +
                    "WHERE GameId = @gameId",
                    new {gameId})
                .Select(p =>
                    new Player((string) p.Id, (string) p.Name, p.IsInPenaltyBox > 0, (int) p.Place, (int) p.GoldCoins,
                        new Question((int) p.LastQuestionId, 0, (string)p.LastQuestionText, (string)p.LastQuestionAnswer)))
                .ToList();

        public List<Game> GetGames()
        {
            return null;
        }

        public void Save(string gameId, params object[] events)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    foreach (var @event in events)
                    {
                        Save(connection, gameId, (dynamic) @event);
                    }
                    transaction.Commit();
                }
            }
        }

        private void Save(IDbConnection connection, string gameId, object unknownEvent) { }

        private void Save(IDbConnection connection, string gameId, GameStarted gameStarted)
        {
            connection.Execute("INSERT INTO Games(Id, Name) VALUES (@gameId, @name)",
                new {gameId, name = gameStarted.Name});
            foreach (var gameCategory in gameStarted.GameCategories)
            {
                var gameCategoryId = connection.ExecuteScalar("INSERT INTO GameCategory(Name, GameId) VALUES (@gameId, @name);select last_insert_rowid()",
                    new {gameId, name = gameCategory.Name});
                foreach (var gameQuestion in gameCategory.Questions)
                {
                    connection.Execute(
                        "INSERT INTO GameQuestion(QuestionId, NotUsed, GameCategoryId) VALUES (@questionId, @notUsed, @gameCategoryId)",
                        new {questionId = gameQuestion.Question.Id, gameQuestion.NotUsed, gameCategoryId});
                }
            }
        }

        private void Save(IDbConnection connection, string gameId, PlayerAdded playerAdded)
        {
            connection.Execute("INSERT INTO Player(Id, Name, Place, IsInPenaltyBox, GoldCoins, GameId) " +
                               "VALUES (@playerId, @playerName, @place, @isInPenaltyBox, @goldCoins, @gameId)",
                new {playerAdded.PlayerId, playerAdded.PlayerName, playerAdded.Place, playerAdded.IsInPenaltyBox, playerAdded.GoldCoins, gameId});
        }

        private void Save(IDbConnection connection, string gameId, CurrentPlayerInitialized currentPlayerInitialized)
        {
            connection.Execute("UPDATE Games SET CurrentPlayerId = @playerId WHERE Id = @gameId",
                new {gameId, currentPlayerInitialized.PlayerId});
        }
    }
}
