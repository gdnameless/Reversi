using System;
using System.Diagnostics;
using System.Drawing;

namespace Reversi
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Reversi";
            Reversi game = new Reversi();
            Random r = new Random();
            (int Black, int White, int Draw) = (0, 0, 0);
            int games = 10000;
            Stopwatch sw = new Stopwatch();
            for (int i = 0; i < 20; i++)
            {
                (int X, int Y) = game.ValidMoves[r.Next(game.ValidMoves.Length)];
                game.MakeMove(X, Y);
            }
            sw.Start();
            /*Bitmap image = game.CreateImage();
            image.Save("test.png");
            image.Dispose();
            sw.Stop();
            Console.WriteLine($"Board position:");
            game.PrintBoard();
            Console.WriteLine($"Generating, saving and disposing image finished in {sw.Elapsed}s");
            Console.ReadKey();*/
            for (int i = 0; i < games; i++)
            {
                game.Reset();
                //Console.Clear();
                //game.PrintBoard();
                while (!game.Finished)
                {
                    //Console.ReadKey();
                    (int X, int Y) = game.ValidMoves[r.Next(game.ValidMoves.Length)];
                    game.MakeMove(X, Y);
                    //Console.SetCursorPosition(0, 0);
                    //game.PrintBoard();
                    /*Bitmap image = game.CreateImage();
                    image.Save("test.png");
                    image.Dispose();*/
                }
                if (game.Winner == true)
                    Black++;
                else if (game.Winner == false)
                    White++;
                else
                    Draw++;
                //Console.WriteLine($"Winner: {(game.Winner == true ? "Black" : game.Winner == false ? "White" : "Draw")}");
                //Console.ReadKey();
            }
            sw.Stop();
            Console.WriteLine($"{games} Reversi Random Move Games:\nBlack won {Black} times\nWhite won {White} times\n{Draw} games were drawn\nfinished in {sw.Elapsed}s");
            Console.ReadKey();
        }
    }
}
