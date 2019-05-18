using GeomaceRL.Spell;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeomaceRL
{
    internal static class SpellHandler
    {
        public static List<Type> AllSpells { get; }

        static SpellHandler()
        {
            Type spellType = typeof(ISpell);
            AllSpells = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => spellType.IsAssignableFrom(p) && p.IsClass)
                .ToList();
        }

        public static ISpell RandomSpell() => (ISpell)Activator.CreateInstance(AllSpells[Game.Rand.Next(AllSpells.Count)]);
    }
}
