using System;

namespace GeomaceRL
{
    public struct LocCost : IComparable<LocCost>
    {
        public Loc Loc { get; }
        public int Cost { get; }

        public LocCost(in Loc loc, int cost)
        {
            Loc = loc;
            Cost = cost;
        }

        public int CompareTo(LocCost other)
        {
            return Cost - other.Cost;
        }
    }
}
