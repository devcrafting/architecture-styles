using System.Collections.Generic;

namespace RichDomainModel.Domain
{
    public interface IQuestionRepository
    {
        List<Question> GetRandomForCategory(string categoryName, int nbQuestions);
    }
}