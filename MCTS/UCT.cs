
namespace MCTS
{
    using Interfaces;
    using System;
    using System.Threading.Tasks;

    public static class UCT
    {
        public static IMove ComputeUCT (IGameState gameState, int itermax, bool verbose, Action<string> printfn)
        {
            var rootNode = new Node(null, null, gameState);
            var player = gameState.CurrentPlayer();


            Parallel.For(0, itermax,
                i =>
                {
                    var node = rootNode;
                    var state = gameState;
                    //for (var i = 0; i < itermax; i++)
                    // Select
                    while (node.NodeIsFullyExpandedAndNonterminal)
                    {
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
             );
            if (verbose)
            {
                printfn(rootNode.TreeToString(0));
            }

            return rootNode.MostVisitedMove();
        }
    }
}
