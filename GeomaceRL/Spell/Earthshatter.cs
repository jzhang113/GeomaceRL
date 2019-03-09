using GeomaceRL.Command;
using System;
using System.Collections.Generic;

namespace GeomaceRL.Spell
{
    internal class Earthshatter : ISpell
    {
        public string Name => "Earthshatter";
        public string Abbrev => "ES";
        public SpellCost Cost => new SpellCost(Element.Earth, Constants.EARTHSHATTER_COST);
        public TargetZone Zone => new TargetZone(TargetShape.Self, 1);

        public ICommand Evoke(Actor.Actor source, IEnumerable<Loc> targets)
        {
            return new AttackCommand(source, 10, targets);
        }
    }
}
