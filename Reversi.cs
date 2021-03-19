using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Reversi
{
    public class Reversi
    {
        public bool Turn { get; private set; }

        public bool?[,] Board { get { return (bool?[,])board.Clone(); } }
        bool?[,] board;
        bool?[,] startingboard = new bool?[,]
        {
            { null, null, null, null, null, null, null, null },
            { null, null, null, null, null, null, null, null },
            { null, null, null, null, null, null, null, null },
            { null, null, null, false, true, null, null, null },
            { null, null, null, true, false, null, null, null },
            { null, null, null, null, null, null, null, null },
            { null, null, null, null, null, null, null, null },
            { null, null, null, null, null, null, null, null }
        };
        int width, height;

        public (int X, int Y)[] ValidMoves { get { return validmoves.ToArray(); } }
        readonly List<(int X, int Y)> validmoves;

        public bool Finished { get; private set; }

        public bool? Winner { get; private set; }

        public void SetStartingBoard(bool?[,] Board)
            => startingboard = (bool?[,])Board.Clone();

        public Reversi()
        {
            Finished = false;
            validmoves = new List<(int X, int Y)>();
            Reset();
        }

        public delegate void NotifyGameReset();
        public event NotifyGameReset GameReset;

        protected virtual void OnGameReset()
            => GameReset?.Invoke();

        public void Reset()
        {

            board = (bool?[,])startingboard.Clone();
            width = board.GetLength(0);
            height = board.GetLength(1);
            Turn = true;
            GetValidMoves();
            if (validmoves.Count == 0)
            {
                Finished = true;
                DetermineWinner();
            }
            else
                Finished = false;

            OnGameReset();
        }

        public delegate void NotifyMoveMade(int X, int Y);
        public event NotifyMoveMade MoveMade;

        protected virtual void OnMoveMade(int X, int Y)
            => MoveMade?.Invoke(X, Y);

        // returns whether the move was valid or not
        public bool MakeMove((int X, int Y) Move)
            => MakeMove(Move.X, Move.Y);

        public bool MakeMove(int X, int Y)
        {
            if (Finished || board[X, Y] != null || !validmoves.Contains((X, Y)))
                return false;

            board[X, Y] = Turn;

            // flip disks
            foreach ((int X, int Y) direction in Directions)
            {
                int x = X + direction.X;
                int y = Y + direction.Y;
                if (x >= 0 && x < width && y >= 0 && y < height && board[x, y] == !Turn)
                {
                    x += direction.X;
                    y += direction.Y;
                    while (x >= 0 && x < width && y >= 0 && y < height)
                    {
                        if (board[x, y] == Turn)
                        {
                            x -= direction.X;
                            y -= direction.Y;
                            while (x != X || y != Y)
                            {
                                board[x, y] = Turn;
                                x -= direction.X;
                                y -= direction.Y;
                            }
                            goto next;
                        }
                        else if (board[x, y] == null)
                            goto next;
                        x += direction.X;
                        y += direction.Y;
                    }
                }
                next:;
            }

            Turn = !Turn;
            GetValidMoves();
            if (validmoves.Count == 0)
            {
                Finished = true;
                DetermineWinner();
            }

            OnMoveMade(X, Y);

            return true;
        }

        void DetermineWinner()
        {
            int diff = 0;
            foreach (bool? piece in board)
                if (piece == true) // black
                    diff++;
                else if (piece == false) // white
                    diff--;

            // diff > 0 -> more black disks
            // diff < 0 -> more white disks
            // diff = 0 -> equal amount of black and white disks
            Winner = diff > 0 ? true : (diff < 0 ? (bool?)false : null);
        }

        /*
         * used for the eight directions to check for neighbouring disks
         * X X X
         *  \|/
         * X-O-X
         *  /|\
         * X X X
         */
        readonly (int X, int Y)[] Directions = new (int X, int Y)[]
        {
            (1, 0),
            (1, 1),
            (0, 1),
            (-1, 1),
            (-1, 0),
            (-1, -1),
            (0, -1),
            (1, -1)
        };

        void GetValidMoves()
        {
            validmoves.Clear();
            for (int X = 0; X < width; X++)
                for (int Y = 0; Y < height; Y++)
                    if (board[X, Y] == null)
                    {
                        foreach ((int x, int y) direction in Directions)
                        {
                            int x = X + direction.x;
                            int y = Y + direction.y;
                            if (x >= 0 && x < width && y >= 0 && y < height && board[x, y] == !Turn)
                            {
                                x += direction.x;
                                y += direction.y;
                                while (x >= 0 && x < width && y >= 0 && y < height)
                                {
                                    if (board[x, y] == Turn)
                                    {
                                        validmoves.Add((X, Y));
                                        goto valid;
                                    }
                                    if (board[x, y] == null)
                                        break;
                                    x += direction.x;
                                    y += direction.y;
                                }
                            }
                        }
                        valid:;
                    }
        }

        public void PrintBoard()
        {
            for (int Y = 0; Y < width; Y++)
            {
                for (int X = 0; X < height; X++)
                    Console.Write(board[X, Y] == true ? 'B' : board[X, Y] == false ? 'W' : '_');
                Console.WriteLine();
            }
        }

        const int size = 64;
        readonly int halfsize = size / 2;
        readonly int sizeminusone = size - 1;
        readonly Pen p = new Pen(Color.Black, 1);
        readonly SolidBrush brush = new SolidBrush(Color.Black);
        readonly Font f = new Font(FontFamily.GenericSansSerif, 22, FontStyle.Bold);
        readonly Color validmoveb = Color.FromArgb(127, 0, 0, 0), validmovew = Color.FromArgb(127, 255, 255, 255);
        Bitmap image, background;
        Graphics g;
        int lastw = 0, lasth = 0;
        public Bitmap CreateImage()
        {
            int w = width + 1, h = height + 1;
            if (lastw != width || lasth != height || image != null /* in case the image gets disposed from outside */)
            {
                if (image != null)
                {
                    image.Dispose();
                    background.Dispose();
                    g.Dispose();
                }
                background = new Bitmap(width * size, height * size);
                g = Graphics.FromImage(background);
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.Clear(Color.FromArgb(135, 199, 255));

                // draw vertical lines
                brush.Color = Color.Black;
                int drawy = height * size;
                for (int x = 1; x <= width; x++)
                    g.DrawLine(p, x * size - 1, 0, x * size - 1, drawy);

                // draw horizontal lines
                int drawx = width * size;
                for (int y = 1; y <= height; y++)
                    g.DrawLine(p, 0, y * size - 1, drawx, y * size - 1);

                g.Dispose();

                image = new Bitmap(w * size, h * size);
                g = Graphics.FromImage(image);
                g.Clear(Color.White);
                g.SmoothingMode = SmoothingMode.HighQuality;

                // draw vertical lines and column text
                drawy += sizeminusone;
                for (int x = 1; x <= w; x++)
                {
                    drawx = x * size - 1;
                    g.DrawLine(p, drawx, 0, drawx, drawy);
                    if (x == 1)
                        continue;
                    string text = (x - 1).ToString();
                    SizeF textsize = g.MeasureString(text, f);
                    g.DrawString(text, f, brush, drawx - halfsize - textsize.Width / 2, halfsize - textsize.Height / 2);
                }

                // draw horizontal lines and row text
                drawx += sizeminusone;
                for (int y = 1; y <= h; y++)
                {
                    drawy = y * size - 1;
                    g.DrawLine(p, 0, drawy, drawx, drawy);
                    if (y == 1)
                        continue;
                    string text = (y - 1).ToString();
                    SizeF textsize = g.MeasureString(text, f);
                    g.DrawString(text, f, brush, halfsize - textsize.Width / 2, drawy - halfsize - textsize.Height / 2);
                }

                g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                lastw = width;
                lasth = height;
            }

            // apply the background to the main image
            g.DrawImage(background, size, size);

            Color validmovecolor = Turn ? validmoveb : validmovew;

            // draw disks
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    if (board[x, y] != null)
                    {
                        brush.Color = board[x, y] == true ? Color.Black : Color.White;
                        g.FillEllipse(brush, (x + 1) * size, (y + 1) * size, sizeminusone, sizeminusone);
                    }
                    else if (validmoves.Contains((x, y)))
                    {
                        brush.Color = validmovecolor;
                        g.FillEllipse(brush, (x + 1) * size, (y + 1) * size, sizeminusone, sizeminusone);
                    }

            return image;
        }
    }
}
