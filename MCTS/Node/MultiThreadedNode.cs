﻿
namespace MCTS.Node
{
    using Interfaces;
    using Enum;
    using System.Collections.Concurrent;
    using System.Threading;
    using Utils;
    using System.Linq;
    using System;
    using System.Text;
    using System.Collections.Generic;

    internal class MultiThreadedNode : NodeBase
    {


        private long wins;
        private long visits;
        private ConcurrentBag<MultiThreadedNode> childs;
        private ConcurrentStack<IMove> untriedMoves;

        internal MultiThreadedNode(INode parent, IMove move, IGameState gameState, float uctk)
            : base(parent, move, uctk)
        {
            this.wins =  0L;
            this.visits = 0L;

            this.childs = new ConcurrentBag<MultiThreadedNode>();
            var moves = gameState.GetMoves();
            var shuffled = moves.Shuffle();

            this.untriedMoves = new ConcurrentStack<IMove>(shuffled); //randomize Moves
        }

        public override long Wins
        {
            get
            {
                return Interlocked.Read(ref this.wins);
            }
        }

        public override long Visits
        {
            get
            {
                return Interlocked.Read(ref this.visits);
            }
        }

        public override IEnumerable<INode> Childs
        {
            get
            {
                return this.childs;
            }
        }

        public override IEnumerable<IMove> UntriedMoves
        {
            get
            {
                return this.untriedMoves;
            }
        }

        public override bool NodeIsFullyExpandedAndNonterminal
        {
            get
            {
                return (this.untriedMoves.IsEmpty  && this.childs.Any());
            }
        }


        public override Tuple<bool, IMove> GetRandomMoveOrIsFalse()
        {
            IMove move;
            var result = this.untriedMoves.TryPop(out move);
            return new Tuple<bool, IMove>(result, move);
        }

        public override INode AddChild(IMove move, IGameState gameState)
        {
            var node = new MultiThreadedNode(this, move, gameState, this.UCTK);
            this.childs.Add(node);
            return node;
        }

        public override void Update(EGameFinalStatus status)
        {
            Interlocked.Increment(ref this.visits);
            if (status == EGameFinalStatus.GameWon)
            {
                Interlocked.Increment(ref this.wins);
            }
        }
    }
}
