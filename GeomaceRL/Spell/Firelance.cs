using GeomaceRL.Animation;
using GeomaceRL.Command;
using System.Collections.Generic;

namespace GeomaceRL.Spell
{
    internal class Firelance : ISpell
    {
        public string Name => "Firelance";
        public string Abbrev => "FL";
        public bool Instant => false;

        public int Charges { get; set; } = -1;

        public SpellCost Cost => new SpellCost(Element.Fire, (Constants.FIRELANCE_MIN_COST, Constants.FIRELANCE_MAX_COST));
        public TargetZone Zone => new TargetZone(TargetShape.Beam, Constants.FIRELANCE_RANGE);

        public ICommand Evoke(Actor.Actor source, IEnumerable<Loc> targets, (int, int) used)
        {
            Game.CurrentAnimations.Add(new LaserAnimation(targets, Element.Fire.Color(), Colors.FireAccent));
            return new AttackCommand(source, (Element.Fire, Constants.FIRELANCE_DAMAGE), targets);
        }
    }
}
