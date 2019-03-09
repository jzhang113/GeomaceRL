using GeomaceRL.Animation;
using GeomaceRL.Command;
using System.Collections.Generic;

namespace GeomaceRL.Spell
{
    internal class Firelance : ISpell
    {
        public string Name => "Firelance";
        public string Abbrev => "FL";
        public SpellCost Cost => new SpellCost(Element.Fire, Constants.FIRELANCE_COST);
        public TargetZone Zone => new TargetZone(TargetShape.Beam, Constants.FIRELANCE_RANGE);

        public ICommand Evoke(Actor.Actor source, IEnumerable<Loc> targets)
        {
            // TODO: self-casted firelance is legal but hits nothing and spends mana
            Game.CurrentAnimations.Add(new LaserAnimation(targets, Element.Fire.Color(), Colors.FireAccent));
            return new AttackCommand(source, Constants.FIRELANCE_DAMAGE, targets);
        }
    }
}
