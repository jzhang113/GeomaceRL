using GeomaceRL.Command;
using GeomaceRL.Spell;
using Optional;
using System.Collections.Generic;

namespace GeomaceRL.Actor
{
    public class Player : Actor
    {
        //public EquipmentHandler Equipment { get; }

        public IDictionary<Element, int> Mana { get; }
        public IList<ISpell> SpellList { get; }

        public Player(in Loc pos) : base(pos, Constants.PLAYER_HP, Colors.Player, '@')
        {
            //Equipment = new EquipmentHandler();
            Mana = new Dictionary<Element, int>()
            {
                [Element.Fire] = 0,
                [Element.Earth] = 0,
                [Element.Lightning] = 0,
                [Element.Water] = 0,
            };

            SpellList = new List<ISpell>()
            {
                new Firelance(),
                new Firebolt(),
                new Waterstream(),
                new Earthpillar(),
                new Earthshatter()
            };

            Name = "Player";
            Speed = 2;
        }

        // Wait for the input system to set NextCommand. Since Commands don't repeat, clear
        // NextCommand once it has been sent.
        public override Option<ICommand> GetAction()
        {
            return Option.None<ICommand>();
        }

        public override Option<ICommand> TriggerDeath()
        {
            base.TriggerDeath();
            Game.GameOver();
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
