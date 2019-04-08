using System.Collections.Generic;

namespace AnemicDomainModel.Domain
{
    public interface IQuestionRepository
    {
        List<Question> GetRandomForCategory(string categoryName, int nbQuestions);
    }
}