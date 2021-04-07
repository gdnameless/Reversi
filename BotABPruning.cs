using System.Collections.Generic;

namespace Reversi
{
    public class BotABPruning : Bot
    {
        BotReversi Game;
        int Depth = 4;

        public int NodesVisited { get; private set; }
        public int Eval { get; private set; }
        public int Pruned { get; private set; }

        public BotABPruning(Reversi Game)
        {
            SubscribeToGame(Game);
        }

        public override void SubscribeToGame(Reversi Game)
        {
            this.Game = new BotReversi(Game);
        }

        public void SetDepth(int Depth)
            => this.Depth = Depth;

        public override (int X, int Y) RequestMove()
        {
            NodesVisited = 0;
            Pruned = 0;
            int Alpha = int.MinValue, Beta = int.MaxValue;
            (int Index, int Eval) BestMove = (-1, Game.Turn ? int.MinValue : int.MaxValue);
            List<(int, int, Directions)> ValidMoves = Game.ValidMoves;
            for (int i = 0; i < ValidMoves.Count; i++)
            {
                BotReversi Node = new BotReversi(Game);
                Node.MakeMove(i);
                (int Index, int Eval) Move = (i, ABPruning(Node, Depth, Alpha, Beta));
                if (Game.Turn)
                {
                    if (Move.Eval > BestMove.Eval)
                    {
                        BestMove = Move;
                        if (BestMove.Eval > Alpha)
                            Alpha = BestMove.Eval;
                    }
                }
                else
                {
                    if (Move.Eval < BestMove.Eval)
                    {
                        BestMove = Move;
                        if (BestMove.Eval < Beta)
                            Beta = BestMove.Eval;
                    }
                }
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
                int Result = ABPruning(Node, Depth - 1, Alpha, Beta);
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
    }
}