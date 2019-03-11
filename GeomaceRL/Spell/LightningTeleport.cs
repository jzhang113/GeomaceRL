using GeomaceRL.Command;
using System.Collections.Generic;
using System.Linq;

namespace GeomaceRL.Spell
{
    internal class LightningTeleport : ISpell
    {
        public string Name => "Teleport";
        public string Abbrev => "TP";
        public SpellCost Cost => new SpellCost(Element.Lightning, (Constants.TELEPORT_MIN, Constants.TELEPORT_COST));
        public TargetZone Zone => new TargetZone(TargetShape.Range, Constants.TELEPORT_RANGE, 0, false);

        public ICommand Evoke(Actor.Actor source, IEnumerable<Loc> targets, (int, int) used)
        {
            var target = targets.First();
            int accuracy = Constants.TELEPORT_COST - used.Item1;
            var landing = Game.MapHandler.GetPointsInRadius(target, accuracy);

            int index = Game.Rand.Next(landing.Count());
            Loc square = landing.ElementAt(index);

            Game.MapHandler.GetActor(square).MatchSome(actor =>
            {
                Game.MessagePanel.AddMessage($"Player lands on {actor.Name}");
                actor.TriggerDeath();
                if (actor is Actor.Bomber)
                {
                    Game.Player.TakeDamage((actor.Element, Constants.GEN_ATTACK * 2), actor.Pos);
                }
            });

            Game.MapHandler.SetActorPosition(source, square);
            return new WaitCommand(source);
        }
    }
}
