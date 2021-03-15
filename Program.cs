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
            int images = 10;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            int width = 0, height = 0;
            for (int i = 0; i < images;)
            {
                if (game.Finished)
                {
                    Bitmap image = game.CreateImage();
                    image.Save($"test{width}-{height}.png");
                    i++;
                    width = r.Next(3, 16);
                    height = r.Next(3, 16);
                    bool?[,] newboard = new bool?[width, height];
                    int w = width / 2, h = height / 2;
                    newboard[w, h] = true;
                    newboard[w + 1, h] = false;
                    newboard[w, h + 1] = false;
                    newboard[w + 1, h + 1] = true;
                    game.SetStartingBoard(newboard);
                    game.Reset();
                }
                (int X, int Y) = game.ValidMoves[r.Next(game.ValidMoves.Length)];
                game.MakeMove(X, Y);
            }
            sw.Stop();
            Console.WriteLine($"{images} images generated in {sw.Elapsed}s (avg. {(float)sw.ElapsedMilliseconds / images}ms)");
            Console.ReadKey();
        }
    }
}
