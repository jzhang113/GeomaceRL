using GeomaceRL.Actor;
using GeomaceRL.Command;
using System.Collections.Generic;

namespace GeomaceRL.Spell
{
    internal class Earthpillar : ISpell
    {
        public string Name => "Pillars";
        public string Abbrev => "PS";
        public bool Instant => false;

        public SpellCost Cost => new SpellCost(
            Element.Earth, Constants.PILLARS_COST,
            Element.Lightning, Constants.PILLARS_COST);

        public TargetZone Zone => new TargetZone(TargetShape.Self, Constants.PILLARS_RANGE);

        public ICommand Evoke(Actor.Actor source, IEnumerable<Loc> targets, (int, int) used)
        {
            foreach (Loc loc in targets)
            {
                (Element elem, int amount) = Game.MapHandler.Mana[loc.X, loc.Y];

                if ((elem == Element.Earth || elem == Element.Lightning) && amount > 0)
                {
                    Game.MapHandler.AddPillar(new Pillar(loc));
                }
            }

            return new WaitCommand(source);
        }
    }
}
