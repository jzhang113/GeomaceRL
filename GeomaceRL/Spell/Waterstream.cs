using GeomaceRL.Animation;
using GeomaceRL.Command;
using System.Collections.Generic;
using System.Linq;

namespace GeomaceRL.Spell
{
    internal class Waterstream : ISpell
    {
        public string Name => "Jetstream";
        public string Abbrev => "JS";
        public bool Instant => false;

        public SpellCost Cost => new SpellCost(Element.Water, (Constants.JETSTREAM_MIN_COST, Constants.JETSTREAM_MAX_COST));
        public TargetZone Zone => new TargetZone(TargetShape.Beam, 2 * Cost.MainManaUsed());

        public ICommand Evoke(Actor.Actor source, IEnumerable<Loc> targets, (int, int) used)
        {
            Game.CurrentAnimations.Add(
                new LaserAnimation(targets, Element.Water.Color(), Colors.WaterAccent));

            Loc dir = Distance.GetNearestDirection(source.Pos, targets.First());
            int power = used.Item1;

            return new AttackMoveCommand(source, (Element.Water, Constants.JETSTREAM_DAMAGE), targets, dir, power);
        }
    }
}
