namespace MCTS.V1.UCT
{
    using System;

    using Interfaces;
    using Node;

    public static class SingleThreaded
    {
        public static IMove ComputeSingleThreadedUCT(IGameState gameState, int itermax, bool verbose, Action<string> printfn, float uctk)
        {
            var rootNode = new SingleThreadedNode(null, null, gameState, uctk);

            for (var i = 0; i < itermax; i++)
            {
                INode node = rootNode;
                var state = gameState.Clone();

                // Select
                while (node.NodeIsFullyExpandedAndNonterminal)
                {
                    //if (verbose)
                    //{
                    //    printfn(node.DisplayUTC());
                    //}
                    node = node.UCTSelectChild();
                    state.DoMove(node.Move);
                }

                // Expand
                var result = node.GetRandomMoveOrIsFalse();
                if (result.Item1)
                {
                    var move = result.Item2;
                    state.DoMove(move);
                    Func<INode> constructor = () => new SingleThreadedNode(node, move, state, node.UCTK);
                    node = node.AddChild(constructor);
                }

                // Rollout
                state.PlayRandomlyUntilTheEnd();

                // Backpropagate
                while (node != null)
                {
                    node.Update(state.GetResult(node.PlayerJustMoved));
                    node = node.Parent;
                }
            }
            if (verbose)
            {
                //printfn(rootNode.DisplayTree(0));
                printfn(rootNode.DisplayMostVisistedChild());
            }

            return rootNode.MostVisitedMove();
        }
    }
}
