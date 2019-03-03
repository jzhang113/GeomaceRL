using BearLib;
using GeomaceRL.UI;

namespace GeomaceRL.Animation
{
    internal class MoveAnimation : IAnimation
    {
        public int Turn { get; } = EventScheduler.Turn;

        private const int _MAX_FRAME = 4;

        private readonly Actor.Actor _source;
        private readonly Loc _prev;

        private readonly int _dx;
        private readonly int _dy;
        private int _frame;

        public MoveAnimation(Actor.Actor source, in Loc prev)
        {
            _source = source;
            _prev = prev;

            _dx = source.Pos.X - prev.X;
            _dy = source.Pos.Y - prev.Y;
            _frame = 0;
        }

        public bool Update()
        {
            if (_frame >= _MAX_FRAME)
            {
                return true;
            }
            else
            {
                _frame++;
                return false;
            }
        }

        public void Draw(LayerInfo layer)
        {
            double moveFrac = (double)_frame / _MAX_FRAME;
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
