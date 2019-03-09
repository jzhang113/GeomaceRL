﻿using GeomaceRL.Actor;
using GeomaceRL.Command;
using System.Collections.Generic;

namespace GeomaceRL.Spell
{
    internal class EarthPillars : ISpell
    {
        public string Name => "Pillars";
        public string Abbrev => "PR";

        public SpellCost Cost => new SpellCost(
            Element.Earth, Constants.PILLARS_COST,
            Element.Metal, Constants.PILLARS_COST);

        public TargetZone Zone => new TargetZone(TargetShape.Self, Constants.PILLARS_RANGE);

        public ICommand Evoke(Actor.Actor source, IEnumerable<Loc> targets)
        {
            foreach (Loc loc in targets)
            {
                (Element elem, int amount) = Game.MapHandler.Mana[loc.X, loc.Y];

                if ((elem == Element.Earth || elem == Element.Metal) && amount > 0)
                {
                    Game.MapHandler.AddPillar(new Pillar(loc));
                }
            }

            return new WaitCommand(source);
        }
    }
}
