using System;
using System.Collections.Generic;
using System.Linq;
using RichDomainModelWithoutORM.Domain.Events;

namespace RichDomainModelWithoutORM.Domain
{
    public class Player
    {
        public Player(string id, string playerName) : this(playerName)
        {
            Id = id;
        }

        public Player(string playerName)
        {
            Name = playerName;
            Place = 0;
            IsInPenaltyBox = false;
            GoldCoins = 0;
        }

        public Player(string id, string playerName, bool isInPenaltyBox, int place, int goldCoins, Question lastQuestion)
        {
            Id = id;
            Name = playerName;
            IsInPenaltyBox = isInPenaltyBox;
            Place = place;
            GoldCoins = goldCoins;
            LastQuestion = lastQuestion;
        }

        public string Id { get; }
        public string Name { get; }
        public int Place { get; }
        public bool IsInPenaltyBox { get; private set; }
        public int GoldCoins { get; private set; }
        public Question LastQuestion { get; private set; }

        internal void CheckCanMove()
        {
            if (this.LastQuestion != null)
                throw new Exception("Player already moved, need to answer now");
        }

        internal bool CannotGoOutOfPenaltyBox(int diceRoll) =>
            IsInPenaltyBox && diceRoll % 2 == 0;

        internal IEnumerable<IDomainEvent> Move(int diceRoll, QuestionsDeck questionsDeck)
        {
            if (IsInPenaltyBox)
                yield return new GetOutOfPenaltyBox(Id);
            var newPlace = (Place + diceRoll) % 12;
            yield return new Moved(Id, newPlace);
            yield return questionsDeck.Draw(newPlace);
        }

        internal IDomainEvent Answer(string answer)
        {
            if (LastQuestion.Answer == answer)
                return new GoldCoinEarned(Id, GoldCoins + 1);
            else
                return new GoneToPenaltyBox(Id);
        }
    }
}
