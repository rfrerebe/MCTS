using NUnit.Framework;
using System.Collections.Generic;
using MCTS.Utils;
namespace MCTSTest
{
    public class MCTSTest
    {
        [Test]
        public void TestShuffleIsEquivalentAndIsNotOrdered()
        {
            var list = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var shuffled = new List<int>(list).Shuffle();
            Assert.That(shuffled, Is.EquivalentTo(list));
            Assert.That(shuffled, Is.Not.Ordered);
        }



    }
}
