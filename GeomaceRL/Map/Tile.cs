using BearLib;
using GeomaceRL.UI;
using System.Drawing;

namespace GeomaceRL.Map
{
    public class Tile
    {
        public int X { get; }
        public int Y { get; }
        public Color Color { get; }

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

        public Tile(int x, int y, in Color color)
        {
            X = x;
            Y = y;
            IsWall = true;
            Color = color;
        }

        public void Draw(LayerInfo layer)
        {
            int dispX = X - Camera.X;
            int dispY = Y - Camera.Y;

            // Multipliers for the alpha of colors of background colors indicating mana
            const double backgroundAlphaMult = 0.6;
            const double unseenAlphaMult = 0.8;

            Element element = Game.MapHandler.Mana[X, Y];
            if (element != Element.None)
            {
                Color color = element.Color();
                Terminal.Color(color.Blend(Color.Black, backgroundAlphaMult));

                if (!IsVisible)
                    Terminal.Color(color.Blend(Color.Black, unseenAlphaMult));

                Terminal.Layer(layer.Z - 1);
                layer.Put(dispX, dispY, '█');
                Terminal.Layer(layer.Z);
            }

            Terminal.Color(Color);
            layer.Put(dispX, dispY, IsWall ? '#' : '.');
        }
    }
}