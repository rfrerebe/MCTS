
namespace MCTS
{
    using Interfaces;

    public static class UCT
    {
        public static IMove ComputeUCT (IGameState gameState, int itermax, bool verbose)
        {
            var rootNode = new Node(null, null, gameState);

            for (var i = 0; i < itermax; i++)
            {
                var node = rootNode;
                var state = gameState.Clone();

                // Select
                while (node.NodeIsFullyExpandedAndNonterminal)
                {
                    node = node.UCTSelectChild();
                    state.DoMove(node.Move);
                }

                // Expand
                var result = node.GetRandomMoveOrIsFalse();
                if (result.Item1)
                {
                    var move = result.Item2;
                    state.DoMove(move);
                    node = node.AddChild(move, state);
                }

                // Rollout
                state.PlayRandomlyUntilTheEnd();

                // Backpropagate
                while (node != null)
                {
                    node.Update(state.IsGameWon(node.PlayerJustMoved));
                    node = node.ParentNode;
                }
            }
            if (verbose)
            {
                rootNode.TreeToString(0);
            }

            return rootNode.MostVisitedMove();
        }
    }
}
