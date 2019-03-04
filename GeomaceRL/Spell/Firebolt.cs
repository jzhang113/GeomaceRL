using GeomaceRL.Command;

namespace GeomaceRL.Spell
{
    internal class Firebolt : ISpell
    {
        public (Element, int) Cost => (Element.Fire, 1);

        public ICommand Evoke(Actor.Actor source, in Loc target)
        {
            Game.MessagePanel.AddMessage($"{source.Name} casts Firebolt");
            return new AttackCommand(source, 10, target);
        }
    }
}
