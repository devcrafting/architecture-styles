using System;
using System.Collections.Generic;
using EventSourcingCQRS.Domain.Events;

namespace EventSourcingCQRS.Domain
{
    public class Player
    {
        public Player(string id, string playerName) : this(id, playerName, false, 0, 0, null)
        {
            Id = id;
        }

        public Player(string id, string playerName, bool isInPenaltyBox, int place, int goldCoins, string expectedAnswer)
        {
            Id = id;
            _isInPenaltyBox = isInPenaltyBox;
            _place = place;
            _goldCoins = goldCoins;
            _expectedAnswer = expectedAnswer;
        }

        // all properties made private
        public readonly string Id;
        // We do not need Name anymore to take a decision : public Name { get; }
        private int _place;
        private bool _isInPenaltyBox;
        private int _goldCoins;
        private string _expectedAnswer; // we do not need the full LastQuestion to take a decision, just the answer

        public void Apply(Moved moved)
        {
            _place = moved.NewPlace;
        }

        public void Apply(QuestionAsked questionAsked)
        {
            _expectedAnswer = questionAsked.Answer;
        }

        internal void CheckCanMove()
        {
            if (!string.IsNullOrEmpty(this._expectedAnswer))
                throw new Exception("Player already moved, need to answer now");
        }

        internal bool CannotGoOutOfPenaltyBox(int diceRoll) =>
            _isInPenaltyBox && diceRoll % 2 == 0;

        internal IEnumerable<IDomainEvent> Move(int diceRoll, QuestionsDeck questionsDeck)
        {
            if (_isInPenaltyBox)
                yield return new GetOutOfPenaltyBox(Id);
            var newPlace = (_place + diceRoll) % 12;
            yield return new Moved(Id, newPlace);
            yield return questionsDeck.Draw(newPlace);
        }

        internal IDomainEvent Answer(string answer)
        {
            if (_expectedAnswer == answer)
                return new GoldCoinEarned(Id, _goldCoins + 1);
            return new GoneToPenaltyBox(Id);
        }
    }
}
