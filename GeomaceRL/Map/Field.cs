using System.Collections;
using System.Collections.Generic;

namespace GeomaceRL.Map
{
    internal class Field : IEnumerable<Tile>
    {
        private readonly Tile[] _field;
        private readonly int _width;
        private readonly int _height;

        public Field(int width, int height)
        {
            _width = width;
            _height = height;

            _field = new Tile[width * height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                    _field[(y * height) + x] = new Tile(x, y);
            }
        }

        public Tile this[int i, int j] => _field[i + (_width * j)];

        public Tile this[Loc point] => _field[point.X + (_width * point.Y)];

        public bool IsValid(int i, int j) => i >= 0 && i < _width && j >= 0 && j < _height;

        public bool IsValid(in Loc point) =>
            point.X >= 0 && point.X < _width && point.Y >= 0 && point.Y < _height;

        public IEnumerator<Tile> GetEnumerator()
        {
            foreach (Tile tile in _field)
                yield return tile;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
