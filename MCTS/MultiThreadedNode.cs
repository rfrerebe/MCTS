
namespace MCTS
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

    internal class MultiThreadedNode
    {
        private float UCTK;

        private IMove move;
        private MultiThreadedNode parent;
        private long wins;
        private long visits;
        private ConcurrentBag<MultiThreadedNode> childs;
        private ConcurrentStack<IMove> untriedMoves;
        //private int movesCount;

        internal MultiThreadedNode(MultiThreadedNode parent, IMove move, IGameState gameState, float uctk)
        {
            this.move = move; // null for root Node
            this.parent = parent; // null for root Node
            this.UCTK = uctk;
            this.wins =  0L;
            this.visits = 0L;

            this.childs = new ConcurrentBag<MultiThreadedNode>();
            var moves = gameState.GetMoves();
            var shuffled = moves.Shuffle();

            this.untriedMoves = new ConcurrentStack<IMove>(shuffled); //randomize Moves
        }

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
                return (this.untriedMoves.Count == 0 && this.childs.Count != 0);
            }
        }

        internal IMove Move
        {
            get
            {
                return this.move;
            }
        }

        internal MultiThreadedNode ParentNode
        {
            get
            {
                return this.parent;
            }
        }

        internal string DisplayBestWinVisitRatioChild()
        {
            var values = this.childs.Select(node => new Tuple<long, long, long, string>(100 * node.Wins / node.Visits, node.Wins, node.Visits, DisplayNode(node))).OrderByDescending(t => t.Item1);
            StringBuilder sb = new StringBuilder();

            sb.Append("MVC :");
            foreach (var value in values)
            {
                sb.AppendFormat(" {0}%={1}/{2} {3}", value.Item1, value.Item2, value.Item3, value.Item4);
            }
            return sb.ToString();
        }

        internal IMove MostVisitedMove()
        {
            return this.childs.OrderByDescending(node => node.Visits).First().Move;
        }

        internal string DisplayUTC()
        {
            var values = this.childs.Select(node => new Tuple<double, string>(ComputeUTC(node), DisplayNode(node))).OrderByDescending(t => t.Item1);
            StringBuilder sb = new StringBuilder();

            sb.Append("UTC : ");
            foreach (var value in values)
            {
                sb.AppendFormat("{0:0.00} {1},", value.Item1, value.Item2);
            }
            return sb.ToString();
        }

        private string DisplayNode(MultiThreadedNode node)
        {
            var list = new List<string>();
            var sb = new StringBuilder();
            while (node.Move != null)
            {
                list.Add(node.Move.Name);
                node = node.ParentNode;
            }

            list.Reverse();
            foreach (var move in list)
            {
                sb.AppendFormat("->{0}", move);
            }
            return sb.ToString();
        }

        /// Use the UCB1 formula to select a child node. Often a constant UCTK is applied so we have
        /// lambda c: c.wins/c.visits + UCTK * sqrt(2*log(self.visits)/c.visits) to vary the amount of
        /// exploration versus exploitation.
        internal MultiThreadedNode UCTSelectChild()
        {
            // bigger is first
            var list = this.childs.OrderByDescending(ComputeUTC);
            return list.First();
        }

        private double ComputeUTC(MultiThreadedNode node)
        {
            return (node.Wins / node.Visits) + (UCTK * Math.Sqrt(2 * Math.Log(this.Visits) / node.Visits));
        }

        internal Tuple<bool, IMove> GetRandomMoveOrIsFalse()
        {
            if (this.untriedMoves.Count != 0)
            {
                IMove move;
                var result = this.untriedMoves.TryPop(out move);
                return new Tuple<bool, IMove>(result, move);
            }
            return new Tuple<bool, IMove>(false, null); ;
        }

        internal MultiThreadedNode AddChild(IMove move, IGameState gameState)
        {
            var node = new MultiThreadedNode(this, move, gameState, this.UCTK);
            this.childs.Add(node);
            return node;
        }

        internal void Update(EGameFinalStatus status)
        {
            Interlocked.Increment(ref this.visits);
            if (status == EGameFinalStatus.GameWon)
            {
                Interlocked.Increment(ref this.wins);
            }
        }

        internal string Display()
        {
            var move = this.move != null ? this.move.Name : "No Move";
            return string.Format("[M: {0} W/V:{1}/{2} U:{3} C:{4}]", move, this.Wins, this.Visits, this.untriedMoves.Count, this.childs.Count);
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

