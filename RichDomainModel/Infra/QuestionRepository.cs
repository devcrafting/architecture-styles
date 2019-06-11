using System.Collections.Generic;
using System.Linq;
using RichDomainModel.Domain;

namespace RichDomainModel.Infra
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly TriviaDbContext dbContext;

        public QuestionRepository(TriviaDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IEnumerable<Question> GetRandomForCategory(string categoryName, int nbQuestions)
        {
            return dbContext.Question
                .Where(q => q.Category.Name == categoryName)
                .Take(nbQuestions)
                .ToList();
        }
    }
}
