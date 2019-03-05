using BearLib;
using GeomaceRL.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeomaceRL.Animation
{
    internal class LaserAnimation : IAnimation
    {
        public int Turn { get; } = EventScheduler.Turn;

        private readonly IList<Loc> _targets;
        private int _frame;
        private readonly char _symb1;
        private readonly char _symb2;
        private readonly char _symb3;

        public LaserAnimation(IEnumerable<Loc> targets)
        {
            _targets = targets.ToList();
            _frame = 0;

            _symb1 = '.';
            _symb2 = '*';
            _symb3 = '▓';
        }

        public bool Update()
        {
            if (_frame < 40)
            {
                _frame++;
                return false;
            }
            else
            {
                return true;
            }

        }

        public void Cleanup() { }

        public void Draw(LayerInfo layer)
        {
            Terminal.Color(Colors.Fire);
            Terminal.Layer(layer.Z + 1);

            for (int i = 0; i < _targets.Count; i++)
            {
                (int x, int y) = _targets[i];

                if (_frame < 6)
                {
                    layer.Put(x - Camera.X, y - Camera.Y, _symb1);
                }
                else
                {
                    layer.Put(x - Camera.X, y - Camera.Y, _symb2);
                }

                if (_frame >= 12 && _frame <= 35)
                {
                    Terminal.Layer(layer.Z + 2);
                    layer.Put(x - Camera.X, y - Camera.Y, _symb3);
                    Terminal.Layer(layer.Z + 1);
                }
            }

            Terminal.Layer(layer.Z);
        }
    }
}
