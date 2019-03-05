using BearLib;
using GeomaceRL.UI;
using System;

namespace GeomaceRL.Animation
{
    internal class MoveAnimation : IAnimation
    {
        public int Turn { get; } = EventScheduler.Turn;
        public TimeSpan Duration { get; } = Game.FrameRate * 4;
        public TimeSpan StartTime { get; }
        public TimeSpan EndTime { get; }

        private readonly Actor.Actor _source;
        private readonly Loc _prev;

        private readonly int _dx;
        private readonly int _dy;

        public MoveAnimation(Actor.Actor source, in Loc prev)
        {
            _source = source;
            _prev = prev;

            _dx = source.Pos.X - prev.X;
            _dy = source.Pos.Y - prev.Y;

            StartTime = Game.Ticks;
            EndTime = StartTime + Duration;

            source.ShouldDraw = false;
        }

        public bool Update()
        {
            if (Game.Ticks >= EndTime)
            {
                Cleanup();
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Cleanup()
        {
            _source.ShouldDraw = true;
        }

        public void Draw(LayerInfo layer)
        {
            double moveFrac = (Game.Ticks - StartTime) / Duration;
            int xFrac = (int)(_dx * moveFrac * Terminal.State(Terminal.TK_CELL_WIDTH));
            int yFrac = (int)(_dy * moveFrac * Terminal.State(Terminal.TK_CELL_HEIGHT));

            Terminal.Color(_source.Color);
            Terminal.Layer(layer.Z + 1);
            Terminal.PutExt(
                layer.X + _prev.X - Camera.X,
                layer.Y + _prev.Y - Camera.Y,
                xFrac, yFrac, _source.Symbol);
            Terminal.Layer(layer.Z);
        }
    }
}
