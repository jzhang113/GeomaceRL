using GeomaceRL.UI;
using System;

namespace GeomaceRL.Map
{
    public class Tile
    {
        public int X { get; }
        public int Y { get; }

        public float Light
        {
            get => _light;
            internal set
            {
                if (value < 0)
                    _light = 0;
                else if (value > 1)
                    _light = 1;
                else
                    _light = value;
            }
        }

        public int Fuel { get; internal set; }
        public bool IsOccupied { get; internal set; }
        public bool IsExplored { get; internal set; }
        public bool BlocksLight { get; internal set; }
        public bool LosExists { get; internal set; }

        public bool IsVisible => LosExists && Light > Constants.MIN_VISIBLE_LIGHT_LEVEL;
        public bool IsWall { get; set; }
        public bool IsWalkable => !IsWall && !IsOccupied;
        public bool IsLightable => !IsWall && !BlocksLight;

        private float _light;

        public Tile(int x, int y)
        {
            X = x;
            Y = y;
            IsWall = true;
        }

        public void Draw(LayerInfo layer)
        {
            int dispX = X - Camera.X;
            int dispY = Y - Camera.Y;

            if (IsWall)
            {
                layer.Put(dispX, dispY, '#');
            }
            else
            {
                layer.Put(dispX, dispY, '.');
            }
        }
    }
}