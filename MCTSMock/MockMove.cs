using System;
using MCTS.Interfaces;

namespace MCTSMock
{
    class MockMove : IMove
    {
        private readonly int winProbability;

        public MockMove(int winProbability)
        {
            if (winProbability >= 0 && winProbability <= 100)
            {
                this.winProbability = winProbability;
            }
            else throw new ArgumentOutOfRangeException("winProbability", winProbability, "Invalid winProbability. It must be between 0 and 100 included");
        }
        
        string IMove.Name
        {
            get
            {
                return this.winProbability.ToString();
            }
        }

        IGameState IMove.DoMove()
        {
            return new MockGame(this.winProbability);
        }
    }
}
