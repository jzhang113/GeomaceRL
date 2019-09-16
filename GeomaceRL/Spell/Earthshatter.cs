using GeomaceRL.Animation;
using GeomaceRL.Command;
using System.Collections.Generic;
using System.Linq;

namespace GeomaceRL.Spell
{
    internal class Earthshatter : ISpell
    {
        public string Name => "Earthshatter";
        public string Abbrev => "ES";
        public bool Instant => true;

        public SpellCost Cost => new SpellCost(Element.Earth, (Constants.EARTHSHATTER_MIN_COST, Constants.EARTHSHATTER_MAX_COST));
        public TargetZone Zone => new TargetZone(TargetShape.Self, 0, Constants.EARTHSHATTER_RANGE);

        public ICommand Evoke(Actor.Actor source, IEnumerable<Loc> targets, (int, int) used)
        {
            Game.CurrentAnimations.Add(new FlashAnimation(targets.Where(pos => Distance.Chebyshev(pos, source.Pos) == 1), Element.Earth.Color()));
            Game.CurrentAnimations.Add(new FlashAnimation(targets.Where(pos => Distance.Chebyshev(pos, source.Pos) == 2), Element.Earth.Color()));
            Game.CurrentAnimations.Add(new FlashAnimation(targets.Where(pos => Distance.Chebyshev(pos, source.Pos) == 3), Element.Earth.Color()));

            return new AttackCommand(source, (Element.Earth, Constants.EARTHSHATTER_DAMAGE), targets);
        }
    }
}
