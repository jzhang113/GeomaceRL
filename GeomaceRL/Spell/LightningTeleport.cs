using GeomaceRL.Command;
using System.Collections.Generic;
using System.Linq;

namespace GeomaceRL.Spell
{
    internal class LightningTeleport : ISpell
    {
        public string Name => "Teleport";
        public string Abbrev => "TP";
        public SpellCost Cost => new SpellCost(Element.Lightning, (Constants.TELEPORT_MIN, Constants.TELEPORT_COST));
        public TargetZone Zone => new TargetZone(TargetShape.Range, Constants.TELEPORT_RANGE);

        public ICommand Evoke(Actor.Actor source, IEnumerable<Loc> targets, (int, int) used)
        {
            var target = targets.First();
            int accuracy = Constants.TELEPORT_COST - used.Item1;
            var landing = Game.MapHandler.GetPointsInRadius(target, accuracy).Where(point =>
                Game.MapHandler.Field[point].IsWalkable);

            int index = Game.Rand.Next(landing.Count());
            var square = landing.ElementAt(index);

            Game.MapHandler.SetActorPosition(source, square);
            return new WaitCommand(source);
        }
    }
}
