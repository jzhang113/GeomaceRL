using GeomaceRL.Command;
using System.Collections.Generic;

namespace GeomaceRL.Spell
{
    public interface ISpell
    {
        string Abbrev { get; }

        (Element, int) Cost { get; }

        TargetZone Zone { get; }

        ICommand Evoke(Actor.Actor source, IEnumerable<Loc> targets);
    }
}
