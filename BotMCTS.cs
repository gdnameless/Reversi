using System;
using System.Collections.Generic;

namespace Reversi
{
    public class BotMCTS : Bot
    {
        BotReversi Game;
        public int Playouts { get; private set; }
        public int Nodes { get; internal set; } = 0;
        static readonly Random r = new Random();

        public BotMCTS(Reversi Game)
        {
            SubscribeToGame(Game);
        }

        public override void SubscribeToGame(Reversi Game)
        {
            if (this.Game != null)
            {
                this.Game.GameReset -= OnGameReset;
                this.Game.MoveMade -= OnMoveMade;
            }
            this.Game = new BotReversi(Game);
            this.Game.GameReset += OnGameReset;
            this.Game.MoveMade += OnMoveMade;
        }

        public (int X, int Y, float Value)[] Evaluation;

        public int Timeout { get; private set; } = 1000;
        public void SetTimeout(int ms)
            => Timeout = ms;

        public int MaxNodes { get; private set; } = 500;
        public void SetMaxNodes(int Nodes)
            => MaxNodes = Nodes;

        void OnMoveMade(int MoveIndex)
        {
            if (Root == null)
                return;
            Nodes--;
            Root = Root.Choose(MoveIndex);
            Root.CreateChildren();
            GC.Collect();
        }

        void OnGameReset()
        {
            Root = null;
            GC.Collect();
            Nodes = 0;
        }

        Node Root;

        public override (int X, int Y) RequestMove()
        {
            if (Game.ValidMoves.Count < 1)
                return (-1, -1);
            Playouts = 0;
            if (Root == null)
            {
                Root = new Node(new BotReversi(Game), null, this);
                Root.CreateChildren();
            }
            Evaluation = new (int X, int Y, float Value)[Root.Children.Length];
            int End = Environment.TickCount + Timeout;
            while (Environment.TickCount < End)
            {
                if (Playouts > 1000)
                    Playouts = Playouts;
                int Move = r.Next(Game.ValidMoves.Count);
                Root.Select(Move);
                Playouts++;
            }
            float Eval = float.NegativeInfinity;
            int Index = -1;
            for (int i = 0; i < Root.Children.Length; i++)
            {
                Node n = Root.Children[i];
                float e = 100 - (n == null ? 0 : (float)n.Wins / n.Total * 100);
                if (e > Eval)
                {
                    Eval = e;
                    Index = i;
                }
                Evaluation[i] = (Game.ValidMoves[i].X, Game.ValidMoves[i].Y, e);
            }
            return (Game.ValidMoves[Index].X, Game.ValidMoves[Index].Y);
        }

        class Node
        {
            static readonly Random r = BotMCTS.r;

            public Node Parent;
            public Node[] Children;
            public BotReversi Game;
            public bool Decided, Winner;
            public int Wins, Total;
            readonly BotMCTS Bot;

            public Node(BotReversi Game, Node Parent, BotMCTS Bot)
            {
                this.Game = Game;
                Decided = Game.ValidMoves.Count == 0;
                if (Decided)
                {
                    Winner = Game.DarkDisks > Game.LightDisks || (Game.LightDisks <= Game.DarkDisks && r.Next(2) < 1);
                    Children = new Node[0];
                }
                else
                    Children = new Node[Game.ValidMoves.Count];
                this.Parent = Parent;
                this.Bot = Bot;
                Bot.Nodes++;
            }

            public void CreateChildren()
            {
                if (!Decided)
                {
                    for (int i = 0; i < Children.Length; i++)
                    {
                        Node n = Children[i];
                        if (n == null)
                        {
                            BotReversi temp = new BotReversi(Game);
                            temp.MakeMove(i);
                            Children[i] = new Node(temp, this, Bot);
                        }
                    }
                }
            }

            public Node Choose(int Move)
            {
                Node result = Children[Move];
                if (result == null)
                {
                    Bot.Nodes = 0;
                    return null;
                }
                foreach (Node n in Children)
                {
                    if (n == null || n == result)
                        continue;
                    Bot.Nodes -= n.CountChildren();
                }
                result.Parent = null;
                return result;
            }

            public int CountChildren()
            {
                int children = 1;
                foreach (Node n in Children)
                    if (n != null)
                        children += n.CountChildren();
                return children;
            }

            // 1. Selection
            public void Select(int Move)
            {
                Node Child = Children[Move];
                if (Child != null)
                    if (Child.Decided)
                        Backpropagate(Child.Winner == Game.Turn);
                    else
                        Child.Select(r.Next(Child.Children.Length));
                else
                    Expand(Move);
            }

            // 2. Expansion
            public void Expand(int Move)
            {
                BotReversi g = new BotReversi(Game);
                g.MakeMove(Move);
                if (Bot.Nodes < Bot.MaxNodes)
                {
                    Children[Move] = new Node(g, this, Bot);
                    Children[Move].Simulate();
                }
                else
                {
                    List<(int, int, Directions)> ValidMoves = g.ValidMoves;
                    while (ValidMoves.Count > 0)
                        g.MakeMove(r.Next(ValidMoves.Count));
                    bool Win;
                    if (g.DarkDisks == g.LightDisks)
                        Win = r.Next(2) < 1;
                    else
                        Win = g.DarkDisks > g.LightDisks == Game.Turn;
                    Backpropagate(Win);
                }
            }

            // 3. Simulation
            public void Simulate()
            {
                BotReversi g = new BotReversi(Game);
                List<(int, int, Directions)> ValidMoves = g.ValidMoves;
                while (ValidMoves.Count > 0)
                    g.MakeMove(r.Next(ValidMoves.Count));
                bool Win;
                if (g.DarkDisks == g.LightDisks)
                    Win = r.Next(2) < 1;
                else
                    Win = g.DarkDisks > g.LightDisks == Game.Turn;
                Backpropagate(Win);
            }

            // 4. Backpropagation
            public void Backpropagate(bool Win)
            {
                Total++;
                if (Win)
                    Wins++;
                Node p = Parent;
                while (p != null)
                {
                    p.Total++;
                    Win = !Win;
                    if (Win)
                        p.Wins++;
                    p = p.Parent;
                }
            }
        }
    }
}
