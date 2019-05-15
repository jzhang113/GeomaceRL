using GeomaceRL.Spell;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeomaceRL
{
    internal static class SpellHandler
    {
        private static List<Type> AllSpells;

        static SpellHandler()
        {
            var spellType = typeof(ISpell);
            AllSpells = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => spellType.IsAssignableFrom(p) && p.IsClass)
                .ToList();
        }

        public static ISpell RandomSpell() => (ISpell)Activator.CreateInstance(AllSpells[Game.Rand.Next(AllSpells.Count)]);
    }
}
