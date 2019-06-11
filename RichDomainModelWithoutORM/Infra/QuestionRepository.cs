using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using Dapper;
using RichDomainModelWithoutORM.Domain;

namespace RichDomainModelWithoutORM.Infra
{
    public class QuestionRepository : IQuestionRepository
    {
        public IEnumerable<Question> GetRandomForCategory(string categoryName, int nbQuestions)
        {
            using (var connection = new SQLiteConnection("DataSource=./trivia.db"))
            {
                return connection.Query("SELECT q.Id, q.Text, q.Answer FROM Question q " +
                                        "INNER JOIN Category c ON q.CategoryId = c.Id " +
                                        "WHERE c.Name = @categoryName " +
                                        "LIMIT @nbQuestions", new {categoryName, nbQuestions})
                    .Select(q => new Question((int)q.Id, 0, (string)q.Text, (string)q.Answer))
                    .ToList();
            }
        }
    }
}
