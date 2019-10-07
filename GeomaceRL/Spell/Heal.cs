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

        public int Charges { get; set; } = Constants.HEAL_CHARGES;

        public SpellCost Cost => new SpellCost(
            Element.Water, (Constants.HEAL_MIN_COST, Constants.HEAL_MAX_COST),
            Element.Air, (Constants.HEAL_MIN_COST, Constants.HEAL_MAX_COST));
        public TargetZone Zone => new TargetZone(TargetShape.Self, 0, 1);

        public ICommand Evoke(Actor.Actor source, IEnumerable<Loc> targets, (int, int) used)
        {
            source.Health = source.MaxHealth;
            Game.Animations.Add(source.Id, new FlashAnimation(targets, Colors.Lightning));

            return new WaitCommand(source);
        }
    }
}
