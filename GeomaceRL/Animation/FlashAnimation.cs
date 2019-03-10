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
        public TimeSpan StartTime { get; }
        public TimeSpan EndTime { get; }

        private readonly IEnumerable<Loc> _pos;
        private readonly Color _color;

        public FlashAnimation(IEnumerable<Loc> pos, in Color color)
        {
            _pos = pos;
            _color = color;

            StartTime = Game.Ticks;
            EndTime = StartTime + Duration;
        }

        public bool Update() => Game.Ticks >= EndTime;

        public void Cleanup() { }

        public void Draw(LayerInfo layer)
        {
            double fracPassed = (Game.Ticks - StartTime) / Duration;
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
