using GeomaceRL.Animation;
using GeomaceRL.Command;
using Optional;
using System.Drawing;
using System.Linq;

namespace GeomaceRL.Actor
{
    internal class Pillar : Actor
    {
        public Pillar(in Loc pos) : base(pos, 1, Element.Earth, 'o')
        {
            Color = Colors.Wall;
            Name = "Pillar";
            BlocksLight = true;
        }

        public override Option<ICommand> TriggerDeath()
        {
            Game.MapHandler.RemoveActor(this);

            if (Game.MapHandler.Field[Pos].IsVisible)
            {
                Game.MessagePanel.AddMessage("A pillar shatters");
                Game.MapHandler.Refresh();
            }

            return Option.None<ICommand>();
        }

        internal override int TakeDamage((Element elem, int power) attack, in Loc from)
        {
            base.TakeDamage((Element.None, attack.power), from);
            Loc hitDir = Distance.GetNearestDirection(Pos, from);

            // get random point in front
            for (int i = 0; i < 10; i++)
            {
                int dist = Game.Rand.Next(0, 10);
                int dir = Game.Rand.Next(0, 1);
                Loc newDir = (dir == 0) ? hitDir.Left().Left() : hitDir.Right().Right();
                int dist2 = Game.Rand.Next(0, dist / 2);

                Loc target = Pos + (hitDir.X * dist, hitDir.Y * dist) + (newDir.X * dist2, newDir.Y * dist2);
                if (!Game.MapHandler.Field.IsValid(target))
                    continue;

                Game.CurrentAnimations.Add(
                    new TrailAnimation(
                        Game.MapHandler.GetStraightLinePath(Pos, target)
                            .Where(point => !Game.MapHandler.Field[point].IsWall), Color, 1));
            }

            return attack.power;
        }
    }
}
