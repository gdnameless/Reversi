using System;
using System.Diagnostics;
using System.Drawing;

namespace Reversi
{
    class Program
    {
        static void Main(string[] args)
        {
            /*Console.Title = "Reversi";
            Reversi Game = new Reversi();
            BotABPruning BotAlpha = new BotABPruning(Game);
            Stopwatch movetime = new Stopwatch();
            (int Black, int White) Depth = (4, 3);
            (int Black, int White, int Draws) Wins = (0, 0, 0);
            while (true)
            {
                while (!Game.Finished)
                {
                    int depth = Game.Turn ? Depth.Black : Depth.White;
                    BotAlpha.SetDepth(depth);
                    movetime.Reset();
                    movetime.Start();
                    (int X, int Y) Move = BotAlpha.RequestMove();
                    movetime.Stop();
                    Game.MakeMove(Move);
                    Console.Clear();
                    Game.PrintBoard();
                    Console.WriteLine($"{(!Game.Turn ? "Black" : "White")}'s move: {Move} (depth: {depth} time: {movetime.Elapsed}s)");
                    Console.WriteLine($"Eval: {BotAlpha.Eval}\nNodes visited: {BotAlpha.NodesVisited}\nPrunes: {BotAlpha.Pruned}");
                    Console.ReadKey();
                }
                if (Game.Winner == null)
                {
                    Wins.Draws++;
                }
                else if (Game.Winner == true)
                {
                    Wins.Black++;
                }
                else
                {
                    Wins.White++;
                }
                if (Game.Winner != null)
                    Console.WriteLine($"{(Game.Winner == true ? "Black" : "White")} won!");
                else
                    Console.WriteLine("It's a tie!");
                Console.ReadKey();
                Game.Reset();
            }*/
        }
    }
}
