namespace GeomaceRL.Map
{
    public class Room
    {
        public int X { get; internal set; }
        public int Y { get; internal set; }
        public int Width { get; }
        public int Height { get; }

        public int Area => Width * Height;
        public (int X, int Y) Center => (X + (Width / 2), Y + (Height / 2));

        public int Left => X;
        public int Right => X + Width;
        public int Top => Y;
        public int Bottom => Y + Height;

        public Room(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public Room(int width, int height)
        {
            X = 0;
            Y = 0;
            Width = width;
            Height = height;
        }

        public bool Intersects(Room other)
        {
            return other.X < X + Width
                   && X < other.X + other.Width
                   && other.Y < Y + Height
                   && Y < other.Y + other.Height;
        }

        public void Offset(int x, int y)
        {
            X += x;
            Y += y;
        }
    }
}
