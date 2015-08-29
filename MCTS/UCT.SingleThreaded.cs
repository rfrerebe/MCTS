
namespace MCTS
{
    using System;

    using Interfaces;
    using Node;

    public static partial class UCT
    {
        public static IMove ComputeSingleThreadedUCT(IGameState gameState, int itermax, bool verbose, Action<string> printfn, float uctk)
        {
            var rootNode = new SingleThreadedNode(null, null, gameState, uctk);
            var player = gameState.CurrentPlayer();

            //var taskCount = Math.Min(itermax, rootNode.MovesCount);
            //var tasks = (Enumerable.Range(0, taskCount).Select (i => Task.Factory.StartNew(() => ComputeFirstNodes(rootNode, player)))).ToArray();
            //Task.WaitAll(tasks);

            //var remainingTasks = itermax - rootNode.MovesCount;

            for (var i = 0; i < itermax; i++)
            {
                INode node = rootNode;
                var state = gameState;

                // Select
                while (node.NodeIsFullyExpandedAndNonterminal)
                {
                    //if (verbose)
                    //{
                    //    printfn(node.DisplayUTC());
                    //}
                    node = node.UCTSelectChild();
                    state = node.Move.DoMove();
                }

                // Expand
                var result = node.GetRandomMoveOrIsFalse();
                if (result.Item1)
                {
                    var move = result.Item2;
                    state = move.DoMove();
                    node = node.AddChild(move, state) as SingleThreadedNode;
                }

                // Rollout
                var status = state.PlayRandomlyUntilTheEnd(player);

                // Backpropagate
                while (node != null)
                {
                    node.Update(status);
                    node = node.Parent;
                }
            }
            if (verbose)
            {
                printfn(rootNode.DisplayTree(0));
                //printfn(rootNode.DisplayMostVisistedChild());
            }

            return rootNode.MostVisitedMove();
        }
    }
}
