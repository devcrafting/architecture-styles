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
                return connection.Query("SELECT Id, Name, CurrentPlayerId FROM Games WHERE Id=@gameId", new {gameId})
                    .Select(g =>
                    {
                        var players = GetPlayers(connection, gameId);
                        var currentPlayer = players.Single(p => p.Id == (string) g.CurrentPlayerId);
                        return new Game((string) g.Id, (string) g.Name, players,
                            currentPlayer, GetCategories(connection, gameId));
                    })
                    .Single();
            }
        }

        private List<GameCategory> GetCategories(IDbConnection connection, string gameId) =>
            connection.Query(
                    "SELECT gc.Id, Name, gq.NotUsed, gq.QuestionId, q.Text as QuestionText, q.Answer as QuestionAnswer " +
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
                    "LEFT JOIN Question q ON p.LastQuestionId = q.Id " +
                    "WHERE GameId = @gameId",
                    new {gameId})
                .Select(p =>
                    new Player((string) p.Id, (string) p.Name, p.IsInPenaltyBox > 0, (int) p.Place, (int) p.GoldCoins,
                        p.LastQuestionId == null
                            ? null
                            : new Question((int) p.LastQuestionId, 0, (string) p.LastQuestionText,
                                (string) p.LastQuestionAnswer)))
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

        private void Save(IDbConnection connection, string gameId, object unknownEvent)
        {
        }

        private void Save(IDbConnection connection, string gameId, GameStarted gameStarted)
        {
            connection.Execute("INSERT INTO Games(Id, Name) VALUES (@gameId, @name)",
                new {gameId, name = gameStarted.Name});
            foreach (var gameCategory in gameStarted.GameCategories)
            {
                var gameCategoryId = connection.ExecuteScalar(
                    "INSERT INTO GameCategory(Name, GameId) VALUES (@name, @gameId);select last_insert_rowid()",
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
                new
                {
                    playerAdded.PlayerId, playerAdded.PlayerName, playerAdded.Place, playerAdded.IsInPenaltyBox,
                    playerAdded.GoldCoins, gameId
                });
        }

        private void Save(IDbConnection connection, string gameId, CurrentPlayerChanged currentPlayerChanged)
        {
            connection.Execute("UPDATE Games SET CurrentPlayerId = @playerId WHERE Id = @gameId",
                new {gameId, currentPlayerChanged.PlayerId});
        }

        private void Save(IDbConnection connection, string gameId, GetOutOfPenaltyBox getOutOfPenaltyBox)
        {
            connection.Execute("UPDATE Player SET IsInPenaltyBox = 0 WHERE Id = @playerID",
                new {getOutOfPenaltyBox.PlayerId});
        }

        private void Save(IDbConnection connection, string gameId, Moved moved)
        {
            connection.Execute("UPDATE Player SET Place = @newPlace WHERE Id = @playerId",
                new {moved.PlayerId, moved.NewPlace});
        }

        private void Save(IDbConnection connection, string gameId, QuestionAsked questionAsked)
        {
            connection.Execute("UPDATE Player AS p SET LastQuestionId = @questionId " +
                               "WHERE EXISTS(" +
                               "SELECT * FROM Games g " +
                               "WHERE g.Id = @gameId AND g.CurrentPlayerId = p.Id)",
                new {gameId, questionAsked.QuestionId});
            connection.Execute("UPDATE GameQuestion AS gq SET NotUsed = 0 " +
                               "WHERE QuestionId = @questionId AND EXISTS(" +
                               "SELECT * FROM GameCategory gc " +
                               "WHERE GameId = @gameId AND gc.Id = gq.GameCategoryId)",
                new {gameId, questionAsked.QuestionId});
        }

        private void Save(IDbConnection connection, string gameId, GoldCoinEarned goldCoinEarned)
        {
            connection.Execute("UPDATE Player SET GoldCoins = @goldCoins, LastQuestionId = null WHERE Id = @playerId",
                new {goldCoinEarned.PlayerId, goldCoinEarned.GoldCoins});
        }

        private void Save(IDbConnection connection, string gameId, GoneToPenaltyBox goneToPenaltyBox)
        {
            connection.Execute("UPDATE Player SET IsInPenaltyBox = 1, LastQuestionId = null WHERE Id = @playerId",
                new {goneToPenaltyBox.PlayerId});
        }
    }
}
