using NUnit.Framework;
using System.Collections.Generic;
using MCTS.Utils;
using MCTSMock;
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

        [TestCase(70, 1000)]
        [TestCase(70, 100)]
        [TestCase(70, 10)]
        [TestCase(51, 1000)]
        [TestCase(51, 100)]
        [TestCase(51, 10)]
        public void CheckMCTS(int winProbability, int iter )
        {
            var game = new MockGame(winProbability);
            var move = MCTS.UCT.ComputeSingleThreadedUCT(game, iter, false, null, 1);
            System.Console.WriteLine(move.Name);
            Assert.IsTrue(move.Name.Contains(winProbability.ToString()));
        }

    }
}
