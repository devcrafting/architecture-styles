using System;
using System.Collections.Generic;
using System.Linq;

namespace RichDomainModel.Domain
{
    public class Player
    {
        public Player(int id, string playerName) : this(playerName)
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

        public Player(int id, string playerName, Question lastQuestion) : this(id, playerName)
        {
            LastQuestion = lastQuestion;
        }

        public Player(int id, string playerName, bool isInPenaltyBox) : this(id, playerName)
        {
            IsInPenaltyBox = isInPenaltyBox;
        }

        public int Id { get; }
        public string Name { get; }
        public int Place { get; private set; }
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

        internal GameQuestion Move(int diceRoll, List<GameCategory> categories)
        {
            IsInPenaltyBox = false;
            Place = (Place + diceRoll) % 12;
            var questionToAsk = categories[Place % categories.Count]
                .Questions.First(x => x.NotUsed);
            questionToAsk.NotUsed = false;
            LastQuestion = questionToAsk.Question;
            return questionToAsk;
        }

        internal bool Answer(string answer)
        {
            var goodAnswer = LastQuestion.Answer == answer;
            if (goodAnswer)
                GoldCoins++;
            else
                IsInPenaltyBox = true;
            LastQuestion = null;
            return goodAnswer;
        }
    }
}
