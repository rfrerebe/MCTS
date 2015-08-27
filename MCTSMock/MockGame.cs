using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCTS.Enum;
using MCTS.Interfaces;

namespace MCTSMock
{
    public class MockGame : IGameState
    {
        IPlayer IGameState.CurrentPlayer()
        {
            return new MockPlayer();
        }

        IEnumerable<IMove> IGameState.GetMoves()
        {
 
        }

        EGameFinalStatus IGameState.PlayRandomlyUntilTheEnd(IPlayer player)
        {
            throw new NotImplementedException();
        }
    }
}
