using RichDomainModelWithoutORM.Domain;

namespace RichDomainModelWithoutORM.Tests
{
    internal class FakeDice : IRollDice
    {
        private int v;

        public FakeDice(int v)
        {
            this.v = v;
        }

        public int Roll() => this.v;
    }
}
