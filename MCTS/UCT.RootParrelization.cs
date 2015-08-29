
namespace MCTS
{
    using Interfaces;
    using System;

    public static partial class UCT
    {
        public static IMove ComputeRootParallization(IGameState gameState, int itermax, bool verbose, Action<string> printfn, float uctk)
        {
            var rootNode = new MultiThreadedNode(null, null, gameState, uctk);
            var player = gameState.CurrentPlayer();

            //var taskCount = Math.Min(itermax, rootNode.MovesCount);
            //var tasks = (Enumerable.Range(0, taskCount).Select (i => Task.Factory.StartNew(() => ComputeFirstNodes(rootNode, player)))).ToArray();
            //Task.WaitAll(tasks);

            //var remainingTasks = itermax - rootNode.MovesCount;

            for (var i = 0; i < itermax; i++)
            {
                var node = rootNode;
                var state = gameState;

                // Select
                while (node.NodeIsFullyExpandedAndNonterminal)
                {
                    if (verbose)
                    {
                        printfn(node.DisplayUTC());
                    }
                    node = node.UCTSelectChild();
                    state = node.Move.DoMove();
                }

                // Expand
                var result = node.GetRandomMoveOrIsFalse();
                if (result.Item1)
                {
                    var move = result.Item2;
                    state = move.DoMove();
                    node = node.AddChild(move, state);
                }

                // Rollout
                var status = state.PlayRandomlyUntilTheEnd(player);

                // Backpropagate
                while (node != null)
                {
                    node.Update(status);
                    node = node.ParentNode;
                }
            }
            if (verbose)
            {
                printfn(rootNode.TreeToString(0));
                //printfn(rootNode.DisplayMostVisistedChild());
            }

            return rootNode.MostVisitedMove();
        }
    }
}
