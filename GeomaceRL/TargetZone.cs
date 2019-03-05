using System;
using System.Collections.Generic;

namespace GeomaceRL
{
    public enum TargetShape
    {
        Self,
        Range,
        Ray,
        Directional
    }

    public class TargetZone
    {
        public TargetShape Shape { get; }
        public int Range { get; }
        public int Radius { get; }
        public bool Projectile { get; }
        public ICollection<Loc> Trail { get; }

        private ICollection<Loc> Targets { get; }

        public TargetZone(TargetShape shape, int range = 1, int radius = 0, bool projectile = true)
        {
            Shape = shape;
            Range = range;
            Radius = radius;
            Projectile = projectile;
            Trail = new List<Loc>();
            Targets = new List<Loc>();
        }

        public IEnumerable<Loc> GetTilesInRange(Actor.Actor current, in Loc target)
        {
            Targets.Clear();

            switch (Shape)
            {
                case TargetShape.Self:
                    foreach (Loc point in Game.MapHandler.GetPointsInRadius(current.Pos, Radius))
                    {
                        if (InRange(current, point))
                            Targets.Add(point);
                    }
                    return Targets;
                case TargetShape.Range:
                    Loc collision = target;

                    // for simplicity, assume that the travel path is only 1 tile wide
                    // TODO: trail should be as wide as the Radius
                    if (Projectile)
                    {
                        collision = current.Pos;
                        Trail.Clear();

                        foreach (Loc point in Game.MapHandler.GetStraightLinePath(current.Pos, target))
                        {
                            Trail.Add(point);
                            collision = point;

                            if (!Game.MapHandler.Field[point.X, point.Y].IsWalkable)
                                break;
                        }
                    }

                    foreach (Loc point in Game.MapHandler.GetPointsInRadius(collision, Radius))
                    {
                        // TODO: prevent large radius spells from hitting past walls.
                        Targets.Add(point);
                    }
                    return Targets;
                case TargetShape.Ray:
                    IEnumerable<Loc> path = Game.MapHandler.GetStraightLinePath(current.Pos, target);
                    if (Projectile)
                    {
                        foreach (Loc point in path)
                        {
                            // since each step takes us farther away, we can stop checking as soon
                            // as one tile falls out of range
                            if (!InRange(current, point))
                                break;

                            Targets.Add(point);

                            // projectiles stop at the first blocked tile
                            if (!Game.MapHandler.Field[point].IsWalkable)
                                break;
                        }

                        return Targets;
                    }
                    else
                    {
                        return path;
                    }
                case TargetShape.Directional:
                    var (dx, dy) = Distance.GetNearestDirection(target, current.Pos);
                    int limit = Math.Max(Math.Abs(target.X - current.Pos.X), Math.Abs(target.Y - current.Pos.Y));

                    for (int i = 1; i <= limit; i++)
                    {
                        Loc posInDir = current.Pos + (i * dx, i * dy);

                        // since each step takes us farther away, we can stop checking as soon as one
                        // tile falls out of range
                        if (!InRange(current, posInDir))
                            break;

                        Targets.Add(posInDir);

                        // projectiles stop at the first blocked tile
                        if (Projectile && !Game.MapHandler.Field[posInDir].IsWalkable)
                            break;
                    }
                    return Targets;
                default:
                    throw new ArgumentException("unknown skill shape");
            }
        }

        private bool InRange(Actor.Actor actor, in Loc target)
        {
            // square ranges
            int distance = Math.Max(Math.Abs(actor.Pos.X - target.X), Math.Abs(actor.Pos.Y - target.Y));
            return distance <= Range;
        }
    }
}
