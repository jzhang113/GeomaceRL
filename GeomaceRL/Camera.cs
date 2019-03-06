using System;

namespace GeomaceRL
{
    internal static class Camera
    {
        public static int X { get; private set; }
        public static int Y { get; private set; }

        internal static void UpdateCamera()
        {
            const int screenWidth = Constants.MAPVIEW_WIDTH;
            const int screenHeight = Constants.MAPVIEW_HEIGHT;

            // set left and top limits for the camera
            int startX = Math.Max(Game.Player.Pos.X - (screenWidth / 2), 0);
            int startY = Math.Max(Game.Player.Pos.Y - (screenHeight / 2), 0);

            // set right and bottom limits for the camera
            const int xDiff = Constants.MAP_WIDTH - screenWidth;
            const int yDiff = Constants.MAP_HEIGHT - screenHeight;
            X = xDiff < 0 ? 0 : Math.Min(xDiff, startX);
            Y = yDiff < 0 ? 0 : Math.Min(yDiff, startY);
        }

        internal static bool OnScreen(in Loc pos)
        {
            (int x, int y) = pos;
            return x >= X && x < X + Constants.MAPVIEW_WIDTH && y >= Y && y < Y + Constants.MAPVIEW_HEIGHT;
        }
    }
}