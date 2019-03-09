using GeomaceRL.Command;
using System.Collections.Generic;

namespace GeomaceRL.Spell
{
    internal class Firebolt : ISpell
    {
        public string Name => "Firebolt";
        public string Abbrev => "FB";
        public SpellCost Cost => new SpellCost(Element.Fire, Constants.FIREBOLT_COST);
        public TargetZone Zone => new TargetZone(TargetShape.Range, Constants.FIREBOLT_RANGE);

        public ICommand Evoke(Actor.Actor source, IEnumerable<Loc> targets)
        {
            return new AttackCommand(source, Constants.FIREBOLT_DAMAGE, targets);
        }
    }
}
