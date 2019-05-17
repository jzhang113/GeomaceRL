using GeomaceRL.Command;
using System.Collections.Generic;

namespace GeomaceRL.Spell
{
    public interface ISpell
    {
        string Name { get; }

        string Abbrev { get; }

        bool Instant { get; }

        SpellCost Cost { get; }

        TargetZone Zone { get; }

        ICommand Evoke(Actor.Actor source, IEnumerable<Loc> targets, (int, int) used);
    }
}
