using System.Collections.Generic;

namespace RichDomainModelWithoutORM.Domain
{
    public interface IQuestionRepository
    {
        IEnumerable<Question> GetRandomForCategory(string categoryName, int nbQuestions);
    }
}
