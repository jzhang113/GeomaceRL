using BearLib;
using GeomaceRL.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace GeomaceRL.Animation
{
    internal class TrailAnimation : IAnimation
    {
        public int Turn { get; } = EventScheduler.Turn;
        public TimeSpan Duration { get; } = Game.FrameRate * 2;
        public TimeSpan StartTime { get; }
        public TimeSpan EndTime { get; }

        private readonly IList<Loc> _path;
        private readonly Color _color;

        public TrailAnimation(IEnumerable<Loc> path, in Color color)
        {
            _path = path.ToList();
            _color = color;

            StartTime = Game.Ticks;
            EndTime = StartTime + (Duration * _path.Count);
        }

        public bool Update() => Game.Ticks >= EndTime;

        public void Cleanup() { }

        public void Draw(LayerInfo layer)
        {
            double timePassed = (Game.Ticks - StartTime) / Duration;
            int intPassed = (int)timePassed;
            double fracPassed = timePassed - intPassed;

            Terminal.Layer(layer.Z + 1);
            Terminal.Color(_color);
            int current = 0;

            for (; current < intPassed; current++)
            {
                (int x, int y) = _path[current];
                layer.Put(x - Camera.X, y - Camera.Y, '▒');
            }

            Terminal.Layer(layer.Z);
        }
    }
}
