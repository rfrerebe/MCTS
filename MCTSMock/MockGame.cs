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
            throw new NotImplementedException();
        }

        IEnumerable<IMove> IGameState.GetMoves()
        {
            throw new NotImplementedException();
        }

        EGameFinalStatus IGameState.PlayRandomlyUntilTheEnd(IPlayer player)
        {
            throw new NotImplementedException();
        }
    }
}
