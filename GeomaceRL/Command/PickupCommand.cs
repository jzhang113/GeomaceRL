using GeomaceRL.Actor;
using GeomaceRL.Animation;
using GeomaceRL.Items;
using Optional;

namespace GeomaceRL.Command
{
    internal class PickupCommand : ICommand
    {
        public Option<IAnimation> Animation => Option.None<IAnimation>();

        private readonly Actor.Actor _source;
        private Item _item;

        public PickupCommand(Actor.Actor source, in Item item)
        {
            _source = source;
            _item = item;
        }

        public Option<ICommand> Execute()
        {
            Game.MessagePanel.AddMessage($"You pick up a {_item.Name}");

            if (_item is SpellScroll scroll)
            {
                if (_source is Player player)
                {
                    player.SpellList.Add(scroll.Spell);
                }
            }

            Game.MapHandler.RemoveItem(_item);
            return Option.None<ICommand>();
        }
    }
}
