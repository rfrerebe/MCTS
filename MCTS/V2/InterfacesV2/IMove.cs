namespace MCTS.V2.Interfaces
{
    public interface IMove
    {
        string Name { get; }

        IGameState DoMove();
    }
}
