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
        public TimeSpan Duration { get; }
        public TimeSpan CurrentTime { get; private set; }
        public TimeSpan EndTime { get; }

        private readonly IList<Loc> _path;
        private readonly Color _color;

        public TrailAnimation(IEnumerable<Loc> path, in Color color, int speed)
        {
            _path = path.ToList();
            _color = color;

            Duration = Game.FrameRate * speed;
            CurrentTime = TimeSpan.Zero;
            EndTime = CurrentTime + (Duration * _path.Count);
        }

        public bool Update(TimeSpan dt)
        {
            CurrentTime += dt;
            return CurrentTime >= EndTime;
        }

        public void Cleanup() { }

        public void Draw(LayerInfo layer)
        {
            double timePassed = CurrentTime / Duration;
            int intPassed = (int)timePassed;
            double fracPassed = timePassed - intPassed;

            Terminal.Layer(layer.Z + 1);
            Terminal.Color(_color);
            int current = 0;

            for (; current <= intPassed; current++)
            {
                (int x, int y) = _path[current];
                layer.Put(x - Camera.X, y - Camera.Y, '▒');
            }

            Terminal.Layer(layer.Z);
        }
    }
}
