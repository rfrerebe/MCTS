
namespace MCTS
{   
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading;
    using System.Text;
        
    using Enum;
    using Interfaces;
    using Utils;

    internal class Node
    {
        private float UCTK = 0.3F;

        private IMove move;
        private Node parent;
        private long wins;
        private long visits;
        private ConcurrentBag<Node> childs;
        private ConcurrentStack<IMove> untriedMoves;
        //private IPlayer playerJustMoved;

        internal Node(Node parent, IMove move, IGameState gameState)
        {
            this.move = move; // null for root Node
            this.parent = parent; // null for root Node

            this.wins = 0L;
            this.visits = 0L;

            this.childs = new ConcurrentBag<Node>();
            var moveList = gameState.GetMoves();
            var shuffleList = moveList.Shuffle();
            this.untriedMoves = new ConcurrentStack<IMove>(shuffleList); //randomize Moves
            //this.playerJustMoved = gameState.JustMoved();
        }

        //internal IPlayer PlayerJustMoved
        //{
        //    get
        //    {
        //        return this.playerJustMoved;
        //    }
        //}

        internal long Wins
        {
            get
            {
                return Interlocked.Read(ref this.wins);
            }
        }

        internal long Visits
        {
            get
            {
                return Interlocked.Read(ref this.visits);
            }
        }

        internal bool NodeIsFullyExpandedAndNonterminal
        {
            get
            {
                return (this.untriedMoves.IsEmpty && ! this.childs.IsEmpty);
            }
        }       

        internal IMove Move
        {
            get
            {
                return this.move;
            }
        }

        internal Node ParentNode
        {
            get
            {
                return this.parent;
            }
        }

        internal IMove MostVisitedMove()
        {
            return this.childs.OrderByDescending(node => node.Visits).First().Move;
        }
           

        /// Use the UCB1 formula to select a child node. Often a constant UCTK is applied so we have
        /// lambda c: c.wins/c.visits + UCTK * sqrt(2*log(self.visits)/c.visits) to vary the amount of
        /// exploration versus exploitation.
        internal Node UCTSelectChild()
        {
            // bigger is first
            return this.childs.OrderByDescending(node => node.Wins / node.Visits + UCTK * Math.Sqrt(2.0 * Math.Log(this.Visits)) / node.Visits).First();
        }

        internal Tuple<bool,IMove> GetRandomMoveOrIsFalse()
        {
            IMove move;
            if (this.untriedMoves.TryPop(out move))
            {
                return new Tuple<bool,IMove>(true, move);
            }
            return new Tuple<bool, IMove>(false, null); ;
        }

        internal Node AddChild(IMove move, IGameState gameState)
        {
            var node = new Node(this, move, gameState);
            this.childs.Add(node);
            return node;
        }

        internal void Update(EGameFinalStatus status)
        {
            Interlocked.Increment(ref this.visits);
            if(status == EGameFinalStatus.GameWon)
            {
                Interlocked.Increment(ref this.wins);
            }
        }
        
        internal string Display()
        {
            var move = this.move != null ? this.move.Name : "No Move";
            return string.Format("[M: {0} W/V:{1}/{2} U:{3} C:{4}]", move , this.Wins, this.Visits, this.untriedMoves.Count, this.childs.Count);
        }


        internal string TreeToString(int indent)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            sb.Append(new string(' ', indent));
            sb.Append(this.Display());
            foreach (var child in this.childs)
            {
                sb.Append(child.TreeToString(indent + 1));
            }
            return sb.ToString();
        }
    }
}
