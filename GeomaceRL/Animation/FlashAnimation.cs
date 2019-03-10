﻿using BearLib;
using GeomaceRL.UI;
using System;
using System.Drawing;

namespace GeomaceRL.Animation
{
    internal class FlashAnimation : IAnimation
    {
        public int Turn { get; } = EventScheduler.Turn;
        public TimeSpan Duration { get; } = Game.FrameRate * 4;
        public TimeSpan StartTime { get; }
        public TimeSpan EndTime { get; }

        private readonly int _x;
        private readonly int _y;
        private readonly Color _color;

        public FlashAnimation(int x, int y, in Color color)
        {
            _x = x;
            _y = y;
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
            layer.Put(_x - Camera.X, _y - Camera.Y, '▓');
            Terminal.Layer(layer.Z);
        }
    }
}
