namespace GeomaceRL
{
    public readonly struct Loc
    {
        public int X { get; }
        public int Y { get; }

        public Loc(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static Loc operator +(in Loc current, (int X, int Y) offset) =>
            new Loc(current.X + offset.X, current.Y + offset.Y);

        public static Loc operator -(in Loc current, (int X, int Y) offset) =>
            new Loc(current.X - offset.X, current.Y - offset.Y);

        public static Loc operator +(in Loc a, in Loc b) => new Loc(a.X + b.X, a.Y + b.Y);

        public static Loc operator -(in Loc a, in Loc b) => new Loc(a.X - b.X, a.Y - b.Y);

        public void Deconstruct(out int x, out int y)
        {
            x = X;
            y = Y;
        }

        public override string ToString() => $"({X}, {Y})";

        #region equality
        public override bool Equals(object obj)
        {
            if (!(obj is Loc))
                return false;

            var location = (Loc)obj;
            return X == location.X && Y == location.Y;
        }

        public override int GetHashCode()
        {
            int hashCode = 1861411795;
            hashCode = (hashCode * -1521134295) + X.GetHashCode();
            hashCode = (hashCode * -1521134295) + Y.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(in Loc w1, in Loc w2) =>
            w1.X == w2.X && w1.Y == w2.Y;

        public static bool operator !=(in Loc w1, in Loc w2) => !(w1 == w2);
        #endregion
    }
}
