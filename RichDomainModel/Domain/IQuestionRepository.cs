using System.Collections.Generic;

namespace RichDomainModel.Domain
{
    public interface IQuestionRepository
    {
        IEnumerable<Question> GetRandomForCategory(string categoryName, int nbQuestions);
    }
}
