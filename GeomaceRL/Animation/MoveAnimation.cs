using BearLib;
using GeomaceRL.UI;
using System;

namespace GeomaceRL.Animation
{
    internal class MoveAnimation : IAnimation
    {
        public int Turn { get; } = EventScheduler.Turn;
        public TimeSpan Duration { get; } = Game.FrameRate * 2;
        public TimeSpan CurrentTime { get; private set; }
        public TimeSpan EndTime { get; }

        internal readonly Actor.Actor _source;
        private readonly Loc _prev;
        internal bool _multmove;

        private readonly int _dx;
        private readonly int _dy;

        public MoveAnimation(Actor.Actor source, in Loc prev, bool multmove = false)
        {
            _source = source;
            _prev = prev;
            _multmove = multmove;

            _dx = source.Pos.X - prev.X;
            _dy = source.Pos.Y - prev.Y;

            CurrentTime = TimeSpan.Zero;
            EndTime = CurrentTime + Duration;

            _source.ShouldDraw = false;
        }

        public bool Update(TimeSpan dt)
        {
            CurrentTime += dt;
            if (CurrentTime >= EndTime)
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
            _source.Moving = false;
            _source.ShouldDraw = true;
        }

        public void Draw(LayerInfo layer)
        {
            if (!Game.MapHandler.Field[_source.Pos].IsVisible)
                return;

            if (CurrentTime == TimeSpan.Zero)
            {
                if (!_multmove)
                {
                    Terminal.Color(_source.Color);
                    layer.Put(_prev.X - Camera.X, _prev.Y - Camera.Y, _source.Symbol);
                }
            }
            else
            {
                double moveFrac = CurrentTime / Duration;
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
}
