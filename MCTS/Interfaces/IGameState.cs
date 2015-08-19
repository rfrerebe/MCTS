namespace MCTS
{
    using Interfaces;
    using Enum;
    using System.Collections.Generic;

    public interface IGameState
    {
        IGameState Clone();

        IEnumerable<IMove> GetMoves();

        void PlayRandomlyUntilTheEnd();

        void DoMove(IMove move);

        IPlayer JustMoved();

        EGameFinalStatus IsGameWon(IPlayer player);
    }
}
