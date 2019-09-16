using GeomaceRL.Command;
using GeomaceRL.Spell;
using Optional;
using System.Collections.Generic;

namespace GeomaceRL.Actor
{
    public class Player : Actor
    {
        public IDictionary<Element, int> Mana { get; }
        public IList<ISpell> SpellList { get; }

        public Player(in Loc pos) : base(pos, Constants.PLAYER_HP, Element.None, '@')
        {
            Mana = new Dictionary<Element, int>()
            {
                [Element.Fire] = 0,
                [Element.Earth] = 0,
                [Element.Lightning] = 0,
                [Element.Water] = 0,
                [Element.None] = 0
            };

            // TODO: Ensure that starting spells are different
            SpellList = new List<ISpell>()
            {
                SpellHandler.RandomSpell(),
                SpellHandler.RandomSpell(),
                SpellHandler.RandomSpell()
            };

            Name = "Player";
            Speed = 2;
        }

        // Commands processed in main loop
        public override Option<ICommand> GetAction()
        {
            return Option.None<ICommand>();
        }

        internal void ClearMana()
        {
            Mana[Element.Fire] = 0;
            Mana[Element.Earth] = 0;
            Mana[Element.Lightning] = 0;
            Mana[Element.Water] = 0;
        }
    }
}
