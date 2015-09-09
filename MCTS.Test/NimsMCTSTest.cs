namespace MCTSTest
{
    using SimpleGames;
    using NUnit.Framework;
    using MCTS.Interfaces;
    using MCTS;
    using System.Linq;

    class NimsMCTSTest
    {
        [TestCase(5)]
        //[TestCase(16)]
        public void TestNimsMCTS(int token)
        {
            var nims = new Nims.Nims(token) as IGameState;
            while (nims.GetMoves().Any())
            {
                IMove move = UCT.ComputeSingleThreadedUCT(nims, 1000, false, null, 0.7F);
                nims = move.DoMove();
            }
            IPlayer winner;
            if (nims.GetResult(Nims.Player.Player1) == MCTS.Enum.EGameFinalStatus.GameWon)
            {
                winner = Nims.Player.Player1;
            }
            else
            {
                winner = Nims.Player.Player2;
            }
            Assert.That(winner.Equals(Nims.Player.Player1));


    //state = NimState(15) # uncomment to play Nim with the given number of starting chips
    //while (state.GetMoves() != []):
    //    print str(state)
    //    if state.playerJustMoved == 1:
    //        m = UCT(rootstate = state, itermax = 1000, verbose = False) # play with values for itermax and verbose = True
    //    else:
    //        m = UCT(rootstate = state, itermax = 100, verbose = False)
    //    print "Best Move: " + str(m) + "\n"
    //    state.DoMove(m)
    //if state.GetResult(state.playerJustMoved) == 1.0:
    //    print "Player " + str(state.playerJustMoved) + " wins!"
    //elif state.GetResult(state.playerJustMoved) == 0.0:
    //    print "Player " + str(3 - state.playerJustMoved) + " wins!"
    //else: print "Nobody wins!"

        }

    }
}
