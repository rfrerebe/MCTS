namespace MCTSTest
{
    using System.Linq;
    using System;

    using NUnit.Framework;

    using MCTS.Enum;
    using MCTS.V2.Interfaces;
    using MCTS.V2.UCT;

    [TestFixture]
    public class NimsMCTSTestV2
    {
        [TestCase(5, EGameFinalStatus.GameWon)]
        [TestCase(17, EGameFinalStatus.GameWon)]
        [TestCase(4, EGameFinalStatus.GameLost)]
        [TestCase(16, EGameFinalStatus.GameLost)]
        public void NimsMctsV2(int token, EGameFinalStatus status)
        {
            Action<string> print = s => Console.WriteLine(s);
            var firstPlayer = new NimPlayer(1);
            var nims = new NimState(token) as IGameState;
            while (nims.GetMoves().Any())
            {
                print(nims.ToString());
                IMove move = SingleThreaded.ComputeSingleThreadedUCT(nims, 1000, true, print, 0.7F);
                print(move.Name);
                nims = move.DoMove();
            }
            print(nims.GetResult(firstPlayer).ToString());
            Assert.IsTrue(nims.GetResult(firstPlayer) == status);
        }
        

    }
}
