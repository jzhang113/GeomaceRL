using System.Drawing;

namespace GeomaceRL
{
    public enum Element
    {
        None,
        Fire,
        Earth,
        Air,
        Water
    }

    public static class ElementExtension
    {
        public static Element Opposing(this Element element)
        {
            switch (element)
            {
                case Element.Fire: return Element.Water;
                case Element.Earth: return Element.Air;
                case Element.Air: return Element.Earth;
                case Element.Water: return Element.Fire;
                default: return Element.None;
            }
        }

        public static Color Color(this Element element)
        {
            switch(element)
            {
                case Element.Fire: return Colors.Fire;
                case Element.Earth: return Colors.Earth;
                case Element.Air: return Colors.Lightning;
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
                case Element.Air: return "L";
                case Element.Water: return "W";
                default: return " ";
            }
        }
    }
}
