using GeomaceRL.Command;
using System.Collections.Generic;

namespace GeomaceRL.Spell
{
    internal class Heal : ISpell
    {
        public string Name => "Heal";
        public string Abbrev => "H";
        public SpellCost Cost => new SpellCost(Element.Water, 3, Element.Lightning, 3);
        public TargetZone Zone => new TargetZone(TargetShape.Self);

        public ICommand Evoke(Actor.Actor source, IEnumerable<Loc> targets, (int, int) used)
        {
            source.Health = source.MaxHealth;
            return new WaitCommand(source);
        }
    }
}
