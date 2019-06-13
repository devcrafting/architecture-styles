using System.Collections.Generic;

namespace EventSourcingCQRS.Domain
{
    public interface IQuestionRepository
    {
        IEnumerable<Question> GetRandomForCategory(string categoryName, int nbQuestions);
    }
}
