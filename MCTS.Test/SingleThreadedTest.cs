
namespace MCTSTest
{
    using NUnit.Framework;
    using MCTSMock;
    public class SingleThreadedTest
    {

        [TestCase(70, 1000)]
        [TestCase(70, 100)]
        [TestCase(70, 10)]
        [TestCase(51, 1000)]
        [TestCase(51, 100)]
        [TestCase(51, 10)]
        public void CheckMCTSSingleThreaded(int winProbability, int iter)
        {
            var game = new MockGame(winProbability);
            var move = MCTS.UCT.ComputeSingleThreadedUCT(game, iter, false, null, 1);
            System.Console.WriteLine(move.Name);
            Assert.IsTrue(move.Name.Contains(winProbability.ToString()));
        }

    }
}