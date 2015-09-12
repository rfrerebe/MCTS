
namespace MCTS.Node
{
    using System;
    using System.Linq;
    using System.Text;

    using Enum;
    using Interfaces;
    using Utils;
    using System.Collections.Generic;


    internal class SingleThreadedNode : NodeBase
    {
        private readonly List<INode> childs;
        private readonly Stack<IMove> untriedMoves;

        private long wins;
        private long visits;

        internal SingleThreadedNode(INode parent, IMove move, IGameState gameState, float uctk)
            : base(parent, move, uctk, gameState.PlayerJustMoved)
        {
            this.wins = 0L;
            this.visits = 0L;
            this.childs = new List<INode>();
            var moves = gameState.GetMoves();
            var shuffled = moves.Shuffle();
            this.untriedMoves = new Stack<IMove>(shuffled); //randomize Moves
        }

        public override long Wins
        {
            get
            {
                return this.wins;
            }
        }

        public override long Visits
        {
            get
            {
                return this.visits;
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
                return (this.untriedMoves.Count == 0 &&  this.childs.Count != 0);
            }
        }       

        public override Tuple<bool,IMove> GetRandomMoveOrIsFalse()
        {
            if (this.untriedMoves.Count != 0)
            {
                var move = this.untriedMoves.Pop();
                return new Tuple<bool,IMove>(true, move);
            }
            return new Tuple<bool, IMove>(false, null); ;
        }

        public override INode AddChild (Func<float, INode> nodeConstructor)
        {
            var node = nodeConstructor(this.UCTK);
            this.childs.Add(node);
            return node;
        }

        public override void Update(EGameFinalStatus status)
        {
            this.visits++;
            if(status == EGameFinalStatus.GameWon)
            {
                this.wins++;
            }
        }
    }
}
