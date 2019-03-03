using BearLib;
using System.Drawing;

namespace GeomaceRL.UI
{
    public class LayerInfo
    {
        public string Name { get; }
        public int Z { get; }
        public int X { get; }
        public int Y { get; }
        public int Width { get; }
        public int Height { get; }

        public LayerInfo(string name, int z, int x, int y, int width, int height)
        {
            Name = name;
            Z = z;
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public bool PointInside(int x, int y) => x > X && y > Y && x <= X + Width && y <= y + Height;

        public void Put(int x, int y, int code)
        {
            if (x <= Width && y <= Height)
                Terminal.Put(X + x, Y + y, code);
            else
                System.Diagnostics.Debug.WriteLine($"Warning: {x} {y} out of bounds on layer {Name}");
        }

        public void Print(Rectangle layout, string text, ContentAlignment alignment)
        {
            layout.Offset(X, Y);
            Terminal.Print(layout, alignment, $"[font=text]{text}");
        }

        public void Print(int y, string text, ContentAlignment alignment = ContentAlignment.TopLeft)
        {
            if (y < Height)
                Terminal.Print(new Rectangle(X, Y + y, Width, 1), alignment, $"[font=text]{text}");
            else
                System.Diagnostics.Debug.WriteLine($"Warning: line {y} out of bounds on layer {Name}");
        }

        public void Print(int x, int y, string text)
        {
            if (y < Height)
                Terminal.Print(X + x, Y + y, $"[font=text]{text}");
            else
                System.Diagnostics.Debug.WriteLine($"Warning: line {y} out of bounds on layer {Name}");
        }

        public void DrawBorders(BorderInfo border)
        {
            if (border.TopLeftChar != default)
                Terminal.Put(X - 1, Y - 1, border.TopLeftChar);

            if (border.TopRightChar != default)
                Terminal.Put(X + Width, Y - 1, border.TopRightChar);

            if (border.BottomLeftChar != default)
                Terminal.Put(X - 1, Y + Height, border.BottomLeftChar);

            if (border.BottomRightChar != default)
                Terminal.Put(X + Width, Y + Height, border.BottomRightChar);

            if (border.TopChar != default)
                for (int dx = 0; dx < Width; dx++) Terminal.Put(X + dx, Y - 1, border.TopChar);

            if (border.BottomChar != default)
                for (int dx = 0; dx < Width; dx++) Terminal.Put(X + dx, Y + Height, border.BottomChar);

            if (border.LeftChar != default)
                for (int dy = 0; dy < Height; dy++) Terminal.Put(X - 1, Y + dy, border.LeftChar);

            if (border.RightChar != default)
                for (int dy = 0; dy < Height; dy++) Terminal.Put(X + Width, Y + dy, border.RightChar);
        }

        public void Clear() => Terminal.ClearArea(X, Y, Width, Height);
    }
}
