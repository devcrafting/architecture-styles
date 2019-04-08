using System.Collections.Generic;
using System.Linq;
using AnemicDomainModel.Domain;

namespace AnemicDomainModel.Tests
{
    internal class FakeQuestionsRepository : IQuestionRepository
    {
        public List<Question> GetRandomForCategory(string categoryName, int nbQuestions)
        {
            return Enumerable.Range(1, 50)
                .Select(x => new Question { Id = x }).ToList();
        }
    }
}