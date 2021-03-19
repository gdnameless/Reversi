using System;

namespace Reversi
{
    public class BotRandomMove : Bot
    {
        readonly Random r = new Random();

        Reversi Game;

        public BotRandomMove(Reversi Game)
        {
            SubscribeToGame(Game);
        }

        public override (int X, int Y) RequestMove()
        {
            (int X, int Y)[] ValidMoves = Game.ValidMoves;
            return ValidMoves[r.Next(ValidMoves.Length)];
        }

        public override void SubscribeToGame(Reversi Game)
        {
            this.Game = Game;
        }
    }
}
