namespace GeomaceRL
{
    internal static class Constants
    {
        // FOV and lighting stuff
        public const float MIN_VISIBLE_LIGHT_LEVEL = 0.25f;
        public const double LIGHT_DECAY = 0.1;

        // UI constants
        public const int MAP_WIDTH = 60;
        public const int MAP_HEIGHT = 60;
        public const int MAPVIEW_WIDTH = 41;
        public const int MAPVIEW_HEIGHT = 25;
        public const int SIDEBAR_WIDTH = 10;
        public const int MESSAGE_HEIGHT = 5;
        public const int SPELLBAR_HEIGHT = 3;

        public const int SCREEN_WIDTH = MAPVIEW_WIDTH + SIDEBAR_WIDTH + 1;
        public const int SCREEN_HEIGHT = MAPVIEW_HEIGHT + MESSAGE_HEIGHT + SPELLBAR_HEIGHT + 2;

        public const char HEADER_LEFT = '─';  // 196
        public const char HEADER_RIGHT = '─';
        public const char HEADER_SEP = '│';   // 179

        // Misc constants
        public const int MESSAGE_HISTORY_COUNT = 100;
        public const int GEN_ATTACK = 1;
        public const int COLLISION_DAMAGE = 1;

        // Base hp constants
        public const int SPRITE_HP = 1;
        public const int PLAYER_HP = 8;
        public const int ELEMENTAL_HP = 2;
        public const int LEECH_HP = 1;

        // Spell constants
        // Due to UI limitations, MAX_COST should not be > 6
        public const int FIREBOLT_MIN_COST = 2;
        public const int FIREBOLT_MAX_COST = 2;
        public const int FIREBOLT_RANGE = 5;
        public const int FIREBOLT_DAMAGE = 1;

        public const int FIRELANCE_MIN_COST = 1;
        public const int FIRELANCE_MAX_COST = 1;
        public const int FIRELANCE_RANGE = 8;
        public const int FIRELANCE_DAMAGE = 1;

        public const int JETSTREAM_MIN_COST = 1;
        public const int JETSTREAM_MAX_COST = 4;
        public const int JETSTREAM_DAMAGE = 1;

        public const int PILLARS_MIN_COST = 1;
        public const int PILLARS_MAX_COST = 6;
        public const int PILLARS_RANGE = 1;

        public const int EARTHSHATTER_MIN_COST = 2;
        public const int EARTHSHATTER_MAX_COST = 2;
        public const int EARTHSHATTER_DAMAGE = 2;
        public const int EARTHSHATTER_RANGE = 3;

        public const int TELEPORT_MIN = 3;
        public const int TELEPORT_COST = 3;
        public const int TELEPORT_RANGE = 6;

        public const int HEAL_MIN_COST = 2;
        public const int HEAL_MAX_COST = 2;
    }
}
