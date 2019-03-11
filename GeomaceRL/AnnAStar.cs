using System;
using System.Collections.Generic;

namespace GeomaceRL
{
    // Implementing annotated A* - essentially A* but considers a bit of extra information
    // (clearance) as well
    public static class AnnAStar
    {
        public static Dictionary<Loc, int> explored = new Dictionary<Loc, int>();
        public static Dictionary<Loc, Loc> prev = new Dictionary<Loc, Loc>();
        private static readonly MaxHeap<LocCost> frontier = new MaxHeap<LocCost>(16);

        public static IEnumerable<Loc> Search(Loc start, in Loc goal, int clearance)
        {
            frontier.Clear();
            frontier.Add(new LocCost(start, 0));

            prev.Clear();
            prev[start] = start;

            explored.Clear();
            explored[start] = 0;

            while (frontier.Count > 0)
            {
                LocCost current = frontier.PopMax();

                if (current.Loc.X == goal.X && current.Loc.Y == goal.Y)
                    return MakePath(start, goal);

                foreach (Loc next in Game.MapHandler.GetPointsInRadius(current.Loc, 1))
                {
                    if (next == goal)
                    {
                        prev[next] = current.Loc;
                        return MakePath(start, goal);
                    }

                    if (Game.MapHandler.Clearance[next.X, next.Y] < clearance)
                        continue;

                    if (!Game.MapHandler.Field[next].IsWalkable)
                        continue;

                    // assuming all squares have equal movement cost - update if this is not so
                    int newCost = current.Cost - 1;

                    if (!explored.ContainsKey(next) || newCost > explored[next])
                    {
                        explored[next] = newCost;
                        int priority = newCost - Heuristic(next, goal);
                        frontier.Add(new LocCost(next, priority));
                        prev[next] = current.Loc;
                    }
                }
            }

            return new List<Loc>
            {
                start
            };
        }

        private static IEnumerable<Loc> MakePath(in Loc start, in Loc goal)
        {
            var path = new List<Loc>();
            Loc curr = goal;
            Loc step = prev[curr];

            while (curr != start)
            {
                path.Add(curr);

                if (curr == step)
                    break;

                curr = step;
                step = prev[curr];
            }

            path.Reverse();
            return path;
        }

        private static int Heuristic(in Loc a, in Loc b)
        {
            // manhattan distance, exact
            return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
        }
    }
}
