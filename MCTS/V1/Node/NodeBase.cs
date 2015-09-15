namespace MCTS.V1.Node
{
    using Interfaces;
    using Enum;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    internal abstract class NodeBase : INode
    {
        private readonly INode parent;
        private readonly IMove move;
        private readonly float uctk;
        private readonly IPlayer playerJustMoved;


        internal NodeBase (INode parentNode, IMove move, float uctk, IPlayer player)
        {
            this.parent = parentNode;
            this.move = move;
            this.uctk = uctk;
            this.playerJustMoved = player;
        }


        /// Use the UCB1 formula to select a child node. Often a constant UCTK is applied so we have
        /// lambda c: c.wins/c.visits + UCTK * sqrt(2*log(self.visits)/c.visits) to vary the amount of
        /// exploration versus exploitation.
        internal double ComputeUTC(INode node)
        {
            return (node.Wins / node.Visits) + (this.UCTK * Math.Sqrt(2 * Math.Log(this.Visits) / node.Visits));
        }



        public abstract bool NodeIsFullyExpandedAndNonterminal
        {
            get;
        }

        public abstract long Visits
        {
            get;
        }

        public abstract long Wins
        {
            get;
        }

        public float UCTK
        {
            get
            {
                return this.uctk;
            }
        }

        public INode Parent
        {
            get
            {
                return this.parent;
            }
        }

        public abstract IEnumerable<INode> Childs
        {
            get;
        }

        public abstract IEnumerable<IMove> UntriedMoves
        {
            get;
        }

        public IMove Move
        {
            get
            {
                return this.move;
            }
        }

        public IPlayer PlayerJustMoved
        {
            get
            {
                return this.playerJustMoved;
            }
        }

        public string DisplayMostVisistedChild()
        {
            var sb = new StringBuilder();
            foreach (var node in this.Childs)
            {
                sb.AppendFormat("N:{0} W/V:{1}/{2}", node.Move.Name, node.Wins, node.Visits);
                sb.AppendLine();
            }
            return sb.ToString();
        }

        //internal string DisplayTree()
        //{
        //    throw new NotImplementedException();
        //}

        //internal string DisplayUTC()
        //{
        //    throw new NotImplementedException();
        //}

        public abstract INode AddChild(Func<INode> nodeConstructor);

        public abstract Tuple<bool, IMove> GetRandomMoveOrIsFalse();

        public  IMove MostVisitedMove()
        {
            var firstVisitOrdered = this.Childs.OrderByDescending(node => node.Visits).First();

            // most visited move is a really above other moves.
            // done to avoid :
            //      Node 1 : Visit = 334, Win = 2 (selected)
            //      Node 2 : Visit = 333, Win = 330 (most promising)
            //      Node 3 : Visit = 333, Win = 5
            // return first 
            if (firstVisitOrdered.Visits > (this.Visits / this.Childs.Count()) + 1)
            {
                return firstVisitOrdered.Move;
            }
            // otherwise return most wins    
            else
            {
                return this.Childs.OrderByDescending(node => node.Wins).First().Move;
            }
        }
        public INode UCTSelectChild()
        {
            // bigger is first
            return this.Childs.OrderByDescending(ComputeUTC).First();
        }

        public abstract void Update(EGameFinalStatus status);


        internal string DisplayBestWinVisitRatioChild()
        {
            var values = this.Childs.Select(node => new Tuple<long, long, long, string>(100 * node.Wins / node.Visits, node.Wins, node.Visits, DisplayNode(node))).OrderByDescending(t => t.Item1);
            StringBuilder sb = new StringBuilder();

            sb.Append("MVC :");
            foreach (var value in values)
            {
                sb.AppendFormat(" {0}%={1}/{2} {3}", value.Item1, value.Item2, value.Item3, value.Item4);
            }
            return sb.ToString();
        }

        internal string DisplayUTC()
        {
            var values = this.Childs.Select(node => new Tuple<double, string>(ComputeUTC(node), DisplayNode(node))).OrderByDescending(t => t.Item1);
            StringBuilder sb = new StringBuilder();

            sb.Append("UTC : ");
            foreach (var value in values)
            {
                sb.AppendFormat("{0:0.00} {1},", value.Item1, value.Item2);
            }
            return sb.ToString();
        }

        internal string Display()
        {
            var move = this.Move != null ? this.Move.Name : "No Move";
            return string.Format("[M: {0} W/V:{1}/{2} U:{3} C:{4}]", move, this.Wins, this.Visits, this.UntriedMoves.Count(), this.Childs.Count());
        }

        public string DisplayTree(int indent)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            sb.Append(new string(' ', indent));
            sb.Append(this.Display());
            foreach (var child in this.Childs)
            {
                sb.Append(child.DisplayTree(indent + 1));
            }
            return sb.ToString();
        }

        private string DisplayNode(INode node)
        {
            var list = new List<string>();
            var sb = new StringBuilder();
            while (node.Move != null)
            {
                list.Add(node.Move.Name);
                node = node.Parent;
            }

            list.Reverse();
            foreach (var move in list)
            {
                sb.AppendFormat("->{0}", move);
            }
            return sb.ToString();
        }
    }
}
