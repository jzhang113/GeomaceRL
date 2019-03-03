namespace GeomaceRL
{
    internal static class Constants
    {
        public const float MIN_VISIBLE_LIGHT_LEVEL = 0.25f;
        public const double LIGHT_DECAY = 0.1;

        // UI constants
        public const int MAP_WIDTH = 60;
        public const int MAP_HEIGHT = 60;
        public const int MAPVIEW_WIDTH = 40;
        public const int MAPVIEW_HEIGHT = 40;
        public const int SIDEBAR_WIDTH = 20;
        public const int MESSAGE_HEIGHT = 10;

        public const int SCREEN_WIDTH = MAPVIEW_WIDTH + SIDEBAR_WIDTH + 1;
        public const int SCREEN_HEIGHT = MAPVIEW_HEIGHT + MESSAGE_HEIGHT + 1;

        public const char HEADER_LEFT = '╡';  // 181
        public const char HEADER_RIGHT = '╞'; // 198
        public const char HEADER_SEP = '│';   // 179

        // Misc constants
        public const int MESSAGE_HISTORY_COUNT = 100;
    }
}
