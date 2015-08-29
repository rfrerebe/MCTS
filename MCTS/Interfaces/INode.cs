namespace MCTS.Interfaces
{
    using System;
    using System.Collections.Generic;

    using Enum;

    internal interface INode
    {
        long Wins { get; }

        long Visits { get; }

        float UCTK { get; }

        INode Parent { get; }

        IEnumerable<INode> Childs { get; }

        IEnumerable<IMove> UntriedMoves { get; }

        IMove Move { get; }

        bool NodeIsFullyExpandedAndNonterminal { get; }

        INode AddChild(IMove move, IGameState gameState);

        Tuple<bool, IMove> GetRandomMoveOrIsFalse();

        IMove MostVisitedMove();

        INode UCTSelectChild();

        void Update(EGameFinalStatus status);

        string DisplayTree(int indent);
    }
}
