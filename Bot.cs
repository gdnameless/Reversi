using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reversi
{
    public abstract class Bot
    {
        public abstract (int X, int Y) RequestMove();

        public abstract void SubscribeToGame(Reversi Game);
    }
}
