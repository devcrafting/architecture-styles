using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;
using Dapper;
using EventSourcingCQRS.Domain;
using EventSourcingCQRS.Domain.Events;
using Newtonsoft.Json;

namespace EventSourcingCQRS.Infra
{
    public class GameRepository : IGameRepository
    {
        static GameRepository()
        {
            if (File.Exists("trivia.db")) return;
            using (var connection = GetConnection())
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("EventSourcingCQRS.Infra.createDatabase.sql"))
            using (var reader = new StreamReader(stream))
            {
                connection.Open();
                connection.Execute(reader.ReadToEnd());
            }
        }

        private static IDbConnection GetConnection()
        {
            return new SQLiteConnection("Data Source=trivia.db");
        }

        public Game Get(string gameId)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var events = connection.Query(
                    "SELECT EventType, Data " +
                    "FROM Events " +
                    "WHERE AggregateName = 'Game' AND AggregateId = @gameId " +
                    "ORDER BY EventId", new { gameId });
                
                return new Game(events.Select(GetEvent));
            }
        }

        private IDomainEvent GetEvent(dynamic @event)
        {
            var eventType = Assembly.GetExecutingAssembly().GetType(@event.EventType);
            return JsonConvert.DeserializeObject(@event.Data, eventType);
        }

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
                    connection.Execute(
                        "INSERT INTO Events(AggregateName, AggregateId, EventType, Data, Metadata) " +
                        "VALUES (@AggregateName, @AggregateId, @EventType, @Data, @Metadata)",
                        events.Select(e =>
                            new
                            {
                                AggregateName = "Game",
                                AggregateId = gameId,
                                EventType = e.GetType().FullName,
                                Data = JsonConvert.SerializeObject(e),
                                // Metadata should be a class with additional info like at least CorrelationId, CausationId, UserId...
                                Metadata = JsonConvert.SerializeObject(new { HappenedAt = DateTime.Now })
                            }));

                    transaction.Commit();
                }
            }
        }
    }
}
