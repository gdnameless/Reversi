using System;
using System.Collections.Generic;
using System.Numerics;

namespace Reversi
{
    // implementation intended for bots to use

    [Flags]
    public enum Directions
    {
        None = 0,
        Left = 1,
        Right = 2,
        Top = 4,
        Bottom = 8,
        TopLeft = 16,
        BottomLeft = 32,
        TopRight = 64,
        BottomRight = 128
    }

    class BotReversi
    {
        public bool Turn;
        public bool?[,] Board;
        public int BoardWidth, BoardHeight, DarkDisks, LightDisks;
        public readonly List<(int X, int Y, Directions)> ValidMoves;
        public readonly HashSet<(int X, int Y)> PotentialMoves;
        int InnerBoardLeft = 2, InnerBoardTop = 2, InnerBoardRight, InnerBoardBottom;
        Reversi Game;

        public BotReversi(Reversi Game)
        {
            ValidMoves = new List<(int X, int Y, Directions)>();
            PotentialMoves = new HashSet<(int X, int Y)>();
            SubscribeToGame(Game);
        }

        public BotReversi(BotReversi BotGame)
        {
            Board = (bool?[,])BotGame.Board.Clone();
            ValidMoves = new List<(int X, int Y, Directions)>(BotGame.ValidMoves);
            PotentialMoves = new HashSet<(int X, int Y)>(BotGame.PotentialMoves);
            DarkDisks = BotGame.DarkDisks;
            LightDisks = BotGame.LightDisks;
            Turn = BotGame.Turn;
            BoardWidth = BotGame.BoardWidth;
            BoardHeight = BotGame.BoardHeight;
            InnerBoardBottom = BotGame.InnerBoardBottom;
            InnerBoardLeft = BotGame.InnerBoardLeft;
            InnerBoardRight = BotGame.InnerBoardRight;
            InnerBoardTop = BotGame.InnerBoardTop;
        }

        public void SubscribeToGame(Reversi Game)
        {
            PotentialMoves.Clear();

            if (this.Game != Game)
            {
                if (this.Game != null)
                {
                    this.Game.GameReset -= OnGameReset;
                    this.Game.MoveMade -= OnMoveMade;
                }
                this.Game = Game;
                Game.GameReset += OnGameReset;
                Game.MoveMade += OnMoveMade;
            }

            DarkDisks = 0;
            LightDisks = 0;
            Board = (bool?[,])Game.Board.Clone();
            BoardWidth = Board.GetLength(0);
            BoardHeight = Board.GetLength(1);
            InnerBoardRight = BoardWidth - 2;
            InnerBoardBottom = BoardHeight - 2;
            Turn = Game.Turn;

            int w = BoardWidth - 1, h = BoardHeight - 1;

            // check the corners of the board
            // top left
            if (CheckSquare(0, 0))
                if (Board[1, 1] != null ||
                    Board[0, 1] != null ||
                    Board[1, 0] != null)
                    PotentialMoves.Add((0, 0));

            // top right
            if (CheckSquare(w, 0))
                if (Board[w - 1, 1] != null ||
                    Board[w, 1] != null ||
                    Board[w - 1, 0] != null)
                    PotentialMoves.Add((w, 0));

            // bottom right
            if (CheckSquare(w, h))
                if (Board[w - 1, h - 1] != null ||
                    Board[w, h - 1] != null ||
                    Board[w - 1, h] != null)
                    PotentialMoves.Add((w, h));

            // bottom left
            if (CheckSquare(0, h))
                if (Board[1, h - 1] != null ||
                    Board[0, h - 1] != null ||
                    Board[1, h] != null)
                    PotentialMoves.Add((0, h));

            // check the edges of the board
            // top
            for (int x = 1; x < w; x++)
                if (CheckSquare(x, 0))
                    if (Board[x, 1] != null ||
                        Board[x + 1, 0] != null ||
                        Board[x - 1, 0] != null ||
                        Board[x + 1, 1] != null ||
                        Board[x - 1, 1] != null)
                        PotentialMoves.Add((x, 0));

            // bottom
            for (int x = 1; x < w; x++)
                if (CheckSquare(x, h))
                    if (Board[x, h - 1] != null ||
                        Board[x + 1, h] != null ||
                        Board[x - 1, h] != null ||
                        Board[x + 1, h - 1] != null ||
                        Board[x - 1, h - 1] != null)
                        PotentialMoves.Add((x, h));

            // left
            for (int y = 1; y < h; y++)
                if (CheckSquare(0, y))
                    if (Board[1, y] != null ||
                        Board[0, y + 1] != null ||
                        Board[0, y - 1] != null ||
                        Board[1, y + 1] != null ||
                        Board[1, y - 1] != null)
                        PotentialMoves.Add((0, y));

            // right
            for (int y = 1; y < h; y++)
                if (CheckSquare(w, y))
                    if (Board[w - 1, y] != null ||
                        Board[w, y + 1] != null ||
                        Board[w, y - 1] != null ||
                        Board[w - 1, y + 1] != null ||
                        Board[w - 1, y - 1] != null)
                        PotentialMoves.Add((w, y));

            // check the rest of the board
            for (int x = 1; x < w; x++)
                for (int y = 1; y < h; y++)
                    if (CheckSquare(x, y))
                        if (Board[x + 1, y] != null ||
                            Board[x + 1, y + 1] != null ||
                            Board[x, y + 1] != null ||
                            Board[x - 1, y + 1] != null ||
                            Board[x - 1, y] != null ||
                            Board[x - 1, y - 1] != null ||
                            Board[x, y - 1] != null ||
                            Board[x + 1, y - 1] != null)
                            PotentialMoves.Add((x, y));

            GetValidMoves();
        }

        public BigInteger GetPosition()
        {
            BigInteger Position = 0, BaseValue = 1;
            foreach (bool? cell in Board)
            {
                Position += BaseValue * (cell == null ? 0 : cell == true ? 1 : 2);
                BaseValue *= 3;
            }
            Position += BaseValue * (Turn ? 0 : 1);
            return Position;
        }

        public delegate void NotifyMoveMade(int MoveIndex);
        public event NotifyMoveMade MoveMade;

        void OnMoveMade(int X, int Y)
        {
            MakeMove(X, Y);
        }

        public delegate void NotifyGameReset();
        public event NotifyGameReset GameReset;

        void OnGameReset()
        {
            SubscribeToGame(Game);
            GameReset?.Invoke();
        }

        bool CheckSquare(int x, int y)
        {
            switch (Board[x, y])
            {
                case true:
                    DarkDisks++;
                    return false;
                case false:
                    LightDisks++;
                    return false;
                default:
                    return true;
            }
        }

        public void GetValidMoves()
        {
            ValidMoves.Clear();
            foreach ((int X, int Y) in PotentialMoves)
            {
                if (Board[X, Y] != null)
                    continue;

                int x, y;
                bool checkleft = X >= InnerBoardLeft,
                    checkright = X <= InnerBoardRight,
                    checktop = Y >= InnerBoardTop,
                    checkbottom = Y <= InnerBoardBottom;
                Directions dirs = Directions.None;

                if (checkleft)
                {
                    x = X - 1;
                    y = Y;
                    if (Board[x, y] == !Turn)
                        while (--x >= 0)
                            if (Board[x, y] == Turn)
                            {
                                dirs |= Directions.Left;
                                break;
                            }
                            else if (Board[x, y] == null)
                                break;
                }
                if (checkright)
                {
                    x = X + 1;
                    y = Y;
                    if (Board[x, y] == !Turn)
                        while (++x < BoardWidth)
                            if (Board[x, y] == Turn)
                            {
                                dirs |= Directions.Right;
                                break;
                            }
                            else if (Board[x, y] == null)
                                break;
                }
                if (checktop)
                {
                    x = X;
                    y = Y - 1;
                    if (Board[x, y] == !Turn)
                        while (--y >= 0)
                            if (Board[x, y] == Turn)
                            {
                                dirs |= Directions.Top;
                                break;
                            }
                            else if (Board[x, y] == null)
                                break;
                }
                if (checkbottom)
                {
                    x = X;
                    y = Y + 1;
                    if (Board[x, y] == !Turn)
                        while (++y < BoardHeight)
                            if (Board[x, y] == Turn)
                            {
                                dirs |= Directions.Bottom;
                                break;
                            }
                            else if (Board[x, y] == null)
                                break;
                }
                if (checkleft && checktop)
                {
                    x = X - 1;
                    y = Y - 1;
                    if (Board[x, y] == !Turn)
                        while (--x >= 0 && --y >= 0)
                            if (Board[x, y] == Turn)
                            {
                                dirs |= Directions.TopLeft;
                                break;
                            }
                            else if (Board[x, y] == null)
                                break;
                }
                if (checkleft && checkbottom)
                {
                    x = X - 1;
                    y = Y + 1;
                    if (Board[x, y] == !Turn)
                        while (--x >= 0 && ++y < BoardHeight)
                            if (Board[x, y] == Turn)
                            {
                                dirs |= Directions.BottomLeft;
                                break;
                            }
                            else if (Board[x, y] == null)
                                break;
                }
                if (checkright && checktop)
                {
                    x = X + 1;
                    y = Y - 1;
                    if (Board[x, y] == !Turn)
                        while (++x < BoardWidth && --y >= 0)
                            if (Board[x, y] == Turn)
                            {
                                dirs |= Directions.TopRight;
                                break;
                            }
                            else if (Board[x, y] == null)
                                break;
                }
                if (checkright && checkbottom)
                {
                    x = X + 1;
                    y = Y + 1;
                    if (Board[x, y] == !Turn)
                        while (++x < BoardWidth && ++y < BoardHeight)
                            if (Board[x, y] == Turn)
                            {
                                dirs |= Directions.BottomRight;
                                break;
                            }
                            else if (Board[x, y] == null)
                                break;
                }
                if (dirs != Directions.None)
                    ValidMoves.Add((X, Y, dirs));
            }
        }

        public void MakeMove((int X, int Y) Move)
            => MakeMove(Move.X, Move.Y);

        public void MakeMove(int X, int Y)
        {
            for (int i = 0; i < ValidMoves.Count; i++)
                if (X == ValidMoves[i].X && Y == ValidMoves[i].Y)
                {
                    MakeMove(i);
                    return;
                }
            throw new Exception("Tried to make an invalid move");
        }

        public void MakeMove(int ValidMoveIndex)
        {
            if (ValidMoveIndex < 0 || ValidMoveIndex >= ValidMoves.Count)
                throw new Exception("Tried to make an invalid move");
            MoveMade?.Invoke(ValidMoveIndex);
            (int X, int Y, Directions dirs) = ValidMoves[ValidMoveIndex];
            PotentialMoves.Remove((X, Y));
            if (X > 0)
                PotentialMoves.Add((X - 1, Y));
            if (Y > 0)
                PotentialMoves.Add((X, Y - 1));
            if (X < BoardWidth - 1)
                PotentialMoves.Add((X + 1, Y));
            if (Y < BoardHeight - 1)
                PotentialMoves.Add((X, Y + 1));
            if (X > 0 && Y > 0)
                PotentialMoves.Add((X - 1, Y - 1));
            if (X < BoardWidth - 1 && Y > 0)
                PotentialMoves.Add((X + 1, Y - 1));
            if (X > 0 && Y < BoardHeight - 1)
                PotentialMoves.Add((X - 1, Y + 1));
            if (X < BoardWidth - 1 && Y < BoardHeight - 1)
                PotentialMoves.Add((X + 1, Y + 1));

            int disksflipped = 0;
            if ((dirs & Directions.Right) == Directions.Right)
            {
                int x = X + 1;
                do
                {
                    Board[x++, Y] = Turn;
                    disksflipped++;
                }
                while (Board[x, Y] != Turn);
            }
            if ((dirs & Directions.Left) == Directions.Left)
            {
                int x = X - 1;
                do
                {
                    Board[x--, Y] = Turn;
                    disksflipped++;
                }
                while (Board[x, Y] != Turn);
            }
            if ((dirs & Directions.Top) == Directions.Top)
            {
                int y = Y - 1;
                do
                {
                    Board[X, y--] = Turn;
                    disksflipped++;
                }
                while (Board[X, y] != Turn);
            }
            if ((dirs & Directions.Bottom) == Directions.Bottom)
            {
                int y = Y + 1;
                do
                {
                    Board[X, y++] = Turn;
                    disksflipped++;
                }
                while (Board[X, y] != Turn);
            }
            if ((dirs & Directions.TopRight) == Directions.TopRight)
            {
                int x = X + 1, y = Y - 1;
                do
                {
                    Board[x++, y--] = Turn;
                    disksflipped++;
                }
                while (Board[x, y] != Turn);
            }
            if ((dirs & Directions.BottomRight) == Directions.BottomRight)
            {
                int x = X + 1, y = Y + 1;
                do
                {
                    Board[x++, y++] = Turn;
                    disksflipped++;
                }
                while (Board[x, y] != Turn);
            }
            if ((dirs & Directions.TopLeft) == Directions.TopLeft)
            {
                int x = X - 1, y = Y - 1;
                do
                {
                    Board[x--, y--] = Turn;
                    disksflipped++;
                }
                while (Board[x, y] != Turn);
            }
            if ((dirs & Directions.BottomLeft) == Directions.BottomLeft)
            {
                int x = X - 1, y = Y + 1;
                do
                {
                    Board[x--, y++] = Turn;
                    disksflipped++;
                }
                while (Board[x, y] != Turn);
            }

            Board[X, Y] = Turn;

            if (Turn)
            {
                DarkDisks += disksflipped + 1;
                LightDisks -= disksflipped;
            }
            else
            {
                DarkDisks -= disksflipped;
                LightDisks += disksflipped + 1;
            }

            Turn = !Turn;

            GetValidMoves();
        }
    }
}
