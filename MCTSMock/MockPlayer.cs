using System;
using MCTS.Interfaces;

namespace MCTSMock
{
    class MockPlayer : IPlayer
    {
        string IPlayer.Name
        {
            get
            {
                return "Alfred";
            }
        }
    }
}
