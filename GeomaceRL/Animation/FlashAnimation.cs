using BearLib;
using GeomaceRL.UI;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace GeomaceRL.Animation
{
    internal class FlashAnimation : IAnimation
    {
        public int Turn { get; } = EventScheduler.Turn;
        public TimeSpan Duration { get; } = Game.FrameRate * 4;
        public TimeSpan CurrentTime { get; private set; }
        public TimeSpan EndTime { get; }

        private readonly IEnumerable<Loc> _pos;
        private readonly Color _color;

        public FlashAnimation(IEnumerable<Loc> pos, in Color color)
        {
            _pos = pos;
            _color = color;

            CurrentTime = TimeSpan.Zero;
            EndTime = CurrentTime + Duration;
        }

        public bool Update(TimeSpan dt)
        {
            CurrentTime += dt;
            return CurrentTime >= EndTime;
        }

        public void Cleanup() { }

        public void Draw(LayerInfo layer)
        {
            double fracPassed = CurrentTime / Duration;
            Color between = _color.Blend(Colors.Floor, fracPassed);
            Terminal.Color(between);
            Terminal.Layer(layer.Z + 1);
            foreach (Loc pos in _pos) {
                layer.Put(pos.X - Camera.X, pos.Y - Camera.Y, '▓');
            }
            Terminal.Layer(layer.Z);
        }
    }
}
