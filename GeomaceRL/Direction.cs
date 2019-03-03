namespace GeomaceRL
{
    internal static class Direction
    {
        public static readonly Loc N = new Loc(0, -1);
        public static readonly Loc E = new Loc(1, 0);
        public static readonly Loc S = new Loc(0, 1);
        public static readonly Loc W = new Loc(-1, 0);
        public static readonly Loc NE = new Loc(1, -1);
        public static readonly Loc SE = new Loc(1, 1);
        public static readonly Loc SW = new Loc(-1, 1);
        public static readonly Loc NW = new Loc(-1, -1);
        public static readonly Loc Center = new Loc(0, 0);

        public static readonly Loc[] DirectionList = {
            N,
            E,
            S,
            W,
            NE,
            SE,
            SW,
            NW
        };

        public static Loc Right(this in Loc dir)
        {
            if (dir == N)
                return NE;
            else if (dir == NE)
                return E;
            else if (dir == E)
                return SE;
            else if (dir == SE)
                return S;
            else if (dir == S)
                return SW;
            else if (dir == SW)
                return W;
            else if (dir == W)
                return NW;
            else if (dir == NW)
                return N;
            else
                return Center;
        }

        public static Loc Left(this in Loc dir)
        {
            if (dir == N)
                return NW;
            else if (dir == NE)
                return N;
            else if (dir == E)
                return NE;
            else if (dir == SE)
                return E;
            else if (dir == S)
                return SE;
            else if (dir == SW)
                return S;
            else if (dir == W)
                return SW;
            else if (dir == NW)
                return W;
            else
                return Center;
        }
    }
}
