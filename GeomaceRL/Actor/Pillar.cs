using GeomaceRL.Animation;
using System.Drawing;

namespace GeomaceRL.Actor
{
    internal class Pillar : Actor
    {
        public Pillar(in Loc pos, in Color color) : base(pos, 1, color, 'o')
        {
            Name = "Pillar";
            BlocksLight = true;
        }

        internal override void TakeDamage(int power, in Loc from)
        {
            base.TakeDamage(power, from);
            Loc hitDir = Distance.GetNearestDirection(Pos, from);

            Loc next = Pos + hitDir;
            Game.CurrentAnimations.Add(new TrailAnimation(new Loc[] { next, next + hitDir, next + hitDir + hitDir } , Color));
        }
    }
}
