
namespace MCTS
{
    using System;

    using Interfaces;
    using Node;
    using System.Linq;
    using System.Threading.Tasks;
    using Enum;

    public static partial class UCT
    {
        public static IMove ComputeRootParallization(IGameState gameState, int itermax, bool verbose, Action<string> printfn, float uctk)
        {
            var rootNode = new MultiThreadedNode(null, null, gameState, uctk);
            var player = gameState.CurrentPlayer();


            var tasks = (Enumerable.Range(0, 2).Select(i => Task.Factory.StartNew(() => ComputeFirstNodes(rootNode, player, itermax, gameState)))).ToArray();
            Task.WaitAll(tasks);
            return rootNode.MostVisitedMove();

        }

        private static void Select(INode node, IGameState state)
        {
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
        }

        private static void Expand(INode node, IGameState state)
        {
            // Expand
            var result = node.GetRandomMoveOrIsFalse();
            if (result.Item1)
            {
                var move = result.Item2;
                state = move.DoMove();
                Func<float, INode> constructor = (f) => new SingleThreadedNode(node, move, state, f);
                node = node.AddChild(constructor);
            }
        }

        private static EGameFinalStatus Rollout( IGameState state, IPlayer player)
        {
            return state.PlayRandomlyUntilTheEnd(player);
        }

        private static void Backpropagate(INode node, EGameFinalStatus status)
        {
            // Backpropagate
            while (node != null)
            {
                node.Update(status);
                node = node.Parent;
            }
        }

        private static void ComputeFirstNodes(INode rootNode, IPlayer player, int itermax, IGameState gameState)
        {
 
                INode node = rootNode;
                var state = gameState;

                Expand(node, state);

                var status = Rollout(state, player);

                Backpropagate(node, status);

        }

        private static void Compute(INode rootNode, IPlayer player, int itermax, IGameState gameState)
        {
            for (var i = 0; i < itermax; i++)
            {
                INode node = rootNode;
                var state = gameState;

                Select(node, state);

                Expand(node, state);

                var status = Rollout(state, player);

                Backpropagate(node, status);

            }
            //if (verbose)
            //{
            //    //printfn(rootNode.DisplayTree(0));
            //    //printfn(rootNode.DisplayMostVisistedChild());
            //}


        }
    }
}
