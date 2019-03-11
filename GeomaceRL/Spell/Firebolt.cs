using GeomaceRL.Animation;
using GeomaceRL.Command;
using System.Collections.Generic;

namespace GeomaceRL.Spell
{
    internal class Firebolt : ISpell
    {
        public string Name => "Fireball";
        public string Abbrev => "FB";
        public SpellCost Cost => new SpellCost(Element.Fire, Constants.FIREBOLT_COST);
        public TargetZone Zone => new TargetZone(TargetShape.Range, Constants.FIREBOLT_RANGE, 1);

        public ICommand Evoke(Actor.Actor source, IEnumerable<Loc> targets)
        {
            Game.CurrentAnimations.Add(new FlashAnimation(targets, Element.Fire.Color()));
            return new AttackCommand(source, (Element.Fire, Constants.FIREBOLT_DAMAGE), targets);
        }
    }
}
