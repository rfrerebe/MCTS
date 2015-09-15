using System;
using System.Collections.Generic;
using MCTS.Enum;
using MCTS.V1.Interfaces;
using System.Linq;

namespace MCTSTest.V1
{
    public class NimPlayer : IPlayer
    {
        private int player;

        public NimPlayer(int player)
        {
            this.player = player;
        }

        public string Name
        {
            get
            {
                return this.player.ToString();
            }
        }
    }

    public class NimsMove : IMove
    {
        private int move;

        public NimsMove(int move)
        {
            this.move = move;
        }

        public string Name
        {
            get
            {
                return this.move.ToString();
            }
        }
    }

    public class NimState : IGameState
    {
        private int playerJustMoved;

        private int chips;

        IPlayer IGameState.PlayerJustMoved
        {
            get
            {
                return new NimPlayer(this.playerJustMoved);
            }
        }

        public NimState(int chips)
        {
            this.playerJustMoved = 2;
            this.chips = chips;
        }

        private NimState(int chips, int playerJustMoved)
        {
            this.playerJustMoved = playerJustMoved;
            this.chips = chips;
        }

        public IGameState Clone()
        {
            return new NimState(this.chips, this.playerJustMoved);
        }

        public IEnumerable<IMove> GetMoves()
        {
            return Enumerable.Range(1, Math.Min(3, this.chips)).Select(m => new NimsMove(m));
        }

        public void PlayRandomlyUntilTheEnd()
        {
            var r = new Random();
            while(this.chips != 0)
            {
                this.chips = this.chips - r.Next(1, Math.Min(3, this.chips) + 1);
                this.playerJustMoved = 3 - this.playerJustMoved;
            }
        }

        public void DoMove(IMove move)
        {
            int m;
            if(int.TryParse(move.Name, out m))
            {
                if (m >= 1 && m <= 3)
                {
                    this.chips -= m;
                    this.playerJustMoved = 3 - this.playerJustMoved;
                    return;
                }
            }
            throw new InvalidOperationException(string.Format("Can't apply move {0}", move.Name));
        }

        public EGameFinalStatus GetResult(IPlayer player)
        {
            if (this.chips != 0)
            {
                throw new InvalidOperationException("Can't request GetResult if game is not ended");
            }
            int p;
            if (! int.TryParse(player.Name, out p))
            {
                throw new InvalidOperationException(string.Format("Not a proper name, name must be 1 or 2. It was {0}",player.Name));
            }
            if (p == this.playerJustMoved)
            {
                return EGameFinalStatus.GameWon;
            }
            else
            {
                return EGameFinalStatus.GameLost;
            }
        }

        public override string ToString()
        {
            return string.Format("Chips:{0} PlayerJustMoved:{1}", this.chips, this.playerJustMoved);
        }

        public IPlayer PlayerJustMoved()
        {
            return new NimPlayer(this.playerJustMoved);
        }

    }
}
