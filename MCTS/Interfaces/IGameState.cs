namespace MCTS.Interfaces
{
    using Enum;
    using System.Collections.Generic;

    public interface IGameState
    {
        IEnumerable<IMove> GetMoves();

        EGameFinalStatus PlayRandomlyUntilTheEnd(IPlayer player);

        EGameFinalStatus GetResult(IPlayer player);

        IPlayer CurrentPlayer();
    }
}
