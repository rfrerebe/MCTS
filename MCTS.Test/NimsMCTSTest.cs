namespace MCTSTest
{
    using NUnit.Framework;
    using MCTS.Interfaces;
    using MCTS;
    using MCTS.Enum;
    using System.Linq;
    using System;

    class NimsMCTSTest
    {
        [TestCase(5, EGameFinalStatus.GameWon)]
        [TestCase(17, EGameFinalStatus.GameWon)]
        [TestCase(4, EGameFinalStatus.GameLost)]
        [TestCase(16, EGameFinalStatus.GameLost)]
        public void NimsMCTS(int token, EGameFinalStatus status)
        {
            Action<string> print = s => Console.WriteLine(s);
            var firstPlayer = new NimPlayer(1);
            var nims = new NimState(token) as IGameState;
            while (nims.GetMoves().Any())
            {
                print(nims.ToString());
                IMove move = UCT.ComputeSingleThreadedUCT(nims, 1000, true, print, 0.3F);
                print(move.Name);
                nims.DoMove(move);
            }
            Assert.IsTrue(nims.GetResult(firstPlayer) == status);
        }
        

    }
}
