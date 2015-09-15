
namespace MCTS.V2.UCT
{
    using System;

    using Interfaces;
    using Node;
    using System.Linq;
    using System.Threading.Tasks;
    using Enum;

    public static class MultiThreaded
    {
        public static IMove ComputeRootParallization(IGameState gameState, int itermax, bool verbose, Action<string> printfn, float uctk)
        {
            var rootNode = new MultiThreadedNode(null, null, gameState, uctk);
            var player = gameState.PlayerJustMoved;
            var processors = Environment.ProcessorCount;

            var tasks1 = (Enumerable.Range(0, processors).Select(i => Task.Factory.StartNew(() => ComputeFirstNodes(rootNode, player, itermax, gameState)))).ToArray();
            Task.WaitAll(tasks1);

            var tasks2 = (Enumerable.Range(0, processors).Select(i => Task.Factory.StartNew(() => Compute(rootNode, player, itermax, gameState)))).ToArray();
            Task.WaitAll(tasks2);
            return rootNode.MostVisitedMove();
        }

        private static IGameState Select(INode node, IGameState state)
        {
            // Select
            while (node.NodeIsFullyExpandedAndNonterminal)
            {
                //if (verbose)
                //{
                //    printfn(node.DisplayUTC());
                //}
                node = node.UCTSelectChild();
                return node.Move.DoMove();
            }
            return state;
        }

        private static bool Expand(INode node, IGameState state)
        {
            // Expand
            var result = node.GetRandomMoveOrIsFalse();
            if (result.Item1)
            {
                var move = result.Item2;
                state = move.DoMove();
                Func<INode> constructor = () => new SingleThreadedNode(node, move, state, node.UCTK);
                node = node.AddChild(constructor);
                return true;
            }
            return false;
        }

        private static IGameState Rollout(IGameState state)
        {
            return state.PlayRandomlyUntilTheEnd();
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

            if (Expand(node, state))
            {
                var status = Rollout(state).GetResult(player);
                Backpropagate(node, status);
            }
        }

        private static void Compute(INode rootNode, IPlayer player, int itermax, IGameState gameState)
        {
            for (var i = 0; i < itermax; i++)
            {
                INode node = rootNode;
                var state = gameState;

                state = Select(node, state);

                if (Expand(node, state))
                {
                    var status = Rollout(state).GetResult(player);
                    Backpropagate(node, status);
                }

            }

        }
    }
}
