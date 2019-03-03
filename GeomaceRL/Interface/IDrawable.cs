using GeomaceRL.UI;
using System.Drawing;

namespace GeomaceRL.Interface
{
    public interface IDrawable
    {
        Loc Pos { get; set; }
        Color Color { get; }
        char Symbol { get; }

        void Draw(LayerInfo layer);
    }
}
