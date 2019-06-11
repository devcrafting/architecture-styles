using System;

namespace RichDomainModelWithoutORM.Domain
{
    public class RandomDice : IRollDice
    {
        public int Roll() => new Random().Next(1, 6);
    }
}