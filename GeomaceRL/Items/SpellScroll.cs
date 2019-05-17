using GeomaceRL.Spell;

namespace GeomaceRL.Items
{
    public class SpellScroll : Item
    {
        public ISpell Spell { get; }

        public SpellScroll(in Loc pos, ISpell spell) : base(pos, $"scroll of {spell.Name}", Swatch.DbGrass, '?')
        {
            Spell = spell;
        }
    }
}
