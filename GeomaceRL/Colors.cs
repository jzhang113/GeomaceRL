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
        public static Color Wood =              Color.FromArgb(0, 255, 0);
        public static Color Fire =              Color.FromArgb(249, 75, 10);
        public static Color Earth =             Color.FromArgb(255, 255, 0);
        public static Color Metal =             Swatch.DbMetal;
        public static Color Water =             Color.FromArgb(0, 160, 255);
        public static Color Neutral =           Swatch.DbLight;

        // Map features
        public static Color Door =              Swatch.DbBrightWood;
        public static Color Exit =              Swatch.DbLight;
        public static Color FireAccent =        Color.FromArgb(240, 188, 25);

        public static Color Blend(this in Color c1, in Color c2, double alpha)
        {
            return Color.FromArgb(
                (int)(c1.R + (c2.R - c1.R) * alpha),
                (int)(c1.G + (c2.G - c1.G) * alpha),
                (int)(c1.B + (c2.B - c1.B) * alpha));
        }
    }
}
