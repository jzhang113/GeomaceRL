using GeomaceRL.Command;

namespace GeomaceRL.Spell
{
    public interface ISpell
    {
        (Element, int) Cost { get; }

        ICommand Evoke(Actor.Actor source, in Loc target);
    }
}
