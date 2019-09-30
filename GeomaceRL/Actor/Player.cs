using GeomaceRL.Command;
using GeomaceRL.Spell;
using Optional;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeomaceRL.Actor
{
    public class Player : Actor
    {
        public IDictionary<Element, int> Mana { get; }
        public IList<ISpell> SpellList { get; }

        // Play statistics
        public int SpellsKnown { get; private set; }
        public IDictionary<Type, int> KillCount { get; }

        public Player(in Loc pos) : base(pos, Element.None)
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

            SpellsKnown = SpellList.Count;

            Type actorType = typeof(Actor);
            KillCount = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => actorType.IsAssignableFrom(p) && p.IsClass)
                .ToDictionary(x => x, _ => 0);
        }

        public void LearnSpell(ISpell spell)
        {
            // TODO: if player already knows a non consumable spell, upgrade?
            SpellList.Add(spell);
            SpellsKnown++;
        }

        // Commands processed in main loop
        public override Option<ICommand> GetAction() => Option.None<ICommand>();

        public override Option<ICommand> TriggerDeath()
        {
            Game.MessagePanel.AddMessage("Game over! Press any key to continue");

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
