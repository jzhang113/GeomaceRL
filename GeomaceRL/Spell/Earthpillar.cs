using GeomaceRL.Actor;
using GeomaceRL.Command;
using System.Collections.Generic;

namespace GeomaceRL.Spell
{
    internal class Earthpillar : ISpell
    {
        public string Name => "Pillars";
        public string Abbrev => "PS";
        public bool Instant => true;

        public int Charges { get; set; } = -1;

        public SpellCost Cost => new SpellCost(
            Element.Earth, (Constants.PILLARS_MIN_COST, Constants.PILLARS_MAX_COST),
            Element.Lightning, (Constants.PILLARS_MIN_COST, Constants.PILLARS_MAX_COST));

        public TargetZone Zone => new TargetZone(TargetShape.Self, 0, Constants.PILLARS_RANGE);

        public ICommand Evoke(Actor.Actor source, IEnumerable<Loc> targets, (int, int) used)
        {
            foreach (Loc loc in targets)
            {
                Element elem = Game.MapHandler.Mana[loc.X, loc.Y];

                // NOTE: We actually want to create a pillar wherever a mana was expended
                // However, since we aren't actually tracking this information, and the
                // Evoke is processed immediately after the mana has been paid, we just cheat
                // create a pillar on Elementless tiles.
                // This behavior can be exploited to gain up to seven free pillars. While
                // this is not a huge advantage currently, this behavior should also be
                // revisted if additional synergies with pillars are added.
                if (elem == Element.None)
                {
                    Game.MapHandler.AddPillar(new Pillar(loc));
                }
            }

            return new WaitCommand(source);
        }
    }
}
