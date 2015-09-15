namespace MCTS.V2.Interfaces
{
    using Enum;
    using System.Collections.Generic;

    public interface IGameState
    {
        IEnumerable<IMove> GetMoves();

        IGameState PlayRandomlyUntilTheEnd();

        EGameFinalStatus GetResult(IPlayer player);

        IPlayer PlayerJustMoved { get; }
    }
}
