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

        public SpellCost Cost => new SpellCost(
            Element.Water, (Constants.HEAL_MIN_COST, Constants.HEAL_MAX_COST),
            Element.Lightning, (Constants.HEAL_MIN_COST, Constants.HEAL_MAX_COST));
        public TargetZone Zone => new TargetZone(TargetShape.Self, 0, 1);

        public ICommand Evoke(Actor.Actor source, IEnumerable<Loc> targets, (int, int) used)
        {
            source.Health = source.MaxHealth;
            Game.CurrentAnimations.Add(new FlashAnimation(targets, Colors.Lightning));

            // TODO: better implementation of consumable spells
            if (source is Actor.Player player)
                player.SpellList.Remove(this);

            return new WaitCommand(source);
        }
    }
}
