using BearLib;
using GeomaceRL.Interface;
using GeomaceRL.UI;
using System.Drawing;

namespace GeomaceRL.Items
{
    public abstract class Item : IDrawable
    {
        public string Name { get; }
        public Color Color { get; }
        public char Symbol { get; }

        public Loc Pos { get; set; }
        public bool ShouldDraw { get; set; }

        public Item(in Loc pos, string name, Color color, char symbol)
        {
            Name = name;
            Pos = pos;
            Color = color;
            Symbol = symbol;
            ShouldDraw = true;
        }

        public void Draw(LayerInfo layer)
        {
            if (!ShouldDraw)
                return;

            Terminal.Color(Color);
            layer.Put(Pos.X - Camera.X, Pos.Y - Camera.Y, Symbol);
        }
    }
}
