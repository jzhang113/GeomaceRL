using System.Drawing;

namespace GeomaceRL
{
    public enum Element
    {
        Fire,
        Earth,
        Metal,
        Water,
        Wood
    }

    public static class ElementExtension
    {
        public static Color Color(this Element element)
        {
            switch(element)
            {
                case Element.Fire: return Colors.Fire;
                case Element.Earth: return Colors.Earth;
                case Element.Metal: return Colors.Metal;
                case Element.Water: return Colors.Water;
                case Element.Wood: return Colors.Wood;
                default: return Colors.Neutral;
            }
        }

        public static string Abbrev(this Element element)
        {
            switch (element)
            {
                case Element.Fire: return "f";
                case Element.Earth: return "e";
                case Element.Metal: return "m";
                case Element.Water: return "w";
                case Element.Wood: return "d";
                default: return " ";
            }
        }
    }
}
