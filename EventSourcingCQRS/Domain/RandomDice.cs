using System;

namespace EventSourcingCQRS.Domain
{
    public class RandomDice : IRollDice
    {
        public int Roll() => new Random().Next(1, 6);
    }
}