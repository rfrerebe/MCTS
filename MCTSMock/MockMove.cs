using System;
using MCTS.Interfaces;

namespace MCTSMock
{
    class MockMove : IMove
    {
        private const string name = "Move";
        string IMove.Name
        {
            get
            {
                return name;
            }
        }

        IGameState IMove.DoMove()
        {
            return new MockGame();
        }
    }
}
