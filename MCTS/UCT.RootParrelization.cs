
namespace MCTS
{
    using System;

    using Interfaces;
    using Node;
    using System.Linq;
    using System.Threading.Tasks;

    public static partial class UCT
    {
        public static IMove ComputeRootParallization(IGameState gameState, int itermax, bool verbose, Action<string> printfn, float uctk)
        {
            var rootNode = new MultiThreadedNode(null, null, gameState, uctk);
            var player = gameState.CurrentPlayer();


            var tasks = (Enumerable.Range(0, 4).Select(i => Task.Factory.StartNew(() => ComputeFirstNodes(rootNode, player, itermax, gameState)))).ToArray();
            Task.WaitAll(tasks);
            return rootNode.MostVisitedMove();

        }

        private static void ComputeFirstNodes(INode rootNode, IPlayer player, int itermax, IGameState gameState)
        {
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
                    Func<float, INode> constructor = (f) => new SingleThreadedNode(node, move, state, f);
                    node = node.AddChild(constructor);
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
            //if (verbose)
            //{
            //    //printfn(rootNode.DisplayTree(0));
            //    //printfn(rootNode.DisplayMostVisistedChild());
            //}


        }
    }
}
