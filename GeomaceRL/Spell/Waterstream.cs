using GeomaceRL.Animation;
using GeomaceRL.Command;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeomaceRL.Spell
{
    internal class Waterstream : ISpell
    {
        public string Name => "Jetstream";
        public string Abbrev => "JS";
        public SpellCost Cost => new SpellCost(Element.Water, (1, Constants.JETSTREAM_COST));
        public TargetZone Zone => new TargetZone(TargetShape.Beam, 2 * Cost.MainManaUsed());

        public ICommand Evoke(Actor.Actor source, IEnumerable<Loc> targets)
        {
            Game.CurrentAnimations.Add(
                new LaserAnimation(targets, Element.Water.Color(), Colors.WaterAccent));

            Loc dir = Distance.GetNearestDirection(source.Pos, targets.First());
            int power = Cost.MainManaUsed();

            return new AttackMoveCommand(source, Constants.JETSTREAM_DAMAGE, targets, dir, power);
        }
    }
}
