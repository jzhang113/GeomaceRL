using GeomaceRL.Command;
using System.Collections.Generic;

namespace GeomaceRL.Spell
{
    public interface ISpell
    {
        string Name { get; }

        string Abbrev { get; }

        // Instant spells are untargetted.
        bool Instant { get; }

        // Number of times this spell may be cast. Spells with -1 charges are infinite.
        int Charges { get; set; }

        SpellCost Cost { get; }

        TargetZone Zone { get; }

        ICommand Evoke(Actor.Actor source, IEnumerable<Loc> targets, (int, int) used);
    }
}
