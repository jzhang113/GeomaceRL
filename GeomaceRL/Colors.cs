using System.Drawing;

namespace GeomaceRL
{
    internal static class Colors
    {
        public static Color FloorBackground =   Swatch.SecondaryDarkest;
        public static Color Floor =             Swatch.Secondary;

        public static Color WallBackground =    Swatch.SecondaryDarkest;
        public static Color Wall =              Swatch.DbStone;

        // Units
        public static Color Player =            Swatch.DbLight;

        // UI
        public static Color Path =              Swatch.Alternate;
        public static Color Target =            Swatch.PrimaryDarker;
        public static Color TargetBackground =  Swatch.Secondary;
        public static Color Cursor =            Swatch.PrimaryDarkest;

        public static Color Text =              Swatch.DbLight;
        public static Color DimText =           Swatch.SecondaryDarkest;
        public static Color RowHighlight =      Swatch.ComplimentDarkest;
        public static Color ButtonBackground =  Swatch.Alternate;
        public static Color ButtonBorder =      Swatch.Secondary;
        public static Color ButtonHover =       Swatch.AlternateDarker;

        public static Color BorderColor =       Swatch.Primary;
        public static Color HighlightColor =    Swatch.Secondary;

        public static Color PlayerThreat =      Swatch.Primary;
        public static Color EnemyThreat =       Swatch.Compliment;

        // Elements
        public static Color Wood =              Swatch.DbGrass;
        public static Color Fire =              Color.FromArgb(255, 185, 0);
        public static Color Earth =             Swatch.DbStone;
        public static Color Metal =             Swatch.DbMetal;
        public static Color Water =             Swatch.DbWater;
        public static Color Neutral =           Swatch.DbLight;

        // Map features
        public static Color Door =              Swatch.DbBrightWood;
        public static Color Exit =              Swatch.Alternate;
        public static Color Hook =              Swatch.DbLight;
        public static Color FireAccent =        Swatch.DbBlood;
    }
}
