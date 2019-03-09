using BearLib;
using Optional;
using System;

namespace GeomaceRL.Spell
{
    public class SpellCost
    {
        private struct ElemCost
        {
            public Element Element { get; }
            public (int Min, int Max) Cost { get; }

            public ElemCost(Element element, (int, int) cost)
            {
                Element = element;
                Cost = cost;
            }
        }

        public Element MainElem => _main.Element;
        public Option<Element> AltElem => _alt.Map(elem => elem.Element);

        private readonly ElemCost _main;
        private readonly Option<ElemCost> _alt;

        public SpellCost(Element elem, int amount)
        {
            _main = new ElemCost(elem, (amount, amount));
            _alt = Option.None<ElemCost>();
        }

        public SpellCost(Element elem, (int, int) range)
        {
            _main = new ElemCost(elem, range);
            _alt = Option.None<ElemCost>();
        }

        public SpellCost(Element main, int mainAmount, Element alt, int altAmount)
        {
            _main = new ElemCost(main, (mainAmount, mainAmount));
            _alt = Option.Some(new ElemCost(alt, (altAmount, altAmount)));
        }

        public SpellCost(Element main, (int, int) mainRange, Element alt, (int, int) altRange)
        {
            _main = new ElemCost(main, mainRange);
            _alt = Option.Some(new ElemCost(alt, altRange));
        }

        public int MainManaUsed() => ManaUsed(_main);

        public int AltManaUsed()
        {
            return _alt.Match(
                some: ManaUsed,
                none: () => 0);
        }

        public string GetMainString() => GetElemString(_main);

        public string GetAltString()
        {
            return _alt.Match(
                some: GetElemString,
                none: () => "");
        }

        private int ManaUsed(ElemCost elem)
        {
            if (Game.Player.Mana[elem.Element] < elem.Cost.Min)
                return -1;
            else
                return Math.Min(Game.Player.Mana[elem.Element], elem.Cost.Max);
        }

        private string GetElemString(ElemCost elem)
        {
            Terminal.Color(elem.Element.Color());
            return FormatRange(elem.Cost) + elem.Element.Abbrev();
        }

        private static string FormatRange((int, int) range)
        {
            if (range.Item1 == range.Item2)
                return range.Item1.ToString();
            else
                return "V";
        }
    }
}
