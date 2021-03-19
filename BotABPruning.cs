using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reversi
{
    class BotABPruning : Bot
    {
        BotReversi Game;
        int Depth = 1;

        public int NodesVisited { get; private set; }
        public int Eval { get; private set; }
        public int Pruned { get; private set; }

        public BotABPruning(Reversi Game)
        {
            SubscribeToGame(Game);
        }

        public void SetDepth(int Depth)
            => this.Depth = Depth;

        public override (int X, int Y) RequestMove()
        {
            NodesVisited = 0;
            Pruned = 0;
            (int Index, int Eval) BestMove = (-1, Game.Turn ? int.MinValue : int.MaxValue);
            List<(int, int, Directions)> ValidMoves = Game.ValidMoves;
            for (int i = 0; i < ValidMoves.Count; i++)
            {
                BotReversi Node = new BotReversi(Game);
                Node.MakeMove(i);
                (int Index, int Eval) Move = (i, ABPruning(Node, Depth, int.MinValue, int.MaxValue));
                if (Game.Turn == BestMove.Eval < Move.Eval)
                    BestMove = Move;
            }
            (int X, int Y, Directions _) = ValidMoves[BestMove.Index];
            Eval = BestMove.Eval;
            return (X, Y);
        }

        int ABPruning(BotReversi Game, int Depth, int Alpha, int Beta)
        {
            NodesVisited++;
            if (Depth == 0 || Game.ValidMoves.Count == 0)
                return Game.DarkDisks - Game.LightDisks;
            int Eval;
            if (Game.Turn)
            {
                Eval = int.MinValue;
                foreach ((int X, int Y, Directions _) in Game.ValidMoves)
                {
                    BotReversi Node = new BotReversi(Game);
                    Node.MakeMove(X, Y);
                    int Result = ABPruning(Node, Depth - 1, Alpha, Beta);
                    if (Result > Eval)
                        if ((Eval = Result) > Alpha)
                            if ((Alpha = Result) >= Beta)
                            {
                                Pruned++;
                                break;
                            }
                }
                return Eval;
            }
            Eval = int.MaxValue;
            foreach ((int X, int Y, Directions _) in Game.ValidMoves)
            {
                BotReversi Node = new BotReversi(Game);
                Node.MakeMove(X, Y);
                int Result = ABPruning(Node, Depth - 1, Beta, Alpha);
                if (Result < Eval)
                    if ((Eval = Result) < Beta)
                        if ((Beta = Result) <= Alpha)
                        {
                            Pruned++;
                            break;
                        }
            }
            return Eval;
        }

        public override void SubscribeToGame(Reversi Game)
        {
            this.Game = new BotReversi(Game);
        }
    }
}
