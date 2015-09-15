namespace MCTSTest.V1
{
    using System.Linq;
    using System;

    using NUnit.Framework;

    using MCTS.Enum;
    using MCTS.V1.Interfaces;
    using MCTS.V1.UCT;

    [TestFixture]
    public class NimsMCTSTest
    {
        [TestCase(5, EGameFinalStatus.GameWon)]
        [TestCase(17, EGameFinalStatus.GameWon)]
        [TestCase(4, EGameFinalStatus.GameLost)]
        [TestCase(16, EGameFinalStatus.GameLost)]
        public void NimsMctsV1(int token, EGameFinalStatus status)
        {
            Action<string> print = s => Console.WriteLine(s);
            var firstPlayer = new NimPlayer(1);
            var nims = new NimState(token) as IGameState;
            while (nims.GetMoves().Any())
            {
                print(nims.ToString());
                IMove move = SingleThreaded.ComputeSingleThreadedUCT(nims, 1000, true, print, 0.7F);
                print(move.Name);
                nims.DoMove(move);
            }
            print(nims.GetResult(firstPlayer).ToString());
            Assert.IsTrue(nims.GetResult(firstPlayer) == status);
        }
        

    }
}
