using BearLib;
using GeomaceRL.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace GeomaceRL.Animation
{
    internal class LaserAnimation : IAnimation
    {
        public int Turn { get; } = EventScheduler.Turn;
        public TimeSpan Duration { get; } = Game.FrameRate * 30;
        public TimeSpan StartTime { get; }
        public TimeSpan EndTime { get; }

        private readonly IList<Loc> _targets;
        private readonly Color _color;
        private readonly Color _altColor;
        private readonly char _symb1;
        private readonly char _symb2;
        private readonly char _symb3;

        public LaserAnimation(IEnumerable<Loc> targets, in Color main, in Color alt)
        {
            _targets = targets.ToList();
            _color = main;
            _altColor = alt;

            StartTime = Game.Ticks;
            EndTime = StartTime + Duration;

            _symb1 = '.';
            _symb2 = '*';
            _symb3 = '▓';
        }

        public bool Update() => Game.Ticks >= EndTime;

        public void Cleanup() { }

        public void Draw(LayerInfo layer)
        {
            int frames = (int)((Game.Ticks - StartTime) / Game.FrameRate);

            Terminal.Color(_color);
            Terminal.Layer(layer.Z + 2);

            for (int i = 0; i < _targets.Count; i++)
            {
                (int x, int y) = _targets[i];

                if (frames < 6 || frames > 27)
                {
                    layer.Put(x - Camera.X, y - Camera.Y, _symb1);
                }
                else
                {
                    layer.Put(x - Camera.X, y - Camera.Y, _symb2);
                }
            }

            Color newColor = _color;
            if (frames >= 12 && frames <= 24)
            {
                if (frames % 4 == 0)
                    newColor = _color.Blend(_altColor, Game.VisRand.NextDouble());

                Terminal.Color(newColor);
                Terminal.Layer(layer.Z + 3);

                for (int i = 0; i < _targets.Count; i++)
                {
                    (int x, int y) = _targets[i];
                    layer.Put(x - Camera.X, y - Camera.Y, _symb3);
                }
            }

            Terminal.Layer(layer.Z);
        }
    }
}
