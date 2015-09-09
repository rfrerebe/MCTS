using System;
using System.Collections.Generic;
using MCTS.Enum;
using MCTS.Interfaces;
using MCTS.Utils;
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

        public EGameFinalStatus GetResult(IPlayer player)
        {
            throw new NotImplementedException();
        }

        IPlayer IGameState.CurrentPlayer()
        {
            return new MockPlayer();
        }

        IEnumerable<IMove> IGameState.GetMoves()
        {
            List<IMove> list = new List<IMove>();
            if (this.winProbability >=1 && this.winProbability < 99)
            {
                list.Add(new MockMove(this.winProbability + 1));
                list.Add(new MockMove(100 - this.winProbability - 1));
            }
            else
            {

                list.Add(new MockMove(99));
                list.Add(new MockMove(0));
            }
            return list.Shuffle();
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
