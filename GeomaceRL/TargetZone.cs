using System;
using System.Collections.Generic;
using System.Linq;

namespace GeomaceRL
{
    public enum TargetShape
    {
        Self,
        Range,
        Ray,
        Pierce,
        Directional,
        Beam
    }

    public class TargetZone
    {
        public TargetShape Shape { get; }
        public int Range { get; }
        public int Radius { get; }
        public bool Projectile { get; }
        public ICollection<Loc> Trail { get; }

        public bool Pierce { get; }

        private ICollection<Loc> Targets { get; }

        public TargetZone(TargetShape shape, int range = 1, int radius = 0, bool projectile = true)
        {
            Shape = shape;
            Range = range;
            Radius = radius;
            Projectile = projectile;
            Trail = new List<Loc>();
            Targets = new List<Loc>();

            Pierce = shape == TargetShape.Pierce;
        }

        public IEnumerable<Loc> GetTilesInRange(Actor.Actor current, in Loc target)
        {
            Targets.Clear();

            switch (Shape)
            {
                case TargetShape.Self:
                    foreach (Loc point in Game.MapHandler.GetPointsInRadius(current.Pos, Range))
                    {
                        if (Projectile && point == current.Pos)
                        {
                            continue;
                        }

                        if (!Game.MapHandler.Field[point].IsWall)
                        {
                            Targets.Add(point);
                        }
                    }
                    return Targets;
                case TargetShape.Range:
                    return GetRangeTiles(current.Pos, target);
                case TargetShape.Ray:
                    return GetRayTiles(current.Pos, target);
                case TargetShape.Pierce:
                    return GetPierceTiles(current.Pos, target);
                case TargetShape.Directional:
                    return GetDirectionalTiles(current.Pos, target);
                case TargetShape.Beam:
                    return GetBeamTiles(current.Pos, target);
                default:
                    throw new ArgumentException("unknown skill shape");
            }
        }

        private IEnumerable<Loc> GetRangeTiles(in Loc source, in Loc target)
        {
            Loc collision = target;

            // for simplicity, assume that the travel path is only 1 tile wide
            // TODO: trail should be as wide as the Radius
            if (Projectile)
            {
                collision = source;
                Trail.Clear();

                foreach (Loc point in Game.MapHandler.GetStraightLinePath(source, target))
                {
                    Trail.Add(point);
                    collision = point;

                    if (!Game.MapHandler.Field[point.X, point.Y].IsWalkable)
                    {
                        break;
                    }
                }
            }

            foreach (Loc point in Game.MapHandler.GetPointsInRadius(collision, Radius))
            {
                // TODO: prevent large radius spells from hitting past walls.
                Targets.Add(point);
            }
            return Targets;
        }

        private IEnumerable<Loc> GetRayTiles(in Loc source, in Loc target)
        {
            IEnumerable<Loc> path = Game.MapHandler.GetStraightLinePath(source, target);
            if (Projectile)
            {
                foreach (Loc point in path)
                {
                    // since each step takes us farther away, we can stop checking as soon
                    // as one tile falls out of range
                    if (!InRange(source, point))
                    {
                        break;
                    }

                    Targets.Add(point);

                    // projectiles stop at the first blocked tile
                    if (!Game.MapHandler.Field[point].IsWalkable)
                    {
                        break;
                    }
                }

                return Targets;
            }
            else
            {
                return path;
            }
        }

        // similar to ray but goes through walls
        private IEnumerable<Loc> GetPierceTiles(in Loc source, in Loc target)
        {
            return Game.MapHandler.GetStraightLinePath(source, target);
        }

        private IEnumerable<Loc> GetDirectionalTiles(in Loc source, in Loc target)
        {
            (int dx, int dy) = Distance.GetNearestDirection(target, source);
            int limit = Math.Max(Math.Abs(target.X - source.X), Math.Abs(target.Y - source.Y));

            for (int i = 1; i <= limit; i++)
            {
                Loc posInDir = source + (i * dx, i * dy);

                // since each step takes us farther away, we can stop checking as soon as one
                // tile falls out of range
                if (!InRange(source, posInDir))
                {
                    break;
                }

                Targets.Add(posInDir);

                // projectiles stop at the first blocked tile
                if (Projectile && !Game.MapHandler.Field[posInDir].IsWalkable)
                {
                    break;
                }
            }
            return Targets;
        }

        // similar to directional but always goes to max range
        private IEnumerable<Loc> GetBeamTiles(in Loc source, in Loc target)
        {
            (int dx, int dy) = Distance.GetNearestDirection(target, source);
            if (dx == 0 && dy == 0)
            {
                return Enumerable.Empty<Loc>();
            }

            int step = 1;
            Loc posInDir = source;

            do
            {
                posInDir = source + (step * dx, step * dy);

                if (!InRange(source, posInDir))
                {
                    break;
                }

                Targets.Add(posInDir);
                step++;
            } while (!Game.MapHandler.Field[posInDir].IsWall);

            return Targets;
        }

        private bool InRange(in Loc source, in Loc target)
        {
            // square ranges
            int distance = Math.Max(Math.Abs(source.X - target.X), Math.Abs(source.Y - target.Y));
            return distance <= Range;
        }
    }
}
