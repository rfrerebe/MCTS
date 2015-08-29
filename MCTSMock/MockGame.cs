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
        private readonly int winProbability;

        public MockGame(int winProbability)
        {
            if (winProbability >= 0 && winProbability < 100)
            {
                this.winProbability = winProbability;
            }
            else
            {
                throw new ArgumentOutOfRangeException(
                    "winProbability", 
                    winProbability, 
                    "Invalid winProbability. It must be between 0 (included) and 100 (exluded)");
            }
        }
        IPlayer IGameState.CurrentPlayer()
        {
            return new MockPlayer();
        }

        IEnumerable<IMove> IGameState.GetMoves()
        {
             return new List<IMove>()
                {
                    new MockMove(this.winProbability),
                    new MockMove(100 - this.winProbability)
                };
        }

        EGameFinalStatus IGameState.PlayRandomlyUntilTheEnd(IPlayer player)
        {
            int seed = (int) DateTime.Now.Ticks* 7;
            var random = new Random(seed);

            if (random.Next(100) < this.winProbability)
            {
                return EGameFinalStatus.GameWon;
            }
            else
            {
                return EGameFinalStatus.GameLost;
            }
        }
    }
}
