using System;

namespace GeomaceRL
{
    // Helper methods for calculating distances
    public static class Distance
    {
        public static Loc GetNearestDirection(in Loc a, in Loc b)
        {
            int dx = a.X - b.X;
            int dy = a.Y - b.Y;
            int ax = Math.Abs(dx);
            int ay = Math.Abs(dy);
            bool straight = Math.Abs(ax - ay) > Math.Max(ax / 2, ay / 2);
            int sx = 0, sy = 0;

            if (straight)
            {
                if (ax > ay)
                    sx = Math.Sign(dx);
                else
                    sy = Math.Sign(dy);
            }
            else
            {
                sx = Math.Sign(dx);
                sy = Math.Sign(dy);
            }

            switch (sx + (3 * sy) + 5)
            {
                case 1: return Direction.NW;
                case 2: return Direction.N;
                case 3: return Direction.NE;
                case 4: return Direction.W;
                case 5: return Direction.Center;
                case 6: return Direction.E;
                case 7: return Direction.SW;
                case 8: return Direction.S;
                case 9: return Direction.SE;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        public static double EuclideanDistance(int x1, int y1, int x2, int y2)
        {
            return Math.Sqrt(EuclideanDistanceSquared(x1, y1, x2, y2));
        }

        public static int EuclideanDistanceSquared(int x1, int y1, int x2, int y2)
        {
            int dx = x1 - x2;
            int dy = y1 - y2;

            return (dx * dx) + (dy * dy);
        }
    }
}
