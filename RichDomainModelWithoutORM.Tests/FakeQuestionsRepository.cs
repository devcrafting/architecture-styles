using System.Collections.Generic;
using System.Linq;
using RichDomainModel.Domain;

namespace RichDomainModelWithoutORM.Tests
{
    internal class FakeQuestionsRepository : IQuestionRepository
    {
        public IEnumerable<Question> GetRandomForCategory(string categoryName, int nbQuestions)
        {
            return Enumerable.Range(1, 50)
                .Select(x => new Question(x, 1, "some question", "its answer")).ToList();
        }
    }
}
