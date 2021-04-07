using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Reversi
{
    class Program
    {
        /*static int Black = 0, White = 0, Draw = 0;
        static bool StopGames = false;

        static void Main(string[] args)
        {
            Console.Title = "Reversi";
            /*Thread[] Threads = new Thread[Environment.ProcessorCount];
            for (int i = 0; i < Threads.Length; i++)
                Threads[i] = new Thread(new ThreadStart(RunGames));
            Console.SetCursorPosition(0, 4);
            Console.WriteLine($"Running with {Environment.ProcessorCount} threads");
            Console.SetCursorPosition(0, 0);
            PrintScores();*/
            /*Reversi Game = new Reversi();

            BotMCTS Black = new BotMCTS(Game);
            Black.SetTimeout(1000);
            BotABPruning White = new BotABPruning(Game);
            White.SetDepth(6);
            //Game.PrintBoard();

            while (true)
            {
                if (Game.Finished)
                {
                    UpdateScores(Game.Winner);
                    Game.Reset();
                }
                if (Game.Turn)
                {
                    (int X, int Y) Move = Black.RequestMove();
                    Console.Clear();
                    Console.SetCursorPosition(0, 3);
                    Console.WriteLine($"Nodes: {Black.Nodes}");
                    Game.MakeMove(Move);
                    Console.WriteLine($"Nodes: {Black.Nodes}");
                    foreach ((int X, int Y, float Value) in from eval in Black.Evaluation orderby eval.Value descending select eval)
                    {
                        Console.WriteLine($"({X}, {Y}) -> {Value}");
                    }
                }
                else
                {
                    Game.MakeMove(White.RequestMove());
                }
            }

            /*while (!Game.Finished)
            {
                Console.WriteLine($"Turn: {Game.Turn}");
                Console.ReadKey();
                Console.Clear();
                if (Game.Turn)
                {
                    Game.MakeMove(Black.RequestMove());
                    Game.PrintBoard();
                    foreach ((int X, int Y, float Value) in from eval in Black.Evaluation orderby eval.Value descending select eval)
                    {
                        Console.WriteLine($"({X}, {Y}) -> {Value}");
                    }
                }
                else
                {
                    Game.MakeMove(White.RequestMove());
                    Game.PrintBoard();
                    Console.WriteLine(White.Eval);
                }
            }
            Console.WriteLine($"Winner: {Game.Winner}");

            Console.ReadKey();

            /*foreach (Thread t in Threads)
            {
                t.Start();
                Thread.Sleep(50);
            }
            Console.ReadKey();
            foreach (Thread t in Threads)
            {
                StopGames = true;
                t.Join();
            }*/
        /*}

        static void RunGames()
        {
            Reversi Game = new Reversi();
            BotMCTS White = new BotMCTS(Game);
            White.SetTimeout(1000);
            BotABPruning Black = new BotABPruning(Game);
            Black.SetDepth(1);
            while (true)
            {
                if (StopGames)
                    break;
                if (Game.Finished)
                {
                    UpdateScores(Game.Winner);
                    Game.Reset();
                }
                if (Game.Turn)
                    Game.MakeMove(Black.RequestMove());
                else
                    Game.MakeMove(White.RequestMove());
            }
        }

        static void UpdateScores(bool? Outcome)
        {
            if (Outcome == null)
                Draw++;
            else if (Outcome == true)
                Black++;
            else
                White++;
            PrintScores();
        }

        static void PrintScores()
        {
            Console.SetCursorPosition(0, 0);
            Console.WriteLine($"Black: {Black}\nWhite: {White}\nDraws: {Draw}");
        }*/

        private static void Main(string[] args)
        {

        }
    }
}
