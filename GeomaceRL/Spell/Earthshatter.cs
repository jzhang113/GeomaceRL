using GeomaceRL.Animation;
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
        public TargetZone Zone => new TargetZone(TargetShape.Pierce, 3);

        public ICommand Evoke(Actor.Actor source, IEnumerable<Loc> targets)
        {
            foreach (Loc point in targets)
            {
                Game.MapHandler.Field[point].IsWall = false;
            }

            Game.CurrentAnimations.Add(new TrailAnimation(targets, Element.Earth.Color(), 3));
            return new AttackCommand(source, (Element.Earth, Constants.EARTHSHATTER_DAMAGE), targets);
        }
    }
}
