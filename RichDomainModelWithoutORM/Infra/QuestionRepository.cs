using System.Collections.Generic;
using RichDomainModelWithoutORM.Domain;

namespace RichDomainModelWithoutORM.Infra
{
    public class QuestionRepository : IQuestionRepository
    {
        public IEnumerable<Question> GetRandomForCategory(string categoryName, int nbQuestions)
        {
            return null;
        }
    }
}
