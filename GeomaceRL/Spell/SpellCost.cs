using System;

namespace GeomaceRL.Spell
{
    public class SpellCost
    {
        public Element MainElem { get; }
        public Element AltElem { get; }
        public (int, int) MainCost { get; }
        public (int, int) AltCost { get; }

        public SpellCost(Element main, (int, int) mainRange)
        {
            MainElem = main;
            MainCost = mainRange;
            AltElem = Element.None;
            AltCost = (0, 0);
        }

        public SpellCost(Element main, (int, int) mainRange, Element alt, (int, int) altRange)
        {
            MainElem = main;
            MainCost = mainRange;
            AltElem = alt;
            AltCost = altRange;
        }

        public int MainManaUsed() => ManaUsed(MainElem, MainCost);

        public int AltManaUsed() => ManaUsed(AltElem, AltCost);

        private int ManaUsed(Element elem, (int Min, int Max) cost)
        {
            if (elem == Element.None)
                return 0;
            else if (Game.Player.Mana[elem] < cost.Min)
                return Game.Player.Mana[elem];
            else
                return Math.Min(Game.Player.Mana[elem], cost.Max);
        }
    }
}
