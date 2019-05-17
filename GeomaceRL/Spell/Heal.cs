using GeomaceRL.Animation;
using GeomaceRL.Command;
using System.Collections.Generic;

namespace GeomaceRL.Spell
{
    internal class Heal : ISpell
    {
        public string Name => "Heal";
        public string Abbrev => "H";
        public bool Instant => true;

        public SpellCost Cost => new SpellCost(Element.Water, 3, Element.Lightning, 3);
        public TargetZone Zone => new TargetZone(TargetShape.Self, 0, 1);

        public ICommand Evoke(Actor.Actor source, IEnumerable<Loc> targets, (int, int) used)
        {
            source.Health = source.MaxHealth;
            Game.CurrentAnimations.Add(new FlashAnimation(targets, Colors.Lightning));

            return new WaitCommand(source);
        }
    }
}
