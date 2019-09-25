using GeomaceRL.Animation;
using GeomaceRL.Command;
using System.Collections.Generic;

namespace GeomaceRL.Spell
{
    internal class Firebolt : ISpell
    {
        public string Name => "Fireball";
        public string Abbrev => "FB";
        public bool Instant => false;

        public int Charges { get; set; } = -1;

        public SpellCost Cost => new SpellCost(Element.Fire, (Constants.FIREBOLT_MIN_COST, Constants.FIREBOLT_MAX_COST));
        public TargetZone Zone => new TargetZone(TargetShape.Range, Constants.FIREBOLT_RANGE, 1);

        public ICommand Evoke(Actor.Actor source, IEnumerable<Loc> targets, (int, int) used)
        {
            Game.Animations.Add(source.Id, new FlashAnimation(targets, Element.Fire.Color()));
            return new AttackCommand(source, (Element.Fire, Constants.FIREBOLT_DAMAGE), targets);
        }
    }
}
