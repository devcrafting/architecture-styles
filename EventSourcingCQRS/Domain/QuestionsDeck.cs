using System.Collections.Generic;
using System.Linq;
using EventSourcingCQRS.Domain.Events;

namespace EventSourcingCQRS.Domain
{
    public class QuestionsDeck
    {
        private readonly List<GameCategory> _categories = new List<GameCategory>();

        public QuestionsDeck(IEnumerable<GameCategory> categories)
        {
            _categories.AddRange(categories);
        }

        public IEnumerable<GameCategory> Categories => _categories;

        public IDomainEvent Draw(int newPlace)
        {
            var questionToAsk = _categories[newPlace % _categories.Count]
                .Questions.First(x => x.NotUsed).Question;
            return new QuestionAsked(questionToAsk.Id, questionToAsk.Text);
        }
    }
}
