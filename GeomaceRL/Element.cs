using System.Drawing;

namespace GeomaceRL
{
    public enum Element
    {
        Fire,
        Earth,
        Lightning,
        Water,
    }

    public static class ElementExtension
    {
        public static Color Color(this Element element)
        {
            switch(element)
            {
                case Element.Fire: return Colors.Fire;
                case Element.Earth: return Colors.Earth;
                case Element.Lightning: return Colors.Lightning;
                case Element.Water: return Colors.Water;
                default: return Colors.Neutral;
            }
        }

        public static string Abbrev(this Element element)
        {
            switch (element)
            {
                case Element.Fire: return "F";
                case Element.Earth: return "E";
                case Element.Lightning: return "L";
                case Element.Water: return "W";
                default: return " ";
            }
        }
    }
}
