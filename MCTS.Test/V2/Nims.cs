using System;
using System.Collections.Generic;
using MCTS.Enum;
using MCTS.V2.Interfaces;
using System.Linq;

namespace MCTSTest
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
        private int remaining;
        private int player;

        public NimsMove(int remaining, int player)
        {
            if (0 > remaining)
            {
                throw new ArgumentException("Remaining can not be smaller than 0", "remaining");
            }
            if (player !=1 || player != 2)
            {
                throw new ArgumentException("Player can only be 1 or 2", "player");
            }
            this.remaining = remaining;
            this.player = player;
        }

        public string Name
        {
            get
            {
                return this.remaining.ToString();
            }
        }

        public IGameState DoMove()
        {
            return new NimState(this.remaining, this.player);               
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

        /// <summary>
        ///  Used to start game
        /// </summary>
        /// <param name="chips"></param>
        public NimState(int chips)
        {
            this.playerJustMoved = 2;
            this.chips = chips;
        }

        /// <summary>
        /// Used internally by Move class
        /// </summary>
        /// <param name="chips"></param>
        /// <param name="playerJustMoved"></param>
        internal NimState(int chips, int playerJustMoved)
        {
            this.playerJustMoved = playerJustMoved;
            this.chips = chips;
        }

        public IEnumerable<IMove> GetMoves()
        {
            return Enumerable.Range(1, Math.Min(3, this.chips)).Select(m => new NimsMove(this.chips - m, 3 - this.playerJustMoved));
        }

        public IGameState PlayRandomlyUntilTheEnd()
        {
            var r = new Random();
            while(this.chips != 0)
            {
                this.chips = this.chips - r.Next(1, Math.Min(3, this.chips) + 1);
                this.playerJustMoved = 3 - this.playerJustMoved;
            }
            return this;
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
