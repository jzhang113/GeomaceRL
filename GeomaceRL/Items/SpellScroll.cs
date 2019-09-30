using GeomaceRL.Actor;
using GeomaceRL.Spell;

namespace GeomaceRL.Items
{
    public class SpellScroll : Item
    {
        public ISpell Spell { get; }

        public SpellScroll(in Loc pos, ISpell spell) : base(pos, $"scroll of {spell.Name}", Colors.Mapping[spell.GetType()], '?')
        {
            Spell = spell;
        }

        public void LearnSpell(Actor.Actor actor)
        {
            if (actor is Player player)
            {
                player.LearnSpell(Spell);
            }
        }
    }
}
